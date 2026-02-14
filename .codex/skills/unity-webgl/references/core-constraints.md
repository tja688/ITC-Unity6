# Unity WebGL Core Constraints

## Purpose
Use this document to reason about runtime limits before writing code.
Treat these constraints as design-time guardrails for Unity 2022.3 WebGL.

## Runtime Model
- Assume browser sandbox runtime, not native OS runtime.
- Assume main-thread-first execution model for C# logic.
- Assume memory pressure can fail due to contiguous heap growth constraints.

## Constraint Areas

### Memory and GC
- Unity WebGL heap is contiguous and can grow up to 2GB, but growth can fail.
- GC timing differs from native targets; frame-bound execution can amplify temporary allocations.
- Avoid large per-frame transient allocations in loops.
- Prefer preallocation and reuse for strings, lists, arrays, and buffers.
- Keep startup payload small; move non-critical assets to on-demand loading.

### Threading and Concurrency
- Do not depend on managed multithreading in WebGL gameplay/runtime code.
- Replace background-thread assumptions with coroutine/state-machine slicing.
- If native threading is explicitly enabled, treat deployment headers as mandatory design work.

### Networking
- Use UnityWebRequest (or explicit JS bridge for web-native protocols).
- Avoid System.Net usage in WebGL runtime paths.
- Avoid blocking wait loops around `isDone`; always yield asynchronously.

### Filesystem and Logging
- Do not assume direct local filesystem access.
- Route diagnostics to browser console and controlled telemetry endpoints.
- Do not use `file://` loading as a release validation method.

### Audio and Input
- Require user gesture for autoplay-sensitive actions.
- Do not rely on unsupported microphone path in WebGL.
- Treat fullscreen and pointer lock as user-action-gated operations.

### JS Interop
- Keep `.jslib` and `.jspre` plugins ES5-compatible.
- Use deterministic bridge signatures and input validation for JS-C# calls.

## Design Prompts Before Implementation
1. Which rule IDs in `rules-and-decision-matrix.md` can this change violate?
2. Is any feature implicitly using threads, blocking waits, or unsupported APIs?
3. Does the change increase startup memory or `.data` footprint?
4. Does the feature require browser security capabilities (CORS, COEP, HTTPS)?
5. Is there a fallback path when browser capabilities are unavailable?
