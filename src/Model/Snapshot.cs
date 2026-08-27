using System.Collections.Generic;

namespace UrbanBrain.Model;

/// <summary>
/// L2 快照 —— 采集层和推理层之间的稳定契约。
///
/// 设计原则：
/// 1. 只用基础类型，不出现 Unity/ECS 类型 —— 这样能直接 JSON 序列化，
///    也能在没有游戏的机器上反序列化分析。
/// 2. 字段名要自解释 —— 这份 JSON 会直接喂给 LLM，字段名就是给模型的提示。
/// 3. 既给聚合指标也给原始量 —— 聚合指标供快速判断，原始量供核对 AI 有没有瞎说。
/// </summary>
public class CitySnapshot
{
    public string schemaVersion = "1.0";

    /// <summary>导出时的真实世界时间，便于对齐日志。</summary>
    public string exportedAt;

    /// <summary>游戏内模拟帧号。</summary>
    public uint simulationFrame;

    public List<IntersectionSnapshot> intersections = new();
}

/// <summary>一个信号控制路口。</summary>
public class IntersectionSnapshot
{
    public int id;

    public float x;
    public float z;

    /// <summary>原版划分的信号组（相位）数量。</summary>
    public int signalGroupCount;

    /// <summary>当前正在放行的信号组，从 1 开始；0 表示无。</summary>
    public int currentSignalGroup;

    /// <summary>信号状态机原始值。语义待确认，先原样带出。</summary>
    public int signalState;

    /// <summary>相位计时器原始值。</summary>
    public int signalTimer;

    /// <summary>是否已被 Urban Brain 接管。</summary>
    public bool controlled;

    public List<ApproachSnapshot> approaches = new();

    // ---- 聚合指标，供快速排序找出问题路口 ----

    /// <summary>各进口拥堵度里最差的那个。</summary>
    public float worstCongestion;

    /// <summary>所有进口的车道总数。</summary>
    public int totalCarLanes;
}

/// <summary>路口的一个进口方向（一条相连的道路）。</summary>
public class ApproachSnapshot
{
    public int edgeId;

    public float x;
    public float z;

    /// <summary>
    /// 相对路口中心的方位角（度，正北为 0，顺时针）。
    /// 让 AI 能说"北进口"而不是"边 #12345"。
    /// </summary>
    public float bearing;

    /// <summary>方位的可读名称：N / NE / E / SE / S / SW / W / NW。</summary>
    public string direction;

    // ---- 渠化（每种转向有几条车道）----
    public int laneLeft;
    public int laneStraight;
    public int laneRight;
    public int laneUTurn;

    /// <summary>公交专用道数量（按转向合计）。</summary>
    public int lanePublicOnly;

    /// <summary>人行横道数量。</summary>
    public int pedestrianCrossings;

    // ---- 实测流量 ----
    public FlowSnapshot flow = new();
}

/// <summary>
/// 从 Game.Net.LaneFlow 差分算出的通行情况。
///
/// LaneFlow 里 m_Distance / m_Duration 是累计量（float4，四个时间桶），
/// 距离除以时长即为平均通行速度。
/// </summary>
public class FlowSnapshot
{
    /// <summary>有流量数据的车道数。为 0 说明这个进口没采到数据。</summary>
    public int sampledLanes;

    /// <summary>累计通行距离（四桶合计）。</summary>
    public float totalDistance;

    /// <summary>累计通行时长（四桶合计）。</summary>
    public float totalDuration;

    /// <summary>平均通行速度 = totalDistance / totalDuration。</summary>
    public float avgSpeed;

    /// <summary>该进口车道的限速（取最大值）。</summary>
    public float speedLimit;

    /// <summary>
    /// 拥堵度 = 1 - avgSpeed / speedLimit，范围 0~1，越大越堵。
    /// 数据不足时为 -1。
    /// </summary>
    public float congestion = -1f;

    /// <summary>当前有车占用的车道数（来自 LaneSignal 的申请者）。</summary>
    public int occupiedLanes;
}
