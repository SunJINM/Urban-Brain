using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UrbanBrain.Model;

namespace UrbanBrain.Advisor;

/// <summary>
/// 调用 Claude API 生成配时方案。
///
/// 为什么用裸 HTTP 而不是官方 C# SDK：模组跑在 net48 + Unity Mono 上，
/// 引入 NuGet 依赖需要连同一串 DLL 一起分发，容易和游戏自带程序集冲突。
/// 游戏本身已带 Newtonsoft.Json，用 HttpClient 直接发请求依赖最少。
///
/// 线程约定：请求在后台线程跑，结果丢进队列，由 <see cref="Systems.AdvisorSystem"/>
/// 在主线程取出应用。绝不能在 Unity 主线程上阻塞等待网络。
/// </summary>
public static class AiAdvisor
{
    private const string kEndpoint = "https://api.anthropic.com/v1/messages";
    private const string kApiVersion = "2023-06-01";

    /// <summary>默认模型。可在设置里改。</summary>
    public const string kDefaultModel = "claude-opus-5";

    private static readonly HttpClient s_Http = CreateClient();

    private static HttpClient CreateClient()
    {
        // net48 默认不启用 TLS 1.2，不设的话握手会直接失败
        try
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
        catch (Exception e)
        {
            Mod.log.Warn($"⚠ 设置 TLS 1.2 失败，HTTPS 请求可能无法建立：{e.Message}");
        }

        return new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
    }

    /// <summary>
    /// 请求一个路口的配时方案。
    /// 返回 null 表示失败，失败原因已写进日志。
    /// </summary>
    public static async Task<SignalPlanProposal> ProposeAsync(
        IntersectionSnapshot snap, string apiKey, string model, SignalPlanProposal ruleBaseline)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Mod.log.Warn("⚠ 没有配置 API key，无法调用 AI。请在设置里填入。");
            return null;
        }

        string body = BuildRequestBody(snap, model, ruleBaseline);

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, kEndpoint);
            req.Headers.Add("x-api-key", apiKey);
            req.Headers.Add("anthropic-version", kApiVersion);
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage resp = await s_Http.SendAsync(req).ConfigureAwait(false);
            string text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                Mod.log.Warn($"⚠ API 返回 {(int)resp.StatusCode}：{Truncate(text, 800)}");
                return null;
            }

            return ParseResponse(text, snap.id);
        }
        catch (TaskCanceledException)
        {
            Mod.log.Warn("⚠ API 请求超时（120 秒）。");
            return null;
        }
        catch (Exception e)
        {
            Mod.log.Warn($"⚠ API 请求异常：{e}");
            return null;
        }
    }

    // ------------------------------------------------------------------
    // 请求构造
    // ------------------------------------------------------------------

    private static string BuildRequestBody(IntersectionSnapshot snap, string model, SignalPlanProposal baseline)
    {
        var payload = new JObject
        {
            ["model"] = string.IsNullOrEmpty(model) ? kDefaultModel : model,
            ["max_tokens"] = 16000,
            ["system"] = BuildSystemPrompt(),
            ["thinking"] = new JObject { ["type"] = "adaptive" },
            ["output_config"] = new JObject
            {
                ["effort"] = "high",
                ["format"] = new JObject
                {
                    ["type"] = "json_schema",
                    ["schema"] = JObject.Parse(kResponseSchema),
                },
            },
            ["messages"] = new JArray
            {
                new JObject
                {
                    ["role"] = "user",
                    ["content"] = BuildUserPrompt(snap, baseline),
                },
            },
        };

        return payload.ToString(Formatting.None);
    }

    private static string BuildSystemPrompt()
    {
        return
            "你是一位交通信号配时工程师，正在为城市模拟游戏《Cities: Skylines II》里的路口设计信号方案。\n\n" +
            "数据说明：\n" +
            "- approaches 是路口的各个进口，按数组下标编号，movements 里的 approach 必须引用这个下标\n" +
            "- direction 是方位名（N/NE/E/...），仅供你表述，程序以 approach 下标为准\n" +
            "- laneLeft/laneStraight/laneRight/laneUTurn 是该进口各转向的车道数\n" +
            "- flow.congestion 是拥堵度，0 通畅、1 完全堵死；等于 -1 表示没采到数据\n" +
            "- flow.avgSpeed 与 flow.speedLimit 分别是实测平均速度和限速\n\n" +
            "设计要求：\n" +
            "1. 每个相位放行的流向之间不能有冲突。对向直行可以同放；" +
            "对向左转可以同放；但同一进口的左转和对向直行不能同放。\n" +
            "2. 右转在没有专用相位时通常可以跟随同进口的直行放行。\n" +
            "3. 绿灯时长按流量和拥堵度分配，单相位不少于 5 秒，总周期建议 40 到 120 秒。\n" +
            "4. 相位数量越少周期越短，只有左转需求确实大时才拆专用左转相位。\n" +
            "5. 某个流向如果不出现在任何相位里，等于被永久禁止。只有当你确信该转向应当被禁" +
            "（例如左转流量极小却严重阻塞直行）时才这样做，并在 rationale 里说明。\n\n" +
            "诚实性要求（重要）：\n" +
            "- 如果 flow.congestion 全是 -1，说明没有流量数据，你只能依据车道数推断。" +
            "此时必须在 warnings 里写明这一点，不要编造流量层面的因果解释。\n" +
            "- reason 字段要给出具体依据（引用实际数字），不要写空泛的套话。\n" +
            "- 如果数据不足以支持某个判断，就说数据不足，不要猜。";
    }

    private static string BuildUserPrompt(IntersectionSnapshot snap, SignalPlanProposal baseline)
    {
        var sb = new StringBuilder();
        sb.AppendLine("请为下面这个路口设计信号配时方案。");
        sb.AppendLine();
        sb.AppendLine("路口现状数据：");
        sb.AppendLine("```json");
        sb.AppendLine(SnapshotExporter.ToJson(snap, indented: true));
        sb.AppendLine("```");

        if (baseline != null && baseline.phases.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("作为参考，下面是规则引擎按常规方法生成的基准方案。");
            sb.AppendLine("如果你认为它已经足够好，可以沿用并说明理由；");
            sb.AppendLine("如果你能做得更好，请给出你的方案并说清楚好在哪里。");
            sb.AppendLine("```json");
            sb.AppendLine(SnapshotExporter.ToJson(baseline, indented: true));
            sb.AppendLine("```");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 输出格式约定。用 json_schema 强制模型返回可解析的结构，
    /// 避免解析自由文本这种脆弱做法。
    /// </summary>
    private const string kResponseSchema = @"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""required"": [""rationale"", ""phases"", ""warnings""],
  ""properties"": {
    ""rationale"": { ""type"": ""string"" },
    ""warnings"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
    ""phases"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""required"": [""name"", ""movements"", ""minDuration"", ""maxDuration"", ""targetDuration"", ""reason""],
        ""properties"": {
          ""name"": { ""type"": ""string"" },
          ""reason"": { ""type"": ""string"" },
          ""minDuration"": { ""type"": ""integer"" },
          ""maxDuration"": { ""type"": ""integer"" },
          ""targetDuration"": { ""type"": ""number"" },
          ""movements"": {
            ""type"": ""array"",
            ""items"": {
              ""type"": ""object"",
              ""additionalProperties"": false,
              ""required"": [""approach"", ""direction"", ""turn""],
              ""properties"": {
                ""approach"": { ""type"": ""integer"" },
                ""direction"": { ""type"": ""string"" },
                ""turn"": { ""type"": ""string"", ""enum"": [""left"", ""straight"", ""right"", ""uturn""] }
              }
            }
          }
        }
      }
    }
  }
}";

    // ------------------------------------------------------------------
    // 响应解析
    // ------------------------------------------------------------------

    private static SignalPlanProposal ParseResponse(string responseText, int intersectionId)
    {
        JObject root = JObject.Parse(responseText);

        string stopReason = (string)root["stop_reason"];
        if (stopReason == "refusal")
        {
            Mod.log.Warn("⚠ 模型拒绝了这次请求。");
            return null;
        }

        // 结构化输出仍然放在 content 数组的 text block 里
        string json = null;
        if (root["content"] is JArray content)
        {
            foreach (JToken block in content)
            {
                if ((string)block["type"] == "text")
                {
                    json = (string)block["text"];
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(json))
        {
            Mod.log.Warn($"⚠ 响应里没有找到文本内容：{Truncate(responseText, 500)}");
            return null;
        }

        SignalPlanProposal plan;
        try
        {
            plan = JsonConvert.DeserializeObject<SignalPlanProposal>(json);
        }
        catch (Exception e)
        {
            Mod.log.Warn($"⚠ 解析模型输出失败：{e.Message}\n原文：{Truncate(json, 500)}");
            return null;
        }

        if (plan == null)
        {
            return null;
        }

        plan.intersectionId = intersectionId;
        plan.source = "ai";
        plan.phases ??= new List<PhaseProposal>();
        plan.warnings ??= new List<string>();

        float cycle = 0f;
        foreach (PhaseProposal p in plan.phases)
        {
            cycle += p.targetDuration;
        }
        plan.cycleLength = cycle;

        JToken usage = root["usage"];
        if (usage != null)
        {
            Mod.log.Info($"AI 方案生成完毕：{plan.phases.Count} 个相位，周期 {cycle:F0} 秒 " +
                         $"(输入 {usage["input_tokens"]} tok，输出 {usage["output_tokens"]} tok)");
        }

        return plan;
    }

    private static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s) || s.Length <= max)
        {
            return s;
        }
        return s.Substring(0, max) + "…";
    }
}
