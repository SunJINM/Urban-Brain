using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace UrbanBrain;

public class Mod : IMod
{
    public static readonly string kId = typeof(Mod).Assembly.GetName().Name;

    public static ILog log = LogManager.GetLogger(kId).SetShowsErrorsInUI(false);

    public static Setting setting;

    public static World world;

    /// <summary>
    /// 反射改写原版查询是否成功。失败时接管按钮会被禁用 ——
    /// 因为此时我们和原版会同时写同一个组件，结果不可预测，宁可不接管。
    /// </summary>
    public static bool takeoverAvailable;

    /// <summary>原版信号 System 里持有路口查询的私有字段名（来自反编译，游戏更新可能改动）。</summary>
    private const string kVanillaQueryField = "m_TrafficLightQuery";

    public void OnLoad(UpdateSystem updateSystem)
    {
        log.Info("========== Urban Brain 开始加载 ==========");

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
        {
            log.Info($"模组文件位置：{asset.path}");
        }

        world = updateSystem.World;

        setting = new Setting(this);
        setting.RegisterInOptionsUI();
        AssetDatabase.global.LoadSettings(kId, setting, new Setting(this));

        var locale = new Locale(setting);
        GameManager.instance.localizationManager.AddSource("zh-HANS", locale);
        GameManager.instance.localizationManager.AddSource("en-US", locale);

        TryDetachVanillaSystem();

        // 我们的接管逻辑必须跑在原版之前
        updateSystem.UpdateBefore<Systems.SignalOverrideSystem, Game.Simulation.TrafficLightSystem>(
            SystemUpdatePhase.GameSimulation);

        log.Info($"========== Urban Brain 加载完成（接管能力：{(takeoverAvailable ? "可用" : "不可用")}）==========");
    }

    /// <summary>
    /// 把原版 TrafficLightSystem 的查询条件改成"排除带 ControlledSignal 标记的路口"，
    /// 这样被我们接管的路口原版就不管了。
    /// </summary>
    private void TryDetachVanillaSystem()
    {
        try
        {
            var vanilla = world.GetOrCreateSystemManaged<Game.Simulation.TrafficLightSystem>();

            var none = new NativeList<ComponentType>(1, Allocator.Temp);
            none.Add(ComponentType.ReadOnly<Components.ControlledSignal>());

            takeoverAvailable = Utils.EntityQueryUtils.TryUpdateEntityQuery(vanilla, kVanillaQueryField, none);
            none.Dispose();

            if (takeoverAvailable)
            {
                log.Info($"已改写原版 TrafficLightSystem.{kVanillaQueryField}，接管通道就绪。");
            }
            else
            {
                log.Warn($"⚠ 在 Game.Simulation.TrafficLightSystem 上找不到字段 {kVanillaQueryField}。");
                log.Warn("⚠ 这说明游戏更新后字段改名了。接管功能已禁用（否则会和原版抢同一份数据）。");
                log.Warn("⚠ 请把这几行日志发回，需要重新确认字段名。");
            }
        }
        catch (System.Exception e)
        {
            takeoverAvailable = false;
            log.Warn($"⚠ 改写原版查询时抛异常，接管功能已禁用：{e}");
        }
    }

    public void OnDispose()
    {
        log.Info("Urban Brain 卸载");
        if (setting != null)
        {
            setting.UnregisterInOptionsUI();
            setting = null;
        }
    }
}
