# Arrays Pooling

Core Pool Implementation
- ArrayPool<T>: Assets/Plugins/ToolBuddy/Assets/Arrays Pooling/Pools/ArrayPool.cs
- ArrayPoolsProvider: Assets/Plugins/ToolBuddy/Assets/Arrays Pooling/ArrayPoolsProvider.cs

ArrayPool<T> Key Parameters
- ElementsCapacity: Max total elements kept in pool (sum of array lengths).
- LogAllocations: Log each new allocation beyond pool availability.

Key Methods
- Allocate(minSize, clearArray)
- AllocateExactSize(size, clearArray)
- Free(SubArray<T> or T[])
- Resize / ResizeAndClear / ResizeCopyless
- Clone(array or SubArray)

ArrayPoolsSettings Component
Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Pools/ArrayPoolsSettings.cs

Purpose
- Editor-facing component to configure ArrayPool capacities used by Curvy.
- Auto-applies to ArrayPools.* static pools on enable/validate.

Serialized Fields
- Vector2Capacity (default 100000)
- Vector3Capacity (default 100000)
- Vector4Capacity (default 100000)
- IntCapacity (default 100000)
- FloatCapacity (default 10000)
- CGSpotCapacity (default 10000)
- LogAllocations (default false)

Operational Notes
- CurvyGlobalManager requires ArrayPoolsSettings, so _CurvyGlobal_ holds this component.
- Changing capacities updates the static pools immediately when enabled.
