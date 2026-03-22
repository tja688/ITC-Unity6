#if UNITY_EDITOR
using ITC.SignMiniGame;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.SignMiniGame
{
    public static class StampPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/StampPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/SignMiniGame/重建 04印章盖印 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var root = new GameObject("StampPanel", typeof(RectTransform), typeof(StampPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.72f));
            StretchRect(dim.GetComponent<RectTransform>());

            var stampRoot = CreateImage("StampRoot", canvas.transform, new Color(0.12f, 0.10f, 0.09f, 0.97f));
            SetRect(
                stampRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1520f, 900f));

            var headerText = CreateText("HeaderText", stampRoot.transform, "印章盖印", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -54f),
                new Vector2(1080f, 72f));

            var hintText = CreateText("HintText", stampRoot.transform, "先选印章类型，再在蓄力循环里按下“盖印”。", 28,
                TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -112f),
                new Vector2(1160f, 60f));

            var guideToggleButton = CreateButton("GuideToggleButton", stampRoot.transform, "说明：开", 24,
                new Color(0.19f, 0.27f, 0.35f, 1f), out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-44f, -44f),
                new Vector2(186f, 58f));

            var typeButtonsRoot = new GameObject("TypeButtons", typeof(RectTransform));
            typeButtonsRoot.transform.SetParent(stampRoot.transform, false);
            SetRect(
                typeButtonsRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.72f),
                new Vector2(0.5f, 0.72f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1240f, 108f));

            var moneyButton = CreateButton("MoneyButton", typeButtonsRoot.transform, "金钱", 30,
                new Color(0.47f, 0.31f, 0.16f, 1f), out _);
            SetRect(moneyButton.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f), Vector2.zero, new Vector2(260f, 92f));

            var fameButton = CreateButton("FameButton", typeButtonsRoot.transform, "名利", 30,
                new Color(0.38f, 0.22f, 0.44f, 1f), out _);
            SetRect(fameButton.GetComponent<RectTransform>(), new Vector2(0.34f, 0.5f), new Vector2(0.34f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 92f));

            var skillButton = CreateButton("SkillButton", typeButtonsRoot.transform, "特技", 30,
                new Color(0.15f, 0.35f, 0.50f, 1f), out _);
            SetRect(skillButton.GetComponent<RectTransform>(), new Vector2(0.67f, 0.5f), new Vector2(0.67f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 92f));

            var eventButton = CreateButton("EventButton", typeButtonsRoot.transform, "事件", 30,
                new Color(0.41f, 0.19f, 0.19f, 1f), out _);
            SetRect(eventButton.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f), Vector2.zero, new Vector2(260f, 92f));

            var chargeArea = CreateImage("ChargeArea", stampRoot.transform, new Color(0.19f, 0.16f, 0.13f, 1f));
            SetRect(
                chargeArea.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.46f),
                new Vector2(0.5f, 0.46f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1260f, 280f));

            var chargeTrack = CreateImage("ChargeTrack", chargeArea.transform, new Color(0.08f, 0.08f, 0.08f, 0.96f));
            SetRect(
                chargeTrack.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.65f),
                new Vector2(0.5f, 0.65f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1080f, 84f));
            chargeTrack.GetComponent<Image>().raycastTarget = false;

            var normalWindow = CreateImage("NormalWindow", chargeTrack.transform, new Color(0.95f, 0.68f, 0.24f, 0.30f));
            StretchVerticalByNormalizedRange(normalWindow.GetComponent<RectTransform>(), 0.6f, 0.95f);
            normalWindow.GetComponent<Image>().raycastTarget = false;

            var perfectWindow = CreateImage("PerfectWindow", chargeTrack.transform, new Color(0.26f, 0.82f, 0.43f, 0.38f));
            StretchVerticalByNormalizedRange(perfectWindow.GetComponent<RectTransform>(), 0.72f, 0.84f);
            perfectWindow.GetComponent<Image>().raycastTarget = false;

            var chargeFill = CreateImage("ChargeFill", chargeTrack.transform, new Color(0.80f, 0.18f, 0.15f, 0.76f));
            StretchRect(chargeFill.GetComponent<RectTransform>());
            var chargeFillImage = chargeFill.GetComponent<Image>();
            chargeFillImage.type = Image.Type.Filled;
            chargeFillImage.fillMethod = Image.FillMethod.Horizontal;
            chargeFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            chargeFillImage.fillAmount = 0f;
            chargeFillImage.raycastTarget = false;

            var marker = CreateImage("Marker", chargeTrack.transform, new Color(1f, 1f, 1f, 0.98f));
            SetRect(
                marker.GetComponent<RectTransform>(),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(8f, 110f));
            marker.GetComponent<Image>().raycastTarget = false;

            var targetArea = CreateImage("TargetArea", chargeArea.transform, new Color(0.34f, 0.11f, 0.11f, 0.88f));
            SetRect(
                targetArea.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.2f),
                new Vector2(0.5f, 0.2f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(360f, 88f));
            var targetLabel = CreateText("Label", targetArea.transform, "盖印目标区域", 28, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchRect(targetLabel.rectTransform);

            var controlsRoot = new GameObject("Controls", typeof(RectTransform));
            controlsRoot.transform.SetParent(stampRoot.transform, false);
            SetRect(
                controlsRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.2f),
                new Vector2(0.5f, 0.2f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(980f, 96f));

            var stampButton = CreateButton("StampButton", controlsRoot.transform, "请先选印章", 32,
                new Color(0.42f, 0.15f, 0.13f, 1f), out _);
            SetRect(
                stampButton.GetComponent<RectTransform>(),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                Vector2.zero,
                new Vector2(430f, 92f));

            var reselectButton = CreateButton("ReselectButton", controlsRoot.transform, "重选印章", 30,
                new Color(0.19f, 0.24f, 0.30f, 1f), out _);
            SetRect(
                reselectButton.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                Vector2.zero,
                new Vector2(430f, 92f));

            var statusText = CreateText("StatusText", stampRoot.transform, "请选择印章类型。", 28,
                TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 84f),
                new Vector2(1180f, 78f));

            CreateGuideBubble(
                stampRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 先选择印章类型。\n2. 点击开始盖印进入蓄力循环。\n3. 在命中窗口按下盖印完成判定。\n4. 教学模式可重选一次印章。",
                new Vector2(-500f, 266f),
                new Vector2(520f, 210f));
            CreateGuideBubble(typeButtonsRoot.transform, "Guide_印章选择", "印章选择区：四选一。", new Vector2(0f, 94f), new Vector2(300f, 72f));
            CreateGuideBubble(chargeArea.transform, "Guide_蓄力区", "蓄力条：绿色=完美窗，黄色=普通窗。", new Vector2(0f, 114f), new Vector2(380f, 72f));
            CreateGuideBubble(stampButton.transform, "Guide_盖印按钮", "开始蓄力/按下盖印。", new Vector2(0f, 86f), new Vector2(280f, 60f));
            CreateGuideBubble(reselectButton.transform, "Guide_重选按钮", "教学模式仅可重选一次。", new Vector2(0f, 86f), new Vector2(300f, 60f));
            CreateGuideBubble(statusText.transform, "Guide_状态文本", "状态反馈：当前阶段与判定结果。", new Vector2(0f, 74f), new Vector2(360f, 60f));
            CreateGuideBubble(guideToggleButton.transform, "Guide_说明开关", "一键开关全部标注。", new Vector2(-150f, -84f), new Vector2(230f, 58f));

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            var importer = AssetImporter.GetAtPath(PrefabPath);
            if (importer != null)
            {
                importer.assetBundleName = "sign_minigame_ui";
                importer.assetBundleVariant = string.Empty;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[StampPanelPrefabBuilder] 已重建: {PrefabPath}");
        }

        private static TMP_FontAsset LoadUIFont()
        {
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
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

            var uiFont = LoadUIFont();
            if (uiFont != null)
            {
                text.font = uiFont;
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
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(StampGuideTag));
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

        private static void StretchVerticalByNormalizedRange(RectTransform rectTransform, float minX, float maxX)
        {
            SetRect(
                rectTransform,
                new Vector2(minX, 0f),
                new Vector2(maxX, 1f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
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

