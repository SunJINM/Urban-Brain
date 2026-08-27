using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace UrbanBrain.Utils;

/// <summary>
/// 通过反射改写游戏内置 System 的 EntityQuery，往里追加 None 条件。
///
/// 这是接管游戏系统的关键手法（比 Harmony 干净）：
/// 给我们要接管的实体打上标记组件，再把原版 System 的查询条件改成"排除带该标记的实体"，
/// 原版就自动不再处理这些实体了，我们和原版可以并存。
///
/// 之所以不用 Harmony：游戏大量代码经 Burst 编译成非托管代码，Harmony 只能 patch 托管代码。
///
/// 手法参考 TrafficLightsEnhancement 的同名工具类。
/// </summary>
public static class EntityQueryUtils
{
    public static EntityQuery GetEntityQuery(object obj, string fieldName)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null)
        {
            return default;
        }
        return (EntityQuery)fieldInfo.GetValue(obj);
    }

    public static bool SetEntityQuery(object obj, string fieldName, EntityQuery entityQuery)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null)
        {
            return false;
        }
        fieldInfo.SetValue(obj, entityQuery);
        return true;
    }

    /// <summary>
    /// 给指定 System 的某个 EntityQuery 字段追加 None 条件。
    /// 返回 false 表示字段名找不到（很可能是游戏更新改了字段名）。
    /// </summary>
    public static bool TryUpdateEntityQuery(SystemBase systemBase, string fieldName, NativeList<ComponentType> none)
    {
        FieldInfo fieldInfo = systemBase.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null)
        {
            return false;
        }

        EntityQuery query = (EntityQuery)fieldInfo.GetValue(systemBase);
        EntityQuery newQuery = BuildWithNone(query, none).Build(systemBase);
        fieldInfo.SetValue(systemBase, newQuery);
        return true;
    }

    private static EntityQueryBuilder BuildWithNone(EntityQuery oldQuery, NativeList<ComponentType> none)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        var descArray = oldQuery.GetEntityQueryDescs();
        for (int i = 0; i < descArray.Length; i++)
        {
            EntityQueryDesc desc = descArray[i];
            var oldAny = ToNativeList(desc.Any);
            var oldNone = ToNativeList(desc.None);
            var oldAll = ToNativeList(desc.All);
            var oldDisabled = ToNativeList(desc.Disabled);
            var oldAbsent = ToNativeList(desc.Absent);
            var oldPresent = ToNativeList(desc.Present);

            builder.WithAny(ref oldAny);
            builder.WithNone(ref oldNone);
            builder.WithAll(ref oldAll);
            builder.WithDisabled(ref oldDisabled);
            builder.WithAbsent(ref oldAbsent);
            builder.WithPresent(ref oldPresent);

            // 追加我们的排除条件
            builder.WithNone(ref none);

            if (i < descArray.Length - 1)
            {
                builder.AddAdditionalQuery();
            }
        }
        return builder;
    }

    private static NativeList<T> ToNativeList<T>(T[] array) where T : unmanaged
    {
        var list = new NativeList<T>(array.Length, Allocator.Temp);
        foreach (var item in array)
        {
            list.Add(item);
        }
        return list;
    }
}
