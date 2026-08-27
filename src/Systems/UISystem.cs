using Colossal.UI.Binding;
using Game.UI;
using Unity.Entities;
using UrbanBrain.Advisor;
using UrbanBrain.Model;

namespace UrbanBrain.Systems;

/// <summary>
/// 游戏内面板的数据通道。
///
/// 绑定形式抄自 TrafficLightsEnhancement：
/// GetterValueBinding 供前端拉数据，CallBinding 供前端触发动作。
/// 所有数据以 JSON 字符串过界，避免为每个结构体写 IJsonWritable。
///
/// 前端在 ui/ 目录，用 bindValue("UrbanBrain", "GetStatus", ...) 取值，
/// 用 engine.call("UrbanBrain.CallExportSnapshot", ...) 触发动作。
/// </summary>
public partial class UISystem : UISystemBase
{
    private const string kGroup = "UrbanBrain";

    private GetterValueBinding<string> m_StatusBinding;
    private GetterValueBinding<string> m_PlanBinding;

    private AdvisorSystem m_Advisor;
    private IntersectionScanSystem m_Scan;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_Advisor = World.GetOrCreateSystemManaged<AdvisorSystem>();
        m_Scan = World.GetOrCreateSystemManaged<IntersectionScanSystem>();

        AddBinding(m_StatusBinding = new GetterValueBinding<string>(kGroup, "GetStatus", GetStatus));
        AddBinding(m_PlanBinding = new GetterValueBinding<string>(kGroup, "GetPlan", GetPlan));

        AddBinding(new CallBinding<string, string>(kGroup, "CallExportSnapshot", CallExportSnapshot));
        AddBinding(new CallBinding<string, string>(kGroup, "CallApplyRule", CallApplyRule));
        AddBinding(new CallBinding<string, string>(kGroup, "CallApplyAi", CallApplyAi));
        AddBinding(new CallBinding<string, string>(kGroup, "CallRelease", CallRelease));
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_StatusBinding?.Update();
        m_PlanBinding?.Update();
    }

    // ------------------------------------------------------------------
    // 读
    // ------------------------------------------------------------------

    private string GetStatus()
    {
        var setting = Mod.setting;

        int controlled = 0;
        if (setting != null && !setting.IsNotInGame())
        {
            var query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Components.ControlledSignal>());
            controlled = query.CalculateEntityCount();
        }

        var status = new
        {
            mode = setting?.CurrentMode ?? "未初始化",
            controlled,
            takeoverAvailable = Mod.takeoverAvailable,
            apiConfigured = setting != null && !string.IsNullOrEmpty(setting.ApiKey),
            model = setting?.Model,
            aiBusy = m_Advisor != null && m_Advisor.m_AiBusy,
            inGame = setting != null && !setting.IsNotInGame(),
        };

        return SnapshotExporter.ToJson(status);
    }

    private string GetPlan()
    {
        var payload = new
        {
            rule = m_Advisor?.m_LastRulePlan,
            ai = m_Advisor?.m_LastAiPlan,
        };
        return SnapshotExporter.ToJson(payload);
    }

    // ------------------------------------------------------------------
    // 写
    // ------------------------------------------------------------------

    // 这几个方法都返回一句结果说明，前端可以直接弹给用户看。

    private string CallExportSnapshot(string _)
    {
        CitySnapshot snapshot = m_Scan.ScanCity();
        string path = SnapshotExporter.Export(snapshot);
        return path != null
            ? $"已导出 {snapshot.intersections.Count} 个路口到 {path}"
            : "导出失败，详见日志";
    }

    private string CallApplyRule(string _)
    {
        Entity node = FindWorst();
        if (node == Entity.Null)
        {
            Mod.log.Warn("⚠ 没找到可处理的信号路口。");
            return "没找到可处理的信号路口";
        }
        SignalPlanProposal plan = m_Advisor.ProposeByRule(node);
        bool ok = m_Advisor.ApplyPlan(node, plan);
        return ok
            ? $"规则方案已应用到路口 {node.Index}：{plan.phases.Count} 个相位"
            : "应用失败，详见日志";
    }

    private string CallApplyAi(string _)
    {
        Entity node = FindWorst();
        if (node == Entity.Null)
        {
            Mod.log.Warn("⚠ 没找到可处理的信号路口。");
            return "没找到可处理的信号路口";
        }
        m_Advisor.RequestAiPlan(node, true);
        return $"已向 AI 发起请求（路口 {node.Index}），结果到达后自动应用";
    }

    private string CallRelease(string _)
    {
        var query = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Components.ControlledSignal>());
        int count = query.CalculateEntityCount();
        EntityManager.RemoveComponent<Components.ControlledSignal>(query);
        Mod.setting.ModeValue = (int)OverrideMode.Off;
        Mod.log.Info($"已释放 {count} 个路口。");
        return $"已释放 {count} 个路口";
    }

    /// <summary>全城最堵的信号路口。没做路口选取工具，先用这个当操作目标。</summary>
    private Entity FindWorst()
    {
        CitySnapshot city = m_Scan.ScanCity();

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

        var query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Game.Net.TrafficLights>());
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
        return found;
    }
}
