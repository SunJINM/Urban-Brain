using Game;
using Game.Net;
using Unity.Collections;
using Unity.Entities;
using UrbanBrain.Model;
using UrbanBrain.Utils;

namespace UrbanBrain.Systems;

/// <summary>
/// 路口扫描服务。
///
/// 它不做每帧的事情（OnUpdate 是空的），存在的意义是持有一堆 ComponentLookup，
/// 供导出、规则引擎、AI 顾问按需调用。扫描全城是重活，只在用户主动触发时才跑。
/// </summary>
public partial class IntersectionScanSystem : GameSystemBase
{
    private EntityQuery m_SignalNodeQuery;
    private Game.Simulation.SimulationSystem m_SimulationSystem;

    private BufferLookup<SubLane> m_SubLane;
    private BufferLookup<ConnectedEdge> m_ConnectedEdge;
    private ComponentLookup<Lane> m_Lane;
    private ComponentLookup<Edge> m_Edge;
    private ComponentLookup<EdgeGeometry> m_EdgeGeometry;
    private ComponentLookup<CarLane> m_CarLane;
    private ComponentLookup<MasterLane> m_MasterLane;
    private ComponentLookup<PedestrianLane> m_PedestrianLane;
    private ComponentLookup<LaneFlow> m_LaneFlow;
    private ComponentLookup<LaneSignal> m_LaneSignal;
    private ComponentLookup<Node> m_Node;
    private ComponentLookup<TrafficLights> m_TrafficLights;
    private ComponentLookup<Components.ControlledSignal> m_Controlled;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_SimulationSystem = World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>();
        m_SignalNodeQuery = GetEntityQuery(ComponentType.ReadOnly<TrafficLights>());

        m_SubLane = GetBufferLookup<SubLane>(true);
        m_ConnectedEdge = GetBufferLookup<ConnectedEdge>(true);
        m_Lane = GetComponentLookup<Lane>(true);
        m_Edge = GetComponentLookup<Edge>(true);
        m_EdgeGeometry = GetComponentLookup<EdgeGeometry>(true);
        m_CarLane = GetComponentLookup<CarLane>(true);
        m_MasterLane = GetComponentLookup<MasterLane>(true);
        m_PedestrianLane = GetComponentLookup<PedestrianLane>(true);
        m_LaneFlow = GetComponentLookup<LaneFlow>(true);
        m_LaneSignal = GetComponentLookup<LaneSignal>(true);
        m_Node = GetComponentLookup<Node>(true);
        m_TrafficLights = GetComponentLookup<TrafficLights>(true);
        m_Controlled = GetComponentLookup<Components.ControlledSignal>(true);

        // 纯服务型系统，不需要每帧跑
        Enabled = false;
    }

    protected override void OnUpdate()
    {
    }

    private IntersectionScanner BuildScanner()
    {
        m_SubLane.Update(this);
        m_ConnectedEdge.Update(this);
        m_Lane.Update(this);
        m_Edge.Update(this);
        m_EdgeGeometry.Update(this);
        m_CarLane.Update(this);
        m_MasterLane.Update(this);
        m_PedestrianLane.Update(this);
        m_LaneFlow.Update(this);
        m_LaneSignal.Update(this);
        m_Node.Update(this);
        m_TrafficLights.Update(this);
        m_Controlled.Update(this);

        return new IntersectionScanner
        {
            m_SubLane = m_SubLane,
            m_ConnectedEdge = m_ConnectedEdge,
            m_Lane = m_Lane,
            m_Edge = m_Edge,
            m_EdgeGeometry = m_EdgeGeometry,
            m_CarLane = m_CarLane,
            m_MasterLane = m_MasterLane,
            m_PedestrianLane = m_PedestrianLane,
            m_LaneFlow = m_LaneFlow,
            m_LaneSignal = m_LaneSignal,
            m_Node = m_Node,
            m_TrafficLights = m_TrafficLights,
        };
    }

    /// <summary>扫描单个路口。</summary>
    public IntersectionSnapshot ScanOne(Entity nodeEntity)
    {
        var scanner = BuildScanner();
        return scanner.Scan(nodeEntity, m_Controlled.HasComponent(nodeEntity));
    }

    /// <summary>扫描单个路口内每条连接车道的角色，供应用相位方案时写入 LaneRole buffer。</summary>
    public System.Collections.Generic.List<Components.LaneRole> ScanRoles(Entity nodeEntity)
    {
        var scanner = BuildScanner();
        return scanner.ScanRoles(nodeEntity);
    }

    /// <summary>扫描全城所有信号路口。</summary>
    public CitySnapshot ScanCity()
    {
        var scanner = BuildScanner();

        var snapshot = new CitySnapshot
        {
            exportedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            simulationFrame = m_SimulationSystem != null ? m_SimulationSystem.frameIndex : 0u,
        };

        var entities = m_SignalNodeQuery.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < entities.Length; i++)
        {
            snapshot.intersections.Add(scanner.Scan(entities[i], m_Controlled.HasComponent(entities[i])));
        }
        entities.Dispose();

        Mod.log.Info($"扫描完成：{snapshot.intersections.Count} 个信号路口");
        return snapshot;
    }
}
