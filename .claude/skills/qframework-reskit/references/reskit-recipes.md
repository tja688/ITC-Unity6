# ResKit Recipes

## Init Matrix

| Context | Required init behavior |
| --- | --- |
| Editor + `SimulationMode = true` | Do not require manual init for normal simulation flow. |
| Device mode / real runtime AB mode | Call `ResKit.Init()` or `ResKit.InitAsync()` after startup before runtime loading. |
| WebGL | Use `ResKit.InitAsync()` (async init path). |

## Canonical Snippets

### 1) Sync load

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoSyncLoad : MonoBehaviour
{
    private ResLoader mLoader;

    private void Awake()
    {
        mLoader = ResLoader.Allocate();
    }

    private void Start()
    {
        var prefab = mLoader.LoadSync<GameObject>("UIHomePanel");
        if (prefab != null) Instantiate(prefab);
    }

    private void OnDestroy()
    {
        mLoader?.Recycle2Cache();
        mLoader = null;
    }
}
```

### 2) Async queue load

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoAsyncLoad : MonoBehaviour
{
    private ResLoader mLoader;

    private void Awake()
    {
        mLoader = ResLoader.Allocate();
    }

    private void Start()
    {
        mLoader.Add2Load<GameObject>("UIHomePanel", (ok, res) =>
        {
            if (!ok) return;
            var prefab = res.Asset as GameObject;
            if (prefab != null) Instantiate(prefab);
        });

        mLoader.LoadAsync();
    }

    private void OnDestroy()
    {
        mLoader?.Recycle2Cache();
        mLoader = null;
    }
}
```

### 3) Scene load (sync / async)

```csharp
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class DemoSceneLoad : MonoBehaviour
{
    private ResLoader mLoader;

    private void Awake()
    {
        mLoader = ResLoader.Allocate();
    }

    public void LoadSyncScene()
    {
        mLoader.LoadSceneSync("BattleScene", LoadSceneMode.Single);
    }

    public void LoadAsyncScene()
    {
        mLoader.LoadSceneAsync("BattleScene", LoadSceneMode.Single, LocalPhysicsMode.None, op =>
        {
            // Use op.progress if needed.
        });
    }

    private void OnDestroy()
    {
        mLoader?.Recycle2Cache();
        mLoader = null;
    }
}
```

### 4) Loader lifecycle sample

```csharp
using QFramework;
using UnityEngine;

public sealed class DemoLoaderLifecycle : MonoBehaviour
{
    private ResLoader mLoader;

    private void Awake()
    {
        mLoader = ResLoader.Allocate();
    }

    private void OnDisable()
    {
        // Optional early release for heavy transient loads.
        mLoader?.ReleaseAllRes();
    }

    private void OnDestroy()
    {
        mLoader?.Recycle2Cache();
        mLoader = null;
    }
}
```

## Integration Snippets

Use existing adapter hooks instead of inventing new integration patterns:

- Audio loader pool hook: `Assets/QFramework/Toolkits/SupportOldQF/Scripts/AudioKitWithResKitInit.cs`
- UI panel loader pool hook: `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs`

Both files show the expected pattern:

1. Allocate and hold a `ResLoader` in loader instance.
2. Load via `LoadSync<T>` or `Add2Load<T> + LoadAsync`.
3. Recycle loader in `Unload()`.

## Anti-Patterns

- `new ResLoader()` instead of `ResLoader.Allocate()`.
- Forgetting to recycle loader (`Recycle2Cache`) or lacking a `ReleaseAllRes` teardown path.
- Device mode resource loading without `ResKit.Init()` or `ResKit.InitAsync()` after startup.
- Using resource-loading callbacks to mutate model state directly and bypass command/CQRS flow.
