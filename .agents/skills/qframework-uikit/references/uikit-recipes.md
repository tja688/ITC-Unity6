# UIKit Recipes

## AI-Critical Runtime Path

### 1) Open a panel with typed data

```csharp
using QFramework;

public sealed class InventoryPanelData : UIPanelData
{
    public int TabIndex;
}

public static class InventoryUIFlow
{
    public static void OpenInventory(int tabIndex)
    {
        UIKit.OpenPanel<UIInventoryPanel>(
            uiData: new InventoryPanelData { TabIndex = tabIndex },
            canvasLevel: UILevel.Common);
    }
}
```

### 2) Async open flow

```csharp
using System.Collections;
using QFramework;
using UnityEngine;

public sealed class AsyncOpenExample : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return UIKit.OpenPanelAsync<UIInventoryPanel>(UILevel.Common);

        var panel = UIKit.GetPanel<UIInventoryPanel>();
        if (panel != null)
        {
            // Continue post-open logic here.
        }
    }
}
```

### 3) Panel lifecycle skeleton

```csharp
using QFramework;

public sealed class UIInventoryPanel : UIPanel
{
    protected override void OnInit(IUIData uiData = null)
    {
        // Bind references / one-time setup.
    }

    protected override void OnOpen(IUIData uiData = null)
    {
        var data = uiData as InventoryPanelData;
        // Refresh UI from data/state.
    }

    protected override void OnClose()
    {
        // Release panel-owned runtime resources.
    }
}
```

### 4) Stack/back navigation only when intended

```csharp
using QFramework;

public static class UINavFlow
{
    public static void OpenDetailsFromList()
    {
        UIKit.Stack.Push(UIKit.GetPanel<UIListPanel>());
        UIKit.OpenPanel<UIDetailPanel>();
    }

    public static void BackFromDetails()
    {
        UIKit.Back<UIDetailPanel>();
    }
}
```

### 5) ResKit-backed loader pool wiring (runtime integration point)

Use existing adapter:

- `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs`

Expected loader pattern:

1. Allocate one `ResLoader` per loader instance.
2. Load prefab via `LoadSync<GameObject>` or `Add2Load<GameObject> + LoadAsync`.
3. Recycle loader in `Unload()`.

## External / Low-Relevance Path (Default: Do Not Touch)

These are usually outside AI business-code implementation scope unless explicitly requested:

- `Assets/QFramework/Toolkits/UIKit/Editor/CodeGen/*`
- `Assets/QFramework/Toolkits/UIKit/Editor/UIKitSetttingData.cs`
- `Assets/QFramework/Toolkits/UIKit/Editor/UIKitHierarchyMenu.cs`
- Generated files such as `*.Designer.cs`

If user explicitly asks for codegen work:

1. Keep edits localized to editor/codegen paths.
2. Preserve generation templates and avoid runtime API drift.
3. Do not mix runtime bug fixes with codegen refactors in one change.

## Anti-Patterns

- Directly manipulating `UIKit.Table` or `UIManager` from feature code when `UIKit` API already covers the operation.
- Passing untyped/implicit panel payloads instead of `IUIData`.
- Bypassing panel lifecycle by mutating panel internals without `OnInit`/`OnOpen`/`OnClose`.
- Writing custom loader pools without deterministic `Unload` behavior.
- Hand-editing generated `*.Designer.cs` for normal feature changes.
- Mixing UI side-effect code with direct model mutation that bypasses command/CQRS ownership.
