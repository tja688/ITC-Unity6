#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class BeanSellPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/BeanSellPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/Contracting/重建 06豆罐头推销 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            var root = new GameObject("BeanSellPanel", typeof(RectTransform), typeof(BeanSellPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.58f));
            StretchRect(dim.GetComponent<RectTransform>());

            var beanRoot = CreateImage("BeanRoot", canvas.transform, new Color(0.11f, 0.10f, 0.08f, 0.97f));
            SetRect(
                beanRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1320f, 760f));

            var resultGlow = CreateImage("ResultGlow", beanRoot.transform, new Color(0.24f, 0.76f, 0.34f, 0.25f));
            SetRect(
                resultGlow.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1220f, 620f));
            resultGlow.GetComponent<Image>().raycastTarget = false;

            var headerText = CreateText("HeaderText", beanRoot.transform, "豆罐头推销", 46, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -56f),
                new Vector2(900f, 68f));

            var hintText = CreateText("HintText", beanRoot.transform, "请选择一条话术：强推 / 共情 / 利益。", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -112f),
                new Vector2(980f, 54f));

            var guideToggleButton = CreateButton(
                "GuideToggleButton",
                beanRoot.transform,
                "说明：开",
                24,
                new Color(0.18f, 0.25f, 0.35f, 1f),
                fontAsset,
                out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-40f, -40f),
                new Vector2(186f, 56f));

            var preferenceText = CreateText("PreferenceText", beanRoot.transform, "客户偏好：利益", 30, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                preferenceText.rectTransform,
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(480f, 56f));

            var soldCountText = CreateText("SoldCountText", beanRoot.transform, "今日已售：0", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                soldCountText.rectTransform,
                new Vector2(0.5f, 0.68f),
                new Vector2(0.5f, 0.68f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(420f, 50f));

            var scoreText = CreateText("ScoreText", beanRoot.transform, "分数：待选择", 30, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                scoreText.rectTransform,
                new Vector2(0.5f, 0.20f),
                new Vector2(0.5f, 0.20f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(480f, 56f));

            var statusText = CreateText("StatusText", beanRoot.transform, "请选择话术，完成本次推销判定。", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0.10f),
                new Vector2(0.5f, 0.10f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(980f, 58f));

            var pitchButtonsRoot = new GameObject("PitchButtons", typeof(RectTransform));
            pitchButtonsRoot.transform.SetParent(beanRoot.transform, false);
            SetRect(
                pitchButtonsRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.45f),
                new Vector2(0.5f, 0.45f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1080f, 140f));

            var strongButton = CreateButton("StrongPushButton", pitchButtonsRoot.transform, "强推", 34, new Color(0.49f, 0.18f, 0.16f, 1f), fontAsset, out _);
            SetRect(
                strongButton.GetComponent<RectTransform>(),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                Vector2.zero,
                new Vector2(320f, 120f));

            var empathyButton = CreateButton("EmpathyButton", pitchButtonsRoot.transform, "共情", 34, new Color(0.18f, 0.30f, 0.45f, 1f), fontAsset, out _);
            SetRect(
                empathyButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(320f, 120f));

            var benefitButton = CreateButton("BenefitButton", pitchButtonsRoot.transform, "利益", 34, new Color(0.23f, 0.39f, 0.18f, 1f), fontAsset, out _);
            SetRect(
                benefitButton.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                Vector2.zero,
                new Vector2(320f, 120f));

            CreateGuideBubble(
                beanRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 选择一条推销话术。\n2. 系统按客户偏好与阈值判定成功或失败。\n3. 失败会降低满意度，成功增加已售数。",
                new Vector2(-420f, 230f),
                new Vector2(450f, 200f),
                fontAsset);
            CreateGuideBubble(preferenceText.transform, "Guide_偏好提示", "客户偏好提示（可配置隐藏）。", new Vector2(0f, 56f), new Vector2(280f, 60f), fontAsset);
            CreateGuideBubble(soldCountText.transform, "Guide_当日销量", "记录当日累计售出数。", new Vector2(0f, 52f), new Vector2(240f, 56f), fontAsset);
            CreateGuideBubble(pitchButtonsRoot.transform, "Guide_话术按钮", "话术按钮：强推 / 共情 / 利益。", new Vector2(0f, 112f), new Vector2(320f, 62f), fontAsset);
            CreateGuideBubble(scoreText.transform, "Guide_分数区", "显示 finalScore 与 threshold。", new Vector2(0f, 54f), new Vector2(300f, 56f), fontAsset);
            CreateGuideBubble(statusText.transform, "Guide_反馈区", "结算反馈：成功/失败/跳过。", new Vector2(0f, 56f), new Vector2(300f, 56f), fontAsset);

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
            Debug.Log($"[BeanSellPanelPrefabBuilder] 已重建: {PrefabPath}");
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
            var canvas = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
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
            TMP_FontAsset fontAsset,
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

            var text = CreateText("Label", go.transform, label, labelSize, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
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
            FontStyles styles,
            TMP_FontAsset fontAsset)
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
            Vector2 sizeDelta,
            TMP_FontAsset fontAsset)
        {
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(BeanSellGuideTag));
            bubble.transform.SetParent(target, false);
            SetRect(
                bubble.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                anchoredPosition,
                sizeDelta);

            var image = bubble.GetComponent<Image>();
            image.color = new Color(0.07f, 0.07f, 0.07f, 0.88f);
            image.raycastTarget = false;

            var text = CreateText("Text", bubble.transform, content, 20, TextAlignmentOptions.Left, FontStyles.Normal, fontAsset);
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
