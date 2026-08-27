using System.Collections.Generic;
using Colossal;

namespace UrbanBrain;

/// <summary>
/// 设置界面文案。不提供的话界面上会直接显示 locale key。
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
            { m_Setting.GetOptionGroupLocaleID(Setting.kGroupPlan), "配时方案" },
            { m_Setting.GetOptionGroupLocaleID(Setting.kGroupData), "数据导出" },
            { m_Setting.GetOptionGroupLocaleID(Setting.kGroupProbe), "调试探针" },

            // ---- 状态 ----
            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CurrentMode)), "接管模式" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.CurrentMode)), "当前生效的信号控制模式。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ControlledCount)), "已接管路口" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ControlledCount)), "当前由 Urban Brain 控制信号的路口数量。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApiKeyStatus)), "API 状态" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApiKeyStatus)),
                "密钥从文件读取，不保存在设置里，也不进存档。\n" +
                "在提示的路径下建一个纯文本文件，里面只放密钥即可。\n" +
                "同目录放 model.txt 可以覆盖默认模型。" },

            // ---- 方案 ----
            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyRuleToWorst)), "规则方案（最堵路口）" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyRuleToWorst)),
                "扫描全城，挑出最堵的那个路口，用规则引擎生成配时方案并立即应用。\n" +
                "不花钱，不联网。这是 AI 方案的对照组。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyAiToWorst)), "AI 方案（最堵路口）" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyAiToWorst)),
                "同样挑最堵的路口，但交给 AI 设计方案，并把规则方案作为基准一并给它参考。\n" +
                "会产生 API 调用费用。请求在后台进行，结果到达后自动应用，进度看日志。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ApplyRuleToAll)), "规则方案（全城）" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ApplyRuleToAll)),
                "对全城每个信号路口都生成并应用规则方案。路口多时会卡一下。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Release)), "释放所有路口" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.Release)),
                "解除接管，把信号控制权还给游戏原版逻辑。" },

            // ---- 数据 ----
            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExportSnapshot)), "导出城市快照" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExportSnapshot)),
                "把全城所有信号路口的几何与实测流量导成一个 JSON 文件。\n" +
                "这个文件可以拿到别的机器上分析，是排查问题最有用的东西。" },

            // ---- 探针 ----
            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TakeoverFreeze)), "探针：冻结信号" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.TakeoverFreeze)),
                "接管全城红绿灯并把相位计时器持续压回 0。\n" +
                "预期效果：所有路口的灯停在当前相位不再变化。\n" +
                "这是用来验证写入是否生效的，会造成堵车，看完点释放即可。" },

            { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TakeoverFast)), "探针：加速切换" },
            { m_Setting.GetOptionDescLocaleID(nameof(Setting.TakeoverFast)),
                "接管全城红绿灯并把计时器持续顶到高位。\n" +
                "预期效果：所有路口的灯疯狂快速切换。" },
        };
    }

    public void Unload()
    {
    }
}
