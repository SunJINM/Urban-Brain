using System.IO;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using Unity.Entities;
using UrbanBrain.Model;
using UrbanBrain.Systems;

namespace UrbanBrain;

[FileLocation("ModsSettings/UrbanBrain/Settings")]
[SettingsUIGroupOrder(kGroupStatus, kGroupPlan, kGroupData, kGroupProbe)]
[SettingsUIShowGroupName(kGroupStatus, kGroupPlan, kGroupData, kGroupProbe)]
public class Setting : ModSetting
{
    public const string kSection = "Main";

    public const string kGroupStatus = "GroupStatus";
    public const string kGroupPlan = "GroupPlan";
    public const string kGroupData = "GroupData";
    public const string kGroupProbe = "GroupProbe";

    public Setting(IMod mod) : base(mod)
    {
        SetDefaults();
    }

    // ==================================================================
    // 持久化状态
    // ==================================================================

    /// <summary>
    /// 当前接管模式。存成 int 而非 enum，避开设置序列化对枚举处理方式的不确定性。
    /// </summary>
    [SettingsUIHidden]
    public int ModeValue { get; set; }

    public OverrideMode Mode => (OverrideMode)ModeValue;

    // ==================================================================
    // API 凭据 —— 从文件读，不进设置界面也不进存档
    // ==================================================================

    /// <summary>
    /// API key 存放路径。
    ///
    /// 刻意不做成设置界面里的输入框：一来密钥不该出现在会被截图的界面上，
    /// 二来设置文件会被同步/备份，密钥不应该混在里面。
    /// 用户自己建一个纯文本文件，里面只放密钥。
    /// </summary>
    public static string ApiKeyPath =>
        Path.Combine(SnapshotExporter.OutputDirectory, "api-key.txt");

    /// <summary>模型覆盖文件，不存在就用默认模型。</summary>
    public static string ModelPath =>
        Path.Combine(SnapshotExporter.OutputDirectory, "model.txt");

    public string ApiKey
    {
        get
        {
            try
            {
                return File.Exists(ApiKeyPath) ? File.ReadAllText(ApiKeyPath).Trim() : null;
            }
            catch
            {
                return null;
            }
        }
    }

    public string Model
    {
        get
        {
            try
            {
                if (File.Exists(ModelPath))
                {
                    string m = File.ReadAllText(ModelPath).Trim();
                    if (!string.IsNullOrEmpty(m))
                    {
                        return m;
                    }
                }
            }
            catch
            {
                // 读不到就用默认
            }
            return Advisor.AiAdvisor.kDefaultModel;
        }
    }

    // ==================================================================
    // 状态显示
    // ==================================================================

    [SettingsUISection(kSection, kGroupStatus)]
    public string CurrentMode => Mode switch
    {
        OverrideMode.Freeze => "探针：冻结",
        OverrideMode.Fast => "探针：加速",
        OverrideMode.Plan => "按方案运行",
        _ => "未接管",
    };

    [SettingsUISection(kSection, kGroupStatus)]
    public string ControlledCount
    {
        get
        {
            if (!Mod.takeoverAvailable)
            {
                return "接管通道不可用，请查看日志";
            }
            if (IsNotInGame())
            {
                return "未在游戏中";
            }
            var query = Mod.world.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Components.ControlledSignal>());
            return $"{query.CalculateEntityCount()} 个路口";
        }
    }

    [SettingsUISection(kSection, kGroupStatus)]
    public string ApiKeyStatus
    {
        get
        {
            string key = ApiKey;
            if (string.IsNullOrEmpty(key))
            {
                return $"未配置。请建立 {ApiKeyPath}";
            }
            return $"已配置（{key.Length} 字符），模型 {Model}";
        }
    }

    // ==================================================================
    // 方案操作
    // ==================================================================

    [SettingsUISection(kSection, kGroupPlan)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool ApplyRuleToWorst
    {
        set
        {
            Entity node = FindWorstIntersection();
            if (node == Entity.Null)
            {
                Mod.log.Warn("⚠ 没找到可处理的信号路口。");
                return;
            }
            var advisor = Mod.world.GetOrCreateSystemManaged<AdvisorSystem>();
            advisor.ApplyPlan(node, advisor.ProposeByRule(node));
        }
    }

    [SettingsUISection(kSection, kGroupPlan)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotUseAi))]
    public bool ApplyAiToWorst
    {
        set
        {
            Entity node = FindWorstIntersection();
            if (node == Entity.Null)
            {
                Mod.log.Warn("⚠ 没找到可处理的信号路口。");
                return;
            }
            Mod.world.GetOrCreateSystemManaged<AdvisorSystem>().RequestAiPlan(node, true);
        }
    }

    [SettingsUISection(kSection, kGroupPlan)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool ApplyRuleToAll
    {
        set
        {
            Mod.world.GetOrCreateSystemManaged<AdvisorSystem>().ApplyRulePlanToAll();
        }
    }

    [SettingsUISection(kSection, kGroupPlan)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(IsNotInGame))]
    public bool Release
    {
        set
        {
            ModeValue = (int)OverrideMode.Off;
            ReleaseAll();
        }
    }

    // ==================================================================
    // 数据导出
    // ==================================================================

    [SettingsUISection(kSection, kGroupData)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(IsNotInGame))]
    public bool ExportSnapshot
    {
        set
        {
            var scan = Mod.world.GetOrCreateSystemManaged<IntersectionScanSystem>();
            CitySnapshot snapshot = scan.ScanCity();
            string path = SnapshotExporter.Export(snapshot);
            if (path != null)
            {
                Mod.log.Info($"快照文件：{path}");
            }
        }
    }

    // ==================================================================
    // M1 探针（调试用）
    // ==================================================================

    [SettingsUISection(kSection, kGroupProbe)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool TakeoverFreeze
    {
        set
        {
            ModeValue = (int)OverrideMode.Freeze;
            TakeoverAll();
        }
    }

    [SettingsUISection(kSection, kGroupProbe)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool TakeoverFast
    {
        set
        {
            ModeValue = (int)OverrideMode.Fast;
            TakeoverAll();
        }
    }

    // ==================================================================
    // 实现
    // ==================================================================

    /// <summary>找出全城最堵的信号路口。用它代替「选中路口」的交互。</summary>
    private static Entity FindWorstIntersection()
    {
        var scan = Mod.world.GetOrCreateSystemManaged<IntersectionScanSystem>();
        CitySnapshot city = scan.ScanCity();

        int worstId = -1;
        float worst = -1f;
        foreach (IntersectionSnapshot i in city.intersections)
        {
            if (i.worstCongestion > worst)
            {
                worst = i.worstCongestion;
                worstId = i.id;
            }
        }

        if (worstId < 0)
        {
            return Entity.Null;
        }

        var query = Mod.world.EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Game.Net.TrafficLights>());
        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
        Entity found = Entity.Null;
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].Index == worstId)
            {
                found = entities[i];
                break;
            }
        }
        entities.Dispose();

        Mod.log.Info($"目标路口：entity {worstId}，最差进口拥堵度 {worst * 100:F0}%");
        return found;
    }

    private static void TakeoverAll()
    {
        var em = Mod.world.EntityManager;
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<Game.Net.TrafficLights>());
        int count = query.CalculateEntityCount();

        if (count == 0)
        {
            Mod.log.Warn("⚠ 城里一个红绿灯都没找到。要么还没建带信号的路口，要么 TrafficLights 组件名变了。");
            return;
        }

        em.AddComponent<Components.ControlledSignal>(query);
        Mod.log.Info($"已接管 {count} 个路口，模式={Mod.setting.Mode}");
    }

    private static void ReleaseAll()
    {
        var em = Mod.world.EntityManager;
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<Components.ControlledSignal>());
        int count = query.CalculateEntityCount();
        em.RemoveComponent<Components.ControlledSignal>(query);
        Mod.log.Info($"已释放 {count} 个路口，恢复原版控制。");
    }

    // ==================================================================
    // 按钮启用条件
    // ==================================================================

    public bool IsNotInGame()
    {
        return Mod.world == null || GameManager.instance.gameMode != Game.GameMode.Game;
    }

    public bool CannotTakeover()
    {
        return IsNotInGame() || !Mod.takeoverAvailable;
    }

    public bool CannotUseAi()
    {
        return CannotTakeover() || string.IsNullOrEmpty(ApiKey);
    }

    public override void SetDefaults()
    {
        ModeValue = (int)OverrideMode.Off;
    }
}
