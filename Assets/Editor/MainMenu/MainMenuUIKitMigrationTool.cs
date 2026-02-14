using System.IO;
using QFramework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MainMenuUIKitMigrationTool
{
    private const string ScenePath = "Assets/Scenes/MainMenu.unity";
    private const string MenuRootName = "主菜单";
    private const string PrefabDirectory = "Assets/Prefabs/UI";
    private const string PrefabPath = "Assets/Prefabs/UI/MainMenuPanel.prefab";
    private const string LauncherName = "MainMenuFlowLauncher";
    private const string MainMenuBundleName = "menu_core";
    private const string WebGLManifestPath = "Assets/StreamingAssets/AssetBundles/WebGL/menu_core.manifest";
    private const string QAssetsPath = "Assets/QFrameworkData/QAssets.cs";
    private const string MainMenuMaterialPath = "Assets/Arts/Materials/MainMenuUnlit.mat";

    [MenuItem("Tools/ITC/MainMenu/1) Migrate To UIKit Async Panel")]
    public static void MigrateToUIKitAsyncPanel()
    {
        if (!File.Exists(ScenePath))
        {
            Debug.LogError($"Scene not found: {ScenePath}");
            return;
        }

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"Failed to open scene: {ScenePath}");
            return;
        }

        var changed = false;
        var menuRoot = GameObject.Find(MenuRootName);
        if (menuRoot != null)
        {
            if (!menuRoot.TryGetComponent<MainMenuPanel>(out _))
            {
                menuRoot.AddComponent<MainMenuPanel>();
                changed = true;
            }

            if (!Directory.Exists(PrefabDirectory))
            {
                Directory.CreateDirectory(PrefabDirectory);
                changed = true;
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(menuRoot, PrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Failed to save prefab: {PrefabPath}");
                return;
            }

            changed = true;
            SetPrefabAssetBundleName();
            Object.DestroyImmediate(menuRoot);
            changed = true;
        }
        else if (!File.Exists(PrefabPath))
        {
            Debug.LogError($"Cannot find scene object '{MenuRootName}' and prefab does not exist: {PrefabPath}");
            return;
        }

        if (EnsureLauncherExists())
        {
            changed = true;
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("MainMenu migration ready: scene now launches panel via ResKit + UIKit async flow.");
    }

    [MenuItem("Tools/ITC/MainMenu/2) Build WebGL AB (ResKit)")]
    public static void BuildWebGLAssetBundles()
    {
        FixMainMenuSpriteMaterialsInternal(logResult: false);

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            var switched = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            if (!switched)
            {
                Debug.LogWarning("Failed to switch active build target to WebGL, continue with current target.");
            }
        }

        ResKitEditorAPI.SimulationMode = false;
        ResKitEditorAPI.BuildAssetBundles();
        ValidateBuildOutputs();
    }

    [MenuItem("Tools/ITC/MainMenu/3) Fix MainMenu Sprite Material")]
    public static void FixMainMenuSpriteMaterials()
    {
        FixMainMenuSpriteMaterialsInternal(logResult: true);
    }

    private static bool EnsureLauncherExists()
    {
        var changed = false;
        var launcher = GameObject.Find(LauncherName);
        if (launcher == null)
        {
            launcher = new GameObject(LauncherName);
            changed = true;
        }

        if (!launcher.TryGetComponent<MainMenuFlowLauncher>(out _))
        {
            launcher.AddComponent<MainMenuFlowLauncher>();
            changed = true;
        }

        return changed;
    }

    private static void SetPrefabAssetBundleName()
    {
        var prefabImporter = AssetImporter.GetAtPath(PrefabPath);
        if (prefabImporter == null)
        {
            Debug.LogError($"Cannot get prefab importer: {PrefabPath}");
            return;
        }

        if (prefabImporter.assetBundleName != MainMenuBundleName)
        {
            prefabImporter.assetBundleName = MainMenuBundleName;
            prefabImporter.SaveAndReimport();
        }
    }

    private static void ValidateBuildOutputs()
    {
        if (!File.Exists(WebGLManifestPath))
        {
            Debug.LogError($"Build output missing manifest: {WebGLManifestPath}");
            return;
        }

        var manifest = File.ReadAllText(WebGLManifestPath);
        var manifestHasPanelPrefab = manifest.Contains("Assets/Prefabs/UI/MainMenuPanel.prefab");
        if (!manifestHasPanelPrefab)
        {
            Debug.LogWarning("menu_core.manifest does not contain MainMenuPanel.prefab yet; check AB assignment and rebuild.");
        }

        if (!File.Exists(QAssetsPath))
        {
            Debug.LogWarning($"QAssets mapping file is missing: {QAssetsPath}");
            return;
        }

        var qAssets = File.ReadAllText(QAssetsPath);
        var hasPanelName = qAssets.Contains("MainMenuPanel");
        if (manifestHasPanelPrefab && hasPanelName)
        {
            Debug.Log("WebGL AB build verified: MainMenuPanel prefab is packed and QAssets mapping is updated.");
        }
        else
        {
            Debug.LogWarning("AB build finished but verification is partial; inspect menu_core.manifest and QAssets.cs.");
        }
    }

    private static void FixMainMenuSpriteMaterialsInternal(bool logResult)
    {
        if (!File.Exists(PrefabPath))
        {
            Debug.LogWarning($"MainMenu prefab not found, skip material fix: {PrefabPath}");
            return;
        }

        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            Debug.LogError("Cannot find a compatible sprite shader (URP Sprite-Unlit-Default or Sprites/Default).");
            return;
        }

        var material = AssetDatabase.LoadAssetAtPath<Material>(MainMenuMaterialPath);
        if (material == null)
        {
            material = new Material(shader) { name = "MainMenuUnlit" };
            "Assets/Arts/Materials".CreateDirIfNotExists();
            AssetDatabase.CreateAsset(material, MainMenuMaterialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
        }

        material.color = Color.white;
        EditorUtility.SetDirty(material);

        var prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);
        var renderers = prefabRoot.GetComponentsInChildren<SpriteRenderer>(true);

        var changed = false;
        foreach (var spriteRenderer in renderers)
        {
            if (spriteRenderer.sharedMaterial != material)
            {
                spriteRenderer.sharedMaterial = material;
                changed = true;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        if (changed)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (logResult)
        {
            Debug.Log($"MainMenu sprite material fixed. Shader: {shader.name}, SpriteRenderers: {renderers.Length}, Changed: {changed}");
        }
    }
}
