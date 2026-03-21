using ITC.Dialogue;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public static class SignDialogueSceneDeployer
{
    private const string MenuPath = "Tools/ITC/Dialogue/Deploy Sign Scene Dialogue";
    private const string DialoguePrefabPath = "Assets/Prefabs/UI/ITC DialogueSystem.prefab";
    private const string SignScenePath = "Assets/Scenes/SignScene.unity";

    [MenuItem(MenuPath)]
    public static void Deploy()
    {
        if (!AssetDatabase.LoadAssetAtPath<GameObject>(DialoguePrefabPath))
        {
            Debug.LogError($"[SignDialogueSceneDeployer] Missing prefab: {DialoguePrefabPath}");
            return;
        }

        if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(SignScenePath))
        {
            Debug.LogError($"[SignDialogueSceneDeployer] Missing scene: {SignScenePath}");
            return;
        }

        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.isDirty)
        {
            Debug.LogError(
                "[SignDialogueSceneDeployer] Active scene has unsaved changes. Save or discard first, then rerun.");
            return;
        }

        ConfigureDialoguePrefab();
        ConfigureSignScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Restore previously active scene for convenience.
        if (!string.IsNullOrWhiteSpace(activeScene.path) && activeScene.path != SignScenePath)
        {
            EditorSceneManager.OpenScene(activeScene.path, OpenSceneMode.Single);
        }

        Debug.Log("[SignDialogueSceneDeployer] Deploy finished.");
    }

    private static void ConfigureDialoguePrefab()
    {
        using var scope = new PrefabUtility.EditPrefabContentsScope(DialoguePrefabPath);
        var root = scope.prefabContentsRoot;
        if (!root)
        {
            Debug.LogError("[SignDialogueSceneDeployer] Failed to open dialogue prefab root.");
            return;
        }

        var panel = root.transform.Find("DialogueCanvas/DialoguePanel") as RectTransform;
        var canvas = root.transform.Find("DialogueCanvas")?.GetComponent<Canvas>();
        var dialogueRunner = root.transform.Find("DialogueRunner")?.GetComponent<DialogueRunner>();
        var optionsPresenter = panel ? panel.Find("OptionsPanel")?.GetComponent<OptionsPresenter>() : null;
        var textTemplate = panel ? panel.Find("LineText/Text (TMP)")?.GetComponent<TMP_Text>() : null;
        var linePresenter = panel ? panel.GetComponent<TALinePresenter>() : null;
        var continueHandler = panel ? panel.GetComponent<DialogueContinueHandler>() : null;

        if (!panel || !canvas || !dialogueRunner || !optionsPresenter || !textTemplate || !linePresenter || !continueHandler)
        {
            Debug.LogError("[SignDialogueSceneDeployer] Prefab structure mismatch. Required nodes/components not found.");
            return;
        }

        var frameRoot = EnsureRectChild(panel, "SignSlotFrames");
        StretchToParent(frameRoot);

        var npcSlot1Frame = EnsureRectChild(frameRoot, "NPCSlot1Frame");
        ApplyRectLayout(npcSlot1Frame, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f),
            new Vector2(-80f, 238f), new Vector2(1040f, 170f));

        var npcSlot2Frame = EnsureRectChild(frameRoot, "NPCSlot2Frame");
        ApplyRectLayout(npcSlot2Frame, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f),
            new Vector2(-120f, 104f), new Vector2(980f, 150f));

        var playerSlot1Frame = EnsureRectChild(frameRoot, "PlayerSlot1Frame");
        ApplyRectLayout(playerSlot1Frame, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f),
            new Vector2(260f, 214f), new Vector2(820f, 166f));

        var npcSlot2ScrollArea = EnsureRectChild(frameRoot, "NPCSlot2ScrollArea");
        ApplyRectLayout(npcSlot2ScrollArea, npcSlot2Frame.anchorMin, npcSlot2Frame.anchorMax, npcSlot2Frame.pivot,
            npcSlot2Frame.anchoredPosition, npcSlot2Frame.sizeDelta);
        var scrollAreaImage = npcSlot2ScrollArea.GetComponent<UnityEngine.UI.Image>();
        if (!scrollAreaImage)
        {
            scrollAreaImage = npcSlot2ScrollArea.gameObject.AddComponent<UnityEngine.UI.Image>();
        }

        scrollAreaImage.color = new Color(0f, 0f, 0f, 0f);
        scrollAreaImage.raycastTarget = false;

        EnsurePlaceholderOverlay(panel);

        var slotRuntime = panel.GetComponent<SignDialogueSlotRuntime>();
        if (!slotRuntime)
        {
            slotRuntime = panel.gameObject.AddComponent<SignDialogueSlotRuntime>();
        }

        ConfigureSignRuntimeSerialized(
            slotRuntime,
            dialogueRunner,
            canvas,
            optionsPresenter,
            panel,
            npcSlot1Frame,
            npcSlot2Frame,
            playerSlot1Frame,
            npcSlot2ScrollArea,
            textTemplate);

        ConfigureLinePresenterSerialized(linePresenter, slotRuntime);
        ConfigureContinueHandlerSerialized(continueHandler, dialogueRunner, linePresenter, slotRuntime);
        ConfigureDialogueRunnerPresenters(dialogueRunner, linePresenter, optionsPresenter);
    }

    private static void ConfigureSignScene()
    {
        var scene = EditorSceneManager.OpenScene(SignScenePath, OpenSceneMode.Single);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError($"[SignDialogueSceneDeployer] Failed to open scene: {SignScenePath}");
            return;
        }

        var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var removedAny = false;
        foreach (var eventSystem in eventSystems)
        {
            if (!eventSystem)
            {
                continue;
            }

            Object.DestroyImmediate(eventSystem.gameObject);
            removedAny = true;
        }

        if (removedAny)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        EditorSceneManager.SaveScene(scene);
    }

    private static RectTransform EnsureRectChild(Transform parent, string childName)
    {
        var existing = parent.Find(childName) as RectTransform;
        if (existing)
        {
            return existing;
        }

        var go = new GameObject(childName, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.localScale = Vector3.one;
        return rect;
    }

    private static RectTransform EnsurePlaceholderOverlay(RectTransform panel)
    {
        var overlay = EnsureRectChild(panel, "SignPlaceholderMinigameOverlay");
        StretchToParent(overlay);

        var group = overlay.GetComponent<CanvasGroup>();
        if (!group)
        {
            group = overlay.gameObject.AddComponent<CanvasGroup>();
        }

        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;

        var image = overlay.GetComponent<UnityEngine.UI.Image>();
        if (!image)
        {
            image = overlay.gameObject.AddComponent<UnityEngine.UI.Image>();
        }

        image.color = new Color(0.12f, 0.18f, 0.24f, 0.86f);
        image.raycastTarget = true;
        overlay.gameObject.SetActive(false);
        overlay.SetAsLastSibling();
        return overlay;
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void ApplyRectLayout(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.localScale = Vector3.one;
    }

    private static void ConfigureSignRuntimeSerialized(
        SignDialogueSlotRuntime slotRuntime,
        DialogueRunner dialogueRunner,
        Canvas rootCanvas,
        OptionsPresenter optionsPresenter,
        RectTransform panelRect,
        RectTransform npcSlot1Frame,
        RectTransform npcSlot2Frame,
        RectTransform playerSlot1Frame,
        RectTransform npcHistoryScrollArea,
        TMP_Text textTemplate)
    {
        var so = new SerializedObject(slotRuntime);
        so.Update();

        TrySetBool(so, "enableSignSlotRouting", true);
        TrySetBool(so, "suppressLegacyPresenterVisuals", true);

        TrySetObject(so, "dialogueRunner", dialogueRunner);
        TrySetObject(so, "rootCanvas", rootCanvas);
        TrySetObject(so, "optionsPresenter", optionsPresenter);
        TrySetObject(so, "panelRect", panelRect);
        TrySetObject(so, "textTemplate", textTemplate);
        TrySetObject(so, "npcSlot1Frame", npcSlot1Frame);
        TrySetObject(so, "npcSlot2Frame", npcSlot2Frame);
        TrySetObject(so, "playerSlot1Frame", playerSlot1Frame);
        TrySetObject(so, "npcHistoryScrollArea", npcHistoryScrollArea);

        // Keep startup controlled by DialogueSceneFlowLauncher/ITCDialoguePanel start node.
        TrySetBool(so, "autoStartDialogueIfIdle", false);

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(slotRuntime);
    }

    private static void ConfigureLinePresenterSerialized(TALinePresenter linePresenter, SignDialogueSlotRuntime slotRuntime)
    {
        var so = new SerializedObject(linePresenter);
        so.Update();

        TrySetBool(so, "routeToSignSlots", true);
        TrySetObject(so, "signDialogueSlotRuntime", slotRuntime);

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(linePresenter);
    }

    private static void ConfigureContinueHandlerSerialized(
        DialogueContinueHandler continueHandler,
        DialogueRunner dialogueRunner,
        TALinePresenter linePresenter,
        SignDialogueSlotRuntime slotRuntime)
    {
        var so = new SerializedObject(continueHandler);
        so.Update();

        TrySetObject(so, "dialogueRunner", dialogueRunner);
        TrySetObject(so, "linePresenter", linePresenter);
        TrySetObject(so, "signDialogueSlotRuntime", slotRuntime);

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(continueHandler);
    }

    private static void ConfigureDialogueRunnerPresenters(
        DialogueRunner dialogueRunner,
        TALinePresenter linePresenter,
        OptionsPresenter optionsPresenter)
    {
        var so = new SerializedObject(dialogueRunner);
        so.Update();
        var presenters = so.FindProperty("dialoguePresenters");
        if (presenters == null || !presenters.isArray)
        {
            return;
        }

        presenters.arraySize = 0;
        presenters.InsertArrayElementAtIndex(0);
        presenters.GetArrayElementAtIndex(0).objectReferenceValue = linePresenter;
        presenters.InsertArrayElementAtIndex(1);
        presenters.GetArrayElementAtIndex(1).objectReferenceValue = optionsPresenter;

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(dialogueRunner);
    }

    private static void TrySetBool(SerializedObject serializedObject, string propertyName, bool value)
    {
        var property = serializedObject.FindProperty(propertyName);
        if (property != null && property.propertyType == SerializedPropertyType.Boolean)
        {
            property.boolValue = value;
        }
    }

    private static void TrySetObject(SerializedObject serializedObject, string propertyName, Object value)
    {
        var property = serializedObject.FindProperty(propertyName);
        if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
        {
            property.objectReferenceValue = value;
        }
    }
}

