#if UNITY_EDITOR
using ITC.Contracting;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.Contracting
{
    public static class RuneVerifyPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/RuneVerifyPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        [MenuItem("ITC/Contracting/重建 03符文核验 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            var root = new GameObject("RuneVerifyPanel", typeof(RectTransform), typeof(RuneVerifyPanel));
            StretchRect(root.GetComponent<RectTransform>());

            var canvas = CreateCanvas(root.transform);
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.74f));
            StretchRect(dim.GetComponent<RectTransform>());

            var verifyRoot = CreateImage("VerifyRoot", canvas.transform, new Color(0.10f, 0.11f, 0.14f, 0.97f));
            SetRect(
                verifyRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1540f, 940f));

            var guideToggleButton = CreateButton(
                "GuideToggleButton",
                verifyRoot.transform,
                "说明：开",
                24,
                new Color(0.16f, 0.25f, 0.36f, 1f),
                fontAsset,
                out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-42f, -42f),
                new Vector2(188f, 58f));

            var top = new GameObject("Top", typeof(RectTransform));
            top.transform.SetParent(verifyRoot.transform, false);
            SetRect(
                top.GetComponent<RectTransform>(),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -96f),
                new Vector2(1320f, 178f));

            var titleText = CreateText("TitleText", top.transform, "符文核验", 46, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                titleText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -34f),
                new Vector2(900f, 66f));

            var hintText = CreateText(
                "HintText",
                top.transform,
                "在 5 秒内找出 3 个扭曲符文，误点仅触发视觉反馈。",
                28,
                TextAlignmentOptions.Center,
                FontStyles.Normal,
                fontAsset);
            SetRect(
                hintText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -88f),
                new Vector2(1160f, 56f));

            var countdownText = CreateText("CountdownText", top.transform, "5.0s", 40, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            countdownText.color = new Color(1f, 0.92f, 0.68f, 1f);
            SetRect(
                countdownText.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 14f),
                new Vector2(280f, 54f));

            var timerBar = CreateImage("TimerBar", top.transform, new Color(0.19f, 0.23f, 0.30f, 1f));
            SetRect(
                timerBar.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, -26f),
                new Vector2(980f, 26f));

            var timerFill = CreateImage("Fill", timerBar.transform, new Color(0.90f, 0.35f, 0.30f, 1f));
            StretchRect(timerFill.GetComponent<RectTransform>());
            var fillImage = timerFill.GetComponent<Image>();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;

            var gridRoot = new GameObject("GridRoot", typeof(RectTransform), typeof(GridLayoutGroup));
            gridRoot.transform.SetParent(verifyRoot.transform, false);
            SetRect(
                gridRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -8f),
                new Vector2(860f, 690f));

            var gridLayout = gridRoot.GetComponent<GridLayoutGroup>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 4;
            gridLayout.cellSize = new Vector2(132f, 132f);
            gridLayout.spacing = new Vector2(12f, 12f);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;

            for (var i = 0; i < 20; i++)
            {
                CreateRuneCell(gridRoot.transform, i, fontAsset);
            }

            var bottom = new GameObject("Bottom", typeof(RectTransform));
            bottom.transform.SetParent(verifyRoot.transform, false);
            SetRect(
                bottom.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 84f),
                new Vector2(1220f, 132f));

            var foundText = CreateText("FoundText", bottom.transform, "Found 0/3", 34, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            SetRect(
                foundText.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -28f),
                new Vector2(420f, 52f));

            var statusText = CreateText("StatusText", bottom.transform, "找出全部扭曲符文。", 28, TextAlignmentOptions.Center, FontStyles.Normal, fontAsset);
            SetRect(
                statusText.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 20f),
                new Vector2(1080f, 58f));

            CreateGuideBubble(
                verifyRoot.transform,
                "Guide_操作说明",
                "操作说明\n1. 5 秒内找齐 3 个扭曲符文。\n2. 误点不会禁用输入，只给视觉提示。\n3. 右上角可统一开关说明。",
                new Vector2(-508f, 308f),
                new Vector2(500f, 210f),
                fontAsset);
            CreateGuideBubble(gridRoot.transform, "Guide_符文网格", "符文网格\n共 20 格，目标扭曲符文为 3 个。", new Vector2(0f, 268f), new Vector2(340f, 96f), fontAsset);
            CreateGuideBubble(timerBar.transform, "Guide_倒计时条", "倒计时条\n归零即失败。最后 1 秒每 0.25 秒脉冲提示。", new Vector2(0f, 54f), new Vector2(420f, 80f), fontAsset);
            CreateGuideBubble(foundText.transform, "Guide_进度文本", "进度文本\n实时显示 Found x/3。", new Vector2(0f, 54f), new Vector2(260f, 64f), fontAsset);
            CreateGuideBubble(statusText.transform, "Guide_状态文本", "状态文本\n显示命中、误点和结算信息。", new Vector2(0f, 54f), new Vector2(320f, 64f), fontAsset);

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
            Debug.Log($"[RuneVerifyPanelPrefabBuilder] 已重建: {PrefabPath}");
        }

        private static GameObject CreateRuneCell(Transform parent, int index, TMP_FontAsset fontAsset)
        {
            var cell = new GameObject($"RuneCell_{index:00}", typeof(RectTransform), typeof(Image), typeof(Button));
            cell.transform.SetParent(parent, false);
            SetRect(
                cell.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(132f, 132f));

            var cellImage = cell.GetComponent<Image>();
            cellImage.color = new Color(0.17f, 0.23f, 0.32f, 1f);

            var glyph = new GameObject("Glyph", typeof(RectTransform), typeof(Image));
            glyph.transform.SetParent(cell.transform, false);
            SetRect(
                glyph.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(120f, 120f));
            var glyphImage = glyph.GetComponent<Image>();
            glyphImage.color = new Color(0.26f, 0.35f, 0.48f, 1f);
            glyphImage.raycastTarget = false;

            var label = CreateText("Label", cell.transform, "A", 44, TextAlignmentOptions.Center, FontStyles.Bold, fontAsset);
            StretchRect(label.rectTransform);
            label.color = new Color(0.78f, 0.90f, 1f, 1f);
            label.raycastTarget = false;

            return cell;
        }

        private static void CreateGuideBubble(
            Transform target,
            string name,
            string content,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            TMP_FontAsset fontAsset)
        {
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(RuneVerifyGuideTag));
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

