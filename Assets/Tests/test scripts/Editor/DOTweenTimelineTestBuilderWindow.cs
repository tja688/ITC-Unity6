using System;
using System.IO;
using DG.Tweening;
using Dott;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DOTweenTimelineTestBuilderWindow : EditorWindow
{
    private const string RootName = "DOTweenTimeline_TestRoot";
    private const string TagName = "Actor";
    private const string GeneratedFolder = "Assets/Tests/test scripts/Generated";
    private const int TextureSize = 64;

    [SerializeField] private int actorCount = 20;
    [SerializeField] private Vector2 startAreaSize = new Vector2(6f, 4f);
    [SerializeField] private Vector3 startAreaPosition = new Vector3(-4f, 0f, 0f);
    [SerializeField] private Vector2 endAreaSize = new Vector2(6f, 4f);
    [SerializeField] private Vector3 endAreaPosition = new Vector3(4f, 0f, 0f);
    [SerializeField] private Vector2 durationRange = new Vector2(0.6f, 1.6f);
    [SerializeField] private bool addScaleTween;
    [SerializeField] private bool addRotateTween;
    [SerializeField] private Vector2 scaleTargetRange = new Vector2(0.6f, 1.4f);
    [SerializeField] private Vector2 rotateDegreesRange = new Vector2(90f, 360f);
    [SerializeField] private bool clearBeforeCreate = true;
    [SerializeField] private bool useUndo = false;
    [SerializeField] private bool useSeed = false;
    [SerializeField] private int randomSeed = 12345;

    private static readonly Ease[] EasePool =
    {
        Ease.Linear,
        Ease.InOutQuad,
        Ease.OutQuad,
        Ease.InOutSine,
        Ease.OutCubic,
        Ease.InOutCubic,
        Ease.OutBack
    };

    private enum ShapeType
    {
        Square,
        Triangle,
        Circle
    }

    [MenuItem("Tools/DOTweenTimeline/Test Builder")]
    private static void Open()
    {
        GetWindow<DOTweenTimelineTestBuilderWindow>("Dott Test Builder");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Create Test Setup", EditorStyles.boldLabel);
        actorCount = Mathf.Max(1, EditorGUILayout.IntField("Actor Count", actorCount));
        durationRange = DrawMinMax("Duration Range (s)", durationRange, 0.05f, 10f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Start Area", EditorStyles.boldLabel);
        startAreaPosition = EditorGUILayout.Vector3Field("Position", startAreaPosition);
        startAreaPosition.z = 0f;
        startAreaSize = EditorGUILayout.Vector2Field("Size", startAreaSize);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("End Area", EditorStyles.boldLabel);
        endAreaPosition = EditorGUILayout.Vector3Field("Position", endAreaPosition);
        endAreaPosition.z = 0f;
        endAreaSize = EditorGUILayout.Vector2Field("Size", endAreaSize);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Optional Overlap Tweens", EditorStyles.boldLabel);
        addScaleTween = EditorGUILayout.Toggle("Add Scale Tween", addScaleTween);
        if (addScaleTween)
        {
            scaleTargetRange = DrawMinMax("Scale Target Range", scaleTargetRange, 0.1f, 5f);
        }

        addRotateTween = EditorGUILayout.Toggle("Add Rotate Tween", addRotateTween);
        if (addRotateTween)
        {
            rotateDegreesRange = DrawMinMax("Rotate Degrees Range", rotateDegreesRange, 0f, 1080f);
        }

        EditorGUILayout.Space();
        clearBeforeCreate = EditorGUILayout.Toggle("Clear Before Create", clearBeforeCreate);
        useUndo = EditorGUILayout.Toggle("Use Undo (slower)", useUndo);
        useSeed = EditorGUILayout.Toggle("Use Random Seed", useSeed);
        if (useSeed)
        {
            randomSeed = EditorGUILayout.IntField("Seed", randomSeed);
        }

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Create Test Setup", GUILayout.Height(30)))
            {
                CreateTestSetup();
            }

            if (GUILayout.Button("Clear Test Setup", GUILayout.Height(30)))
            {
                ClearTestSetup();
            }
        }
    }

    private void CreateTestSetup()
    {
        if (clearBeforeCreate)
        {
            ClearTestSetup();
        }

        EnsureTagExists(TagName);

        Sprite[] shapes = GetOrCreateShapes();
        if (shapes.Length == 0)
        {
            Debug.LogError("Failed to create shape sprites for test setup.");
            return;
        }

        var root = new GameObject(RootName);
        if (useUndo)
        {
            Undo.RegisterCreatedObjectUndo(root, "Create DOTween Timeline Test Root");
        }

        var timelineGO = new GameObject("DOTweenTimeline");
        timelineGO.transform.SetParent(root.transform, false);
        var timeline = timelineGO.AddComponent<DOTweenTimeline>();

        var startAreaGO = new GameObject("StartArea");
        startAreaGO.transform.SetParent(root.transform, false);
        startAreaGO.transform.position = new Vector3(startAreaPosition.x, startAreaPosition.y, 0f);
        var startArea = startAreaGO.AddComponent<BoxCollider2D>();
        startArea.size = new Vector2(Mathf.Max(0.1f, startAreaSize.x), Mathf.Max(0.1f, startAreaSize.y));
        startArea.isTrigger = true;

        var endAreaGO = new GameObject("EndArea");
        endAreaGO.transform.SetParent(root.transform, false);
        endAreaGO.transform.position = new Vector3(endAreaPosition.x, endAreaPosition.y, 0f);
        var endArea = endAreaGO.AddComponent<BoxCollider2D>();
        endArea.size = new Vector2(Mathf.Max(0.1f, endAreaSize.x), Mathf.Max(0.1f, endAreaSize.y));
        endArea.isTrigger = true;

        var actorsRoot = new GameObject("Actors");
        actorsRoot.transform.SetParent(root.transform, false);

        var marker = root.AddComponent<DOTweenTimelineTestRoot>();
        marker.Assign(timeline, startArea, endArea, actorsRoot.transform);

        System.Random rng = useSeed ? new System.Random(randomSeed) : new System.Random();

        float currentDelay = 0f;
        for (int i = 0; i < actorCount; i++)
        {
            var actor = new GameObject($"Actor_{i:D3}");
            actor.tag = TagName;
            actor.transform.SetParent(actorsRoot.transform, false);

            Vector3 startPos = RandomPointInBox(startArea, rng);
            actor.transform.position = startPos;
            actor.transform.rotation = Quaternion.identity;
            actor.transform.localScale = Vector3.one;

            var renderer = actor.AddComponent<SpriteRenderer>();
            renderer.sprite = shapes[rng.Next(shapes.Length)];
            renderer.color = RandomColor(rng);

            Vector3 endPos = RandomPointInBox(endArea, rng);
            float moveDuration = RandomRange(rng, durationRange, 0.1f);
            Ease moveEase = RandomEase(rng);

            CreateMoveTween(timelineGO, actor.transform, endPos, currentDelay, moveDuration, moveEase, i);

            float groupDuration = moveDuration;

            if (addScaleTween)
            {
                float scaleDuration = RandomRange(rng, durationRange, 0.1f);
                float targetScale = RandomRange(rng, scaleTargetRange, 0.1f);
                Ease scaleEase = RandomEase(rng);
                CreateScaleTween(timelineGO, actor.transform, currentDelay, scaleDuration, targetScale, scaleEase, i);
                groupDuration = Mathf.Max(groupDuration, scaleDuration);
            }

            if (addRotateTween)
            {
                float rotateDuration = RandomRange(rng, durationRange, 0.1f);
                float degrees = RandomRange(rng, rotateDegreesRange, 0f);
                Ease rotateEase = RandomEase(rng);
                CreateRotateTween(timelineGO, actor.transform, currentDelay, rotateDuration, degrees, rotateEase, i);
                groupDuration = Mathf.Max(groupDuration, rotateDuration);
            }

            currentDelay += groupDuration;
        }

        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private void ClearTestSetup()
    {
        DOTweenTimelineTestRoot[] roots = Object.FindObjectsByType<DOTweenTimelineTestRoot>(FindObjectsSortMode.None);
        if (roots.Length == 0)
        {
            return;
        }

        foreach (var root in roots)
        {
            if (root == null)
            {
                continue;
            }

            if (useUndo)
            {
                Undo.DestroyObjectImmediate(root.gameObject);
            }
            else
            {
                Object.DestroyImmediate(root.gameObject);
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private static void CreateMoveTween(GameObject timelineGO, Transform target, Vector3 endPos, float delay, float duration, Ease ease, int index)
    {
        DOTweenAnimation anim = CreateBaseAnimation(timelineGO, target, delay, duration, ease);
        anim.animationType = DOTweenAnimation.AnimationType.Move;
        anim.targetType = DOTweenAnimation.TargetType.Transform;
        anim.endValueV3 = new Vector3(endPos.x, endPos.y, 0f);
        anim.optionalBool0 = false;
        anim.useTargetAsV3 = false;
        anim.id = $"Actor_{index:D3}_Move";
    }

    private static void CreateScaleTween(GameObject timelineGO, Transform target, float delay, float duration, float targetScale, Ease ease, int index)
    {
        DOTweenAnimation anim = CreateBaseAnimation(timelineGO, target, delay, duration, ease);
        anim.animationType = DOTweenAnimation.AnimationType.Scale;
        anim.targetType = DOTweenAnimation.TargetType.Transform;
        anim.optionalBool0 = true;
        anim.endValueFloat = targetScale;
        anim.id = $"Actor_{index:D3}_Scale";
    }

    private static void CreateRotateTween(GameObject timelineGO, Transform target, float delay, float duration, float degrees, Ease ease, int index)
    {
        DOTweenAnimation anim = CreateBaseAnimation(timelineGO, target, delay, duration, ease);
        anim.animationType = DOTweenAnimation.AnimationType.Rotate;
        anim.targetType = DOTweenAnimation.TargetType.Transform;
        anim.endValueV3 = new Vector3(0f, 0f, degrees);
        anim.optionalRotationMode = RotateMode.FastBeyond360;
        anim.isRelative = true;
        anim.id = $"Actor_{index:D3}_Rotate";
    }

    private static DOTweenAnimation CreateBaseAnimation(GameObject timelineGO, Transform target, float delay, float duration, Ease ease)
    {
        DOTweenAnimation anim = timelineGO.AddComponent<DOTweenAnimation>();
        anim.targetIsSelf = false;
        anim.targetGO = target.gameObject;
        anim.tweenTargetIsTargetGO = true;
        anim.target = target;
        anim.targetType = DOTweenAnimation.TargetType.Transform;
        anim.forcedTargetType = DOTweenAnimation.TargetType.Unset;
        anim.delay = Mathf.Max(0f, delay);
        anim.duration = Mathf.Max(0f, duration);
        anim.easeType = ease;
        anim.isActive = true;
        anim.isValid = true;
        anim.isRelative = false;
        anim.isFrom = false;
        anim.isSpeedBased = false;
        anim.autoGenerate = false;
        anim.autoPlay = false;
        anim.autoKill = false;
        anim.loops = 1;
        anim.loopType = LoopType.Restart;
        return anim;
    }

    private static Vector3 RandomPointInBox(BoxCollider2D area, System.Random rng)
    {
        Vector2 size = area.size;
        Vector3 center = area.transform.position + (Vector3)area.offset;
        float x = (float)(center.x + (rng.NextDouble() - 0.5) * size.x);
        float y = (float)(center.y + (rng.NextDouble() - 0.5) * size.y);
        return new Vector3(x, y, 0f);
    }

    private static float RandomRange(System.Random rng, Vector2 range, float minClamp)
    {
        float min = Mathf.Min(range.x, range.y);
        float max = Mathf.Max(range.x, range.y);
        if (Mathf.Approximately(min, max))
        {
            return Mathf.Max(minClamp, min);
        }

        double t = rng.NextDouble();
        float value = (float)(min + (max - min) * t);
        return Mathf.Max(minClamp, value);
    }

    private static Ease RandomEase(System.Random rng)
    {
        return EasePool[rng.Next(EasePool.Length)];
    }

    private static Color RandomColor(System.Random rng)
    {
        float h = (float)rng.NextDouble();
        float s = (float)(0.6f + rng.NextDouble() * 0.4f);
        float v = (float)(0.7f + rng.NextDouble() * 0.3f);
        return Color.HSVToRGB(h, s, v);
    }

    private static Vector2 DrawMinMax(string label, Vector2 range, float minLimit, float maxLimit)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(150f));
        float min = Mathf.Clamp(range.x, minLimit, maxLimit);
        float max = Mathf.Clamp(range.y, minLimit, maxLimit);
        min = EditorGUILayout.FloatField(min, GUILayout.Width(60f));
        EditorGUILayout.MinMaxSlider(ref min, ref max, minLimit, maxLimit);
        max = EditorGUILayout.FloatField(max, GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();
        return new Vector2(min, max);
    }

    private static void EnsureTagExists(string tag)
    {
        foreach (string existingTag in InternalEditorUtility.tags)
        {
            if (existingTag == tag)
            {
                return;
            }
        }

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static Sprite[] GetOrCreateShapes()
    {
        EnsureFolder(GeneratedFolder);
        Sprite square = LoadOrCreateShape("dott_test_square.png", ShapeType.Square);
        Sprite triangle = LoadOrCreateShape("dott_test_triangle.png", ShapeType.Triangle);
        Sprite circle = LoadOrCreateShape("dott_test_circle.png", ShapeType.Circle);

        return new[] { square, triangle, circle };
    }

    private static Sprite LoadOrCreateShape(string fileName, ShapeType shapeType)
    {
        string path = $"{GeneratedFolder}/{fileName}";
        Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
        {
            return existing;
        }

        Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.name = Path.GetFileNameWithoutExtension(fileName);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        Color32[] pixels = new Color32[TextureSize * TextureSize];
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                bool inside = IsInsideShape(x, y, TextureSize, shapeType);
                pixels[y * TextureSize + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        byte[] png = texture.EncodeToPNG();
        File.WriteAllBytes(path, png);
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100f;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static bool IsInsideShape(int x, int y, int size, ShapeType shapeType)
    {
        switch (shapeType)
        {
            case ShapeType.Square:
                return true;
            case ShapeType.Circle:
            {
                float cx = size * 0.5f;
                float cy = size * 0.5f;
                float dx = x - cx + 0.5f;
                float dy = y - cy + 0.5f;
                float r = size * 0.5f - 1f;
                return (dx * dx + dy * dy) <= r * r;
            }
            case ShapeType.Triangle:
            {
                float fx = (float)x / (size - 1);
                float fy = (float)y / (size - 1);
                float edge = 1f - Mathf.Abs(2f * fx - 1f);
                return fy <= edge;
            }
            default:
                return false;
        }
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string folderName = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        if (!string.IsNullOrEmpty(parent))
        {
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
