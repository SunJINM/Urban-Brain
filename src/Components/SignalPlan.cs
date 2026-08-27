using Colossal.Serialization.Entities;
using Unity.Entities;

namespace UrbanBrain.Components;

/// <summary>
/// 转向类型。位序要和 <see cref="SignalPhase"/> 的掩码算法保持一致。
/// </summary>
public enum TurnKind : byte
{
    Left = 0,
    Straight = 1,
    Right = 2,
    UTurn = 3,
}

/// <summary>
/// 一个信号相位。
///
/// 设计上把「配时」和「转向控制」统一成同一件事：
/// 相位决定哪些「进口 × 转向」组合可以放行，以及放行多久。
/// 某个转向如果不出现在任何相位里，就等于被永久禁止 —— 这正是交通工程里
/// 用信号实现禁左的标准做法，不需要改动车道拓扑。
/// </summary>
public struct SignalPhase : IBufferElementData, ISerializable
{
    private ushort m_SchemaVersion;

    /// <summary>
    /// 放行掩码。bit 序号 = 进口序号 * 4 + 转向枚举值。
    /// 支持最多 8 个进口 × 4 种转向 = 32 位。
    /// </summary>
    public uint m_AllowMask;

    /// <summary>最短绿灯（秒）。</summary>
    public ushort m_MinDuration;

    /// <summary>最长绿灯（秒）。</summary>
    public ushort m_MaxDuration;

    /// <summary>目标绿灯（秒）。自适应模式下会在 min/max 之间浮动。</summary>
    public float m_TargetDuration;

    public SignalPhase()
    {
        m_SchemaVersion = 1;
        m_AllowMask = 0;
        m_MinDuration = 5;
        m_MaxDuration = 60;
        m_TargetDuration = 20f;
    }

    public static uint MakeBit(int approachIndex, TurnKind turn)
    {
        if (approachIndex < 0 || approachIndex > 7)
        {
            return 0u;
        }
        return 1u << (approachIndex * 4 + (int)turn);
    }

    public readonly bool Allows(int approachIndex, TurnKind turn)
    {
        uint bit = MakeBit(approachIndex, turn);
        return bit != 0 && (m_AllowMask & bit) != 0;
    }

    public void Allow(int approachIndex, TurnKind turn)
    {
        m_AllowMask |= MakeBit(approachIndex, turn);
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
        writer.Write(m_SchemaVersion);
        writer.Write(m_AllowMask);
        writer.Write(m_MinDuration);
        writer.Write(m_MaxDuration);
        writer.Write(m_TargetDuration);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
        reader.Read(out m_SchemaVersion);
        reader.Read(out m_AllowMask);
        reader.Read(out m_MinDuration);
        reader.Read(out m_MaxDuration);
        reader.Read(out m_TargetDuration);
    }
}

/// <summary>
/// 路口内部一条连接车道的角色：它来自哪个进口、执行什么转向。
///
/// 应用方案时扫描一次算好存下来，运行时直接查表，避免每帧重算几何关系。
/// </summary>
public struct LaneRole : IBufferElementData, ISerializable
{
    private ushort m_SchemaVersion;

    public Entity m_SubLane;

    /// <summary>进口序号，对应快照里 approaches 的下标。</summary>
    public byte m_Approach;

    public TurnKind m_Turn;

    /// <summary>是否人行横道。人行道单独处理，不参与车流转向掩码。</summary>
    public bool m_IsPedestrian;

    public LaneRole()
    {
        m_SchemaVersion = 1;
        m_SubLane = Entity.Null;
        m_Approach = 0;
        m_Turn = TurnKind.Straight;
        m_IsPedestrian = false;
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
        writer.Write(m_SchemaVersion);
        writer.Write(m_SubLane);
        writer.Write(m_Approach);
        writer.Write((byte)m_Turn);
        writer.Write(m_IsPedestrian);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
        reader.Read(out m_SchemaVersion);
        reader.Read(out m_SubLane);
        reader.Read(out m_Approach);
        reader.Read(out byte turn);
        reader.Read(out m_IsPedestrian);
        m_Turn = (TurnKind)turn;
    }
}

/// <summary>被接管路口的运行时状态。</summary>
public struct SignalRuntime : IComponentData, ISerializable
{
    private ushort m_SchemaVersion;

    /// <summary>当前相位在 SignalPhase buffer 里的下标。</summary>
    public int m_PhaseIndex;

    /// <summary>当前相位已经跑了多少秒。</summary>
    public float m_Elapsed;

    /// <summary>是否处于相位切换的黄灯过渡阶段。</summary>
    public bool m_InTransition;

    /// <summary>过渡已进行的秒数。</summary>
    public float m_TransitionElapsed;

    public SignalRuntime()
    {
        m_SchemaVersion = 1;
        m_PhaseIndex = 0;
        m_Elapsed = 0f;
        m_InTransition = false;
        m_TransitionElapsed = 0f;
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
        writer.Write(m_SchemaVersion);
        writer.Write(m_PhaseIndex);
        writer.Write(m_Elapsed);
        writer.Write(m_InTransition);
        writer.Write(m_TransitionElapsed);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
        reader.Read(out m_SchemaVersion);
        reader.Read(out m_PhaseIndex);
        reader.Read(out m_Elapsed);
        reader.Read(out m_InTransition);
        reader.Read(out m_TransitionElapsed);
    }
}
