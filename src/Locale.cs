using System.Collections.Generic;
using Colossal;

namespace UrbanBrain;

/// <summary>
/// 设置界面的文案。不提供的话界面上会直接显示 locale key（形如 Options.OPTION[...]）。
/// 自用项目，中英文界面都给中文内容。
/// </summary>
public class Locale : IDictionarySource
{
    private readonly Setting m_Setting;

    public Locale(Setting setting)
    {
        m_Setting = setting;
    }

    public IEnumerable<KeyValuePair<string, string>> ReadEntries(
        IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
    {
        return new Dictionary<string, string>
        {
            { m_Setting.GetSettingsLocaleID(), "Urban Brain" },
            { m_Setting.GetOptionTabLocaleID(Setting.kSection), "主面板" },

            { m_Setting.GetOptionGroupLocaleID(Setting.kGroupStatus), "当前状态" },
            { m_Setting.GetOptionGroupLocaleID(Setting.kGroupTakeover), "接管操作（M1 验证用）" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CurrentMode)), "接管模式" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.CurrentMode)), "当前生效的信号接管模式。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ControlledCount)), "已接管路口" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ControlledCount)), "当前由 Urban Brain 控制信号的路口数量。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TakeoverFreeze)), "接管并冻结信号" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.TakeoverFreeze)),
                "接管全城红绿灯，并把相位计时器持续压回 0。\n" +
                "预期效果：所有路口的灯不再变化，停在当前相位。\n" +
                "这是验证用的，会造成堵车。点“释放”或重新读档即可恢复。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TakeoverFast)), "接管并加速切换" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.TakeoverFast)),
                "接管全城红绿灯，并把相位计时器持续顶到高位。\n" +
                "预期效果：所有路口的灯疯狂快速切换。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Release)), "释放所有路口" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.Release)),
                "解除接管，把信号控制权交还给游戏原版逻辑。" },
        };
    }

    public void Unload()
    {
    }
}
