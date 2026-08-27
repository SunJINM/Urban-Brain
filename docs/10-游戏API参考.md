# 游戏 ECS 组件参考（提取子集）

> **本文件由 `scripts/extract-api.py` 自动生成，不要手改。**
>
> 数据来源：[Captain-Of-Coit/cs2-ecs-explorer](https://github.com/Captain-Of-Coit/cs2-ecs-explorer) (MIT)，
> 由反编译源码提取。仅收录 Urban Brain 关心的领域，`Game.Prefabs` 等未收录。
>
> ⚠ **该数据集为 2023 年游戏发布初期版本。** 字段可能已随版本变化，
> 每个用到的字段都需在真机编译时确认。已交叉验证的部分见文末。

收录 288 个组件，跨 9 个领域。

---

## Game.Net

*路网与车道 —— 交通感知层的数据源* — 59 个组件

### `Aggregate`

*(标记组件，无字段)*

### `Aggregated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Aggregate` |

被使用：`RaycastSystem`, `AggregateSystem`, `AggregateMeshSystem`, `AggregatedSystem`, `ApplyNetSystem`, `GenerateEdgesSystem`

### `AreaLane`

| 类型 | 字段 |
|---|---|
| `int4` | `m_Nodes` |

被使用：`AreaConnectionSystem`, `AnimalNavigationSystem`, `CarNavigationSystem`, `HumanNavigationSystem`

### `Bottleneck`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Position` |
| `byte` | `m_MinPos` |
| `byte` | `m_MaxPos` |
| `byte` | `m_Timer` |

被使用：`TrafficBottleneckSystem`

### `BuildOrder`

| 类型 | 字段 |
|---|---|
| `uint` | `m_Start` |
| `uint` | `m_End` |

被使用：`FlipTrafficHandednessSystem`, `NodeReductionSystem`, `CellCheckSystem`

### `CarLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |
| `CarLaneFlags` | `m_Flags` |
| `float` | `m_DefaultSpeedLimit` |
| `float` | `m_SpeedLimit` |
| `float` | `m_Curviness` |
| `ushort` | `m_CarriagewayGroup` |
| `byte` | `m_BlockageStart` |
| `byte` | `m_BlockageEnd` |
| `byte` | `m_CautionStart` |
| `byte` | `m_CautionEnd` |
| `byte` | `m_FlowOffset` |
| `byte` | `m_LaneCrossCount` |

被使用：`CurrentDistrictSystem`, `ConnectionWarningSystem`, `FixLaneObjectsSystem`, `LaneConnectionSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `SecondaryLaneReferencesSystem`, `SecondaryLaneSystem`, `TrafficLightInitializationSystem`, `LaneDataUnknownEscalateSystem` …等 13 个

### `Composition`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Edge` |
| `Entity` | `m_StartNode` |
| `Entity` | `m_EndNode` |

被使用：`LotHeightSystem`, `RoadConnectionSystem`, `WaterPoweredInitializeSystem`, `RaycastSystem`, `CompositionSelectSystem`, `CostSystem`, `GeometrySystem`, `LaneSystem`, `NetComponentsSystem`, `OutsideConnectionSystem` …等 34 个

### `ConnectionLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |
| `ConnectionLaneFlags` | `m_Flags` |
| `TrackTypes` | `m_TrackTypes` |
| `RoadTypes` | `m_RoadTypes` |

被使用：`AirwaySystem`, `ConnectionWarningSystem`, `FixLaneObjectsSystem`, `LaneConnectionSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `OutsideConnectionSystem`, `SecondaryLaneReferencesSystem`, `SubLaneSystem`, `RideNeederSystem`

### `Curve`

| 类型 | 字段 |
|---|---|
| `Bezier4x3` | `m_Bezier` |
| `float` | `m_Length` |

被使用：`AreaConnectionSystem`, `SurfaceExpandSystem`, `RoadConnectionSystem`, `WaterPoweredInitializeSystem`, `RaycastSystem`, `InitializeSystem`, `EffectControlSystem`, `SearchSystem`, `AggregateSystem`, `AirwaySystem` …等 95 个

### `Density`

| 类型 | 字段 |
|---|---|
| `float` | `m_Density` |

被使用：`LanesModifiedSystem`, `NetEdgeDensitySystem`, `ServiceCoverageSystem`, `CoveragePreviewSystem`

### `Edge`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Start` |
| `Entity` | `m_End` |

被使用：`SurfaceExpandSystem`, `LotHeightSystem`, `AggregateSystem`, `CompositionSelectSystem`, `ConnectionWarningSystem`, `CostSystem`, `EdgeMappingSystem`, `FlipTrafficHandednessSystem`, `GeometrySystem`, `LaneOverlapSystem` …等 34 个

### `EdgeColor`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Index` |
| `byte` | `m_Value0` |
| `byte` | `m_Value1` |

被使用：`BatchDataSystem`, `NetColorSystem`, `PreCullingSystem`

### `EdgeGeometry`

| 类型 | 字段 |
|---|---|
| `Segment` | `m_Start` |
| `Segment` | `m_End` |
| `Bounds3` | `m_Bounds` |

被使用：`CurrentDistrictSystem`, `LotHeightSystem`, `RoadConnectionSystem`, `RaycastSystem`, `GeometrySystem`, `LaneSystem`, `OutsideConnectionSystem`, `SearchSystem`, `SecondaryLaneSystem`, `UpdateCollectSystem` …等 29 个

### `EdgeLane`

| 类型 | 字段 |
|---|---|
| `float2` | `m_EdgeDelta` |
| `byte` | `m_ConnectedStartCount` |
| `byte` | `m_ConnectedEndCount` |

被使用：`RoadConnectionSystem`, `ConnectionWarningSystem`, `EdgeMappingSystem`, `LaneOverlapSystem`, `LaneSystem`, `OutsideConnectionSystem`, `SecondaryLaneSystem`, `SecondaryObjectSystem`, `LaneDataSystem`, `LanesModifiedSystem` …等 22 个

### `EdgeMapping`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Parent1` |
| `Entity` | `m_Parent2` |
| `float2` | `m_CurveDelta1` |
| `float2` | `m_CurveDelta2` |

被使用：`EdgeMappingSystem`, `NetColorSystem`

### `ElectricityConnection`

*(标记组件，无字段)*

### `Elevation`

| 类型 | 字段 |
|---|---|
| `float2` | `m_Elevation` |

被使用：`CompositionSelectSystem`, `CostSystem`, `FlipTrafficHandednessSystem`, `GeometrySystem`, `LaneSystem`, `ReferencesSystem`, `OverrideSystem`, `SecondaryObjectSystem`, `SubObjectSystem`, `NetXPSystem` …等 13 个

### `EndNodeGeometry`

| 类型 | 字段 |
|---|---|
| `EdgeNodeGeometry` | `m_Geometry` |

被使用：`LotHeightSystem`, `RoadConnectionSystem`, `RaycastSystem`, `GeometrySystem`, `LaneSystem`, `SearchSystem`, `UpdateCollectSystem`, `OverrideSystem`, `SecondaryObjectSystem`, `BatchDataSystem` …等 22 个

### `Fixed`

| 类型 | 字段 |
|---|---|
| `int` | `m_Index` |

被使用：`CompositionSelectSystem`, `SubObjectSystem`, `CourseSplitSystem`, `GenerateEdgesSystem`, `GenerateNodesSystem`, `NodeReductionSystem`

### `GarageLane`

| 类型 | 字段 |
|---|---|
| `ushort` | `m_ParkingFee` |
| `ushort` | `m_ComfortFactor` |
| `ushort` | `m_VehicleCount` |
| `ushort` | `m_VehicleCapacity` |

被使用：`ImpactSystem`, `LanePoliciesSystem`, `LanesModifiedSystem`, `ParkingLaneDataSystem`, `ParkingFacilityAISystem`, `PersonalCarAISystem`, `ResidentAISystem`, `ReferencesSystem`

### `HangingLane`

| 类型 | 字段 |
|---|---|
| `float2` | `m_Distances` |

被使用：`SecondaryLaneSystem`, `BatchDataSystem`

### `LabelExtents`

| 类型 | 字段 |
|---|---|
| `Bounds2` | `m_Bounds` |

### `LandValue`

| 类型 | 字段 |
|---|---|
| `float` | `m_LandValue` |
| `float` | `m_Weight` |

被使用：`NetColorSystem`, `CommercialFindPropertySystem`, `IndustrialFindPropertySystem`, `LandValueSystem`, `RentAdjustSystem`, `RentInitializeSystem`, `ZoneSpawnSystem`, `ApplyNetSystem`, `ZoningInfoSystem`

### `Lane`

| 类型 | 字段 |
|---|---|
| `PathNode` | `m_StartNode` |
| `PathNode` | `m_MiddleNode` |
| `PathNode` | `m_EndNode` |

被使用：`AreaConnectionSystem`, `RoadConnectionSystem`, `AirwaySystem`, `ConnectionWarningSystem`, `EdgeMappingSystem`, `FixLaneObjectsSystem`, `LaneConnectionSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `LaneSystem` …等 36 个

### `LaneColor`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Index` |
| `byte` | `m_Value0` |
| `byte` | `m_Value1` |

被使用：`BatchDataSystem`, `NetColorSystem`, `PreCullingSystem`

### `LaneCondition`

| 类型 | 字段 |
|---|---|
| `float` | `m_Wear` |

被使用：`BatchDataSystem`, `PreCullingSystem`, `CarNavigationSystem`, `MaintenanceVehicleAISystem`, `NetDeteriorationSystem`, `TrainNavigationSystem`

### `LaneConnection`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_StartLane` |
| `Entity` | `m_EndLane` |
| `float` | `m_StartPosition` |
| `float` | `m_EndPosition` |

被使用：`ConnectionWarningSystem`, `LaneConnectionSystem`, `LanesModifiedSystem`

### `LaneFlow`

| 类型 | 字段 |
|---|---|
| `float4` | `m_Duration` |
| `float4` | `m_Distance` |
| `float2` | `m_Next` |

被使用：`TrafficFlowSystem`, `TrainNavigationSystem`

### `LaneGeometry`

*(标记组件，无字段)*

### `LaneReservation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Blocker` |
| `ReservationData` | `m_Next` |
| `ReservationData` | `m_Prev` |

### `LaneSignal`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Petitioner` |
| `Entity` | `m_Blocker` |
| `ushort` | `m_GroupMask` |
| `sbyte` | `m_Priority` |
| `sbyte` | `m_Default` |
| `LaneSignalType` | `m_Signal` |
| `LaneSignalFlags` | `m_Flags` |

被使用：`LaneSystem`, `TrafficLightInitializationSystem`

### `LocalConnect`

*(标记组件，无字段)*

被使用：`ApplyNetSystem`, `GenerateEdgesSystem`, `GenerateNodesSystem`, `GenerateObjectsSystem`

### `Marker`

*(标记组件，无字段)*

被使用：`SearchSystem`, `MarkerCreateSystem`, `OverrideSystem`, `SearchSystem`, `ApplyNetSystem`

### `MasterLane`

| 类型 | 字段 |
|---|---|
| `uint` | `m_Group` |
| `ushort` | `m_MinIndex` |
| `ushort` | `m_MaxIndex` |

被使用：`FixLaneObjectsSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `LaneSystem`, `OutsideConnectionSystem`, `SecondaryLaneSystem`, `TrafficLightInitializationSystem`, `LaneBlockSystem`, `SecondaryObjectSystem`, `LaneDataSystem` …等 17 个

### `NetCondition`

| 类型 | 字段 |
|---|---|
| `float2` | `m_Wear` |

被使用：`NetColorSystem`, `MaintenanceDepotAISystem`, `MaintenanceVehicleAISystem`, `MaintenanceVehicleDispatchSystem`, `NetDeteriorationSystem`, `RoadSafetySystem`, `GenerateEdgesSystem`, `GenerateNodesSystem`

### `Node`

| 类型 | 字段 |
|---|---|
| `float3` | `m_Position` |
| `quaternion` | `m_Rotation` |

### `NodeColor`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Index` |
| `byte` | `m_Value` |

被使用：`BatchDataSystem`, `NetColorSystem`, `PreCullingSystem`

### `NodeGeometry`

| 类型 | 字段 |
|---|---|
| `Bounds3` | `m_Bounds` |
| `float` | `m_Position` |
| `float` | `m_Flatness` |
| `float` | `m_Offset` |

被使用：`RaycastSystem`, `GeometrySystem`, `LaneSystem`, `SearchSystem`, `UpdateCollectSystem`, `AlignSystem`, `NetObjectInitializeSystem`, `BatchDataSystem`, `PreCullingSystem`, `UtilityLodUpdateSystem` …等 14 个

### `NodeLane`

| 类型 | 字段 |
|---|---|
| `float2` | `m_WidthOffset` |
| `byte` | `m_SharedStartCount` |
| `byte` | `m_SharedEndCount` |

被使用：`EdgeMappingSystem`, `LaneOverlapSystem`, `SecondaryLaneSystem`, `BatchDataSystem`

### `Orphan`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Composition` |

被使用：`LotHeightSystem`, `RaycastSystem`, `CompositionSelectSystem`, `GeometrySystem`, `LaneSystem`, `SearchSystem`, `BatchDataSystem`, `BatchInstanceSystem`, `PreCullingSystem`, `RequiredBatchesSystem` …等 18 个

### `OutsideConnection`

| 类型 | 字段 |
|---|---|
| `float` | `m_Delay` |

被使用：`ConnectionWarningSystem`, `GeometrySystem`, `LaneOverlapSystem`, `NodeAlignSystem`, `OverrideSystem`, `SearchSystem`, `CountEmploymentSystem`

### `ParkingLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |
| `PathNode` | `m_SecondaryStartNode` |
| `ParkingLaneFlags` | `m_Flags` |
| `float` | `m_FreeSpace` |
| `ushort` | `m_ParkingFee` |
| `ushort` | `m_ComfortFactor` |
| `ushort` | `m_TaxiAvailability` |
| `ushort` | `m_TaxiFee` |

被使用：`CurrentDistrictSystem`, `LaneConnectionSystem`, `LaneReferencesSystem`, `SecondaryLaneReferencesSystem`, `SecondaryLaneSystem`, `LaneDataUnknownEscalateSystem`, `SubLaneSystem`

### `PedestrianLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |
| `PedestrianLaneFlags` | `m_Flags` |

被使用：`CurrentDistrictSystem`, `ConnectionWarningSystem`, `FixLaneObjectsSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `SecondaryLaneReferencesSystem`, `SecondaryLaneSystem`, `TrafficLightInitializationSystem`, `SubLaneSystem`

### `Pollution`

| 类型 | 字段 |
|---|---|
| `float2` | `m_Pollution` |
| `float2` | `m_Accumulation` |

### `Road`

| 类型 | 字段 |
|---|---|
| `float4` | `m_TrafficFlowDuration0` |
| `float4` | `m_TrafficFlowDuration1` |
| `float4` | `m_TrafficFlowDistance0` |
| `float4` | `m_TrafficFlowDistance1` |
| `RoadFlags` | `m_Flags` |

被使用：`InitializeSystem`, `LaneReferencesSystem`, `SecondaryObjectSystem`, `NetColorSystem`, `AccidentCreatureSystem`, `AccidentVehicleSystem`, `CarNavigationSystem`, `RoadSafetySystem`, `StreetLightSystem`, `TrafficFlowSystem` …等 16 个

### `SecondaryLane`

*(标记组件，无字段)*

被使用：`LaneOverlapSystem`, `LaneReferencesSystem`, `LaneSystem`, `OutsideConnectionSystem`, `SecondaryLaneSystem`, `TrafficLightInitializationSystem`

### `SlaveLane`

| 类型 | 字段 |
|---|---|
| `SlaveLaneFlags` | `m_Flags` |
| `uint` | `m_Group` |
| `ushort` | `m_MinIndex` |
| `ushort` | `m_MaxIndex` |
| `ushort` | `m_SubIndex` |
| `ushort` | `m_MasterIndex` |

被使用：`ConnectionWarningSystem`, `LaneConnectionSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `LaneSystem`, `OutsideConnectionSystem`, `SecondaryLaneSystem`, `TrafficLightInitializationSystem`, `SecondaryObjectSystem`, `SpawnLocationConnectionSystem` …等 31 个

### `Standalone`

*(标记组件，无字段)*

被使用：`NodeAlignSystem`, `ReferencesSystem`, `ApplyNetSystem`, `GenerateNodesSystem`

### `StartNodeGeometry`

| 类型 | 字段 |
|---|---|
| `EdgeNodeGeometry` | `m_Geometry` |

被使用：`LotHeightSystem`, `RoadConnectionSystem`, `RaycastSystem`, `GeometrySystem`, `LaneSystem`, `SearchSystem`, `UpdateCollectSystem`, `OverrideSystem`, `SecondaryObjectSystem`, `BatchDataSystem` …等 22 个

### `SubwayTrack`

*(标记组件，无字段)*

被使用：`NetColorSystem`

### `Taxiway`

*(标记组件，无字段)*

### `TrackLane`

| 类型 | 字段 |
|---|---|
| `TrackLaneFlags` | `m_Flags` |
| `float` | `m_SpeedLimit` |
| `float` | `m_Curviness` |

被使用：`ConnectionWarningSystem`, `FixLaneObjectsSystem`, `LaneOverlapSystem`, `LaneReferencesSystem`, `SecondaryLaneReferencesSystem`, `SecondaryLaneSystem`, `SubLaneSystem`

### `TrafficLights`

| 类型 | 字段 |
|---|---|
| `TrafficLightState` | `m_State` |
| `TrafficLightFlags` | `m_Flags` |
| `byte` | `m_SignalGroupCount` |
| `byte` | `m_CurrentSignalGroup` |
| `byte` | `m_NextSignalGroup` |
| `byte` | `m_Timer` |

被使用：`NetComponentsSystem`, `TrafficLightInitializationSystem`, `SecondaryObjectSystem`, `TrafficLightSystem`, `ApplyNetSystem`

### `TrainTrack`

*(标记组件，无字段)*

被使用：`LaneReferencesSystem`, `NetColorSystem`, `ApplyNetSystem`

### `TramTrack`

*(标记组件，无字段)*

被使用：`LaneReferencesSystem`, `NetColorSystem`, `ApplyNetSystem`, `GenerateEdgesSystem`

### `Upgraded`

| 类型 | 字段 |
|---|---|
| `CompositionFlags` | `m_Flags` |

被使用：`CompositionSelectSystem`, `FlipTrafficHandednessSystem`, `NetPollutionSystem`, `ApplyNetSystem`, `CourseSplitSystem`, `GenerateEdgesSystem`, `GenerateNodesSystem`, `NodeReductionSystem`

### `UtilityLane`

| 类型 | 字段 |
|---|---|
| `UtilityLaneFlags` | `m_Flags` |

被使用：`OverrideSystem`, `SearchSystem`

### `WaterPipeConnection`

| 类型 | 字段 |
|---|---|
| `int` | `m_FreshCapacity` |
| `int` | `m_SewageCapacity` |
| `int` | `m_StormCapacity` |

被使用：`InitializeSystem`

### `Waterway`

*(标记组件，无字段)*

被使用：`NetColorSystem`

---

## Game.Pathfind

*寻路 —— 绕行分析* — 5 个组件

### `CoverageUpdated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Owner` |
| `PathEventData` | `m_Data` |

### `PathInformation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Origin` |
| `Entity` | `m_Destination` |
| `float` | `m_Distance` |
| `float` | `m_Duration` |
| `float` | `m_TotalCost` |
| `PathMethod` | `m_Methods` |
| `PathFlags` | `m_State` |

被使用：`PathfindResultSystem`, `RoutePathReadySystem`, `AmbulanceAISystem`, `AreaLotSimulationSystem`, `DeathcareFacilityAISystem`, `DeliveryTruckAISystem`, `EmergencyShelterAISystem`, `EvacuationDispatchSystem`, `FindJobSystem`, `FindSchoolSystem` …等 57 个

### `PathOwner`

| 类型 | 字段 |
|---|---|
| `int` | `m_ElementIndex` |
| `PathFlags` | `m_State` |

被使用：`GroupSystem`, `InitializeSystem`, `TripResetSystem`, `AddHealthProblemSystem`, `PathOwnerTargetMovedSystem`, `PathfindResultSystem`, `TrimPathsSystem`, `AircraftNavigationSystem`, `AmbulanceAISystem`, `AnimalNavigationSystem` …等 37 个

### `PathTargetMoved`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Target` |
| `float3` | `m_OldLocation` |
| `float3` | `m_NewLocation` |

### `PathUpdated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Owner` |
| `PathEventData` | `m_Data` |

被使用：`RouteBufferSystem`, `RoutePathReadySystem`, `SearchSystem`, `SegmentCurveSystem`

---

## Game.Vehicles

*载具 —— 货运/通勤主体* — 43 个组件

### `Aircraft`

| 类型 | 字段 |
|---|---|
| `AircraftFlags` | `m_Flags` |

被使用：`AircraftMoveSystem`, `AircraftNavigationSystem`, `FireAircraftAISystem`, `MedicalAircraftAISystem`, `PoliceAircraftAISystem`, `TransportAircraftAISystem`

### `AircraftCurrentLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `float3` | `m_CurvePosition` |
| `AircraftLaneFlags` | `m_LaneFlags` |
| `float` | `m_Duration` |
| `float` | `m_Distance` |
| `float` | `m_LanePosition` |

被使用：`FixLaneObjectsSystem`, `LaneObjectSystem`, `AircraftMoveSystem`, `AircraftNavigationSystem`, `FireAircraftAISystem`, `MedicalAircraftAISystem`, `PoliceAircraftAISystem`, `TransportAircraftAISystem`, `ApplyObjectsSystem`, `InitializeSystem` …等 11 个

### `AircraftNavigation`

| 类型 | 字段 |
|---|---|
| `float3` | `m_TargetPosition` |
| `float3` | `m_TargetDirection` |
| `float` | `m_MaxSpeed` |
| `float` | `m_MinClimbAngle` |

被使用：`FixLaneObjectsSystem`, `AircraftMoveSystem`, `AircraftNavigationSystem`, `InitializeSystem`

### `Airplane`

*(标记组件，无字段)*

### `Ambulance`

| 类型 | 字段 |
|---|---|
| `AmbulanceFlags` | `m_State` |
| `Entity` | `m_TargetPatient` |
| `Entity` | `m_TargetLocation` |
| `Entity` | `m_TargetRequest` |
| `float` | `m_PathElementTime` |

### `Blocker`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Blocker` |
| `BlockerType` | `m_Type` |
| `byte` | `m_MaxSpeed` |

被使用：`AircraftNavigationSystem`, `AmbulanceAISystem`, `AnimalNavigationSystem`, `CarNavigationSystem`, `HearseAISystem`, `HumanNavigationSystem`, `StuckMovingObjectSystem`, `TaxiAISystem`, `TrafficBottleneckSystem`, `TrainNavigationSystem` …等 11 个

### `Car`

| 类型 | 字段 |
|---|---|
| `CarFlags` | `m_Flags` |

被使用：`LaneDataSystem`, `ObjectInterpolateSystem`, `AccidentSiteSystem`, `AmbulanceAISystem`, `CarMoveSystem`, `CarNavigationSystem`, `FireEngineAISystem`, `GarbageTruckAISystem`, `HearseAISystem`, `MaintenanceVehicleAISystem` …等 18 个

### `CarCurrentLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `Entity` | `m_ChangeLane` |
| `float3` | `m_CurvePosition` |
| `CarLaneFlags` | `m_LaneFlags` |
| `float` | `m_ChangeProgress` |
| `float` | `m_Duration` |
| `float` | `m_Distance` |
| `float` | `m_LanePosition` |

被使用：`ImpactSystem`, `FixLaneObjectsSystem`, `LaneObjectSystem`, `AmbulanceAISystem`, `CarMoveSystem`, `CarNavigationSystem`, `DeliveryTruckAISystem`, `FireEngineAISystem`, `GarbageTruckAISystem`, `HearseAISystem` …等 22 个

### `CarNavigation`

| 类型 | 字段 |
|---|---|
| `float3` | `m_TargetPosition` |
| `quaternion` | `m_TargetRotation` |
| `float` | `m_MaxSpeed` |

被使用：`FixLaneObjectsSystem`, `CarMoveSystem`, `CarNavigationSystem`, `InitializeSystem`

### `CarTrailer`

*(标记组件，无字段)*

被使用：`ImpactSystem`, `ObjectInterpolateSystem`

### `CarTrailerLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `Entity` | `m_NextLane` |
| `float2` | `m_CurvePosition` |
| `float2` | `m_NextPosition` |
| `float` | `m_Duration` |
| `float` | `m_Distance` |

被使用：`ImpactSystem`, `FixLaneObjectsSystem`, `LaneObjectSystem`, `CarNavigationSystem`, `InitializeSystem`, `ReferencesSystem`

### `CargoTransport`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `CargoTransportFlags` | `m_State` |
| `uint` | `m_DepartureFrame` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |

### `Controller`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Controller` |

被使用：`ImpactSystem`, `FixLaneObjectsSystem`, `MarkerCreateSystem`, `MeshColorSystem`, `ObjectColorSystem`, `ObjectInterpolateSystem`, `ControllerSystem`, `AccidentVehicleSystem`, `AnimalNavigationSystem`, `CarNavigationSystem` …等 20 个

### `DeliveryTruck`

| 类型 | 字段 |
|---|---|
| `DeliveryTruckFlags` | `m_State` |
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |

### `EvacuatingTransport`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`, `ComponentsSystem`

### `FireEngine`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `FireEngineFlags` | `m_State` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |
| `float` | `m_ExtinguishingAmount` |
| `float` | `m_Efficiency` |

### `FixParkingLocation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_ChangeLane` |
| `Entity` | `m_ResetLocation` |

被使用：`FixParkingLocationSystem`

### `GarbageTruck`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `GarbageTruckFlags` | `m_State` |
| `int` | `m_RequestCount` |
| `int` | `m_Garbage` |
| `int` | `m_EstimatedGarbage` |
| `float` | `m_PathElementTime` |

### `Hearse`

| 类型 | 字段 |
|---|---|
| `HearseFlags` | `m_State` |
| `Entity` | `m_TargetCorpse` |
| `Entity` | `m_TargetRequest` |
| `float` | `m_PathElementTime` |

### `Helicopter`

*(标记组件，无字段)*

被使用：`FixLaneObjectsSystem`, `AircraftMoveSystem`, `AircraftNavigationSystem`, `FireRescueDispatchSystem`, `FireStationAISystem`, `HealthcareDispatchSystem`, `HospitalAISystem`, `PolicePatrolDispatchSystem`, `PoliceStationAISystem`, `InitializeSystem`

### `MaintenanceVehicle`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `MaintenanceVehicleFlags` | `m_State` |
| `int` | `m_Maintained` |
| `int` | `m_MaintainEstimate` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |
| `float` | `m_Efficiency` |

### `Odometer`

| 类型 | 字段 |
|---|---|
| `float` | `m_Distance` |

被使用：`AircraftNavigationSystem`, `CarNavigationSystem`, `TaxiAISystem`, `TrainNavigationSystem`, `TransportAircraftAISystem`, `TransportCarAISystem`, `TransportLineSystem`, `TransportTrainAISystem`, `TransportWatercraftAISystem`, `WatercraftNavigationSystem` …等 11 个

### `OutOfControl`

*(标记组件，无字段)*

被使用：`FixLaneObjectsSystem`, `CarNavigationSystem`

### `ParkMaintenanceVehicle`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `ParkedCar`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `float` | `m_CurvePosition` |

被使用：`ImpactSystem`, `FixLaneObjectsSystem`, `ParkingLaneDataSystem`, `LaneObjectSystem`, `AmbulanceAISystem`, `HearseAISystem`, `LeisureSystem`, `PersonalCarAISystem`, `ResidentAISystem`, `ResourceBuyerSystem` …等 15 个

### `PassengerTransport`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`, `ComponentsSystem`

### `PersonalCar`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Keeper` |
| `PersonalCarFlags` | `m_State` |

### `PoliceCar`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `PoliceCarFlags` | `m_State` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |
| `uint` | `m_ShiftTime` |
| `uint` | `m_EstimatedShift` |
| `PolicePurpose` | `m_PurposeMask` |

### `PostVan`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `PostVanFlags` | `m_State` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |
| `int` | `m_DeliveringMail` |
| `int` | `m_CollectedMail` |
| `int` | `m_DeliveryEstimate` |
| `int` | `m_CollectEstimate` |

### `PrisonerTransport`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`, `ComponentsSystem`

### `Produced`

| 类型 | 字段 |
|---|---|
| `float` | `m_Completed` |

被使用：`TransportDepotAISystem`, `VehicleLaunchSystem`

### `PublicTransport`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `PublicTransportFlags` | `m_State` |
| `uint` | `m_DepartureFrame` |
| `int` | `m_RequestCount` |
| `float` | `m_PathElementTime` |

### `ReturnLoad`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |

被使用：`DeliveryTruckAISystem`, `GarbageFacilityAISystem`, `PostFacilityAISystem`

### `RoadMaintenanceVehicle`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `Taxi`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `TaxiFlags` | `m_State` |
| `float` | `m_PathElementTime` |
| `float` | `m_StartDistance` |
| `int` | `m_ExtraPathElementCount` |
| `ushort` | `m_NextStartingFee` |
| `ushort` | `m_CurrentFee` |

### `Train`

| 类型 | 字段 |
|---|---|
| `TrainFlags` | `m_Flags` |

被使用：`FixLaneObjectsSystem`, `ObjectInterpolateSystem`, `TransformFrameSystem`, `AnimalNavigationSystem`, `CarNavigationSystem`, `HumanNavigationSystem`, `PetAISystem`, `ResidentAISystem`, `TrainMoveSystem`, `TrainNavigationSystem` …等 12 个

### `TrainCurrentLane`

| 类型 | 字段 |
|---|---|
| `TrainBogieLane` | `m_Front` |
| `TrainBogieLane` | `m_Rear` |
| `TrainBogieCache` | `m_FrontCache` |
| `TrainBogieCache` | `m_RearCache` |
| `float` | `m_Duration` |
| `float` | `m_Distance` |

被使用：`FixLaneObjectsSystem`, `LaneObjectSystem`, `TrainBogieFrameSystem`, `TrafficBottleneckSystem`, `TrainMoveSystem`, `TrainNavigationSystem`, `TransportTrainAISystem`, `ApplyObjectsSystem`, `InitializeSystem`, `ReferencesSystem`

### `TrainNavigation`

| 类型 | 字段 |
|---|---|
| `TrainBogiePosition` | `m_Front` |
| `TrainBogiePosition` | `m_Rear` |
| `float` | `m_Speed` |

被使用：`TrainMoveSystem`, `TrainNavigationSystem`, `TransportTrainAISystem`, `InitializeSystem`

### `Vehicle`

*(标记组件，无字段)*

被使用：`HouseholdAndCitizenRemoveSystem`, `ImpactSystem`, `MarkerCreateSystem`, `QuantityUpdateSystem`, `SubObjectHiddenSystem`, `SubObjectSystem`, `BatchDataSystem`, `ObjectColorSystem`, `AccidentSiteSystem`, `CarNavigationSystem` …等 16 个

### `Watercraft`

| 类型 | 字段 |
|---|---|
| `WatercraftFlags` | `m_Flags` |

被使用：`SubObjectSystem`, `StreetLightSystem`, `TransportWatercraftAISystem`, `WatercraftNavigationSystem`

### `WatercraftCurrentLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `Entity` | `m_ChangeLane` |
| `float3` | `m_CurvePosition` |
| `WatercraftLaneFlags` | `m_LaneFlags` |
| `float` | `m_ChangeProgress` |
| `float` | `m_Duration` |
| `float` | `m_Distance` |
| `float` | `m_LanePosition` |

被使用：`FixLaneObjectsSystem`, `LaneObjectSystem`, `TransportWatercraftAISystem`, `WatercraftNavigationSystem`, `ApplyObjectsSystem`, `InitializeSystem`, `ReferencesSystem`

### `WatercraftNavigation`

| 类型 | 字段 |
|---|---|
| `float3` | `m_TargetPosition` |
| `float3` | `m_TargetDirection` |
| `float` | `m_MaxSpeed` |

被使用：`FixLaneObjectsSystem`, `WatercraftMoveSystem`, `WatercraftNavigationSystem`, `InitializeSystem`

### `WorkVehicle`

| 类型 | 字段 |
|---|---|
| `WorkVehicleFlags` | `m_State` |
| `float` | `m_WorkAmount` |
| `float` | `m_DoneAmount` |

---

## Game.Citizens

*市民 —— 通勤与出行* — 33 个组件

### `Adult`

*(标记组件，无字段)*

### `Arrived`

*(标记组件，无字段)*

被使用：`CitizenInitializeSystem`, `CitizenTravelPurposeSystem`

### `AttendingMeeting`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Meeting` |

被使用：`MeetingInitializeSystem`, `CitizenBehaviorSystem`, `EventTickSystem`, `MeetingSystem`, `ResidentAISystem`, `ResourceBuyerSystem`, `StudentSystem`, `TripNeededSystem`, `WorkerSystem`

### `CarKeeper`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Car` |

被使用：`CitizenInitializeSystem`, `GroupSystem`, `CarKeeperSystem`, `CitizenBehaviorSystem`, `LeisureSystem`, `ResidentAISystem`, `ResourceBuyerSystem`, `StudentSystem`, `TaxiAISystem`, `TripNeededSystem` …等 13 个

### `Child`

*(标记组件，无字段)*

### `Citizen`

| 类型 | 字段 |
|---|---|
| `ushort` | `m_PseudoRandom` |
| `CitizenFlags` | `m_State` |
| `byte` | `m_WellBeing` |
| `byte` | `m_Health` |
| `byte` | `m_LeisureCounter` |
| `byte` | `m_PenaltyCounter` |
| `short` | `m_BirthDay` |

被使用：`AchievementTriggerSystem`, `CitizenInitializeSystem`, `HouseholdAndCitizenRemoveSystem`, `InitializeSystem`, `ObjectEmergeSystem`, `SubObjectSystem`, `RichPresenceUpdateSystem`, `ObjectColorSystem`, `CitizenSystem`, `ResidentPseudoRandomSystem` …等 59 个

### `CommuterHousehold`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_OriginalFrom` |

被使用：`HouseholdInitializeSystem`, `CitizenBehaviorSystem`, `FindEventAttendantsSystem`, `HouseholdBehaviorSystem`, `LookForPartnerSystem`, `PayWageSystem`, `ResourceBuyerSystem`

### `CoordinatedMeeting`

| 类型 | 字段 |
|---|---|
| `MeetingStatus` | `m_Status` |
| `int` | `m_Phase` |
| `Entity` | `m_Target` |
| `uint` | `m_PhaseEndTime` |

被使用：`CitizenBehaviorSystem`, `MeetingSystem`, `ResidentAISystem`, `ResourceBuyerSystem`, `TripNeededSystem`

### `CrimeVictim`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Effect` |

被使用：`CitizenInitializeSystem`, `CitizenHappinessSystem`, `CrimeEffectSystem`, `CriminalSystem`

### `Criminal`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Event` |
| `ushort` | `m_JailTime` |
| `CriminalFlags` | `m_Flags` |

被使用：`AddCriminalSystem`, `AccidentSiteSystem`, `CrimeCheckSystem`, `CriminalSystem`, `EventTickSystem`, `PoliceStationAISystem`

### `CurrentBuilding`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_CurrentBuilding` |

被使用：`HouseholdAndCitizenRemoveSystem`, `HouseholdInitializeSystem`, `AddHealthProblemSystem`, `IconCommandSystem`, `ObjectEmergeSystem`, `AmbulanceAISystem`, `CitizenBehaviorSystem`, `CitizenEvacuateSystem`, `CitizenFindJobSystem`, `CitizenTravelPurposeSystem` …等 29 个

### `CurrentTransport`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_CurrentTransport` |

被使用：`HouseholdAndCitizenRemoveSystem`, `HouseholdPetRemoveSystem`, `GroupSystem`, `ReferencesSystem`, `AddHealthProblemSystem`, `IconCommandSystem`, `ObjectEmergeSystem`, `PassengerSystem`, `PetSystem`, `ResidentSystem` …等 22 个

### `Elderly`

*(标记组件，无字段)*

### `Followed`

| 类型 | 字段 |
|---|---|
| `uint` | `m_Priority` |
| `bool` | `m_StartedFollowingAsChild` |

被使用：`AchievementTriggerSystem`

### `HappinessEffect`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_HomeEntity` |
| `Entity` | `m_WorkEntity` |
| `Entity` | `m_ClientEntity` |

被使用：`CitizenHappinessSystem`, `HappinessAdjustSystem`

### `HasSchoolSeeker`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Seeker` |

被使用：`HouseholdAndCitizenRemoveSystem`

### `HealthProblem`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Event` |
| `Entity` | `m_HealthcareRequest` |
| `HealthProblemFlags` | `m_Flags` |
| `byte` | `m_Timer` |

被使用：`AddHealthProblemSystem`, `ObjectEmergeSystem`, `RichPresenceUpdateSystem`, `ObjectColorSystem`, `AmbulanceAISystem`, `CitizenBehaviorSystem`, `CitizenHappinessSystem`, `CitizenTravelPurposeSystem`, `CountCompanyDataSystem`, `CountEmploymentSystem` …等 33 个

### `HomelessHousehold`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TempHome` |

被使用：`InitializeSystem`, `SubObjectSystem`, `CitizenHappinessSystem`, `CommercialFindPropertySystem`, `EmergencyShelterAISystem`, `HomelessShelterAISystem`, `HouseholdBehaviorSystem`, `HouseholdFindPropertySystem`, `IndustrialFindPropertySystem`, `CreateChirpSystem`

### `Household`

| 类型 | 字段 |
|---|---|
| `HouseholdFlags` | `m_Flags` |
| `int` | `m_Resources` |
| `short` | `m_LastConsumption` |

被使用：`HouseholdInitializeSystem`, `HouseholdPetInitializeSystem`, `SubObjectSystem`, `RichPresenceUpdateSystem`, `ObjectColorSystem`, `AgingSystem`, `ApplyToSchoolSystem`, `CitizenBehaviorSystem`, `CitizenHappinessSystem`, `CitizenTravelPurposeSystem` …等 37 个

### `HouseholdMember`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Household` |

被使用：`HouseholdAndCitizenRemoveSystem`, `GroupSystem`, `InitializeSystem`, `AddHealthProblemSystem`, `HouseholdCitizenSystem`, `AgingSystem`, `ApplyToSchoolSystem`, `BirthSystem`, `CitizenBehaviorSystem`, `CitizenFindJobSystem` …等 37 个

### `HouseholdNeed`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |

被使用：`CitizenBehaviorSystem`, `HouseholdBehaviorSystem`, `ResidentAISystem`

### `HouseholdPet`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Household` |

被使用：`HouseholdAndCitizenRemoveSystem`, `HouseholdPetInitializeSystem`, `HouseholdPetRemoveSystem`, `GroupSystem`, `ObjectEmergeSystem`, `HouseholdAnimalSystem`, `HouseholdPetBehaviorSystem`, `PartnerSystem`

### `JobSeekerCooldown`

| 类型 | 字段 |
|---|---|
| `uint` | `m_SimulationFrame` |

被使用：`CitizenFindJobSystem`

### `Leisure`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetAgent` |
| `uint` | `m_LastPossibleFrame` |

被使用：`CitizenBehaviorSystem`, `DeathCheckSystem`, `LeisureSystem`

### `LodgingSeeker`

*(标记组件，无字段)*

被使用：`HouseholdBehaviorSystem`, `TouristHouseholdBehaviorSystem`

### `MailSender`

| 类型 | 字段 |
|---|---|
| `ushort` | `m_Amount` |

被使用：`CitizenInitializeSystem`, `CitizenBehaviorSystem`, `TripNeededSystem`

### `ResourceBought`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Seller` |
| `Entity` | `m_Payer` |
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |
| `float` | `m_Distance` |

被使用：`ResourceBuyerSystem`

### `SchoolSeeker`

| 类型 | 字段 |
|---|---|
| `int` | `m_Level` |

被使用：`FindSchoolSystem`

### `Student`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_School` |
| `float` | `m_LastCommuteTime` |
| `byte` | `m_Level` |

### `Teen`

*(标记组件，无字段)*

### `TouristHousehold`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Hotel` |
| `uint` | `m_LeavingTime` |

被使用：`HouseholdInitializeSystem`, `ApplyToSchoolSystem`, `CitizenBehaviorSystem`, `CitizenFindJobSystem`, `EmergencyShelterAISystem`, `HouseholdBehaviorSystem`, `HouseholdMoveAwaySystem`, `LeisureSystem`, `LodgingProviderSystem`, `LookForPartnerSystem` …等 16 个

### `TravelPurpose`

| 类型 | 字段 |
|---|---|
| `Purpose` | `m_Purpose` |
| `int` | `m_Data` |
| `Resource` | `m_Resource` |

被使用：`SchoolUpdatedSystem`, `TripResetSystem`, `AgingSystem`, `AmbulanceAISystem`, `CitizenTravelPurposeSystem`, `CriminalSystem`, `EmergencyShelterAISystem`, `GraduationSystem`, `HealthProblemSystem`, `HearseAISystem` …等 20 个

### `Worker`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Workplace` |
| `float` | `m_LastCommuteTime` |
| `byte` | `m_Level` |
| `Workshift` | `m_Shift` |

被使用：`InitializeSystem`, `ObjectColorSystem`, `ApplyToSchoolSystem`, `CitizenBehaviorSystem`, `CitizenFindJobSystem`, `CitizenTravelPurposeSystem`, `CountEmploymentSystem`, `DeathCheckSystem`, `EmergencyShelterAISystem`, `FindJobSystem` …等 27 个

---

## Game.Companies

*公司 —— 货运需求与盈利* — 24 个组件

### `BuyingCompany`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_LastTradePartner` |
| `float` | `m_MeanInputTripLength` |

被使用：`RentAdjustSystem`, `ResourceBuyerSystem`

### `CommercialCompany`

*(标记组件，无字段)*

被使用：`CommercialDemandSystem`, `CommercialFindPropertySystem`, `CompanyStatisticsSystem`, `HouseholdFindPropertySystem`, `IndustrialFindPropertySystem`

### `CompanyData`

| 类型 | 字段 |
|---|---|
| `Random` | `m_RandomSeed` |
| `Entity` | `m_Brand` |

被使用：`CompanyInitializeSystem`, `StorageInitializeSystem`, `BrandPopularitySystem`, `SubObjectSystem`, `MeshColorSystem`, `PrimaryPrefabReferencesSystem`, `AreaSpawnSystem`, `CommercialFindPropertySystem`, `ExtractorCompanySystem`, `GoodsDeliveryRequestSystem` …等 16 个

### `CompanyNotifications`

| 类型 | 字段 |
|---|---|
| `short` | `m_NoInputCounter` |
| `short` | `m_NoCustomersCounter` |
| `Entity` | `m_NoInputEntity` |
| `Entity` | `m_NoCustomersEntity` |

被使用：`BuyingCompanySystem`, `RemovedSystem`, `RentAdjustSystem`, `ServiceCompanySystem`

### `CompanyTransportInstance`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Company` |

### `Employer`

| 类型 | 字段 |
|---|---|
| `int` | `m_Workers` |

### `ExtractorCompany`

*(标记组件，无字段)*

### `FreeWorkplaces`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Uneducated` |
| `byte` | `m_PoorlyEducated` |
| `byte` | `m_Educated` |
| `byte` | `m_WellEducated` |
| `byte` | `m_HighlyEducated` |

被使用：`CountFreeWorkplacesSystem`, `FindJobSystem`, `WorkProviderStatisticsSystem`

### `IndustrialCompany`

*(标记组件，无字段)*

被使用：`CommercialFindPropertySystem`, `CompanyDividendSystem`, `HouseholdFindPropertySystem`, `IndustrialFindPropertySystem`

### `LodgingProvider`

| 类型 | 字段 |
|---|---|
| `int` | `m_FreeRooms` |
| `int` | `m_Price` |

被使用：`CompanyInitializeSystem`, `HouseholdBehaviorSystem`, `LodgingProviderSystem`, `TourismSystem`, `TouristFindLodgingSystem`, `TouristLeaveSystem`

### `OutsideTrader`

*(标记组件，无字段)*

### `ProcessingCompany`

*(标记组件，无字段)*

### `Profitability`

| 类型 | 字段 |
|---|---|
| `byte` | `m_Profitability` |

被使用：`CompanyInitializeSystem`, `ObjectColorSystem`, `CommercialAISystem`, `ExtractorAISystem`, `IndustrialAISystem`

### `ResourceBuyer`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Payer` |
| `SetupTargetFlags` | `m_Flags` |
| `Resource` | `m_ResourceNeeded` |
| `int` | `m_AmountNeeded` |
| `float3` | `m_Location` |

被使用：`DeathCheckSystem`, `ResourceBuyerSystem`

### `ResourceExporter`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |

被使用：`ResourceExporterSystem`

### `ResourceSeller`

*(标记组件，无字段)*

### `ServiceAvailable`

| 类型 | 字段 |
|---|---|
| `int` | `m_ServiceAvailable` |
| `float` | `m_MeanPriority` |

被使用：`CompanyInitializeSystem`, `CommercialAISystem`, `CompanyBankruptcySystem`, `CountCompanyDataSystem`, `LeisureSystem`, `LodgingProviderSystem`, `ProcessingCompanySystem`, `RentAdjustSystem`, `ResidentAISystem`, `ResourceAvailabilitySystem` …等 12 个

### `ServiceCompanyData`

| 类型 | 字段 |
|---|---|
| `int` | `m_MaxService` |
| `int` | `m_WorkPerUnit` |
| `float` | `m_MaxWorkersPerCell` |
| `int` | `m_ServiceConsuming` |

被使用：`CompanyInitializeSystem`, `CompanyPrefabInitializeSystem`, `CommercialAISystem`, `CommercialFindPropertySystem`, `CountCompanyDataSystem`, `HouseholdFindPropertySystem`, `IndustrialFindPropertySystem`, `LeisureSystem`, `LodgingProviderSystem`, `RentAdjustSystem` …等 12 个

### `StorageCompany`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_LastTradePartner` |

### `StorageLimitData`

| 类型 | 字段 |
|---|---|
| `int` | `m_Limit` |

被使用：`BuyingCompanySystem`, `CityServiceUpkeepSystem`, `ExtractorCompanySystem`, `IndustrialDemandSystem`, `ProcessingCompanySystem`, `StorageCompanySystem`, `StorageTransferSystem`, `TradeSystem`

### `StorageTransfer`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resource` |
| `int` | `m_Amount` |

被使用：`StorageTransferSystem`

### `TransportCompany`

*(标记组件，无字段)*

### `TransportCompanyData`

| 类型 | 字段 |
|---|---|
| `int` | `m_MaxTransports` |

被使用：`TripNeededSystem`

### `WorkProvider`

| 类型 | 字段 |
|---|---|
| `int` | `m_MaxWorkers` |
| `short` | `m_UneducatedCooldown` |
| `short` | `m_EducatedCooldown` |
| `Entity` | `m_UneducatedNotificationEntity` |
| `Entity` | `m_EducatedNotificationEntity` |
| `short` | `m_EfficiencyCooldown` |

被使用：`WorkplaceInitializeSystem`, `ObjectColorSystem`, `CitizenPresenceSystem`, `CitizenTravelPurposeSystem`, `CityServiceBudgetSystem`, `CityServiceStatisticsSystem`, `CommercialAISystem`, `CommercialFindPropertySystem`, `CompanyStatisticsSystem`, `CountCompanyDataSystem` …等 27 个

---

## Game.Buildings

*建筑 —— 废弃/服务/需求的落点* — 79 个组件

### `Abandoned`

| 类型 | 字段 |
|---|---|
| `uint` | `m_AbandonmentTime` |

被使用：`BuildingStateEfficiencySystem`, `InitializeSystem`, `ZoneCheckSystem`, `BatchDataSystem`, `ObjectColorSystem`, `BuildingPollutionAddSystem`, `CommercialFindPropertySystem`, `DestroyAbandonedSystem`, `DirtynessSystem`, `HouseholdFindPropertySystem` …等 14 个

### `AdminBuilding`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `AttractivenessProvider`

| 类型 | 字段 |
|---|---|
| `int` | `m_Attractiveness` |

被使用：`ObjectColorSystem`, `AttractionSystem`, `ResidentAISystem`, `ResourceAvailabilitySystem`, `TourismSystem`

### `Battery`

| 类型 | 字段 |
|---|---|
| `long` | `m_StoredEnergy` |
| `int` | `m_Capacity` |
| `int` | `m_LastFlow` |

被使用：`BatteryInitializeSystem`, `ElectricityStatisticsSystem`

### `Building`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_RoadEdge` |
| `float` | `m_CurvePosition` |
| `uint` | `m_OptionMask` |
| `BuildingFlags` | `m_Flags` |

被使用：`GeometrySystem`, `SurfaceExpandSystem`, `BuildingPoliciesSystem`, `BuildingStateEfficiencySystem`, `InitializeSchoolSystem`, `InitializeSystem`, `RoadConnectionSystem`, `ZoneCheckSystem`, `FaceWeatherSystem`, `IgniteSystem` …等 85 个

### `BuildingCondition`

| 类型 | 字段 |
|---|---|
| `int` | `m_Condition` |

被使用：`ObjectColorSystem`, `BuildingUpkeepSystem`, `DirtynessSystem`, `GarbageTruckAISystem`, `PropertyRenterSystem`

### `BuildingEfficiency`

*(标记组件，无字段)*

### `BuildingNotifications`

| 类型 | 字段 |
|---|---|
| `BuildingNotification` | `m_Notifications` |

被使用：`RentAdjustSystem`

### `CargoTransportStation`

*(标记组件，无字段)*

### `CitizenPresence`

| 类型 | 字段 |
|---|---|
| `sbyte` | `m_Delta` |
| `byte` | `m_Presence` |

被使用：`BatchDataSystem`, `CitizenBehaviorSystem`, `CitizenPresenceSystem`, `CitizenTravelPurposeSystem`, `TripNeededSystem`

### `CityEffectProvider`

*(标记组件，无字段)*

### `CommercialProperty`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resources` |

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `Condemned`

*(标记组件，无字段)*

被使用：`ZoneCheckSystem`

### `CrimeProducer`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_PatrolRequest` |
| `float` | `m_Crime` |

被使用：`AddAccidentSiteSystem`, `ObjectColorSystem`, `CitizenHappinessSystem`, `CrimeAccumulationSystem`, `CrimeStatisticsSystem`, `HouseholdFindPropertySystem`, `PoliceAircraftAISystem`, `PoliceCarAISystem`, `PolicePatrolDispatchSystem`, `PropertyRenterSystem` …等 12 个

### `DeathcareFacility`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `DeathcareFacilityFlags` | `m_Flags` |
| `float` | `m_ProcessingState` |
| `int` | `m_LongTermStoredCount` |

被使用：`HealthcareDispatchSystem`

### `DisasterFacility`

*(标记组件，无字段)*

### `EarlyDisasterWarningSystem`

| 类型 | 字段 |
|---|---|
| `bool` | `m_TurnedOn` |

### `ElectricityConsumer`

| 类型 | 字段 |
|---|---|
| `int` | `m_WantedConsumption` |
| `int` | `m_FulfilledConsumption` |
| `short` | `m_CooldownCounter` |
| `ElectricityConsumerFlags` | `m_Flags` |

被使用：`InitializeSystem`, `RoadConnectionSystem`, `DestroySystem`, `NetColorSystem`, `ObjectColorSystem`, `ObjectInterpolateSystem`, `AdjustElectricityConsumptionSystem`, `CitizenHappinessSystem`, `DispatchElectricitySystem`, `ElectricityBuildingGraphSystem` …等 18 个

### `ElectricityProducer`

| 类型 | 字段 |
|---|---|
| `int` | `m_Capacity` |
| `int` | `m_LastProduction` |

被使用：`ConnectionWarningSystem`, `MarkerCreateSystem`, `ObjectColorSystem`, `ElectricityBuildingGraphSystem`, `ElectricityStatisticsSystem`, `PowerPlantAISystem`

### `EmergencyGenerator`

| 类型 | 字段 |
|---|---|
| `int` | `m_Production` |

### `EmergencyShelter`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `EmergencyShelterFlags` | `m_Flags` |

被使用：`EvacuationDispatchSystem`

### `Extension`

*(标记组件，无字段)*

被使用：`SurfaceExpandSystem`, `BuildingLotRenderSystem`, `PreCullingSystem`, `CollapsedBuildingSystem`

### `ExtractorFacility`

| 类型 | 字段 |
|---|---|
| `ExtractorFlags` | `m_Flags` |
| `byte` | `m_Timer` |

### `ExtractorProperty`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`, `RentAdjustSystem`

### `FireStation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `FireStationFlags` | `m_Flags` |

被使用：`FireRescueDispatchSystem`

### `FirewatchTower`

| 类型 | 字段 |
|---|---|
| `FirewatchTowerFlags` | `m_Flags` |

### `GarbageFacility`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_GarbageDeliverRequest` |
| `Entity` | `m_GarbageReceiveRequest` |
| `Entity` | `m_TargetRequest` |
| `GarbageFacilityFlags` | `m_Flags` |
| `float` | `m_AcceptGarbagePriority` |
| `float` | `m_DeliverGarbagePriority` |
| `int` | `m_ProcessingRate` |

被使用：`GarbageTransferDispatchSystem`

### `GarbageProducer`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_CollectionRequest` |
| `int` | `m_Garbage` |
| `GarbageProducerFlags` | `m_Flags` |

被使用：`InitializeSystem`, `QuantityUpdateSystem`, `SubObjectSystem`, `ObjectColorSystem`, `CitizenHappinessSystem`, `GarbageAccumulationSystem`, `GarbageCollectorDispatchSystem`, `GarbageTruckAISystem`, `HouseholdFindPropertySystem`, `PropertyRenterSystem` …等 12 个

### `GroundPolluter`

*(标记组件，无字段)*

被使用：`InitializeSystem`, `PropertyRenterSystem`

### `HappinessAdjuster`

*(标记组件，无字段)*

### `Hospital`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `HospitalFlags` | `m_Flags` |
| `byte` | `m_TreatmentBonus` |
| `byte` | `m_MinHealth` |
| `byte` | `m_MaxHealth` |

被使用：`HealthcareDispatchSystem`

### `IndustrialProperty`

| 类型 | 字段 |
|---|---|
| `Resource` | `m_Resources` |

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `LeisureProvider`

*(标记组件，无字段)*

### `LocalEffectProvider`

*(标记组件，无字段)*

### `Lot`

| 类型 | 字段 |
|---|---|
| `float3` | `m_FrontHeights` |
| `float3` | `m_RightHeights` |
| `float3` | `m_BackHeights` |
| `float3` | `m_LeftHeights` |

### `MailProducer`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_MailRequest` |
| `ushort` | `m_SendingMail` |
| `ushort` | `m_ReceivingMail` |

被使用：`InitializeSystem`, `QuantityUpdateSystem`, `SubObjectSystem`, `ObjectColorSystem`, `CitizenBehaviorSystem`, `CitizenHappinessSystem`, `HouseholdFindPropertySystem`, `MailAccumulationSystem`, `PostVanAISystem`, `PostVanDispatchSystem` …等 13 个

### `MaintenanceDepot`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `MaintenanceDepotFlags` | `m_Flags` |

### `ModifiedServiceCoverage`

| 类型 | 字段 |
|---|---|
| `float` | `m_Range` |
| `float` | `m_Capacity` |
| `float` | `m_Magnitude` |

被使用：`ParkInitializeSystem`, `ParkAISystem`, `ServiceCoverageSystem`, `CoveragePreviewSystem`

### `OfficeProperty`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `Park`

| 类型 | 字段 |
|---|---|
| `short` | `m_Maintenance` |

被使用：`ParkInitializeSystem`

### `ParkMaintenance`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `ParkingFacility`

| 类型 | 字段 |
|---|---|
| `float` | `m_ComfortFactor` |
| `ParkingFacilityFlags` | `m_Flags` |

### `PoliceStation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_PrisonerTransportRequest` |
| `Entity` | `m_TargetRequest` |
| `PoliceStationFlags` | `m_Flags` |
| `PolicePurpose` | `m_PurposeMask` |

被使用：`InitializeSystem`, `PrisonerTransportDispatchSystem`

### `PostFacility`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_MailDeliverRequest` |
| `Entity` | `m_MailReceiveRequest` |
| `Entity` | `m_TargetRequest` |
| `float` | `m_AcceptMailPriority` |
| `float` | `m_DeliverMailPriority` |
| `PostFacilityFlags` | `m_Flags` |
| `byte` | `m_ProcessingFactor` |

被使用：`InitializeSystem`, `MailTransferDispatchSystem`

### `Prison`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `PrisonFlags` | `m_Flags` |

被使用：`PrisonerTransportDispatchSystem`

### `Property`

*(标记组件，无字段)*

### `PropertyOnMarket`

| 类型 | 字段 |
|---|---|
| `int` | `m_AskingRent` |

被使用：`CommercialFindPropertySystem`, `CompanyBankruptcySystem`, `HouseholdFindPropertySystem`, `IndustrialFindPropertySystem`, `PropertyRenterSystem`, `RentAdjustSystem`, `RentInitializeSystem`

### `PropertyRenter`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Property` |
| `int` | `m_Rent` |
| `int` | `m_MaxRent` |

被使用：`HouseholdAndCitizenRemoveSystem`, `InitializeSystem`, `ObjectColorSystem`, `RenterSystem`, `AircraftNavigationSystem`, `AnimalNavigationSystem`, `ApplyToSchoolSystem`, `BirthSystem`, `BuyingCompanySystem`, `CarNavigationSystem` …等 57 个

### `PropertyToBeOnMarket`

*(标记组件，无字段)*

### `PublicTransportStation`

*(标记组件，无字段)*

### `RenewableElectricityProduction`

*(标记组件，无字段)*

### `RentersUpdated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Property` |

被使用：`SubObjectSystem`, `MeshColorSystem`, `PreCullingSystem`, `RemovedSystem`

### `RescueTarget`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Request` |

被使用：`CollapsedBuildingSystem`, `FireAircraftAISystem`, `FireEngineAISystem`, `FireRescueDispatchSystem`

### `ResearchFacility`

*(标记组件，无字段)*

### `ResidentialProperty`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `SubObjectSystem`, `ObjectColorSystem`

### `ResourceConsumer`

| 类型 | 字段 |
|---|---|
| `byte` | `m_ResourceAvailability` |

被使用：`ResourcesInitializeSystem`

### `ResourceProducer`

*(标记组件，无字段)*

### `RoadConnectionUpdated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Building` |
| `Entity` | `m_Old` |
| `Entity` | `m_New` |

被使用：`ConnectionWarningSystem`, `SpawnLocationConnectionSystem`, `ElectricityRoadConnectionGraphSystem`, `WaterPipeRoadConnectionGraphSystem`

### `RoadMaintenance`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `School`

| 类型 | 字段 |
|---|---|
| `float` | `m_AverageGraduationTime` |
| `float` | `m_AverageFailProbability` |

被使用：`InitializeSchoolSystem`

### `ServiceUpgrade`

*(标记组件，无字段)*

被使用：`ServiceUpgradeSystem`

### `ServiceUsage`

| 类型 | 字段 |
|---|---|
| `float` | `m_Usage` |

被使用：`BatteryAISystem`, `CityServiceBudgetSystem`, `CityServiceUpkeepSystem`, `EmergencyShelterAISystem`, `HospitalAISystem`, `PowerPlantAISystem`

### `SewageOutlet`

| 类型 | 字段 |
|---|---|
| `int` | `m_Capacity` |
| `int` | `m_LastProcessed` |
| `int` | `m_LastPurified` |
| `int` | `m_UsedPurified` |

被使用：`WaterStatisticsSystem`

### `Signature`

*(标记组件，无字段)*

被使用：`LocalEffectSystem`, `AttractionSystem`

### `StorageProperty`

*(标记组件，无字段)*

被使用：`ObjectColorSystem`, `AdjustElectricityConsumptionSystem`, `AdjustWaterConsumptionSystem`

### `StudentsRemoved`

*(标记组件，无字段)*

### `TelecomConsumer`

*(标记组件，无字段)*

### `TelecomFacility`

| 类型 | 字段 |
|---|---|
| `TelecomFacilityFlags` | `m_Flags` |

### `TrafficSpawner`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TrafficRequest` |

被使用：`RandomTrafficDispatchSystem`

### `Transformer`

*(标记组件，无字段)*

### `TransportDepot`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TargetRequest` |
| `TransportDepotFlags` | `m_Flags` |
| `byte` | `m_AvailableVehicles` |
| `float` | `m_MaintenanceRequirement` |

被使用：`TaxiDispatchSystem`

### `TransportStation`

| 类型 | 字段 |
|---|---|
| `float` | `m_ComfortFactor` |
| `float` | `m_LoadingFactor` |
| `EnergyTypes` | `m_CarRefuelTypes` |
| `EnergyTypes` | `m_TrainRefuelTypes` |
| `EnergyTypes` | `m_WatercraftRefuelTypes` |
| `EnergyTypes` | `m_AircraftRefuelTypes` |
| `TransportStationFlags` | `m_Flags` |

### `Warehouse`

*(标记组件，无字段)*

### `WastewaterTreatmentPlant`

| 类型 | 字段 |
|---|---|
| `int` | `m_StoredWater` |
| `int` | `m_LastStoredWater` |

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `WaterConsumer`

| 类型 | 字段 |
|---|---|
| `float` | `m_Pollution` |
| `int` | `m_WantedConsumption` |
| `int` | `m_FulfilledFresh` |
| `int` | `m_FulfilledSewage` |
| `byte` | `m_FreshCooldownCounter` |
| `byte` | `m_SewageCooldownCounter` |
| `WaterConsumerFlags` | `m_Flags` |

被使用：`InitializeSystem`, `RoadConnectionSystem`, `DestroySystem`, `NetColorSystem`, `ObjectColorSystem`, `AdjustWaterConsumptionSystem`, `CitizenHappinessSystem`, `DispatchWaterSystem`, `HouseholdFindPropertySystem`, `PropertyRenterSystem` …等 16 个

### `WaterPowered`

| 类型 | 字段 |
|---|---|
| `float` | `m_Length` |
| `float` | `m_Height` |
| `float` | `m_Estimate` |

被使用：`WaterPoweredInitializeSystem`

### `WaterPumpingStation`

| 类型 | 字段 |
|---|---|
| `float` | `m_Pollution` |
| `int` | `m_Capacity` |
| `int` | `m_LastProduction` |

被使用：`WaterStatisticsSystem`

### `WaterTower`

| 类型 | 字段 |
|---|---|
| `int` | `m_StoredWater` |
| `int` | `m_Polluted` |
| `int` | `m_LastStoredWater` |

### `WelfareOffice`

*(标记组件，无字段)*

---

## Game.Zones

*分区* — 4 个组件

### `Block`

| 类型 | 字段 |
|---|---|
| `float3` | `m_Position` |
| `float2` | `m_Direction` |
| `int2` | `m_Size` |

被使用：`BlockSystem`, `CellCheckSystem`, `SearchSystem`, `UpdateCollectSystem`

### `BuildOrder`

| 类型 | 字段 |
|---|---|
| `uint` | `m_Order` |

### `CurvePosition`

| 类型 | 字段 |
|---|---|
| `float2` | `m_CurvePosition` |

被使用：`ZoneSpawnSystem`

### `ValidArea`

| 类型 | 字段 |
|---|---|
| `int4` | `m_Area` |

被使用：`ZoneCheckSystem`, `PropertyRenterSystem`, `ZoneSpawnSystem`, `CellCheckSystem`

---

## Game.Routes

*公交线路* — 29 个组件

### `AccessLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Lane` |
| `float` | `m_CurvePos` |

被使用：`AreaConnectionSystem`, `SurfaceExpandSystem`, `ConnectionWarningSystem`, `RoutesModifiedSystem`, `WaypointConnectionSystem`, `ResidentAISystem`

### `AirplaneStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `BoardingVehicle`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Vehicle` |
| `Entity` | `m_Testing` |

被使用：`BoardingVehicleSystem`, `ResidentAISystem`, `TaxiAISystem`, `TaxiDispatchSystem`, `TransportAircraftAISystem`, `TransportCarAISystem`, `TransportTrainAISystem`, `TransportWatercraftAISystem`

### `BusStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `Color`

| 类型 | 字段 |
|---|---|
| `Color32` | `m_Color` |

### `ColorUpdated`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Route` |

被使用：`MeshColorSystem`, `PreCullingSystem`

### `Connected`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Connected` |

被使用：`IconCommandSystem`, `RoutesModifiedSystem`, `BoardingVehicleSystem`, `WaypointConnectionSystem`, `ConnectedRouteSystem`, `HumanNavigationSystem`, `ResidentAISystem`, `StorageCompanySystem`, `StorageTransferSystem`, `TransportAircraftAISystem` …等 15 个

### `CurrentRoute`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Route` |

被使用：`MeshColorSystem`, `RouteVehicleSystem`, `ResidentAISystem`, `TaxiAISystem`, `TaxiDispatchSystem`, `TaxiStandSystem`, `TransportAircraftAISystem`, `TransportCarAISystem`, `TransportLineSystem`, `TransportTrainAISystem` …等 12 个

### `HiddenRoute`

*(标记组件，无字段)*

被使用：`RaycastSystem`

### `MailBox`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_CollectRequest` |
| `int` | `m_MailAmount` |

### `PathTargets`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_StartLane` |
| `Entity` | `m_EndLane` |
| `float2` | `m_CurvePositions` |
| `float3` | `m_ReadyStartPosition` |
| `float3` | `m_ReadyEndPosition` |

被使用：`RoutePathReadySystem`, `RoutePathSystem`, `SegmentCurveSystem`, `WaypointConnectionSystem`, `ApplyRoutesSystem`, `GenerateWaypointsSystem`

### `Position`

| 类型 | 字段 |
|---|---|
| `float3` | `m_Position` |

被使用：`RaycastSystem`, `IconCommandSystem`, `RoutesModifiedSystem`, `RouteBufferSystem`, `RoutePathReadySystem`, `RoutePathSystem`, `SearchSystem`, `SegmentCurveSystem`, `WaypointConnectionSystem`, `AircraftNavigationSystem` …等 17 个

### `Route`

| 类型 | 字段 |
|---|---|
| `RouteFlags` | `m_Flags` |
| `uint` | `m_OptionMask` |

被使用：`ModifiedSystem`, `RouteModifierInitializeSystem`, `GuideLinesSystem`, `TransportLineSystem`

### `RouteInfo`

| 类型 | 字段 |
|---|---|
| `float` | `m_Duration` |
| `float` | `m_Distance` |
| `RouteInfoFlags` | `m_Flags` |

被使用：`RoutesModifiedSystem`, `TransportLineSystem`, `ApplyRoutesSystem`

### `RouteLane`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_StartLane` |
| `Entity` | `m_EndLane` |
| `float` | `m_StartCurvePos` |
| `float` | `m_EndCurvePos` |

被使用：`AreaConnectionSystem`, `SurfaceExpandSystem`, `ConnectionWarningSystem`, `RoutesModifiedSystem`, `RoutePathSystem`, `WaypointConnectionSystem`, `TaxiAISystem`, `TaxiStandSystem`, `TransportAircraftAISystem`, `TransportCarAISystem` …等 11 个

### `RouteNumber`

| 类型 | 字段 |
|---|---|
| `int` | `m_Number` |

被使用：`InitializeSystem`

### `Segment`

| 类型 | 字段 |
|---|---|
| `int` | `m_Index` |

### `ShipStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `SubwayStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `TakeoffLocation`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |

### `TaxiStand`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_TaxiRequest` |
| `TaxiStandFlags` | `m_Flags` |
| `ushort` | `m_StartingFee` |

被使用：`MarkerCreateSystem`, `RoutesModifiedSystem`, `ObjectColorSystem`, `AnimalNavigationSystem`, `HumanNavigationSystem`, `TaxiAISystem`, `TaxiDispatchSystem`, `TaxiStandSystem`, `TransportStopSystem`

### `TrainStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `TramStop`

*(标记组件，无字段)*

被使用：`MarkerCreateSystem`, `ObjectColorSystem`

### `TransportLine`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_VehicleRequest` |
| `float` | `m_VehicleInterval` |
| `float` | `m_UnbunchingFactor` |
| `TransportLineFlags` | `m_Flags` |
| `ushort` | `m_TicketPrice` |

被使用：`RoutesModifiedSystem`, `ResidentAISystem`, `TransportLineSystem`, `TransportVehicleDispatchSystem`

### `TransportStop`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_AccessRestriction` |
| `float` | `m_ComfortFactor` |
| `float` | `m_LoadingFactor` |
| `StopFlags` | `m_Flags` |

### `VehicleModel`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_PrimaryPrefab` |
| `Entity` | `m_SecondaryPrefab` |

被使用：`InitializeSystem`, `PrimaryPrefabReferencesSystem`, `TransportDepotAISystem`, `TransportLineSystem`

### `VehicleTiming`

| 类型 | 字段 |
|---|---|
| `uint` | `m_LastDepartureFrame` |
| `float` | `m_AverageTravelTime` |

被使用：`TransportLineSystem`, `ApplyRoutesSystem`

### `WaitingPassengers`

| 类型 | 字段 |
|---|---|
| `int` | `m_Count` |
| `int` | `m_OngoingAccumulation` |
| `int` | `m_ConcludedAccumulation` |
| `ushort` | `m_SuccessAccumulation` |
| `ushort` | `m_AverageWaitingTime` |

被使用：`RoutesModifiedSystem`, `TaxiAISystem`, `TaxiStandSystem`, `WaitingPassengersSystem`

### `Waypoint`

| 类型 | 字段 |
|---|---|
| `int` | `m_Index` |

被使用：`RaycastSystem`, `RoutesModifiedSystem`, `WaypointConnectionSystem`, `RouteWaypointSystem`, `HumanNavigationSystem`, `TransportAircraftAISystem`, `TransportCarAISystem`, `TransportTrainAISystem`, `TransportWatercraftAISystem`, `ApplyRoutesSystem` …等 11 个

---

## Game.City

*城市级聚合* — 12 个组件

### `City`

| 类型 | 字段 |
|---|---|
| `uint` | `m_OptionMask` |

### `CityServiceUpkeep`

*(标记组件，无字段)*

被使用：`WorkplaceInitializeSystem`, `RenterSystem`, `AdjustElectricityConsumptionSystem`, `AdjustWaterConsumptionSystem`, `GoodsDeliveryRequestSystem`, `IndustrialDemandSystem`

### `DangerLevel`

| 类型 | 字段 |
|---|---|
| `float` | `m_DangerLevel` |

### `DevTreePoints`

| 类型 | 字段 |
|---|---|
| `int` | `m_Points` |

被使用：`DevTreeSystem`

### `MilestoneLevel`

| 类型 | 字段 |
|---|---|
| `int` | `m_AchievedMilestone` |

### `MilestoneReachedEvent`

| 类型 | 字段 |
|---|---|
| `Entity` | `m_Milestone` |
| `int` | `m_Index` |

### `PlayerMoney`

| 类型 | 字段 |
|---|---|
| `int` | `m_Money` |
| `bool` | `m_Unlimited` |

被使用：`BudgetApplySystem`, `CityServiceUpkeepSystem`, `LoanUpdateSystem`, `LoanSystem`, `ToolApplySystem`, `ValidationSystem`

### `Population`

| 类型 | 字段 |
|---|---|
| `int` | `m_Population` |
| `int` | `m_PopulationWithMoveIn` |
| `int` | `m_AverageHappiness` |

被使用：`CitizenBehaviorSystem`, `CityServiceBudgetSystem`, `CommercialDemandSystem`, `CommercialSpawnSystem`, `CountPopulationSystem`, `HouseholdBehaviorSystem`, `HouseholdSpawnSystem`, `IndustrialDemandSystem`, `IndustrialSpawnSystem`, `LeisureSystem` …等 15 个

### `ServiceFeeCollector`

*(标记组件，无字段)*

### `StatisticParameter`

| 类型 | 字段 |
|---|---|
| `int` | `m_Value` |

### `Tourism`

| 类型 | 字段 |
|---|---|
| `int` | `m_CurrentTourists` |
| `int` | `m_AverageTourists` |
| `int` | `m_Attractiveness` |
| `int2` | `m_Lodging` |

被使用：`CommercialDemandSystem`, `IndustrialDemandSystem`, `TourismSystem`, `TouristSpawnSystem`

### `XP`

| 类型 | 字段 |
|---|---|
| `int` | `m_XP` |
| `int` | `m_MaximumPopulation` |
| `int` | `m_MaximumIncome` |
| `XPRewardFlags` | `m_XPRewardRecord` |

被使用：`XPAccumulationSystem`, `XPBuiltSystem`, `XPSystem`

---

## 交叉验证

以下字段已通过活跃维护中的 mod 源码交叉确认（说明这部分结构在 2026 年仍然有效）：

| 组件 | 字段 | 验证来源 |
|---|---|---|
| `Game.Net.LaneFlow` | `m_Distance` `m_Duration` `m_Next` | TrafficLightsEnhancement `CustomStateMachine.CalculateFlow` |
| `Game.Net.LaneSignal` | `m_GroupMask` `m_Petitioner` `m_Priority` | TrafficLightsEnhancement `CustomStateMachine` |
| `Game.Net.TrafficLights` | `m_State` `m_CurrentSignalGroup` | TrafficLightsEnhancement `CustomStateMachine` |
| `Game.Citizens.Citizen` / `Worker` / `Student` | (作为 `IJobChunk` 查询类型) | InfoLoom `WorkforceInfoLoomUISystem` |
