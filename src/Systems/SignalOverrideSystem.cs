using Game;
using Game.Net;
using Unity.Collections;
using Unity.Entities;

namespace UrbanBrain.Systems;

/// <summary>
/// M1 的接管模式。功能上都没有实用价值，纯粹是为了做出肉眼可见的效果，
/// 验证"我们确实能写入信号状态并被游戏采纳"。
/// </summary>
public enum OverrideMode
{
    /// <summary>不接管，原版逻辑照常（对照组）。</summary>
    Off = 0,

    /// <summary>每帧把计时器压回 0。若原版靠计时器累加来切换相位，效果应是信号冻结不变。</summary>
    Freeze = 1,

    /// <summary>每帧把计时器顶到接近上限。效果应是相位飞快切换。</summary>
    Fast = 2,
}

/// <summary>
/// 接管被打上 <see cref="Components.ControlledSignal"/> 标记的路口信号。
///
/// 本 System 注册在原版 Game.Simulation.TrafficLightSystem **之前**执行；
/// 同时 Mod.OnLoad 已经把原版的查询条件改成排除这些实体，因此不会互相打架。
/// </summary>
public partial class SignalOverrideSystem : GameSystemBase
{
    private EntityQuery m_Query;

    /// <summary>采样计数器，用于降低日志频率 —— 否则每帧一条会把日志刷爆。</summary>
    private int m_Tick;

    /// <summary>日志采样间隔（帧）。</summary>
    private const int kLogInterval = 256;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_Query = GetEntityQuery(
            ComponentType.ReadWrite<TrafficLights>(),
            ComponentType.ReadOnly<Components.ControlledSignal>());
        RequireForUpdate(m_Query);
    }

    protected override void OnUpdate()
    {
        var setting = Mod.setting;
        if (setting == null || setting.Mode == OverrideMode.Off)
        {
            return;
        }

        var entities = m_Query.ToEntityArray(Allocator.Temp);
        if (entities.Length == 0)
        {
            entities.Dispose();
            return;
        }

        bool shouldLog = (m_Tick % kLogInterval) == 0;
        m_Tick++;

        // 采样第一个路口在"我们动手之前"的状态，用于和动手之后对比。
        TrafficLights before = EntityManager.GetComponentData<TrafficLights>(entities[0]);

        for (int i = 0; i < entities.Length; i++)
        {
            TrafficLights tl = EntityManager.GetComponentData<TrafficLights>(entities[i]);

            switch (setting.Mode)
            {
                case OverrideMode.Freeze:
                    tl.m_Timer = 0;
                    break;

                case OverrideMode.Fast:
                    // m_Timer 是 byte，顶到高位期望触发相位切换
                    tl.m_Timer = 200;
                    break;
            }

            EntityManager.SetComponentData(entities[i], tl);
        }

        if (shouldLog)
        {
            TrafficLights after = EntityManager.GetComponentData<TrafficLights>(entities[0]);
            Mod.log.Info(
                $"[接管中] 模式={setting.Mode} 路口数={entities.Length} | " +
                $"样本 entity={entities[0].Index} " +
                $"state={(int)before.m_State}->{(int)after.m_State} " +
                $"timer={before.m_Timer}->{after.m_Timer} " +
                $"group={before.m_CurrentSignalGroup}/{before.m_SignalGroupCount} " +
                $"next={before.m_NextSignalGroup}");
        }

        entities.Dispose();
    }
}
