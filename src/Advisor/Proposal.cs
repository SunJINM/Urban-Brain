using System.Collections.Generic;

namespace UrbanBrain.Advisor;

/// <summary>
/// 一个路口的配时方案。
///
/// 规则引擎和 AI 产出同一种结构，这样两者才能公平对照 —— 一期的核心问题
/// 「AI 到底比规则强在哪」只有在输出可比时才回答得了。
///
/// 这个结构同时是给 LLM 的输出格式约定，字段名即 prompt 的一部分。
/// </summary>
public class SignalPlanProposal
{
    public int intersectionId;

    /// <summary>rule 或 ai。</summary>
    public string source;

    /// <summary>整体方案的理由，一两句话。</summary>
    public string rationale;

    /// <summary>方案的总周期（秒），由各相位时长加总得出。</summary>
    public float cycleLength;

    public List<PhaseProposal> phases = new();

    /// <summary>生成过程中的告警，例如数据不足。</summary>
    public List<string> warnings = new();
}

public class PhaseProposal
{
    /// <summary>人类可读的相位名，例如「南北直行」。</summary>
    public string name;

    /// <summary>本相位放行的流向。</summary>
    public List<MovementRef> movements = new();

    public int minDuration = 5;
    public int maxDuration = 60;
    public float targetDuration = 20f;

    /// <summary>这个相位为什么这么设计、时长为什么是这个值。</summary>
    public string reason;
}

/// <summary>一个流向：某个进口的某种转向。</summary>
public class MovementRef
{
    /// <summary>进口序号，对应快照 approaches 的下标。</summary>
    public int approach;

    /// <summary>方位名，仅供人和模型阅读，程序以 approach 为准。</summary>
    public string direction;

    /// <summary>left / straight / right / uturn。</summary>
    public string turn;

    public MovementRef()
    {
    }

    public MovementRef(int approach, string direction, string turn)
    {
        this.approach = approach;
        this.direction = direction;
        this.turn = turn;
    }
}
