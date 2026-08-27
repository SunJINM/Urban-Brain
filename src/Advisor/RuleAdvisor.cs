using System;
using System.Collections.Generic;
using System.Linq;
using UrbanBrain.Model;

namespace UrbanBrain.Advisor;

/// <summary>
/// 规则版配时方案生成器 —— AI 的对照组。
///
/// 用的是交通工程里的常规做法：对向进口配成一轴，按需求比例分配绿灯（Webster 思路的简化版），
/// 左转需求大时再拆出专用左转相位。
///
/// 存在意义不是「够用就不接 AI 了」，而是给一期的核心问题提供基准线：
/// 如果 AI 给出的方案并不优于这里的输出，那 AI 这一层就没有带来增量。
/// </summary>
public static class RuleAdvisor
{
    /// <summary>周期时长的上下限（秒）。</summary>
    private const float kMinCycle = 40f;
    private const float kMaxCycle = 120f;

    /// <summary>单相位绿灯下限，太短会导致车流来不及启动。</summary>
    private const int kMinGreen = 8;

    /// <summary>左转需求达到该进口总需求的这个比例时，拆出专用左转相位。</summary>
    private const float kProtectedLeftThreshold = 0.25f;

    public static SignalPlanProposal Propose(IntersectionSnapshot snap)
    {
        var plan = new SignalPlanProposal
        {
            intersectionId = snap.id,
            source = "rule",
        };

        var approaches = snap.approaches;
        if (approaches == null || approaches.Count == 0)
        {
            plan.warnings.Add("该路口没有扫描到任何进口，无法生成方案。");
            plan.rationale = "数据不足。";
            return plan;
        }

        bool flowAvailable = approaches.Any(a => a.flow != null && a.flow.congestion >= 0f);
        if (!flowAvailable)
        {
            plan.warnings.Add("没有采集到流量数据，退化为按车道数分配绿灯。让城市多跑一会儿再重新扫描会更准。");
        }

        // ---- 1. 对向进口配成轴 ----
        List<List<int>> axes = PairApproaches(approaches);

        // ---- 2. 逐轴生成相位 ----
        var phases = new List<PhaseProposal>();
        foreach (List<int> axis in axes)
        {
            float leftDemand = axis.Sum(i => Demand(approaches[i], "left"));
            float totalDemand = axis.Sum(i => Demand(approaches[i], "all"));
            bool protectedLeft = totalDemand > 0.001f && (leftDemand / totalDemand) >= kProtectedLeftThreshold;

            string axisName = string.Join("/", axis.Select(i => approaches[i].direction));

            if (protectedLeft)
            {
                // 直行 + 右转
                var through = new PhaseProposal
                {
                    name = $"{axisName} 直行",
                    reason = "左转需求较大，与直行分开放行，避免左转车堵住直行车道。",
                };
                foreach (int i in axis)
                {
                    through.movements.Add(new MovementRef(i, approaches[i].direction, "straight"));
                    through.movements.Add(new MovementRef(i, approaches[i].direction, "right"));
                }
                phases.Add(through);

                // 专用左转
                var left = new PhaseProposal
                {
                    name = $"{axisName} 左转",
                    reason = $"左转占本轴需求的 {leftDemand / Math.Max(totalDemand, 0.001f) * 100:F0}%，给专用相位。",
                };
                foreach (int i in axis)
                {
                    left.movements.Add(new MovementRef(i, approaches[i].direction, "left"));
                    left.movements.Add(new MovementRef(i, approaches[i].direction, "uturn"));
                }
                phases.Add(left);
            }
            else
            {
                var all = new PhaseProposal
                {
                    name = $"{axisName} 全向",
                    reason = "左转需求不大，与直行合并放行以缩短周期。",
                };
                foreach (int i in axis)
                {
                    all.movements.Add(new MovementRef(i, approaches[i].direction, "left"));
                    all.movements.Add(new MovementRef(i, approaches[i].direction, "straight"));
                    all.movements.Add(new MovementRef(i, approaches[i].direction, "right"));
                    all.movements.Add(new MovementRef(i, approaches[i].direction, "uturn"));
                }
                phases.Add(all);
            }
        }

        if (phases.Count == 0)
        {
            plan.warnings.Add("未能划分出任何相位。");
            plan.rationale = "数据不足。";
            return plan;
        }

        // ---- 3. 按需求比例分配绿灯 ----
        AllocateGreenTime(phases, approaches);

        plan.phases = phases;
        plan.cycleLength = phases.Sum(p => p.targetDuration);
        plan.rationale = BuildRationale(snap, phases, flowAvailable);

        return plan;
    }

    // ------------------------------------------------------------------

    /// <summary>
    /// 把进口两两配成对向轴。
    /// 贪心：每次取还没分配的进口，找方位角最接近它对面（相差 180 度）的伙伴。
    /// 找不到合适伙伴的进口（比如 T 型路口的那一支）自成一轴。
    /// </summary>
    private static List<List<int>> PairApproaches(List<ApproachSnapshot> approaches)
    {
        var axes = new List<List<int>>();
        var used = new bool[approaches.Count];

        for (int i = 0; i < approaches.Count; i++)
        {
            if (used[i])
            {
                continue;
            }

            int best = -1;
            float bestDiff = float.MaxValue;

            for (int j = i + 1; j < approaches.Count; j++)
            {
                if (used[j])
                {
                    continue;
                }
                float diff = Math.Abs(AngleDelta(approaches[i].bearing, approaches[j].bearing) - 180f);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    best = j;
                }
            }

            var axis = new List<int> { i };
            used[i] = true;

            // 偏离对向超过 45 度就不算一轴，宁可单独放行
            if (best >= 0 && bestDiff <= 45f)
            {
                axis.Add(best);
                used[best] = true;
            }

            axes.Add(axis);
        }

        return axes;
    }

    /// <summary>两个方位角之间的夹角，0~180。</summary>
    private static float AngleDelta(float a, float b)
    {
        float d = Math.Abs(a - b) % 360f;
        return d > 180f ? 360f - d : d;
    }

    /// <summary>
    /// 某个进口的需求强度。
    /// 有流量数据时用「车道数 × (1 + 拥堵度)」，拥堵会放大需求；
    /// 没有流量数据时退化成纯车道数。
    /// </summary>
    private static float Demand(ApproachSnapshot a, string turn)
    {
        int lanes = turn switch
        {
            "left" => a.laneLeft + a.laneUTurn,
            "straight" => a.laneStraight,
            "right" => a.laneRight,
            _ => a.laneLeft + a.laneStraight + a.laneRight + a.laneUTurn,
        };

        float congestionFactor = 1f;
        if (a.flow != null && a.flow.congestion >= 0f)
        {
            congestionFactor = 1f + a.flow.congestion;
        }

        return lanes * congestionFactor;
    }

    /// <summary>按各相位的需求占比切分周期。</summary>
    private static void AllocateGreenTime(List<PhaseProposal> phases, List<ApproachSnapshot> approaches)
    {
        var weights = new float[phases.Count];
        float total = 0f;

        for (int p = 0; p < phases.Count; p++)
        {
            float w = 0f;
            foreach (MovementRef m in phases[p].movements)
            {
                if (m.approach >= 0 && m.approach < approaches.Count)
                {
                    w += Demand(approaches[m.approach], m.turn);
                }
            }
            weights[p] = Math.Max(w, 0.1f);
            total += weights[p];
        }

        // 相位越多，周期越长，但夹在上下限之间
        float cycle = Math.Min(kMaxCycle, Math.Max(kMinCycle, phases.Count * 25f));

        for (int p = 0; p < phases.Count; p++)
        {
            float share = weights[p] / total;
            int green = (int)Math.Round(cycle * share);
            green = Math.Max(kMinGreen, green);

            phases[p].targetDuration = green;
            phases[p].minDuration = kMinGreen;
            phases[p].maxDuration = Math.Max(green * 2, 30);
        }
    }

    private static string BuildRationale(IntersectionSnapshot snap, List<PhaseProposal> phases, bool flowAvailable)
    {
        string congestion = snap.worstCongestion >= 0f
            ? $"最堵进口的拥堵度 {snap.worstCongestion * 100:F0}%"
            : "暂无拥堵数据";

        string basis = flowAvailable ? "按实测流量与拥堵度" : "按车道数（无流量数据）";

        return $"{snap.approaches.Count} 个进口，{congestion}。{basis}划分为 {phases.Count} 个相位，" +
               $"周期 {phases.Sum(p => p.targetDuration):F0} 秒。";
    }
}
