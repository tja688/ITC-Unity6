# Unity WebGL Rules and Decision Matrix

## Severity Model
- `BLOCKER`: must be fixed before approval.
- `WARNING`: can proceed only with explicit risk acceptance and follow-up.

## Rule Matrix
| Rule ID | Severity | Category | Detection hint | Decision outcome |
|---|---|---|---|---|
| UWG-THR-001 | BLOCKER | Threading | `System.Threading`, `System.Timers`, `ThreadPool`, `Task.Run`, `new Thread` in C# | Reject until refactored to coroutine/state-machine/server path |
| UWG-NET-001 | BLOCKER | Network | `System.Net` usage in C# | Reject until replaced with UnityWebRequest or JS bridge |
| UWG-NET-002 | BLOCKER | Network | `UnityEngine.Ping` usage | Reject and replace with supported connectivity strategy |
| UWG-NET-003 | BLOCKER | Network | Blocking `while (!...isDone)` loop | Reject until asynchronous flow is used |
| UWG-AUD-001 | BLOCKER | Audio | `Microphone` usage in runtime path | Reject and redesign input strategy |
| UWG-JS-001 | BLOCKER | JS Interop | ES6 syntax in `.jslib` or `.jspre` (`let`, `const`, `=>`, `class`) | Reject until plugin is ES5-compatible |
| UWG-SET-001 | WARNING | Project Settings | `webGLThreadsSupport` set to `1` | Require documented COOP/COEP/CORP plan |
| UWG-SET-002 | WARNING | Project Settings | `webGLDecompressionFallback` set to `1` | Require explicit tradeoff acceptance |
| UWG-SET-003 | WARNING | Project Settings | `webGLDataCaching` set to `0` | Require justification for cache-off strategy |
| UWG-SET-004 | WARNING | Project Settings | `webGLExceptionSupport` not `0` | Require release-performance justification |
| UWG-SET-005 | BLOCKER | Project Settings | `webGLMaximumMemorySize` greater than `2048` | Reject invalid memory cap |
| UWG-SET-006 | WARNING | Project Settings | `webGLUseEmbeddedResources` set to `1` | Require binary-size tradeoff note |

## Recommended vs Banned API Direction
| Area | Prefer | Avoid |
|---|---|---|
| Network | UnityWebRequest + coroutine | `System.Net.*`, `UnityEngine.Ping`, blocking wait loops |
| Concurrency | Coroutine/state machine/frame slicing | Managed threading assumptions |
| Audio | Gesture-triggered playback flows | Microphone dependency in WebGL |
| JS interop | ES5 plugin bridge with validated inputs | ES6 syntax in Unity plugin files |

## Planning-Time Tradeoff Prompts
1. If a warning rule triggers, what measurable benefit justifies the risk?
2. Can the same feature be delivered without violating any blocker rule?
3. Is the rule impact temporary (migration path exists) or structural (permanent debt)?
4. What test proves the chosen workaround is stable on target browsers?

## Review Output Requirements
For each finding:
1. Include `rule_id`.
2. Include location and evidence.
3. Include required remediation.
4. State final decision (`APPROVE`, `APPROVE_WITH_RISKS`, `REJECT`) consistent with severity rules.
