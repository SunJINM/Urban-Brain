namespace Traffic
{
    using System;
    using System.Reflection;
    using Game;
    using Game.Modding;
    using Game.Net;
    using Game.Rendering;
    using Game.Serialization;
    using Traffic.Rendering;
    using Traffic.Systems;
    using Traffic.Systems.ModCompatibility;
    using Traffic.Systems.PrioritySigns;
    using Traffic.Tools;
    using Traffic.UISystems;
    using Unity.Entities;
    using ApplyLaneConnectionsSystem = Traffic.Systems.LaneConnections.ApplyLaneConnectionsSystem;
    using GenerateConnectorsSystem = Traffic.Systems.LaneConnections.GenerateConnectorsSystem;
    using GenerateLaneConnectionsSystem = Traffic.Systems.LaneConnections.GenerateLaneConnectionsSystem;
    using SearchSystem = Traffic.Systems.LaneConnections.SearchSystem;
    using SyncCustomLaneConnectionsSystem = Traffic.Systems.LaneConnections.SyncCustomLaneConnectionsSystem;

    /// <summary>
    /// 车道连接 / 优先级标志模块（源自 Traffic mod，已并入 Urban Brain）。
    ///
    /// 与原版的区别：
    /// 1. 不再实现 IMod —— 整个程序集只有 UrbanBrain.Mod 一个模组入口，
    ///    这里由它在 OnLoad 时创建并驱动。
    /// 2. 去掉了对外部 TLE 的检测与兼容修复 —— 信号功能已由本项目自己集成，
    ///    不存在"另一个 mod 抢 LaneSystem"的场景。
    /// 3. MOD_NAME 保持 "Traffic" 不变 —— 93 条中文翻译的 key 里嵌了这个名字，
    ///    改名会让整份翻译失配。显示名通过翻译文件覆盖，不动 key。
    /// </summary>
    public class Mod
    {
        public const string MOD_NAME = "Traffic";

        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(4);

        public static string InformationalVersion =>
            Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? Version;

        /// <summary>RoadBuilder 是独立模组，装了的话需要调整系统顺序。</summary>
        public static bool IsRBEnabled =>
            _isRBEnabled ??= System.Linq.Enumerable.Any(
                Game.SceneFlow.GameManager.instance.modManager.ListModsEnabled(),
                x => x.StartsWith("RoadBuilder, Version"));

        private static bool? _isRBEnabled;

        internal ModSettings Settings { get; private set; }

        /// <param name="owner">真正注册到游戏里的模组入口，用于取资源路径和挂设置页。</param>
        public void OnLoad(UpdateSystem updateSystem, IMod owner)
        {
            Logger.Info($"车道模块加载中，version: {InformationalVersion}");

            Settings = new ModSettings(owner, false);
            Settings.RegisterKeyBindings();
            Settings.RegisterInOptionsUI();
            Colossal.IO.AssetDatabase.AssetDatabase.global.LoadSettings(
                ModSettings.SETTINGS_ASSET_NAME, Settings, new ModSettings(owner, true));

            if (!Game.SceneFlow.GameManager.instance.localizationManager.activeDictionary.ContainsID(Settings.GetSettingsLocaleID()))
            {
                var source = new Localization.LocaleEN(Settings);
                Game.SceneFlow.GameManager.instance.localizationManager.AddSource("en-US", source);
                Localization.LoadLocales(owner, source.ReadEntries(null, null).Count());
            }
            Settings.ApplyLoadedSettings();

            updateSystem.UpdateAt<ModUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateBefore<PreDeserialize<ModUISystem>>(SystemUpdatePhase.Deserialize);

            updateSystem.UpdateAfter<ToolOverlaySystem, AreaRenderSystem>(SystemUpdatePhase.Rendering);

            // 车道连接的实现方式：禁用原版 LaneSystem，换成带连接支持的改造版。
            // TrafficLaneSystem 是原版 Game.Net.LaneSystem 的反编译改造版（约 9600 行），
            // 强依赖游戏版本 —— 游戏大更新后这里是最先需要复核的地方。
            updateSystem.World.GetOrCreateSystemManaged<LaneSystem>().Enabled = false;
            updateSystem.UpdateBefore<TrafficLaneSystem, LaneSystem>(SystemUpdatePhase.Modification4);
            updateSystem.UpdateBefore<SyncCustomLaneConnectionsSystem, TrafficLaneSystem>(SystemUpdatePhase.Modification4);
            updateSystem.UpdateBefore<SyncCustomPrioritiesSystem, TrafficLaneSystem>(SystemUpdatePhase.Modification4);

            // 存档数据迁移，依赖 NetCompositions，不能放在 Deserialize 阶段
            updateSystem.UpdateBefore<TrafficDataMigrationSystem, SyncCustomLaneConnectionsSystem>(SystemUpdatePhase.Modification4);

            updateSystem.UpdateAt<ModificationDataSyncSystem>(SystemUpdatePhase.Modification4B);
            updateSystem.UpdateAt<GenerateLaneConnectionsSystem>(SystemUpdatePhase.Modification3);
            updateSystem.UpdateAt<GenerateEdgePrioritiesSystem>(SystemUpdatePhase.Modification3);

            updateSystem.UpdateAt<ModRaycastSystem>(SystemUpdatePhase.Raycast);
            updateSystem.UpdateAfter<Traffic.Tools.ValidationSystem, Game.Tools.ValidationSystem>(SystemUpdatePhase.ModificationEnd);

            updateSystem.UpdateAt<PriorityToolSystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAt<LaneConnectorToolSystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateBefore<ApplyLaneConnectionsSystem, ApplyNetSystem>(SystemUpdatePhase.ApplyTool);
            updateSystem.UpdateBefore<ApplyPrioritiesSystem, ApplyNetSystem>(SystemUpdatePhase.ApplyTool);
            updateSystem.UpdateAt<TrafficToolClearSystem>(SystemUpdatePhase.ClearTool);
            updateSystem.UpdateAt<GenerateConnectorsSystem>(SystemUpdatePhase.Modification5);
            updateSystem.UpdateAt<GenerateHandles>(SystemUpdatePhase.Modification5);
            updateSystem.UpdateAt<SearchSystem>(SystemUpdatePhase.Modification5);
            updateSystem.UpdateAt<LaneConnectorToolTooltipSystem>(SystemUpdatePhase.UITooltip);

            updateSystem.UpdateBefore<PreDeserialize<ModDefaultsSystem>>(SystemUpdatePhase.Deserialize);
            updateSystem.UpdateBefore<TrafficDataClearSystem>(SystemUpdatePhase.Deserialize);

            Colossal.Core.MainThreadDispatcher.RegisterUpdater(RoadBuilderCompatibilityHandler);
            Colossal.Core.MainThreadDispatcher.RegisterUpdater(UpdateUIBindings);

            Logger.Info("车道模块加载完成");
        }

        public void OnDispose()
        {
            Settings?.UnregisterInOptionsUI();
            Settings?.Unload();
            Settings = null;
        }

        private static void UpdateUIBindings()
        {
            ModUISystem uiSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ModUISystem>();
            uiSystem.ModSettingsApplied(ModSettings.Instance);
        }

        private static void RoadBuilderCompatibilityHandler()
        {
            if (!IsRBEnabled)
            {
                return;
            }
            Logger.Info("检测到 RoadBuilder，调整系统执行顺序");
            try
            {
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpdateSystem>()
                    .UpdateBefore<RoadBuilderCompatibilitySystem, TrafficLaneSystem>(SystemUpdatePhase.Modification4);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}
