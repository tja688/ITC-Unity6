#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class DocumentReviewPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/DocumentReviewPanel.prefab";

        [MenuItem("ITC/Contracting/重建 01文书审核 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var root = new GameObject("DocumentReviewPanel", typeof(RectTransform), typeof(DocumentReviewPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.72f));
            StretchRect(dim.GetComponent<RectTransform>());

            var reviewRoot = CreateImage("ReviewRoot", canvas.transform, new Color(0.16f, 0.13f, 0.11f, 0.96f));
            SetRect(
                reviewRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1560f, 900f));

            var headerText = CreateText("HeaderText", reviewRoot.transform, "文书审核", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -52f),
                new Vector2(1080f, 72f));

            var hintText = CreateText(
                "HintText",
                reviewRoot.transform,
                "请先检查文书热点，再选择“通过”或“退回”。",
                28,
                TextAlignmentOptions.Center,
                FontStyles.Normal);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -106f),
                new Vector2(1180f, 58f));

            var guideToggleButton = CreateButton("GuideToggleButton", reviewRoot.transform, "说明：开", 24,
                new Color(0.19f, 0.27f, 0.35f, 1f), out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-40f, -44f),
                new Vector2(186f, 58f));

            var documentArea = CreateButton("DocumentArea", reviewRoot.transform, string.Empty, 20,
                new Color(0.90f, 0.84f, 0.70f, 1f), out _);
            SetRect(
                documentArea.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.62f),
                new Vector2(0.5f, 0.62f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1140f, 520f));

            var docTitle = CreateText("DocTitle", documentArea.transform, "签约文书（白模）", 36, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                docTitle.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -36f),
                new Vector2(560f, 56f));

            var hotspotSeal = CreateHotspot(documentArea.transform, "Hotspot_Seal", new Vector2(-360f, -136f), "印章检查热点");
            var hotspotInk = CreateHotspot(documentArea.transform, "Hotspot_Ink", new Vector2(-72f, 44f), "墨迹检查热点");
            var hotspotDate = CreateHotspot(documentArea.transform, "Hotspot_Date", new Vector2(280f, 94f), "日期检查热点");
            var hotspotContent = CreateHotspot(documentArea.transform, "Hotspot_Content", new Vector2(322f, -152f), "条款检查热点");

            var controlsRoot = new GameObject("Controls", typeof(RectTransform));
            controlsRoot.transform.SetParent(reviewRoot.transform, false);
            SetRect(
                controlsRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 84f),
                new Vector2(1300f, 88f));

            var passButton = CreateButton("PassButton", controlsRoot.transform, "通过并提交", 32,
                new Color(0.19f, 0.42f, 0.25f, 1f), out _);
            SetRect(
                passButton.GetComponent<RectTransform>(),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0f),
                new Vector2(380f, 88f));

            var rejectButton = CreateButton("RejectButton", controlsRoot.transform, "退回并选理由", 32,
                new Color(0.52f, 0.21f, 0.20f, 1f), out _);
            SetRect(
                rejectButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(380f, 88f));

            var zoomButton = CreateButton("ZoomButton", controlsRoot.transform, "文档缩放 x1.5", 30,
                new Color(0.20f, 0.30f, 0.44f, 1f), out _);
            SetRect(
                zoomButton.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(0f, 0f),
                new Vector2(380f, 88f));

            var statusText = CreateText("StatusText", reviewRoot.transform, "请先检查热点，再做判定。", 28,
                TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 184f),
                new Vector2(1260f, 76f));

            var rejectReasons = CreateImage("RejectReasons", reviewRoot.transform, new Color(0.10f, 0.10f, 0.10f, 0.93f));
            SetRect(
                rejectReasons.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-38f, 18f),
                new Vector2(436f, 666f));

            var rejectTitle = CreateText("ReasonTitle", rejectReasons.transform, "请选择退回理由", 32,
                TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(
                rejectTitle.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -60f),
                new Vector2(360f, 64f));

            var reasonImage = CreateReasonButton(rejectReasons.transform, "Reason_ImageMismatch", "图像不符", 0);
            var reasonDate = CreateReasonButton(rejectReasons.transform, "Reason_DateMismatch", "日期不符", 1);
            var reasonApplication = CreateReasonButton(rejectReasons.transform, "Reason_ApplicationMismatch", "申请不符", 2);
            var reasonForgery = CreateReasonButton(rejectReasons.transform, "Reason_PaperForgery", "纸张造假", 3);
            var reasonDamage = CreateReasonButton(rejectReasons.transform, "Reason_PaperDamage", "纸张破损", 4);

            var cancelReasonButton = CreateButton("CancelButton", rejectReasons.transform, "返回重选", 28,
                new Color(0.24f, 0.24f, 0.24f, 1f), out _);
            SetRect(
                cancelReasonButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 40f),
                new Vector2(260f, 68f));

            rejectReasons.SetActive(false);

            CreateGuideBubble(
                reviewRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 先点击文书上的橙色热点完成检查。\n2. 可点“文档缩放”观察细节。\n3. 检查完成后点“通过并提交”，或点“退回并选理由”。\n4. 可点击右上角“说明：开/关”统一切换标注。",
                new Vector2(-500f, 300f),
                new Vector2(520f, 220f));

            CreateGuideBubble(documentArea.transform, "Guide_文书区域", "文书区域\n用于承载可检查内容与点击热点。", new Vector2(0f, 210f),
                new Vector2(320f, 100f));
            CreateGuideBubble(hotspotSeal.transform, "Guide_印章热点", "印章检查点", new Vector2(0f, 86f), new Vector2(180f, 58f));
            CreateGuideBubble(hotspotInk.transform, "Guide_墨迹热点", "墨迹检查点", new Vector2(0f, 86f), new Vector2(180f, 58f));
            CreateGuideBubble(hotspotDate.transform, "Guide_日期热点", "日期检查点", new Vector2(0f, 86f), new Vector2(180f, 58f));
            CreateGuideBubble(hotspotContent.transform, "Guide_条款热点", "条款检查点", new Vector2(0f, 86f), new Vector2(180f, 58f));
            CreateGuideBubble(passButton.transform, "Guide_通过按钮", "通过并提交：本轮判定为通过。", new Vector2(0f, 88f), new Vector2(300f, 68f));
            CreateGuideBubble(rejectButton.transform, "Guide_退回按钮", "退回并选理由：进入理由选择。", new Vector2(0f, 88f), new Vector2(320f, 68f));
            CreateGuideBubble(zoomButton.transform, "Guide_缩放按钮", "文档缩放：1x 与 1.5x 切换。", new Vector2(0f, 88f), new Vector2(320f, 68f));
            CreateGuideBubble(guideToggleButton.transform, "Guide_说明开关", "一键开关全部文本标注。", new Vector2(-150f, -84f), new Vector2(250f, 62f));
            CreateGuideBubble(statusText.transform, "Guide_状态文本", "状态提示：显示检查进度与提交反馈。", new Vector2(0f, 74f), new Vector2(360f, 64f));
            CreateGuideBubble(rejectReasons.transform, "Guide_理由面板", "退回理由面板：仅在选择退回时出现。", new Vector2(-210f, 248f), new Vector2(360f, 72f));
            CreateGuideBubble(reasonImage.transform, "Guide_理由1", "理由1：图像不符", new Vector2(0f, 74f), new Vector2(220f, 56f));
            CreateGuideBubble(reasonDate.transform, "Guide_理由2", "理由2：日期不符", new Vector2(0f, 74f), new Vector2(220f, 56f));
            CreateGuideBubble(reasonApplication.transform, "Guide_理由3", "理由3：申请不符", new Vector2(0f, 74f), new Vector2(220f, 56f));
            CreateGuideBubble(reasonForgery.transform, "Guide_理由4", "理由4：纸张造假", new Vector2(0f, 74f), new Vector2(220f, 56f));
            CreateGuideBubble(reasonDamage.transform, "Guide_理由5", "理由5：纸张破损", new Vector2(0f, 74f), new Vector2(220f, 56f));
            CreateGuideBubble(cancelReasonButton.transform, "Guide_返回按钮", "返回：关闭理由面板并回到判定。", new Vector2(0f, 74f), new Vector2(270f, 56f));

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
            Debug.Log($"[DocumentReviewPanelPrefabBuilder] 已重建: {PrefabPath}");
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
            return text;
        }

        private static GameObject CreateHotspot(Transform parent, string name, Vector2 anchoredPosition, string centerLabel)
        {
            var hotspot = CreateButton(name, parent, string.Empty, 20, new Color(0.98f, 0.56f, 0.18f, 0.42f), out _);
            SetRect(
                hotspot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                anchoredPosition,
                new Vector2(196f, 102f));

            var label = CreateText("CenterLabel", hotspot.transform, centerLabel, 22, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchRect(label.rectTransform);
            label.color = new Color(0.24f, 0.16f, 0.06f, 1f);
            label.raycastTarget = false;
            return hotspot;
        }

        private static GameObject CreateReasonButton(Transform parent, string name, string label, int index)
        {
            var reason = CreateButton(name, parent, label, 27, new Color(0.36f, 0.16f, 0.14f, 1f), out _);
            SetRect(
                reason.GetComponent<RectTransform>(),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -148f - index * 90f),
                new Vector2(330f, 72f));
            return reason;
        }

        private static void CreateGuideBubble(
            Transform target,
            string name,
            string content,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(DocumentReviewGuideTag));
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
