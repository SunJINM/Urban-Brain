using Game.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UrbanBrain.Model;

namespace UrbanBrain.Utils;

/// <summary>
/// 扫描一个路口，产出 L2 快照。
///
/// 核心难点是判断「路口内部的连接车道，究竟是从哪条路来、到哪条路去」。
/// 游戏里路口中心有一批 SubLane（连接车道），每条带 Lane 组件，
/// 上面的 m_StartNode / m_EndNode 是 PathNode。进口道路的 SubLane 也有同样结构。
/// 两边 PathNode 相等即可配对，从而知道某条连接车道属于哪个进口、通向哪个出口。
///
/// 这套匹配逻辑移植自 TrafficLightsEnhancement 的 NodeUtils（已在真实游戏中验证过）。
/// </summary>
public struct IntersectionScanner
{
    public BufferLookup<SubLane> m_SubLane;
    public BufferLookup<ConnectedEdge> m_ConnectedEdge;
    public ComponentLookup<Lane> m_Lane;
    public ComponentLookup<Edge> m_Edge;
    public ComponentLookup<EdgeGeometry> m_EdgeGeometry;
    public ComponentLookup<CarLane> m_CarLane;
    public ComponentLookup<MasterLane> m_MasterLane;
    public ComponentLookup<PedestrianLane> m_PedestrianLane;
    public ComponentLookup<LaneFlow> m_LaneFlow;
    public ComponentLookup<LaneSignal> m_LaneSignal;
    public ComponentLookup<Node> m_Node;
    public ComponentLookup<TrafficLights> m_TrafficLights;

    /// <summary>路口内部一条连接车道的来源与去向。</summary>
    private struct LaneConnection
    {
        public Entity m_SourceEdge;
        public Entity m_SourceSubLane;
        public Entity m_DestEdge;
        public Entity m_DestSubLane;
    }

    public IntersectionSnapshot Scan(Entity nodeEntity, bool controlled)
    {
        var snap = new IntersectionSnapshot
        {
            id = nodeEntity.Index,
            controlled = controlled,
        };

        if (m_Node.TryGetComponent(nodeEntity, out Node node))
        {
            snap.x = node.m_Position.x;
            snap.z = node.m_Position.z;
        }

        if (m_TrafficLights.TryGetComponent(nodeEntity, out TrafficLights tl))
        {
            snap.signalGroupCount = tl.m_SignalGroupCount;
            snap.currentSignalGroup = tl.m_CurrentSignalGroup;
            snap.signalState = (int)tl.m_State;
            snap.signalTimer = tl.m_Timer;
        }

        if (!m_SubLane.TryGetBuffer(nodeEntity, out DynamicBuffer<SubLane> nodeSubLanes) ||
            !m_ConnectedEdge.TryGetBuffer(nodeEntity, out DynamicBuffer<ConnectedEdge> connectedEdges))
        {
            return snap;
        }

        var connectionMap = BuildConnectionMap(nodeSubLanes, connectedEdges);
        float3 nodePos = new float3(snap.x, 0, snap.z);

        foreach (ConnectedEdge ce in connectedEdges)
        {
            var approach = ScanApproach(nodeEntity, ce.m_Edge, nodePos, nodeSubLanes, connectionMap);
            snap.approaches.Add(approach);
            snap.totalCarLanes += approach.laneLeft + approach.laneStraight + approach.laneRight;
            if (approach.flow.congestion > snap.worstCongestion)
            {
                snap.worstCongestion = approach.flow.congestion;
            }
        }

        connectionMap.Dispose();
        return snap;
    }

    /// <summary>
    /// 扫描出每条路口内部连接车道的角色（来自哪个进口、执行什么转向）。
    ///
    /// 应用相位方案时调用一次，结果存进 LaneRole buffer，
    /// 运行时直接查表，不必每帧重算几何关系。
    ///
    /// 进口序号与 <see cref="Scan"/> 产出的 approaches 下标一致，
    /// 两者都按 ConnectedEdge buffer 的顺序编号。
    /// </summary>
    public System.Collections.Generic.List<Components.LaneRole> ScanRoles(Entity nodeEntity)
    {
        var result = new System.Collections.Generic.List<Components.LaneRole>();

        if (!m_SubLane.TryGetBuffer(nodeEntity, out DynamicBuffer<SubLane> nodeSubLanes) ||
            !m_ConnectedEdge.TryGetBuffer(nodeEntity, out DynamicBuffer<ConnectedEdge> connectedEdges))
        {
            return result;
        }

        var connectionMap = BuildConnectionMap(nodeSubLanes, connectedEdges);

        for (int edgeIndex = 0; edgeIndex < connectedEdges.Length; edgeIndex++)
        {
            Entity edgeEntity = connectedEdges[edgeIndex].m_Edge;

            foreach (SubLane nodeSubLane in nodeSubLanes)
            {
                Entity sub = nodeSubLane.m_SubLane;
                if (!connectionMap.TryGetValue(sub, out LaneConnection conn) || conn.m_SourceEdge != edgeEntity)
                {
                    continue;
                }
                if (m_MasterLane.HasComponent(sub))
                {
                    continue;
                }

                var role = new Components.LaneRole
                {
                    m_SubLane = sub,
                    m_Approach = (byte)edgeIndex,
                };

                if (m_PedestrianLane.TryGetComponent(sub, out PedestrianLane pl) &&
                    (pl.m_Flags & PedestrianLaneFlags.Crosswalk) != 0)
                {
                    role.m_IsPedestrian = true;
                    role.m_Turn = Components.TurnKind.Straight;
                    result.Add(role);
                    continue;
                }

                if (!m_CarLane.TryGetComponent(sub, out CarLane carLane))
                {
                    continue;
                }

                role.m_Turn = ClassifyTurn(carLane.m_Flags);
                result.Add(role);
            }
        }

        connectionMap.Dispose();
        return result;
    }

    private static Components.TurnKind ClassifyTurn(CarLaneFlags flags)
    {
        if ((flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.UTurnRight)) != 0)
        {
            return Components.TurnKind.UTurn;
        }
        if ((flags & (CarLaneFlags.TurnLeft | CarLaneFlags.GentleTurnLeft)) != 0)
        {
            return Components.TurnKind.Left;
        }
        if ((flags & (CarLaneFlags.TurnRight | CarLaneFlags.GentleTurnRight)) != 0)
        {
            return Components.TurnKind.Right;
        }
        return Components.TurnKind.Straight;
    }

    // ------------------------------------------------------------------
    // 单个进口
    // ------------------------------------------------------------------

    private ApproachSnapshot ScanApproach(
        Entity nodeEntity, Entity edgeEntity, float3 nodePos,
        DynamicBuffer<SubLane> nodeSubLanes,
        NativeHashMap<Entity, LaneConnection> connectionMap)
    {
        var ap = new ApproachSnapshot { edgeId = edgeEntity.Index };

        float3 edgePos = GetEdgePosition(nodeEntity, edgeEntity);
        ap.x = edgePos.x;
        ap.z = edgePos.z;
        ap.bearing = ComputeBearing(nodePos, edgePos);
        ap.direction = BearingToName(ap.bearing);

        // 统计渠化：遍历路口内部车道，挑出源头是本进口的那些
        foreach (SubLane nodeSubLane in nodeSubLanes)
        {
            Entity sub = nodeSubLane.m_SubLane;
            if (!connectionMap.TryGetValue(sub, out LaneConnection conn) || conn.m_SourceEdge != edgeEntity)
            {
                continue;
            }

            // MasterLane 是若干并行车道的聚合表示，跳过以免重复计数
            if (m_MasterLane.HasComponent(sub))
            {
                continue;
            }

            if (m_PedestrianLane.TryGetComponent(sub, out PedestrianLane pl) &&
                (pl.m_Flags & PedestrianLaneFlags.Crosswalk) != 0)
            {
                ap.pedestrianCrossings++;
                continue;
            }

            if (!m_CarLane.TryGetComponent(sub, out CarLane nodeCarLane))
            {
                continue;
            }

            m_CarLane.TryGetComponent(conn.m_SourceSubLane, out CarLane sourceCarLane);
            bool isPublicOnly = (sourceCarLane.m_Flags & CarLaneFlags.PublicOnly) != 0;
            bool isUTurn = (nodeCarLane.m_Flags & (CarLaneFlags.UTurnLeft | CarLaneFlags.UTurnRight)) != 0;

            if (isUTurn)
            {
                ap.laneUTurn++;
            }
            else if ((nodeCarLane.m_Flags & (CarLaneFlags.TurnLeft | CarLaneFlags.GentleTurnLeft)) != 0)
            {
                ap.laneLeft++;
            }
            else if ((nodeCarLane.m_Flags & (CarLaneFlags.TurnRight | CarLaneFlags.GentleTurnRight)) != 0)
            {
                ap.laneRight++;
            }
            else
            {
                ap.laneStraight++;
            }

            if (isPublicOnly)
            {
                ap.lanePublicOnly++;
            }
        }

        ap.flow = ScanFlow(edgeEntity);
        return ap;
    }

    /// <summary>
    /// 采集进口道路上的实测流量。
    /// LaneFlow 的 m_Distance / m_Duration 是累计量（四个时间桶），距离除以时长即平均速度。
    /// </summary>
    private FlowSnapshot ScanFlow(Entity edgeEntity)
    {
        var flow = new FlowSnapshot();

        if (!m_SubLane.TryGetBuffer(edgeEntity, out DynamicBuffer<SubLane> edgeSubLanes))
        {
            return flow;
        }

        foreach (SubLane sl in edgeSubLanes)
        {
            Entity sub = sl.m_SubLane;

            if (m_CarLane.TryGetComponent(sub, out CarLane cl))
            {
                float limit = math.max(cl.m_SpeedLimit, cl.m_DefaultSpeedLimit);
                if (limit > flow.speedLimit)
                {
                    flow.speedLimit = limit;
                }
            }

            if (m_LaneSignal.TryGetComponent(sub, out LaneSignal signal) && signal.m_Petitioner != Entity.Null)
            {
                flow.occupiedLanes++;
            }

            if (!m_LaneFlow.TryGetComponent(sub, out LaneFlow lf))
            {
                continue;
            }

            flow.sampledLanes++;
            flow.totalDistance += math.csum(lf.m_Distance);
            flow.totalDuration += math.csum(lf.m_Duration);
        }

        if (flow.totalDuration > 0.001f)
        {
            flow.avgSpeed = flow.totalDistance / flow.totalDuration;
            if (flow.speedLimit > 0.001f)
            {
                flow.congestion = math.clamp(1f - flow.avgSpeed / flow.speedLimit, 0f, 1f);
            }
        }

        return flow;
    }

    // ------------------------------------------------------------------
    // 连接映射
    // ------------------------------------------------------------------

    /// <summary>
    /// 建立「路口内部车道 → 来源进口 / 去向出口」的映射。
    /// 靠 PathNode 相等配对，移植自 TLE NodeUtils.GetLaneConnectionMap。
    /// </summary>
    private NativeHashMap<Entity, LaneConnection> BuildConnectionMap(
        DynamicBuffer<SubLane> nodeSubLanes, DynamicBuffer<ConnectedEdge> connectedEdges)
    {
        var map = new NativeHashMap<Entity, LaneConnection>(32, Allocator.Temp);

        foreach (SubLane nodeSubLane in nodeSubLanes)
        {
            var conn = new LaneConnection();

            if (m_Lane.TryGetComponent(nodeSubLane.m_SubLane, out Lane nodeLane))
            {
                foreach (ConnectedEdge ce in connectedEdges)
                {
                    if (!m_SubLane.TryGetBuffer(ce.m_Edge, out DynamicBuffer<SubLane> edgeSubLanes))
                    {
                        continue;
                    }
                    foreach (SubLane edgeSubLane in edgeSubLanes)
                    {
                        if (!m_Lane.TryGetComponent(edgeSubLane.m_SubLane, out Lane edgeLane))
                        {
                            continue;
                        }
                        // 路口车道的起点接在某条边车道的端点上，那条边就是来源
                        if (nodeLane.m_StartNode.Equals(edgeLane.m_EndNode) ||
                            nodeLane.m_StartNode.Equals(edgeLane.m_StartNode))
                        {
                            conn.m_SourceEdge = ce.m_Edge;
                            conn.m_SourceSubLane = edgeSubLane.m_SubLane;
                        }
                        // 路口车道的终点接在某条边车道的起点上，那条边就是去向
                        if (nodeLane.m_EndNode.Equals(edgeLane.m_StartNode) ||
                            nodeLane.m_EndNode.Equals(edgeLane.m_EndNode))
                        {
                            conn.m_DestEdge = ce.m_Edge;
                            conn.m_DestSubLane = edgeSubLane.m_SubLane;
                        }
                    }
                }

                // 连接车道之间还可能串联（例如经过路口中间的等待段），再补一轮内部配对
                foreach (SubLane other in nodeSubLanes)
                {
                    if (conn.m_SourceSubLane != Entity.Null && conn.m_DestSubLane != Entity.Null)
                    {
                        break;
                    }
                    if (other.m_SubLane == nodeSubLane.m_SubLane)
                    {
                        continue;
                    }
                    if (m_Lane.TryGetComponent(other.m_SubLane, out Lane otherLane))
                    {
                        if (conn.m_SourceSubLane == Entity.Null && nodeLane.m_StartNode.Equals(otherLane.m_EndNode))
                        {
                            conn.m_SourceSubLane = other.m_SubLane;
                        }
                        if (conn.m_DestSubLane == Entity.Null && nodeLane.m_EndNode.Equals(otherLane.m_StartNode))
                        {
                            conn.m_DestSubLane = other.m_SubLane;
                        }
                    }
                }
            }

            map[nodeSubLane.m_SubLane] = conn;
        }

        return map;
    }

    // ------------------------------------------------------------------
    // 几何辅助
    // ------------------------------------------------------------------

    private float3 GetEdgePosition(Entity nodeEntity, Entity edgeEntity)
    {
        m_Edge.TryGetComponent(edgeEntity, out Edge edge);
        m_EdgeGeometry.TryGetComponent(edgeEntity, out EdgeGeometry geo);

        if (edge.m_Start.Equals(nodeEntity))
        {
            return (geo.m_Start.m_Left.a + geo.m_Start.m_Right.a) / 2;
        }
        if (edge.m_End.Equals(nodeEntity))
        {
            return (geo.m_End.m_Left.d + geo.m_End.m_Right.d) / 2;
        }
        return default;
    }

    /// <summary>方位角，正北为 0 度顺时针递增。假定 +Z 为北，待实机确认。</summary>
    private static float ComputeBearing(float3 from, float3 to)
    {
        float3 d = to - from;
        float deg = math.degrees(math.atan2(d.x, d.z));
        return deg < 0 ? deg + 360f : deg;
    }

    private static string BearingToName(float bearing)
    {
        string[] names = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int idx = (int)math.round(bearing / 45f) % 8;
        return names[idx];
    }
}
