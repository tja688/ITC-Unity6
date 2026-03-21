# ITC-WebGL Project Baseline Snapshot

## Scope
This snapshot records current repository state for project-specific context.
Treat these values as current-state observations, not universal policy.

## Source Files
- `ProjectSettings/ProjectVersion.txt`
- `ProjectSettings/ProjectSettings.asset`

## Unity Version Snapshot
- `m_EditorVersion: 2022.3.62t4`
- `m_EditorVersionWithRevision: 2022.3.62t4 (1ad14fec91c6)`

## WebGL Settings Snapshot
- `webGLMemorySize: 32`
- `webGLExceptionSupport: 1`
- `webGLNameFilesAsHashes: 0`
- `webGLShowDiagnostics: 0`
- `webGLDataCaching: 1`
- `webGLDebugSymbols: 0`
- `webGLEmscriptenArgs:`
- `webGLModulesDirectory:`
- `webGLTemplate: APPLICATION:Default`
- `webGLAnalyzeBuildSize: 0`
- `webGLUseEmbeddedResources: 0`
- `webGLCompressionFormat: 0`
- `webGLWasmArithmeticExceptions: 0`
- `webGLLinkerTarget: 1`
- `webGLThreadsSupport: 0`
- `webGLDecompressionFallback: 0`
- `webGLInitialMemorySize: 32`
- `webGLMaximumMemorySize: 2048`
- `webGLMemoryGrowthMode: 2`
- `webGLMemoryLinearGrowthStep: 16`
- `webGLMemoryGeometricGrowthStep: 0.2`
- `webGLMemoryGeometricGrowthCap: 96`
- `webGLPowerPreference: 2`

## Baseline Interpretation Notes
- Threads support is currently disabled, matching the default low-risk posture.
- Decompression fallback is currently disabled, which is compatible with streaming-oriented hosting setups.
- Exception support is currently non-zero and should be reviewed when preparing strict release builds.
- Maximum memory is currently set to `2048`, the WebGL upper bound in this policy set.

## How to Use This Snapshot
1. Compare planned changes against this baseline before implementation.
2. Report deviations explicitly in review output.
3. Update this file if project settings are intentionally changed.
