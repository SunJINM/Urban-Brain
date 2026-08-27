using Unity.Entities;

namespace UrbanBrain.Components;

/// <summary>
/// 标记组件：打上这个组件的路口，信号由 Urban Brain 接管，原版 TrafficLightSystem 不再处理它。
///
/// 注意：本组件**故意不实现 ISerializable**，因此不会写入存档。
/// 重新读档后所有路口自动回到原版控制 —— 这是 M1 阶段的安全网，
/// 万一接管逻辑把交通搞乱了，重新载入存档即可恢复。
/// </summary>
public struct ControlledSignal : IComponentData, IQueryTypeParameter
{
}
