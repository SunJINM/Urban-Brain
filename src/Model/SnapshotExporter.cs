using System;
using System.IO;
using Newtonsoft.Json;

namespace UrbanBrain.Model;

/// <summary>
/// 把 L2 快照写到磁盘。
///
/// 这个文件是整个项目的支点：拿到它，就能在没有游戏的机器上迭代指标、规则和 prompt，
/// 不必反复进游戏验证。所以导出格式要稳定、可读、自解释。
/// </summary>
public static class SnapshotExporter
{
    /// <summary>导出目录：游戏用户数据目录下的 UrbanBrain 子目录。</summary>
    public static string OutputDirectory
    {
        get
        {
            // persistentDataPath 就是 ...\AppData\LocalLow\Colossal Order\Cities Skylines II
            return Path.Combine(UnityEngine.Application.persistentDataPath, "UrbanBrain");
        }
    }

    /// <summary>
    /// 导出快照，返回写入的文件完整路径；失败返回 null。
    /// </summary>
    public static string Export(CitySnapshot snapshot, string tag = "city")
    {
        try
        {
            Directory.CreateDirectory(OutputDirectory);

            string name = $"{tag}-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            string full = Path.Combine(OutputDirectory, name);

            string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            File.WriteAllText(full, json);

            Mod.log.Info($"快照已导出：{full}（{snapshot.intersections.Count} 个路口，{json.Length} 字节）");
            return full;
        }
        catch (Exception e)
        {
            Mod.log.Warn($"⚠ 导出快照失败：{e}");
            return null;
        }
    }

    /// <summary>序列化成字符串，供 AI 请求直接使用（不落盘）。</summary>
    public static string ToJson(object obj, bool indented = false)
    {
        return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
    }
}
