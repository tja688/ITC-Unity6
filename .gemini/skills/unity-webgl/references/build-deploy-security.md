# Unity WebGL Build, Deploy, and Security

## Purpose
Use this document when planning or reviewing build settings, hosting, and browser security policies.

## Build and Player Settings Checklist
Use these as defaults unless a documented exception is approved.

| Area | Preferred setting | Rationale |
|---|---|---|
| Graphics API | WebGL2-only target | Avoid deprecated WebGL1 dependency and limits |
| Development Build | On for debug, off for release | Release size/performance stability |
| Exceptions | Minimal for release | Reduce size and runtime overhead |
| Data Caching | Enabled by default | Better repeat-load experience |
| Decompression Fallback | Disabled when server headers are controllable | Preserve wasm streaming benefits |
| Threads Support | Off by default | Avoid accidental cross-origin isolation burden |
| Embedded Resources | Off by default | Avoid binary size inflation |

## Required Server Header Behavior
- Correct `Content-Type` for `.wasm` should be `application/wasm`.
- Correct `Content-Encoding` for precompressed files:
- `.gz` -> `gzip`
- `.br` -> `br`
- Prevent double-compression of already precompressed Unity artifacts.
- Keep MIME and encoding behavior deterministic across CDN and origin.

## CORS and Cross-Origin Rules
- Any cross-origin fetch must be explicitly allowed by server-side CORS.
- Do not treat wildcard CORS as a production default.
- If cross-origin isolation is required, ensure all dependent resources satisfy COEP-compatible policies.

## COOP/COEP/CORP Guidance
- Keep `webGLThreadsSupport` off unless native threading is explicitly required.
- If native threading is enabled:
- require COOP (`same-origin`) and COEP (`require-corp`).
- verify all embedded resources satisfy CORP/CORS compatibility.

## CSP and WebAssembly
- If CSP is active, include wasm-compatible policy in `script-src`.
- Prefer `wasm-unsafe-eval` over broad `unsafe-eval` when possible.

## HTTPS Requirement
- Treat HTTPS as mandatory for production, especially for secure-context features.
- Use localhost exceptions only for development.

## Review Prompts
1. Are headers validated in real hosting, not only local dev server?
2. Does CDN behavior preserve encoding and content type exactly?
3. If CSP is strict, is wasm execution explicitly permitted?
4. If threads are enabled, is cross-origin isolation truly complete?
