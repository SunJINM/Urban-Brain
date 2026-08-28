using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace C2VM.TrafficLightsEnhancement;

/// <summary>
/// 信号相位控制模块（源自 Traffic Lights Enhancement，已并入 Urban Brain）。
///
/// 与原版的区别：
/// 1. 不再实现 IMod —— 整个程序集只有 UrbanBrain.Mod 一个模组入口。
/// 2. 去掉了 BepInEx 旧版本检测。
/// 3. 去掉了对 C2VM.CommonLibraries.LaneSystem 的依赖 ——
///    车道转向控制改用 Traffic 的车道连接器实现，不再需要那个模块。
/// 4. m_Id 由"读程序集名"改为固定字符串，避免并入后日志名变化。
///
/// 接管原理：把原版信号系统的查询条件改成"排除带 CustomTrafficLights 标记的路口"，
/// 再把自己的系统插在原版之前。没有标记的路口仍由原版接管，两者互不干扰。
/// </summary>
public class Mod
{
    public const string m_Id = "C2VM.TrafficLightsEnhancement";

    public static readonly string m_InformationalVersion = UrbanBrain.Mod.kVersion;

    public static readonly ILog m_Log = LogManager.GetLogger($"{m_Id}").SetShowsErrorsInUI(false);

    public static C2VM.TrafficLightsEnhancement.Settings m_Settings;

    public static World m_World;

    private static Game.Net.TrafficLightInitializationSystem m_TrafficLightInitializationSystem;

    private static Game.Simulation.TrafficLightSystem m_TrafficLightSystem;

    private static Systems.TrafficLightSystems.Initialisation.PatchedTrafficLightInitializationSystem m_PatchedTrafficLightInitializationSystem;

    private static Systems.TrafficLightSystems.Simulation.PatchedTrafficLightSystem m_PatchedTrafficLightSystem;

    /// <param name="owner">真正注册到游戏里的模组入口，用于挂设置页。</param>
    public void OnLoad(UpdateSystem updateSystem, IMod owner)
    {
        m_Log.Info($"信号模块加载中 v{m_InformationalVersion}");

        m_World = updateSystem.World;

        m_TrafficLightInitializationSystem = m_World.GetOrCreateSystemManaged<Game.Net.TrafficLightInitializationSystem>();
        m_TrafficLightSystem = m_World.GetOrCreateSystemManaged<Game.Simulation.TrafficLightSystem>();
        m_PatchedTrafficLightInitializationSystem = m_World.GetOrCreateSystemManaged<Systems.TrafficLightSystems.Initialisation.PatchedTrafficLightInitializationSystem>();
        m_PatchedTrafficLightSystem = m_World.GetOrCreateSystemManaged<Systems.TrafficLightSystems.Simulation.PatchedTrafficLightSystem>();

        m_Settings = new Settings(owner);

        SystemSetup(updateSystem);

        string netToolSystemToolID = m_World.GetOrCreateSystemManaged<Game.Tools.NetToolSystem>().toolID;
        Assert(netToolSystemToolID == "Net Tool", $"netToolSystemToolID: {netToolSystemToolID}");

        m_Log.Info("信号模块加载完成");
    }

    public void OnDispose()
    {
        m_Log.Info(nameof(OnDispose));
    }

    public void SystemSetup(UpdateSystem updateSystem)
    {
        m_World.GetOrCreateSystemManaged<Game.Tools.NetToolSystem>(); // 确保 NetToolSystem 先于我们的工具创建

        var noneList = new NativeList<ComponentType>(1, Allocator.Temp);
        noneList.Add(ComponentType.ReadOnly<Components.CustomTrafficLights>());

        Utils.EntityQueryUtils.UpdateEntityQuery(m_TrafficLightInitializationSystem, "m_TrafficLightsQuery", noneList);
        Utils.EntityQueryUtils.UpdateEntityQuery(m_TrafficLightSystem, "m_TrafficLightQuery", noneList);

        updateSystem.UpdateBefore<Systems.TrafficLightSystems.Initialisation.PatchedTrafficLightInitializationSystem, Game.Net.TrafficLightInitializationSystem>(SystemUpdatePhase.Modification4B);
        updateSystem.UpdateBefore<Systems.TrafficLightSystems.Simulation.PatchedTrafficLightSystem, Game.Simulation.TrafficLightSystem>(SystemUpdatePhase.GameSimulation);
        updateSystem.UpdateAt<Systems.UI.TooltipSystem>(SystemUpdatePhase.UITooltip);
        updateSystem.UpdateAt<Systems.UI.UISystem>(SystemUpdatePhase.UIUpdate);
        updateSystem.UpdateAt<Systems.Tool.ToolSystem>(SystemUpdatePhase.ToolUpdate);
        updateSystem.UpdateAt<Systems.Update.ModificationUpdateSystem>(SystemUpdatePhase.ModificationEnd);
        updateSystem.UpdateAfter<Systems.Update.SimulationUpdateSystem>(SystemUpdatePhase.GameSimulation);

        SetCompatibilityMode(m_Settings != null && m_Settings.m_CompatibilityMode);
    }

    /// <summary>
    /// 兼容模式：开启后不接管原版信号，只做叠加显示。
    /// 原本是为了和其他同类 mod 共存，本项目保留该开关用于排障 ——
    /// 怀疑信号异常由本模块引起时，可以打开它对比。
    /// </summary>
    public static void SetCompatibilityMode(bool enable)
    {
        m_TrafficLightInitializationSystem.Enabled = enable;
        m_TrafficLightSystem.Enabled = enable;

        m_PatchedTrafficLightInitializationSystem.SetCompatibilityMode(enable);
        m_PatchedTrafficLightSystem.SetCompatibilityMode(enable);

        m_Log.Info($"兼容模式：{enable}");
    }

    public static bool IsCanary()
    {
        #if SHOW_CANARY_BUILD_WARNING
        return true;
        #else
        return false;
        #endif
    }

    public static void Assert(bool condition, string message = "", bool showInUI = false, [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(condition))] string expression = "")
    {
        if (condition == true)
        {
            return;
        }
        bool showsErrorsInUI = m_Log.showsErrorsInUI;
        m_Log.SetShowsErrorsInUI(showInUI);
        m_Log.Error($"⚠ 断言失败！\n{message}\n表达式: {expression}");
        m_Log.SetShowsErrorsInUI(showsErrorsInUI);
    }
}
