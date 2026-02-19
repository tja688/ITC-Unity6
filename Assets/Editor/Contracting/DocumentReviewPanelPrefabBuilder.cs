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

        [MenuItem("ITC/Contracting/Rebuild Document Review Panel Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var root = new GameObject("DocumentReviewPanel", typeof(RectTransform), typeof(DocumentReviewPanel));
            var rootRect = root.GetComponent<RectTransform>();
            StretchRect(rootRect);

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.72f));
            StretchRect(dim.GetComponent<RectTransform>());

            var reviewRoot = CreateImage("ReviewRoot", canvas.transform, new Color(0.16f, 0.13f, 0.11f, 0.96f));
            SetRect(reviewRoot.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1500f, 860f));

            var headerText = CreateText("HeaderText", reviewRoot.transform, "Document Review", 46, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(headerText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -56f),
                new Vector2(1100f, 70f));

            var hintText = CreateText("HintText", reviewRoot.transform, "Inspect the document before making a decision.",
                28, TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(hintText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -112f),
                new Vector2(1180f, 60f));

            var documentArea = CreateButton("DocumentArea", reviewRoot.transform, string.Empty, 28,
                new Color(0.89f, 0.83f, 0.70f, 1f), out _);
            SetRect(documentArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1120f, 500f));

            var stampLabel = CreateText("DocFakeLabel", documentArea.transform, "Contract Document", 36,
                TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(stampLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -36f), new Vector2(540f, 56f));

            CreateHotspot(documentArea.transform, "Hotspot_Seal", new Vector2(-356f, -128f));
            CreateHotspot(documentArea.transform, "Hotspot_Ink", new Vector2(-66f, 42f));
            CreateHotspot(documentArea.transform, "Hotspot_Date", new Vector2(274f, 88f));
            CreateHotspot(documentArea.transform, "Hotspot_Content", new Vector2(314f, -142f));

            var controlsRoot = new GameObject("Controls", typeof(RectTransform));
            controlsRoot.transform.SetParent(reviewRoot.transform, false);
            var controlsRect = controlsRoot.GetComponent<RectTransform>();
            SetRect(controlsRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 84f), new Vector2(1240f, 86f));

            var passButton = CreateButton("PassButton", controlsRoot.transform, "PASS", 34,
                new Color(0.20f, 0.42f, 0.26f, 1f), out _);
            SetRect(passButton.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f), new Vector2(0f, 0f), new Vector2(350f, 86f));

            var rejectButton = CreateButton("RejectButton", controlsRoot.transform, "REJECT", 34,
                new Color(0.52f, 0.21f, 0.20f, 1f), out _);
            SetRect(rejectButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(350f, 86f));

            var zoomButton = CreateButton("ZoomButton", controlsRoot.transform, "Zoom x1.5", 30,
                new Color(0.19f, 0.28f, 0.40f, 1f), out _);
            SetRect(zoomButton.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f), new Vector2(0f, 0f), new Vector2(350f, 86f));

            var statusText = CreateText("StatusText", reviewRoot.transform,
                "Inspect hotspots and decide whether to pass or reject.", 28, TextAlignmentOptions.Center, FontStyles.Normal);
            SetRect(statusText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f), new Vector2(0f, 178f), new Vector2(1260f, 80f));

            var rejectReasons = CreateImage("RejectReasons", reviewRoot.transform, new Color(0.10f, 0.10f, 0.10f, 0.92f));
            SetRect(rejectReasons.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f), new Vector2(-40f, 20f), new Vector2(420f, 640f));

            var rejectTitle = CreateText("ReasonTitle", rejectReasons.transform, "Select Reject Reason",
                32, TextAlignmentOptions.Center, FontStyles.Bold);
            SetRect(rejectTitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f), new Vector2(0f, -58f), new Vector2(360f, 64f));

            CreateReasonButton(rejectReasons.transform, "Reason_ImageMismatch", "Image mismatch", 0);
            CreateReasonButton(rejectReasons.transform, "Reason_DateMismatch", "Date mismatch", 1);
            CreateReasonButton(rejectReasons.transform, "Reason_ApplicationMismatch", "Application mismatch", 2);
            CreateReasonButton(rejectReasons.transform, "Reason_PaperForgery", "Paper forgery", 3);
            CreateReasonButton(rejectReasons.transform, "Reason_PaperDamage", "Paper damage", 4);

            var cancelButton = CreateButton("CancelButton", rejectReasons.transform, "Back", 28,
                new Color(0.25f, 0.25f, 0.25f, 1f), out _);
            SetRect(cancelButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(260f, 68f));

            rejectReasons.SetActive(false);

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
            Debug.Log($"[DocumentReviewPanelPrefabBuilder] Prefab rebuilt at {PrefabPath}");
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
            var rect = canvas.GetComponent<RectTransform>();
            StretchRect(rect);

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
            return text;
        }

        private static void CreateHotspot(Transform parent, string name, Vector2 anchoredPosition)
        {
            var hotspot = CreateButton(name, parent, string.Empty, 20, new Color(0.98f, 0.56f, 0.18f, 0.45f), out _);
            var rect = hotspot.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                anchoredPosition, new Vector2(188f, 98f));
        }

        private static void CreateReasonButton(Transform parent, string name, string label, int index)
        {
            var reason = CreateButton(name, parent, label, 27, new Color(0.36f, 0.16f, 0.14f, 1f), out _);
            var rect = reason.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -146f - index * 90f), new Vector2(326f, 72f));
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
