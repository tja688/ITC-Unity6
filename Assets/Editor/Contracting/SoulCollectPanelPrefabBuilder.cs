#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class SoulCollectPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/SoulCollectPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/Contracting/重建 05灵魂收取 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            var root = new GameObject("SoulCollectPanel", typeof(RectTransform), typeof(SoulCollectPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.72f));
            StretchRect(dim.GetComponent<RectTransform>());

            var soulRoot = CreateImage("SoulRoot", canvas.transform, new Color(0.08f, 0.10f, 0.14f, 0.97f));
            SetRect(
                soulRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1540f, 920f));

            var headerText = CreateText("HeaderText", soulRoot.transform, "灵魂收取分割", 48, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                headerText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -54f),
                new Vector2(1080f, 70f));

            var hintText = CreateText("HintText", soulRoot.transform, "拖动分割线预览比例，确认后立即结算。", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -112f),
                new Vector2(1180f, 56f));

            var guideToggleButton = CreateButton(
                "GuideToggleButton",
                soulRoot.transform,
                "说明：开",
                24,
                new Color(0.17f, 0.25f, 0.36f, 1f),
                fontAsset,
                out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-42f, -42f),
                new Vector2(188f, 58f));

            var soulArea = CreateImage("SoulArea", soulRoot.transform, new Color(0.13f, 0.16f, 0.21f, 1f));
            SetRect(
                soulArea.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.52f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(640f, 640f));
            soulArea.GetComponent<Image>().raycastTarget = false;

            var soulCore = CreateImage("SoulCore", soulArea.transform, new Color(0.47f, 0.83f, 1f, 0.92f));
            SetRect(
                soulCore.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(520f, 520f));
            soulCore.GetComponent<Image>().raycastTarget = false;

            var targetBand = CreateImage("TargetBand", soulArea.transform, new Color(0.22f, 0.69f, 0.36f, 0.24f));
            StretchVerticalByNormalizedRange(targetBand.GetComponent<RectTransform>(), 0.4f, 0.5f);
            targetBand.GetComponent<Image>().raycastTarget = false;

            var underdrawHint = CreateImage("UnderdrawHint", soulArea.transform, new Color(0.5f, 0.5f, 0.5f, 0f));
            StretchVerticalByNormalizedRange(underdrawHint.GetComponent<RectTransform>(), 0f, 0.5f);
            underdrawHint.GetComponent<Image>().raycastTarget = false;

            var overdrawHint = CreateImage("OverdrawHint", soulArea.transform, new Color(0.87f, 0.2f, 0.2f, 0f));
            StretchVerticalByNormalizedRange(overdrawHint.GetComponent<RectTransform>(), 0.5f, 1f);
            overdrawHint.GetComponent<Image>().raycastTarget = false;

            var splitLine = CreateImage("SplitLine", soulArea.transform, new Color(0.96f, 0.98f, 1f, 0.95f));
            SetRect(
                splitLine.GetComponent<RectTransform>(),
                new Vector2(0.45f, 0.5f),
                new Vector2(0.45f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(10f, 580f));
            splitLine.GetComponent<Image>().raycastTarget = false;

            var targetHint = CreateText("TargetHintText", soulArea.transform, "目标 45%", 26, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                targetHint.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -26f),
                new Vector2(320f, 44f));

            var companyLabel = CreateText("CompanyLabel", soulArea.transform, "Company", 26, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                companyLabel.rectTransform,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(24f, 18f),
                new Vector2(190f, 40f));

            var clientLabel = CreateText("ClientLabel", soulArea.transform, "Client", 26, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                clientLabel.rectTransform,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-24f, 18f),
                new Vector2(190f, 40f));

            var bottomRoot = new GameObject("Bottom", typeof(RectTransform));
            bottomRoot.transform.SetParent(soulRoot.transform, false);
            SetRect(
                bottomRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 94f),
                new Vector2(1240f, 240f));

            var percentText = CreateText("PercentText", bottomRoot.transform, "45%", 64, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                percentText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -36f),
                new Vector2(420f, 70f));

            var rangeText = CreateText("RangeText", bottomRoot.transform, "有效区间 40% - 50%", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                rangeText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -92f),
                new Vector2(580f, 48f));

            var statusText = CreateText("StatusText", bottomRoot.transform, "灵魂显现中...", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 24f),
                new Vector2(980f, 54f));

            var controlsRoot = new GameObject("Controls", typeof(RectTransform));
            controlsRoot.transform.SetParent(bottomRoot.transform, false);
            SetRect(
                controlsRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 92f),
                new Vector2(920f, 88f));

            var confirmButton = CreateButton("ConfirmButton", controlsRoot.transform, "确认分割", 30, new Color(0.46f, 0.15f, 0.16f, 1f), fontAsset, out _);
            SetRect(
                confirmButton.GetComponent<RectTransform>(),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                Vector2.zero,
                new Vector2(420f, 86f));

            var resetButton = CreateButton("ResetButton", controlsRoot.transform, "重置位置", 30, new Color(0.18f, 0.24f, 0.31f, 1f), fontAsset, out _);
            SetRect(
                resetButton.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                Vector2.zero,
                new Vector2(420f, 86f));

            CreateGuideBubble(
                soulRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 拖动分割线控制收取比例。\n2. 比例可反复微调，确认后锁定输入。\n3. 高于上限降满意，低于下限记失误。",
                new Vector2(-500f, 290f),
                new Vector2(510f, 210f),
                fontAsset);
            CreateGuideBubble(soulArea.transform, "Guide_灵魂球", "灵魂球：中央交互区域。", new Vector2(0f, 286f), new Vector2(310f, 70f), fontAsset);
            CreateGuideBubble(splitLine.transform, "Guide_分割线", "分割线：当前位置即收取比例。", new Vector2(0f, 0f), new Vector2(280f, 60f), fontAsset);
            CreateGuideBubble(percentText.transform, "Guide_实时比例", "实时百分比（20Hz刷新）。", new Vector2(0f, 56f), new Vector2(300f, 60f), fontAsset);
            CreateGuideBubble(rangeText.transform, "Guide_判定区间", "区间内通过；区间外触发惩罚。", new Vector2(0f, 50f), new Vector2(330f, 60f), fontAsset);
            CreateGuideBubble(confirmButton.transform, "Guide_确认按钮", "确认后锁输入并播放分割结算。", new Vector2(0f, 86f), new Vector2(340f, 60f), fontAsset);
            CreateGuideBubble(resetButton.transform, "Guide_重置按钮", "回到目标附近，便于重新微调。", new Vector2(0f, 86f), new Vector2(340f, 60f), fontAsset);

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
            Debug.Log($"[SoulCollectPanelPrefabBuilder] 已重建: {PrefabPath}");
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
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(SoulCollectGuideTag));
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
