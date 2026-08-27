using System.Collections.Concurrent;
using System.Collections.Generic;
using Game;
using Unity.Collections;
using Unity.Entities;
using UrbanBrain.Advisor;
using UrbanBrain.Components;
using UrbanBrain.Model;

namespace UrbanBrain.Systems;

/// <summary>
/// 方案的生成与应用。
///
/// AI 请求走后台线程，结果丢进 <see cref="m_Pending"/> 队列，
/// 由 OnUpdate 在主线程取出应用 —— ECS 的 EntityManager 只能在主线程碰。
/// </summary>
public partial class AdvisorSystem : GameSystemBase
{
    private IntersectionScanSystem m_ScanSystem;

    /// <summary>后台线程产出的待应用方案。</summary>
    private readonly ConcurrentQueue<PendingPlan> m_Pending = new();

    /// <summary>最近一次生成的方案，供 UI 展示。</summary>
    public SignalPlanProposal m_LastRulePlan;
    public SignalPlanProposal m_LastAiPlan;

    /// <summary>是否有 AI 请求正在飞行中，避免重复点击。</summary>
    public bool m_AiBusy;

    private struct PendingPlan
    {
        public Entity m_Node;
        public SignalPlanProposal m_Plan;
        public bool m_ApplyImmediately;
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ScanSystem = World.GetOrCreateSystemManaged<IntersectionScanSystem>();
    }

    protected override void OnUpdate()
    {
        while (m_Pending.TryDequeue(out PendingPlan pending))
        {
            if (pending.m_Plan == null)
            {
                continue;
            }

            m_LastAiPlan = pending.m_Plan;

            if (pending.m_ApplyImmediately && EntityManager.Exists(pending.m_Node))
            {
                ApplyPlan(pending.m_Node, pending.m_Plan);
            }
        }
    }

    // ------------------------------------------------------------------
    // 生成
    // ------------------------------------------------------------------

    /// <summary>用规则引擎生成方案（同步，本地计算）。</summary>
    public SignalPlanProposal ProposeByRule(Entity node)
    {
        IntersectionSnapshot snap = m_ScanSystem.ScanOne(node);
        SignalPlanProposal plan = RuleAdvisor.Propose(snap);
        m_LastRulePlan = plan;

        Mod.log.Info($"规则方案：路口 {node.Index}，{plan.phases.Count} 个相位，" +
                     $"周期 {plan.cycleLength:F0} 秒。{plan.rationale}");
        foreach (string w in plan.warnings)
        {
            Mod.log.Warn($"⚠ {w}");
        }

        return plan;
    }

    /// <summary>
    /// 请求 AI 方案。请求在后台跑，方法立刻返回。
    /// 结果到达后由 OnUpdate 应用。
    /// </summary>
    public void RequestAiPlan(Entity node, bool applyImmediately)
    {
        if (m_AiBusy)
        {
            Mod.log.Warn("⚠ 上一个 AI 请求还没回来，先等等。");
            return;
        }

        var setting = Mod.setting;
        if (setting == null || string.IsNullOrEmpty(setting.ApiKey))
        {
            Mod.log.Warn("⚠ 没有配置 API key，请在设置里填入后再试。");
            return;
        }

        // 快照和基准方案都要在主线程算好，后台线程不能碰 EntityManager
        IntersectionSnapshot snap = m_ScanSystem.ScanOne(node);
        SignalPlanProposal baseline = RuleAdvisor.Propose(snap);
        m_LastRulePlan = baseline;

        string apiKey = setting.ApiKey;
        string model = setting.Model;

        m_AiBusy = true;
        Mod.log.Info($"正在请求 AI 方案：路口 {node.Index}…");

        System.Threading.Tasks.Task.Run(async () =>
        {
            SignalPlanProposal plan = null;
            try
            {
                plan = await AiAdvisor.ProposeAsync(snap, apiKey, model, baseline);
            }
            catch (System.Exception e)
            {
                Mod.log.Warn($"⚠ AI 请求后台任务异常：{e}");
            }
            finally
            {
                m_AiBusy = false;
            }

            if (plan != null)
            {
                m_Pending.Enqueue(new PendingPlan
                {
                    m_Node = node,
                    m_Plan = plan,
                    m_ApplyImmediately = applyImmediately,
                });
            }
        });
    }

    // ------------------------------------------------------------------
    // 应用
    // ------------------------------------------------------------------

    /// <summary>
    /// 把方案落到路口：写入车道角色表、相位表，并接管信号。
    /// </summary>
    public bool ApplyPlan(Entity node, SignalPlanProposal plan)
    {
        if (plan == null || plan.phases == null || plan.phases.Count == 0)
        {
            Mod.log.Warn("⚠ 方案是空的，不做应用。");
            return false;
        }

        List<LaneRole> roles = m_ScanSystem.ScanRoles(node);
        if (roles.Count == 0)
        {
            Mod.log.Warn($"⚠ 路口 {node.Index} 没扫描到任何车道角色，无法应用方案。" +
                         "可能是路口结构特殊，或者 PathNode 配对逻辑没覆盖到。");
            return false;
        }

        // 车道角色表
        DynamicBuffer<LaneRole> roleBuffer = EntityManager.HasBuffer<LaneRole>(node)
            ? EntityManager.GetBuffer<LaneRole>(node)
            : EntityManager.AddBuffer<LaneRole>(node);
        roleBuffer.Clear();
        foreach (LaneRole r in roles)
        {
            roleBuffer.Add(r);
        }

        // 相位表
        DynamicBuffer<SignalPhase> phaseBuffer = EntityManager.HasBuffer<SignalPhase>(node)
            ? EntityManager.GetBuffer<SignalPhase>(node)
            : EntityManager.AddBuffer<SignalPhase>(node);
        phaseBuffer.Clear();

        foreach (PhaseProposal p in plan.phases)
        {
            var phase = new SignalPhase
            {
                m_MinDuration = (ushort)System.Math.Max(1, p.minDuration),
                m_MaxDuration = (ushort)System.Math.Max(p.minDuration, p.maxDuration),
                m_TargetDuration = p.targetDuration,
            };

            foreach (MovementRef m in p.movements ?? new List<MovementRef>())
            {
                phase.Allow(m.approach, ParseTurn(m.turn));
            }

            phaseBuffer.Add(phase);
        }

        if (!EntityManager.HasComponent<SignalRuntime>(node))
        {
            EntityManager.AddComponent<SignalRuntime>(node);
        }
        EntityManager.SetComponentData(node, new SignalRuntime());

        if (!EntityManager.HasComponent<ControlledSignal>(node))
        {
            EntityManager.AddComponent<ControlledSignal>(node);
        }

        Mod.setting.ModeValue = (int)OverrideMode.Plan;

        Mod.log.Info($"方案已应用：路口 {node.Index}，来源={plan.source}，" +
                     $"{phaseBuffer.Length} 个相位，{roleBuffer.Length} 条车道角色，" +
                     $"周期 {plan.cycleLength:F0} 秒");
        return true;
    }

    /// <summary>对全城所有信号路口应用规则方案。AI 方案不做批量，避免话费失控。</summary>
    public int ApplyRulePlanToAll()
    {
        var query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Game.Net.TrafficLights>());
        var entities = query.ToEntityArray(Allocator.Temp);

        int ok = 0;
        for (int i = 0; i < entities.Length; i++)
        {
            SignalPlanProposal plan = RuleAdvisor.Propose(m_ScanSystem.ScanOne(entities[i]));
            if (ApplyPlan(entities[i], plan))
            {
                ok++;
            }
        }

        entities.Dispose();
        Mod.log.Info($"批量应用规则方案完成：{ok}/{entities.Length} 个路口成功");
        return ok;
    }

    private static TurnKind ParseTurn(string s)
    {
        return s switch
        {
            "left" => TurnKind.Left,
            "right" => TurnKind.Right,
            "uturn" => TurnKind.UTurn,
            _ => TurnKind.Straight,
        };
    }
}
