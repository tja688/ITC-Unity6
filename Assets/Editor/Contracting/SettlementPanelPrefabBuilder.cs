#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class SettlementPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/SettlementPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/Contracting/重建 07结算离场 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            var root = new GameObject("SettlementPanel", typeof(RectTransform), typeof(SettlementPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.65f));
            StretchRect(dim.GetComponent<RectTransform>());

            var settlementRoot = CreateImage("SettlementRoot", canvas.transform, new Color(0.09f, 0.09f, 0.12f, 0.97f));
            SetRect(
                settlementRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1240f, 700f));

            var pulse = CreateImage("Pulse", settlementRoot.transform, new Color(0.64f, 0.56f, 0.18f, 0.24f));
            SetRect(
                pulse.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1120f, 540f));
            pulse.GetComponent<Image>().raycastTarget = false;

            var headerText = CreateText("HeaderText", settlementRoot.transform, "结算反馈", 46, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -54f),
                new Vector2(820f, 66f));

            var summaryText = CreateText("SummaryText", settlementRoot.transform, "满意度 3 | 小费 +0 | 余额 0", 32, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                summaryText.rectTransform,
                new Vector2(0.5f, 0.78f),
                new Vector2(0.5f, 0.78f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(920f, 60f));

            var detailText = CreateText("DetailText", settlementRoot.transform, "文书:passed  QTE错:0  印章:事件/normal  灵魂:45%  豆罐:success", 24, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                detailText.rectTransform,
                new Vector2(0.5f, 0.68f),
                new Vector2(0.5f, 0.68f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1080f, 52f));

            var tipText = CreateText("TipText", settlementRoot.transform, "本次小费：+0", 30, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                tipText.rectTransform,
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 24f),
                new Vector2(420f, 56f));

            var moneyText = CreateText("MoneyText", settlementRoot.transform, "当前收入：0", 30, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                moneyText.rectTransform,
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -30f),
                new Vector2(420f, 56f));

            var tierText = CreateText("TierText", settlementRoot.transform, "评价：普通", 32, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                tierText.rectTransform,
                new Vector2(0.5f, 0.38f),
                new Vector2(0.5f, 0.38f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(420f, 56f));

            var statusText = CreateText("StatusText", settlementRoot.transform, "Collect：聚合本客户结果。", 26, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0.18f),
                new Vector2(0.5f, 0.18f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(920f, 52f));

            var controls = new GameObject("Controls", typeof(RectTransform));
            controls.transform.SetParent(settlementRoot.transform, false);
            SetRect(
                controls.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.08f),
                new Vector2(0.5f, 0.08f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(400f, 84f));

            var skipButton = CreateButton("SkipButton", controls.transform, "跳过展示", 28, new Color(0.26f, 0.30f, 0.36f, 1f), fontAsset, out _);
            SetRect(
                skipButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(320f, 76f));

            var guideToggle = CreateButton("GuideToggleButton", settlementRoot.transform, "说明：开", 24, new Color(0.18f, 0.25f, 0.35f, 1f), fontAsset, out _);
            SetRect(
                guideToggle.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-38f, -38f),
                new Vector2(180f, 54f));

            CreateGuideBubble(
                settlementRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 结算会自动按顺序执行。\n2. 可点击“跳过展示”立即完成。\n3. 结算只做数据收口，不依赖场景对象。",
                new Vector2(-400f, 210f),
                new Vector2(430f, 190f),
                fontAsset);
            CreateGuideBubble(summaryText.transform, "Guide_汇总", "汇总：满意度、小费、余额。", new Vector2(0f, 54f), new Vector2(280f, 58f), fontAsset);
            CreateGuideBubble(detailText.transform, "Guide_来源", "来源：Route_* 与失误/满意度。", new Vector2(0f, 50f), new Vector2(300f, 56f), fontAsset);
            CreateGuideBubble(tipText.transform, "Guide_小费", "小费：按社会阶层与满意度计算。", new Vector2(0f, 50f), new Vector2(320f, 56f), fontAsset);
            CreateGuideBubble(statusText.transform, "Guide_流程", "流程：Collect->Evaluate->Reward->Narrate->RouteNext。", new Vector2(0f, 54f), new Vector2(420f, 58f), fontAsset);

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
            Debug.Log($"[SettlementPanelPrefabBuilder] 已重建: {PrefabPath}");
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
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(SettlementGuideTag));
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
