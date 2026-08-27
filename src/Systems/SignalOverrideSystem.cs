using Game;
using Game.Net;
using Unity.Collections;
using Unity.Entities;
using UrbanBrain.Components;

namespace UrbanBrain.Systems;

/// <summary>
/// 接管模式。Freeze / Fast 是 M1 的验证模式，没有实用价值，留作调试探针；
/// Plan 才是正式的相位方案执行。
/// </summary>
public enum OverrideMode
{
    /// <summary>不接管，原版逻辑照常（对照组）。</summary>
    Off = 0,

    /// <summary>把计时器压回 0。若原版靠计时器累加切相位，效果应是信号冻结。</summary>
    Freeze = 1,

    /// <summary>把计时器顶到接近上限，效果应是相位飞快切换。</summary>
    Fast = 2,

    /// <summary>按 SignalPhase 方案执行。</summary>
    Plan = 3,
}

/// <summary>
/// 接管被打上 <see cref="ControlledSignal"/> 标记的路口信号。
///
/// 注册在原版 Game.Simulation.TrafficLightSystem 之前执行，
/// 且 Mod.OnLoad 已把原版查询改成排除这些实体，因此不会互相打架。
/// </summary>
public partial class SignalOverrideSystem : GameSystemBase
{
    /// <summary>
    /// 模拟帧到秒的换算。CS2 的模拟帧率与游戏内时间的关系待实机校准，
    /// 先按 60 帧一秒处理，方案时长偏差可以靠这个常数统一修正。
    /// </summary>
    private const float kFramesPerSecond = 60f;

    /// <summary>相位切换时的黄灯过渡时长（秒）。</summary>
    private const float kTransitionSeconds = 3f;

    private const int kLogInterval = 256;

    private EntityQuery m_SimpleQuery;
    private EntityQuery m_PlanQuery;

    private ComponentLookup<LaneSignal> m_LaneSignal;

    private int m_Tick;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_SimpleQuery = GetEntityQuery(
            ComponentType.ReadWrite<TrafficLights>(),
            ComponentType.ReadOnly<ControlledSignal>());

        m_PlanQuery = GetEntityQuery(
            ComponentType.ReadWrite<TrafficLights>(),
            ComponentType.ReadOnly<ControlledSignal>(),
            ComponentType.ReadWrite<SignalRuntime>(),
            ComponentType.ReadOnly<SignalPhase>(),
            ComponentType.ReadOnly<LaneRole>());

        m_LaneSignal = GetComponentLookup<LaneSignal>(false);

        RequireForUpdate(m_SimpleQuery);
    }

    protected override void OnUpdate()
    {
        var setting = Mod.setting;
        if (setting == null || setting.Mode == OverrideMode.Off)
        {
            return;
        }

        bool shouldLog = (m_Tick % kLogInterval) == 0;
        m_Tick++;

        if (setting.Mode == OverrideMode.Plan)
        {
            RunPlans(shouldLog);
        }
        else
        {
            RunProbe(setting.Mode, shouldLog);
        }
    }

    // ------------------------------------------------------------------
    // M1 探针模式
    // ------------------------------------------------------------------

    private void RunProbe(OverrideMode mode, bool shouldLog)
    {
        var entities = m_SimpleQuery.ToEntityArray(Allocator.Temp);
        if (entities.Length == 0)
        {
            entities.Dispose();
            return;
        }

        TrafficLights before = EntityManager.GetComponentData<TrafficLights>(entities[0]);

        for (int i = 0; i < entities.Length; i++)
        {
            TrafficLights tl = EntityManager.GetComponentData<TrafficLights>(entities[i]);
            tl.m_Timer = mode == OverrideMode.Freeze ? (byte)0 : (byte)200;
            EntityManager.SetComponentData(entities[i], tl);
        }

        if (shouldLog)
        {
            TrafficLights after = EntityManager.GetComponentData<TrafficLights>(entities[0]);
            Mod.log.Info(
                $"[探针] 模式={mode} 路口数={entities.Length} | " +
                $"样本 entity={entities[0].Index} " +
                $"state={(int)before.m_State}->{(int)after.m_State} " +
                $"timer={before.m_Timer}->{after.m_Timer} " +
                $"group={before.m_CurrentSignalGroup}/{before.m_SignalGroupCount}");
        }

        entities.Dispose();
    }

    // ------------------------------------------------------------------
    // 相位方案执行
    // ------------------------------------------------------------------

    private void RunPlans(bool shouldLog)
    {
        m_LaneSignal.Update(this);

        var entities = m_PlanQuery.ToEntityArray(Allocator.Temp);
        float dt = 1f / kFramesPerSecond;

        for (int i = 0; i < entities.Length; i++)
        {
            Entity node = entities[i];

            var phases = EntityManager.GetBuffer<SignalPhase>(node, true);
            if (phases.Length == 0)
            {
                continue;
            }

            var runtime = EntityManager.GetComponentData<SignalRuntime>(node);
            var roles = EntityManager.GetBuffer<LaneRole>(node, true);

            AdvanceClock(ref runtime, phases, dt);

            SignalPhase current = phases[runtime.m_PhaseIndex % phases.Length];
            ApplySignals(roles, current, runtime.m_InTransition);

            // 同步给原版组件，让灯的渲染和 UI 显示保持一致
            var tl = EntityManager.GetComponentData<TrafficLights>(node);
            tl.m_CurrentSignalGroup = (byte)(runtime.m_PhaseIndex % phases.Length + 1);
            tl.m_SignalGroupCount = (byte)phases.Length;
            tl.m_State = runtime.m_InTransition ? TrafficLightState.Changing : TrafficLightState.Ongoing;
            tl.m_Timer = (byte)Unity.Mathematics.math.min(255f, runtime.m_Elapsed);
            EntityManager.SetComponentData(node, tl);

            EntityManager.SetComponentData(node, runtime);

            if (shouldLog && i == 0)
            {
                Mod.log.Info(
                    $"[方案] 路口数={entities.Length} | 样本 entity={node.Index} " +
                    $"相位={runtime.m_PhaseIndex + 1}/{phases.Length} " +
                    $"已跑={runtime.m_Elapsed:F1}s/{current.m_TargetDuration:F1}s " +
                    $"过渡={runtime.m_InTransition} 车道角色数={roles.Length}");
            }
        }

        entities.Dispose();
    }

    /// <summary>推进相位计时，必要时进入过渡或切到下一相位。</summary>
    private static void AdvanceClock(ref SignalRuntime runtime, DynamicBuffer<SignalPhase> phases, float dt)
    {
        if (runtime.m_InTransition)
        {
            runtime.m_TransitionElapsed += dt;
            if (runtime.m_TransitionElapsed >= kTransitionSeconds)
            {
                runtime.m_InTransition = false;
                runtime.m_TransitionElapsed = 0f;
                runtime.m_Elapsed = 0f;
                runtime.m_PhaseIndex = (runtime.m_PhaseIndex + 1) % phases.Length;
            }
            return;
        }

        runtime.m_Elapsed += dt;

        SignalPhase current = phases[runtime.m_PhaseIndex % phases.Length];
        float target = Unity.Mathematics.math.clamp(
            current.m_TargetDuration, current.m_MinDuration, current.m_MaxDuration);

        if (runtime.m_Elapsed >= target)
        {
            runtime.m_InTransition = true;
            runtime.m_TransitionElapsed = 0f;
        }
    }

    /// <summary>按当前相位的放行掩码，逐条车道写入信号状态。</summary>
    private void ApplySignals(DynamicBuffer<LaneRole> roles, SignalPhase phase, bool inTransition)
    {
        for (int i = 0; i < roles.Length; i++)
        {
            LaneRole role = roles[i];
            if (!m_LaneSignal.HasComponent(role.m_SubLane))
            {
                continue;
            }

            LaneSignal signal = m_LaneSignal[role.m_SubLane];

            bool allowed = role.m_IsPedestrian
                ? PedestrianAllowed(phase, role.m_Approach)
                : phase.Allows(role.m_Approach, role.m_Turn);

            if (!allowed)
            {
                signal.m_Signal = LaneSignalType.Stop;
            }
            else if (inTransition)
            {
                // 过渡期让还在路口里的车走完，但不再放新车进来
                signal.m_Signal = LaneSignalType.SafeStop;
            }
            else
            {
                signal.m_Signal = LaneSignalType.Go;
            }

            m_LaneSignal[role.m_SubLane] = signal;
        }
    }

    /// <summary>
    /// 人行横道的放行规则：本进口的机动车全部禁行时才给行人绿灯。
    /// 这是保守做法，避免右转车和行人抢道。
    /// </summary>
    private static bool PedestrianAllowed(SignalPhase phase, int approach)
    {
        return !phase.Allows(approach, TurnKind.Left)
            && !phase.Allows(approach, TurnKind.Straight)
            && !phase.Allows(approach, TurnKind.Right)
            && !phase.Allows(approach, TurnKind.UTurn);
    }
}
