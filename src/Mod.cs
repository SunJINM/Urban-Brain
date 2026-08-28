using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace UrbanBrain;

/// <summary>
/// Urban Brain 模组入口。
///
/// 本体不实现任何交通逻辑，只负责按正确顺序把两个功能模块拉起来：
///
///   Traffic.Mod                         车道连接、路口优先级标志
///   C2VM.TrafficLightsEnhancement.Mod   信号相位与配时
///
/// 这两个模块分别源自 Traffic 与 Traffic Lights Enhancement，
/// 并入时都去掉了各自的 IMod 实现 —— 一个程序集只能有一个模组入口。
///
/// 它们接管的是不同的游戏系统，互不冲突：
///   Traffic 禁用并替换 Game.Net.LaneSystem
///   TLE     改写 Game.Simulation.TrafficLightSystem 的查询条件
/// </summary>
public class Mod : IMod
{
    /// <summary>
    /// 模组版本。被两个子模块的设置页引用，改动时注意它们的显示。
    /// </summary>
    public const string kVersion = "0.2.0";

    public static readonly string kId = typeof(Mod).Assembly.GetName().Name;

    public static ILog log = LogManager.GetLogger(kId).SetShowsErrorsInUI(false);

    public static World world;

    private Traffic.Mod m_LaneModule;

    private C2VM.TrafficLightsEnhancement.Mod m_SignalModule;

    public void OnLoad(UpdateSystem updateSystem)
    {
        log.Info($"========== Urban Brain v{kVersion} 开始加载 ==========");

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
        {
            log.Info($"模组文件位置：{asset.path}");
        }

        world = updateSystem.World;

        // 顺序说明：车道模块要禁用原版 LaneSystem 并插入替代实现，
        // 信号模块只改查询条件，两者无依赖关系，但先车道后信号更贴近原作者的加载假设。
        try
        {
            m_LaneModule = new Traffic.Mod();
            m_LaneModule.OnLoad(updateSystem, this);
        }
        catch (System.Exception e)
        {
            log.Error($"⚠ 车道模块加载失败，车道连接与优先级功能不可用：{e}");
            log.Error("⚠ 这通常意味着游戏更新后 LaneSystem 结构变了，需要重新核对 TrafficLaneSystem。");
        }

        try
        {
            m_SignalModule = new C2VM.TrafficLightsEnhancement.Mod();
            m_SignalModule.OnLoad(updateSystem, this);
        }
        catch (System.Exception e)
        {
            log.Error($"⚠ 信号模块加载失败，自定义相位与配时不可用：{e}");
            log.Error("⚠ 若日志里有「找不到字段 m_TrafficLightQuery」，说明游戏更新后字段改名了。");
        }

        log.Info("========== Urban Brain 加载完成 ==========");
    }

    public void OnDispose()
    {
        log.Info("Urban Brain 卸载");
        m_SignalModule?.OnDispose();
        m_LaneModule?.OnDispose();
        m_SignalModule = null;
        m_LaneModule = null;
    }
}
