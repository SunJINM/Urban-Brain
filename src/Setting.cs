using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using Unity.Entities;
using UrbanBrain.Systems;

namespace UrbanBrain;

[FileLocation("ModsSettings/UrbanBrain/Settings")]
[SettingsUIGroupOrder(kGroupStatus, kGroupTakeover)]
[SettingsUIShowGroupName(kGroupStatus, kGroupTakeover)]
public class Setting : ModSetting
{
    public const string kSection = "Main";

    public const string kGroupStatus = "GroupStatus";

    public const string kGroupTakeover = "GroupTakeover";

    public Setting(IMod mod) : base(mod)
    {
        SetDefaults();
    }

    // ---------- 持久化状态 ----------

    /// <summary>
    /// 当前接管模式。存成 int 而不是 enum，避免设置序列化对枚举的处理差异。
    /// </summary>
    [SettingsUIHidden]
    public int ModeValue { get; set; }

    public OverrideMode Mode => (OverrideMode)ModeValue;

    // ---------- 状态显示（只读）----------

    [SettingsUISection(kSection, kGroupStatus)]
    public string CurrentMode => Mode switch
    {
        OverrideMode.Freeze => "冻结（计时器压零）",
        OverrideMode.Fast => "加速（计时器顶高）",
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
            if (Mod.world == null || GameManager.instance.gameMode != Game.GameMode.Game)
            {
                return "未在游戏中";
            }
            var query = Mod.world.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Components.ControlledSignal>());
            return $"{query.CalculateEntityCount()} 个路口";
        }
    }

    // ---------- 操作按钮 ----------

    [SettingsUISection(kSection, kGroupTakeover)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool TakeoverFreeze
    {
        set
        {
            ModeValue = (int)OverrideMode.Freeze;
            Takeover();
        }
    }

    [SettingsUISection(kSection, kGroupTakeover)]
    [SettingsUIButton]
    [SettingsUIDisableByCondition(typeof(Setting), nameof(CannotTakeover))]
    public bool TakeoverFast
    {
        set
        {
            ModeValue = (int)OverrideMode.Fast;
            Takeover();
        }
    }

    [SettingsUISection(kSection, kGroupTakeover)]
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

    // ---------- 实现 ----------

    private static void Takeover()
    {
        var em = Mod.world.EntityManager;
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<Game.Net.TrafficLights>());
        int count = query.CalculateEntityCount();

        if (count == 0)
        {
            Mod.log.Warn("⚠ 城里一个红绿灯都没找到。要么城市还没建路口，要么 TrafficLights 组件名变了。");
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

    // ---------- 按钮启用条件 ----------

    public bool IsNotInGame()
    {
        return GameManager.instance.gameMode != Game.GameMode.Game;
    }

    public bool CannotTakeover()
    {
        return IsNotInGame() || !Mod.takeoverAvailable;
    }

    public override void SetDefaults()
    {
        ModeValue = (int)OverrideMode.Off;
    }
}
