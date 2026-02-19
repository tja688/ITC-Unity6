#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class RuneTypingPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/RuneTypingPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/Contracting/重建 02符文控制板 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var root = new GameObject("RuneTypingPanel", typeof(RectTransform), typeof(RuneTypingPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.75f));
            StretchRect(dim.GetComponent<RectTransform>());

            var gameRoot = CreateImage("GameRoot", canvas.transform, new Color(0.12f, 0.12f, 0.15f, 0.96f));
            SetRect(
                gameRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1640f, 940f));

            var headerText = CreateText("HeaderText", gameRoot.transform, "符文控制板 QTE", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -56f),
                new Vector2(980f, 72f));

            var hintText = CreateText("HintText", gameRoot.transform, "WASD/方向键移动，空格或回车确认符文。", 28, TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -110f),
                new Vector2(1160f, 56f));

            var guideToggleButton = CreateButton("GuideToggleButton", gameRoot.transform, "说明：开", 24,
                new Color(0.17f, 0.25f, 0.35f, 1f), out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-40f, -44f),
                new Vector2(188f, 56f));

            var targetSequenceRoot = new GameObject("TargetSequenceRoot", typeof(RectTransform), typeof(Image));
            targetSequenceRoot.transform.SetParent(gameRoot.transform, false);
            var targetBg = targetSequenceRoot.GetComponent<Image>();
            targetBg.color = new Color(0.08f, 0.09f, 0.10f, 0.92f);
            targetBg.raycastTarget = false;
            SetRect(
                targetSequenceRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -188f),
                new Vector2(1280f, 94f));

            for (var i = 0; i < 8; i++)
            {
                var targetText = CreateText(
                    $"Target_{i:00}",
                    targetSequenceRoot.transform,
                    "上",
                    40,
                    TextAlignmentOptions.Center,
                    FontStyles.Bold);
                SetRect(
                    targetText.rectTransform,
                    new Vector2(0f, 0.5f),
                    new Vector2(0f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(100f + i * 150f, 0f),
                    new Vector2(110f, 72f));
            }

            var gridRoot = new GameObject("GridRoot", typeof(RectTransform), typeof(Image));
            gridRoot.transform.SetParent(gameRoot.transform, false);
            var gridBg = gridRoot.GetComponent<Image>();
            gridBg.color = new Color(0.09f, 0.10f, 0.12f, 0.95f);
            SetRect(
                gridRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-220f, -20f),
                new Vector2(760f, 760f));

            // Add a prominent current-target highlight text above the grid
            var currentTargetHighlight = CreateText("CurrentTargetHighlight", gameRoot.transform,
                "当前目标: ▲  (1/4)", 52, TextAlignmentOptions.Center, FontStyles.Bold);
            currentTargetHighlight.color = new Color(1f, 0.92f, 0.2f, 1f);
            SetRect(
                currentTargetHighlight.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-220f, 400f),
                new Vector2(760f, 72f));

            // GridLayoutGroup will arrange cells at runtime; place them at origin here
            const int maxGridSize = 5;
            const float cellSize = 116f;

            for (var row = 0; row < maxGridSize; row++)
            {
                for (var col = 0; col < maxGridSize; col++)
                {
                    var index = row * maxGridSize + col;
                    var glyph = (index % 4) switch
                    {
                        0 => "▲",
                        1 => "▼",
                        2 => "◀",
                        _ => "▶"
                    };
                    var cellButton = CreateButton(
                        $"Cell_{index:00}",
                        gridRoot.transform,
                        glyph,
                        42,
                        new Color(0.18f, 0.20f, 0.23f, 1f),
                        out _);
                    // Set a default size; GridLayoutGroup will override position at runtime
                    SetRect(
                        cellButton.GetComponent<RectTransform>(),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        Vector2.zero,
                        new Vector2(cellSize, cellSize));
                }
            }

            var cursorFrame = new GameObject("CursorFrame", typeof(RectTransform), typeof(Image));
            cursorFrame.transform.SetParent(gridRoot.transform, false);
            var cursorImage = cursorFrame.GetComponent<Image>();
            cursorImage.color = new Color(1f, 0.86f, 0.20f, 0.20f);
            cursorImage.raycastTarget = false;
            SetRect(
                cursorFrame.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(cellSize + 12f, cellSize + 12f));

            var onScreenButtons = new GameObject("OnScreenButtons", typeof(RectTransform));
            onScreenButtons.transform.SetParent(gameRoot.transform, false);
            SetRect(
                onScreenButtons.GetComponent<RectTransform>(),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-70f, 138f),
                new Vector2(380f, 270f));

            var upButton = CreateButton("UpButton", onScreenButtons.transform, "上", 30, new Color(0.19f, 0.29f, 0.41f, 1f), out _);
            var downButton = CreateButton("DownButton", onScreenButtons.transform, "下", 30, new Color(0.19f, 0.29f, 0.41f, 1f), out _);
            var leftButton = CreateButton("LeftButton", onScreenButtons.transform, "左", 30, new Color(0.19f, 0.29f, 0.41f, 1f), out _);
            var rightButton = CreateButton("RightButton", onScreenButtons.transform, "右", 30, new Color(0.19f, 0.29f, 0.41f, 1f), out _);
            var confirmButton = CreateButton("ConfirmButton", onScreenButtons.transform, "确认", 30, new Color(0.29f, 0.42f, 0.20f, 1f), out _);

            SetRect(upButton.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-95f, -20f), new Vector2(110f, 68f));
            SetRect(downButton.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-95f, -176f), new Vector2(110f, 68f));
            SetRect(leftButton.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-175f, -98f), new Vector2(110f, 68f));
            SetRect(rightButton.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-15f, -98f), new Vector2(110f, 68f));
            SetRect(confirmButton.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, 0f), new Vector2(136f, 176f));

            var statusText = CreateText("StatusText", gameRoot.transform, "序列进度：0/0", 30, TextAlignmentOptions.Left, FontStyles.Normal);
            SetRect(
                statusText.rectTransform,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(70f, 62f),
                new Vector2(580f, 60f));

            var errorCountText = CreateText("ErrorCountText", gameRoot.transform, "失误次数：0", 30, TextAlignmentOptions.Left, FontStyles.Bold);
            SetRect(
                errorCountText.rectTransform,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(70f, 22f),
                new Vector2(580f, 60f));

            var countdownText = CreateText("CountdownText", gameRoot.transform, "3", 120, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                countdownText.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-220f, -22f),
                new Vector2(220f, 160f));

            CreateGuideBubble(gameRoot.transform, "Guide_总说明", "操作说明\\n1. 先看顶部目标序列。\\n2. 用 WASD/方向键或右侧按钮移动。\\n3. 用空格/回车或确认键提交。\\n4. 右上角可开关说明。", new Vector2(-580f, 258f), new Vector2(420f, 212f));
            CreateGuideBubble(targetSequenceRoot.transform, "Guide_目标序列", "目标序列：按顺序输入对应符文。", new Vector2(0f, 86f), new Vector2(330f, 70f));
            CreateGuideBubble(gridRoot.transform, "Guide_网格", "网格区：黄色框是当前光标。", new Vector2(0f, 382f), new Vector2(300f, 66f));
            CreateGuideBubble(onScreenButtons.transform, "Guide_屏幕按钮", "屏幕按钮：WebGL 焦点丢失时可用。", new Vector2(-84f, 148f), new Vector2(320f, 70f));
            CreateGuideBubble(statusText.transform, "Guide_状态文本", "状态文本：显示当前输入进度。", new Vector2(0f, 64f), new Vector2(300f, 58f));
            CreateGuideBubble(errorCountText.transform, "Guide_错误计数", "错误计数：仅错误确认才累计。", new Vector2(0f, 64f), new Vector2(320f, 58f));

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            var importer = AssetImporter.GetAtPath(PrefabPath);
            if (importer != null)
            {
                importer.assetBundleName = "contracting_ui";
                importer.assetBundleVariant = string.Empty;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[RuneTypingPanelPrefabBuilder] 已重建: {PrefabPath}");
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            var folder = System.IO.Path.GetFileName(folderPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(folder))
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, folder);
        }

        private static GameObject CreateCanvas(Transform parent)
        {
            var canvas = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvas.transform.SetParent(parent, false);
            StretchRect(canvas.GetComponent<RectTransform>());

            var canvasComp = canvas.GetComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComp.pixelPerfect = false;

            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static GameObject CreateImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return go;
        }

        private static GameObject CreateButton(
            string name,
            Transform parent,
            string label,
            float labelSize,
            Color color,
            out Button button)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.color = color;

            button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
            button.colors = colors;

            var text = CreateText("Label", go.transform, label, labelSize, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchRect(text.rectTransform);
            text.raycastTarget = false;

            return go;
        }

        private static TMP_Text CreateText(
            string name,
            Transform parent,
            string content,
            float fontSize,
            TextAlignmentOptions alignment,
            FontStyles styles)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.fontStyle = styles;
            text.color = Color.white;
            text.textWrappingMode = TextWrappingModes.Normal;

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            if (fontAsset != null)
            {
                text.font = fontAsset;
            }

            return text;
        }

        private static void CreateGuideBubble(
            Transform target,
            string name,
            string content,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(RuneTypingGuideTag));
            bubble.transform.SetParent(target, false);
            var bubbleRect = bubble.GetComponent<RectTransform>();
            SetRect(
                bubbleRect,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                anchoredPosition,
                sizeDelta);

            var image = bubble.GetComponent<Image>();
            image.color = new Color(0.07f, 0.07f, 0.07f, 0.88f);
            image.raycastTarget = false;

            var text = CreateText("Text", bubble.transform, content, 20, TextAlignmentOptions.Left, FontStyles.Normal);
            StretchRect(text.rectTransform);
            text.margin = new Vector4(14f, 10f, 14f, 10f);
            text.color = new Color(1f, 0.95f, 0.72f, 1f);
            text.raycastTarget = false;
        }

        private static void StretchRect(RectTransform rectTransform)
        {
            SetRect(rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        }

        private static void SetRect(
            RectTransform rectTransform,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }
    }
}
#endif
