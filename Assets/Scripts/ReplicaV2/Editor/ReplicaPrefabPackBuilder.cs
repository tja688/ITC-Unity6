using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ReplicaPrefabPackBuilder
{
    private const string EffectsScriptsDir = "Assets/Scripts/ReplicaV2/Effects";
    private const string PrefabRootDir = "Assets/ReplicaV2/Prefabs/Effects";
    private const string GeneratedConfigDir = "Assets/ReplicaV2/Configs/Generated";
    private const string GeneratedScenePath = "Assets/Scenes/ReplicaV2_PrefabPackShowcase.unity";

    private sealed class EffectDescriptor
    {
        public string Name;
        public Type BuilderType;
        public Type HostBridgeType;
        public Type ConfigType;
    }

    private sealed class BuildResult
    {
        public string Name;
        public bool Success;
        public string Reason;
        public string HostPrefabPath;
    }

    [MenuItem("ReplicaV2/Prefab Pack/Build Full Prefab Showcase Scene")]
    public static void BuildFullPrefabShowcaseScene()
    {
        EnsureFolder(PrefabRootDir);
        EnsureFolder(GeneratedConfigDir);

        var effects = DiscoverEffects();
        var results = new List<BuildResult>(effects.Count);

        foreach (var effect in effects)
        {
            results.Add(BuildEffectPackage(effect));
        }

        var successResults = results.Where(r => r.Success && !string.IsNullOrEmpty(r.HostPrefabPath)).ToList();
        CreateShowcaseScene(successResults);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrintSummary(results, successResults.Count);
    }

    private static List<EffectDescriptor> DiscoverEffects()
    {
        if (!Directory.Exists(EffectsScriptsDir))
        {
            return new List<EffectDescriptor>();
        }

        var folders = Directory.GetDirectories(EffectsScriptsDir);
        var output = new List<EffectDescriptor>(folders.Length);

        foreach (var folder in folders)
        {
            var effectName = Path.GetFileName(folder);

            var builderType = FindType(effectName + "EffectViewBuilder") ?? FindType(effectName + "ViewBuilder");
            var hostBridgeType = FindType(effectName + "EffectHostBridge") ?? FindType(effectName + "HostBridge");
            var configType = FindType(effectName + "EffectConfig") ?? FindType(effectName + "Config");

            if (builderType == null || hostBridgeType == null)
            {
                continue;
            }

            output.Add(new EffectDescriptor
            {
                Name = effectName,
                BuilderType = builderType,
                HostBridgeType = hostBridgeType,
                ConfigType = configType
            });
        }

        output.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
        return output;
    }

    private static BuildResult BuildEffectPackage(EffectDescriptor effect)
    {
        var result = new BuildResult { Name = effect.Name };
        var tempObjects = new List<UnityEngine.Object>();

        try
        {
            var mountGo = new GameObject("Bake_" + effect.Name + "_Mount", typeof(RectTransform));
            var mountRoot = mountGo.GetComponent<RectTransform>();
            tempObjects.Add(mountGo);

            var buildMethod = ResolveBuildMethod(effect.BuilderType);
            var args = CreateBuildArguments(buildMethod, mountRoot, tempObjects);
            var view = buildMethod.Invoke(null, args);
            if (view == null)
            {
                throw new InvalidOperationException(effect.Name + " Build returned null.");
            }

            var viewRoot = ResolveViewRoot(view);
            if (viewRoot == null)
            {
                throw new InvalidOperationException(effect.Name + " view root not found.");
            }

            var viewRootGo = viewRoot.gameObject;
            TryInvokeLink(effect.BuilderType, viewRootGo, view);
            SetLayerRecursive(viewRootGo, 5);

            viewRoot.localPosition = Vector3.zero;
            viewRoot.anchoredPosition = Vector2.zero;
            viewRoot.localRotation = Quaternion.identity;
            viewRoot.localScale = Vector3.one;

            var viewPrefabPath = PrefabRootDir + "/" + effect.Name + "_Baked.prefab";
            var viewPrefab = PrefabUtility.SaveAsPrefabAsset(viewRootGo, viewPrefabPath);
            if (viewPrefab == null)
            {
                throw new InvalidOperationException("Failed to save view prefab: " + viewPrefabPath);
            }

            ScriptableObject configAsset = null;
            if (effect.ConfigType != null && typeof(ScriptableObject).IsAssignableFrom(effect.ConfigType))
            {
                configAsset = GetOrCreateConfigAsset(effect, viewPrefab);
            }

            var hostGo = new GameObject(effect.Name + "_Host", typeof(RectTransform));
            tempObjects.Add(hostGo);

            var hostRect = hostGo.GetComponent<RectTransform>();
            hostRect.anchorMin = new Vector2(0.5f, 0.5f);
            hostRect.anchorMax = new Vector2(0.5f, 0.5f);
            hostRect.pivot = new Vector2(0.5f, 0.5f);
            hostRect.sizeDelta = new Vector2(420f, 320f);

            var hostBridge = hostGo.AddComponent(effect.HostBridgeType);
            SetLayerRecursive(hostGo, 5);

            BindHostBridgeReferences(hostBridge, configAsset, viewPrefab);

            var hostPrefabPath = PrefabRootDir + "/" + effect.Name + "_Host.prefab";
            var hostPrefab = PrefabUtility.SaveAsPrefabAsset(hostGo, hostPrefabPath);
            if (hostPrefab == null)
            {
                throw new InvalidOperationException("Failed to save host prefab: " + hostPrefabPath);
            }

            result.Success = true;
            result.HostPrefabPath = hostPrefabPath;
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Reason = ex.Message;
            return result;
        }
        finally
        {
            for (var i = 0; i < tempObjects.Count; i++)
            {
                if (tempObjects[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(tempObjects[i]);
                }
            }
        }
    }

    private static MethodInfo ResolveBuildMethod(Type builderType)
    {
        var method = builderType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "Build");

        if (method == null)
        {
            throw new MissingMethodException("Build method missing in " + builderType.Name);
        }

        return method;
    }

    private static object[] CreateBuildArguments(MethodInfo buildMethod, RectTransform mountRoot, List<UnityEngine.Object> tempObjects)
    {
        var parameters = buildMethod.GetParameters();
        var args = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var pType = parameters[i].ParameterType;
            if (i == 0 && typeof(RectTransform).IsAssignableFrom(pType))
            {
                args[i] = mountRoot;
                continue;
            }

            if (typeof(ScriptableObject).IsAssignableFrom(pType))
            {
                var so = ScriptableObject.CreateInstance(pType);
                tempObjects.Add(so);
                args[i] = so;
                continue;
            }

            args[i] = CreateDefaultValue(pType);
        }

        return args;
    }

    private static object CreateDefaultValue(Type t)
    {
        if (t.IsValueType)
        {
            return Activator.CreateInstance(t);
        }

        try
        {
            return Activator.CreateInstance(t);
        }
        catch
        {
            return null;
        }
    }

    private static RectTransform ResolveViewRoot(object view)
    {
        var viewType = view.GetType();
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        var namedRootField = viewType.GetField("Root", flags) ?? viewType.GetField("root", flags);
        if (namedRootField != null && typeof(RectTransform).IsAssignableFrom(namedRootField.FieldType))
        {
            return namedRootField.GetValue(view) as RectTransform;
        }

        var namedRootProp = viewType.GetProperty("Root", flags) ?? viewType.GetProperty("root", flags);
        if (namedRootProp != null && typeof(RectTransform).IsAssignableFrom(namedRootProp.PropertyType))
        {
            return namedRootProp.GetValue(view, null) as RectTransform;
        }

        var anyRectField = viewType.GetFields(flags).FirstOrDefault(f => typeof(RectTransform).IsAssignableFrom(f.FieldType));
        if (anyRectField != null)
        {
            return anyRectField.GetValue(view) as RectTransform;
        }

        return null;
    }

    private static void TryInvokeLink(Type builderType, GameObject root, object view)
    {
        var methods = builderType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "Link")
            .ToArray();

        for (var i = 0; i < methods.Length; i++)
        {
            var ps = methods[i].GetParameters();
            if (ps.Length != 2)
            {
                continue;
            }

            if (!typeof(GameObject).IsAssignableFrom(ps[0].ParameterType))
            {
                continue;
            }

            if (!ps[1].ParameterType.IsInstanceOfType(view))
            {
                continue;
            }

            methods[i].Invoke(null, new[] { root, view });
            return;
        }
    }

    private static ScriptableObject GetOrCreateConfigAsset(EffectDescriptor effect, GameObject viewPrefab)
    {
        var configPath = GeneratedConfigDir + "/" + effect.Name + "EffectConfig_Auto.asset";
        var existing = AssetDatabase.LoadAssetAtPath(configPath, effect.ConfigType) as ScriptableObject;
        ScriptableObject configAsset;

        if (existing != null)
        {
            configAsset = existing;
        }
        else
        {
            configAsset = ScriptableObject.CreateInstance(effect.ConfigType);
            AssetDatabase.CreateAsset(configAsset, configPath);
        }

        var prefabField = effect.ConfigType.GetField("Prefab", BindingFlags.Public | BindingFlags.Instance);
        if (prefabField != null && typeof(GameObject).IsAssignableFrom(prefabField.FieldType))
        {
            prefabField.SetValue(configAsset, viewPrefab);
            EditorUtility.SetDirty(configAsset);
        }

        return configAsset;
    }

    private static void BindHostBridgeReferences(Component hostBridge, ScriptableObject configAsset, GameObject viewPrefab)
    {
        var so = new SerializedObject(hostBridge);

        var configProp = so.FindProperty("mConfig");
        if (configProp != null && configAsset != null)
        {
            configProp.objectReferenceValue = configAsset;
        }

        var prefabOverwriteProp = so.FindProperty("mPrefabOverwrite");
        if (prefabOverwriteProp != null)
        {
            prefabOverwriteProp.objectReferenceValue = viewPrefab;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateShowcaseScene(List<BuildResult> results)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateMainCamera();
        CreateEventSystem();

        var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(Image));
        var canvasRect = canvasGo.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
        canvasGo.layer = 5;

        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var bg = canvasGo.GetComponent<Image>();
        bg.color = new Color(0.08f, 0.11f, 0.16f, 1f);
        bg.raycastTarget = false;

        CreateLabel("Title", canvasRect, "ReplicaV2 Prefab Pack Showcase", 48, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.95f, 0.97f, 1f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -52f), new Vector2(1200f, 70f));
        CreateLabel("Hint", canvasRect, "Scroll to review all packed effects", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.76f, 0.84f, 0.96f, 0.95f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(900f, 40f));

        var scrollRoot = new GameObject("ScrollRoot", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        var scrollRect = scrollRoot.GetComponent<RectTransform>();
        scrollRect.SetParent(canvasRect, false);
        scrollRect.anchorMin = new Vector2(0f, 0f);
        scrollRect.anchorMax = new Vector2(1f, 1f);
        scrollRect.offsetMin = new Vector2(40f, 30f);
        scrollRect.offsetMax = new Vector2(-40f, -140f);
        scrollRoot.layer = 5;

        var scrollBg = scrollRoot.GetComponent<Image>();
        scrollBg.color = new Color(0f, 0f, 0f, 0.16f);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.SetParent(scrollRect, false);
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.layer = 5;
        var viewportImage = viewport.GetComponent<Image>();
        // Mask uses alpha-clip material; keep alpha solid so stencil writes reliably.
        viewportImage.color = new Color(1f, 1f, 1f, 1f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content", typeof(RectTransform));
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.SetParent(viewportRect, false);
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(0f, 1f);
        contentRect.pivot = new Vector2(0f, 1f);
        content.layer = 5;

        var scroll = scrollRoot.GetComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        var itemSize = new Vector2(420f, 320f);
        const float padLeft = 20f;
        const float padTop = 20f;
        const float spacingX = 24f;
        const float spacingY = 24f;
        const int columns = 4;

        for (var i = 0; i < results.Count; i++)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(results[i].HostPrefabPath);
            if (prefab == null)
            {
                continue;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab, contentRect) as GameObject;
            if (instance == null)
            {
                continue;
            }

            instance.name = results[i].Name;
            SetLayerRecursive(instance, 5);

            var rect = instance.GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = instance.AddComponent<RectTransform>();
            }

            var row = i / columns;
            var col = i % columns;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = itemSize;
            rect.anchoredPosition = new Vector2(
                padLeft + col * (itemSize.x + spacingX),
                -(padTop + row * (itemSize.y + spacingY)));

            CreateLabel(
                "EffectName",
                rect,
                results[i].Name,
                22,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.93f, 0.95f, 1f, 0.95f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(6f, 0f),
                new Vector2(320f, 34f));
        }

        var rowCount = Mathf.Max(1, Mathf.CeilToInt(results.Count / (float)columns));
        var contentHeight = padTop + rowCount * itemSize.y + Mathf.Max(0, rowCount - 1) * spacingY + padTop;
        var contentWidth = padLeft + columns * itemSize.x + (columns - 1) * spacingX + padLeft;
        contentRect.sizeDelta = new Vector2(contentWidth, contentHeight);
        contentRect.anchoredPosition = Vector2.zero;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, GeneratedScenePath);
    }

    private static void CreateMainCamera()
    {
        var cameraGo = new GameObject("Main Camera", typeof(Camera));
        cameraGo.tag = "MainCamera";
        var cam = cameraGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.07f, 0.12f, 1f);
        cameraGo.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static void CreateEventSystem()
    {
        var eventSystemGo = new GameObject("EventSystem", typeof(EventSystem));

        var inputSystemType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputSystemType != null)
        {
            eventSystemGo.AddComponent(inputSystemType);
        }
        else
        {
            var standaloneType = Type.GetType("UnityEngine.EventSystems.StandaloneInputModule, UnityEngine.UI");
            if (standaloneType != null)
            {
                eventSystemGo.AddComponent(standaloneType);
            }
        }
    }

    private static Text CreateLabel(
        string name,
        RectTransform parent,
        string text,
        int fontSize,
        FontStyle fontStyle,
        TextAnchor alignment,
        Color color,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPos,
        Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        go.layer = 5;

        var uiText = go.GetComponent<Text>();
        uiText.text = text;
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.fontSize = fontSize;
        uiText.fontStyle = fontStyle;
        uiText.alignment = alignment;
        uiText.color = color;
        uiText.raycastTarget = false;
        return uiText;
    }

    private static Type FindType(string typeName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var type = asm.GetType(typeName);
                if (type != null)
                {
                    return type;
                }

                type = asm.GetTypes().FirstOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    return type;
                }
            }
            catch
            {
            }
        }

        return null;
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        var segments = folderPath.Split('/');
        var current = segments[0];
        for (var i = 1; i < segments.Length; i++)
        {
            var next = current + "/" + segments[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, segments[i]);
            }

            current = next;
        }
    }

    private static void PrintSummary(List<BuildResult> results, int successCount)
    {
        var failed = results.Where(r => !r.Success).ToList();
        Debug.Log("[ReplicaPrefabPackBuilder] Completed. Success: " + successCount + " / " + results.Count + ". Scene: " + GeneratedScenePath);

        if (failed.Count == 0)
        {
            return;
        }

        for (var i = 0; i < failed.Count; i++)
        {
            Debug.LogWarning("[ReplicaPrefabPackBuilder] Skipped " + failed[i].Name + ": " + failed[i].Reason);
        }
    }
}
