#if UNITY_EDITOR
using ITC.SignMiniGame;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.EditorTools.SignMiniGame
{
    public static class RuneTypingPanelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string PrefabPath = PrefabFolder + "/RuneTypingPanel.prefab";
        private const string FontAssetPath = "Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset";

        // ── Layout constants ─────────────────────────────────────
        // 参考分辨率 1920×1080；GameRoot 留出边距后 1400×880
        private const float PanelW = 1400f;
        private const float PanelH = 880f;

        // 顶部信息栏高度
        private const float HeaderH = 60f;
        private const float HintH = 44f;
        private const float TargetH = 88f;

        // 格子区 (居中，正方形)
        private const float GridSize = 560f;

        // 右侧方向键区
        private const float DpadW = 320f;
        private const float DpadH = 300f;
        private const float BtnSize = 84f;

        // 底部信息栏
        private const float FooterH = 80f;

        [MenuItem("ITC/SignMiniGame/重建 02符文控制板 白模Prefab")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder(PrefabFolder);

            // ── Root ─────────────────────────────────────────────
            var root = new GameObject("RuneTypingPanel", typeof(RectTransform), typeof(RuneTypingPanel));
            StretchRect(root.GetComponent<RectTransform>());

            // ── Canvas ───────────────────────────────────────────
            var canvas = CreateCanvas(root.transform);

            // 全屏半透明遮罩
            var dim = CreateImage("Dim", canvas.transform, new Color(0f, 0f, 0f, 0.72f));
            StretchRect(dim.GetComponent<RectTransform>());

            // ── GameRoot（主面板容器）─────────────────────────────
            var gameRoot = CreateImage("GameRoot", canvas.transform, new Color(0.10f, 0.11f, 0.14f, 0.97f));
            SetRect(
                gameRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(PanelW, PanelH));

            float curY = PanelH * 0.5f; // 从顶部往下排

            // ── 标题行 ───────────────────────────────────────────
            curY -= 16f; // 顶部内边距
            curY -= HeaderH * 0.5f;
            var headerText = CreateText("HeaderText", gameRoot.transform,
                "符文控制板 QTE", 40, TextAlignmentOptions.Center, FontStyles.Bold);
            SetAnchorCenter(headerText.rectTransform, new Vector2(0f, curY), new Vector2(PanelW - 240f, HeaderH));
            curY -= HeaderH * 0.5f + 4f;

            // 右上角说明开关
            var guideToggleButton = CreateButton("GuideToggleButton", gameRoot.transform,
                "说明：开", 22, new Color(0.20f, 0.28f, 0.40f, 1f), out _);
            SetRect(
                guideToggleButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-20f, -16f),
                new Vector2(152f, 48f));

            // ── 操作提示行 ───────────────────────────────────────
            curY -= HintH * 0.5f;
            var hintText = CreateText("HintText", gameRoot.transform,
                "WASD/方向键移动符文光标，空格或回车确认。", 26, TextAlignmentOptions.Center, FontStyles.Normal);
            hintText.color = new Color(0.78f, 0.82f, 0.90f, 1f);
            SetAnchorCenter(hintText.rectTransform, new Vector2(0f, curY), new Vector2(PanelW - 80f, HintH));
            curY -= HintH * 0.5f + 8f;

            // ── 当前目标高亮行 ───────────────────────────────────
            curY -= TargetH * 0.5f;
            var currentTargetHighlight = CreateText("CurrentTargetHighlight", gameRoot.transform,
                "当前目标：上  (1/4)", 48, TextAlignmentOptions.Center, FontStyles.Bold);
            currentTargetHighlight.color = new Color(1f, 0.88f, 0.20f, 1f);
            SetAnchorCenter(currentTargetHighlight.rectTransform, new Vector2(0f, curY), new Vector2(PanelW - 80f, TargetH));
            curY -= TargetH * 0.5f + 8f;

            // ── 目标序列条 ───────────────────────────────────────
            const float seqH = 76f;
            curY -= seqH * 0.5f;
            var targetSequenceRoot = new GameObject("TargetSequenceRoot", typeof(RectTransform), typeof(Image));
            targetSequenceRoot.transform.SetParent(gameRoot.transform, false);
            targetSequenceRoot.GetComponent<Image>().color = new Color(0.06f, 0.07f, 0.09f, 0.95f);
            targetSequenceRoot.GetComponent<Image>().raycastTarget = false;
            SetAnchorCenter(
                targetSequenceRoot.GetComponent<RectTransform>(),
                new Vector2(0f, curY),
                new Vector2(PanelW - 80f, seqH));

            // 目标序列改用 HorizontalLayoutGroup，避免文字超出框外
            var seqHlg = targetSequenceRoot.AddComponent<HorizontalLayoutGroup>();
            seqHlg.childAlignment = TextAnchor.MiddleCenter;
            seqHlg.spacing = 8f;
            seqHlg.childControlWidth = true;
            seqHlg.childControlHeight = true;
            seqHlg.childForceExpandWidth = true;
            seqHlg.childForceExpandHeight = true;
            seqHlg.padding = new RectOffset(16, 16, 8, 8);

            for (var i = 0; i < 8; i++)
            {
                var t = CreateText($"Target_{i:00}", targetSequenceRoot.transform,
                    "上", 34, TextAlignmentOptions.Center, FontStyles.Bold);
                // LayoutElement 让 HorizontalLayoutGroup 均分
                var le = t.gameObject.AddComponent<LayoutElement>();
                le.minWidth = 80f;
                le.flexibleWidth = 1f;
            }

            curY -= seqH * 0.5f + 16f;

            // ── 主体区（格子 + 右侧方向键）──────────────────────
            // 格子区中心 X 偏左，给右侧方向键腾空

            // 剩余高度用于格子区
            float remainH = PanelH * 0.5f - (PanelH * 0.5f - curY); // 从当前 curY 到底部(含footer)
            // 格子区取正方形
            float gridActualSize = Mathf.Min(GridSize, remainH - FooterH - 32f);
            gridActualSize = Mathf.Max(gridActualSize, 320f);

            float gridCenterY = curY - gridActualSize * 0.5f;
            float gridCenterX = -(DpadW * 0.5f + 12f); // 偏左
            float dpadCenterX = gridActualSize * 0.5f + 24f;

            // GridRoot
            var gridRoot = new GameObject("GridRoot", typeof(RectTransform), typeof(Image));
            gridRoot.transform.SetParent(gameRoot.transform, false);
            gridRoot.GetComponent<Image>().color = new Color(0.07f, 0.08f, 0.10f, 0.98f);
            gridRoot.GetComponent<Image>().raycastTarget = false;
            SetAnchorCenter(
                gridRoot.GetComponent<RectTransform>(),
                new Vector2(gridCenterX, gridCenterY),
                new Vector2(gridActualSize, gridActualSize));

            // 25 个格子（5x5 最大），位置由 GridLayoutGroup 在运行时设置
            const int maxCells = 25;
            for (var idx = 0; idx < maxCells; idx++)
            {
                var glyph = (idx % 4) switch { 0 => "上", 1 => "右", 2 => "下", _ => "左" };
                var cellGo = CreateButton($"Cell_{idx:00}", gridRoot.transform,
                    glyph, 38, new Color(0.18f, 0.20f, 0.24f, 1f), out _);
                // GridLayoutGroup 在运行时覆盖位置；prefab 里给个默认大小即可
                SetRect(
                    cellGo.GetComponent<RectTransform>(),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero,
                    new Vector2(96f, 96f));
            }
            // 注意：不再有 CursorFrame！光标由格子颜色变化表示。

            // ── 右侧屏幕方向键（D-Pad + 确认）──────────────────
            var onScreenButtons = new GameObject("OnScreenButtons", typeof(RectTransform));
            onScreenButtons.transform.SetParent(gameRoot.transform, false);
            SetAnchorCenter(
                onScreenButtons.GetComponent<RectTransform>(),
                new Vector2(dpadCenterX, gridCenterY),
                new Vector2(DpadW, DpadH));

            var upButton = CreateButton("UpButton", onScreenButtons.transform, "上", 28, new Color(0.18f, 0.28f, 0.42f, 1f), out _);
            var downButton = CreateButton("DownButton", onScreenButtons.transform, "下", 28, new Color(0.18f, 0.28f, 0.42f, 1f), out _);
            var leftButton = CreateButton("LeftButton", onScreenButtons.transform, "左", 28, new Color(0.18f, 0.28f, 0.42f, 1f), out _);
            var rightButton = CreateButton("RightButton", onScreenButtons.transform, "右", 28, new Color(0.18f, 0.28f, 0.42f, 1f), out _);
            var confirmButton = CreateButton("ConfirmButton", onScreenButtons.transform, "确认", 26, new Color(0.24f, 0.48f, 0.22f, 1f), out _);

            // 十字键：上中、下中、左中、右中
            float dpadCx = 0f, dpadCy = DpadH * 0.5f - BtnSize * 0.5f - 8f;
            SetRect(upButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(dpadCx, dpadCy - 0f), new Vector2(BtnSize, BtnSize));
            SetRect(downButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(dpadCx, dpadCy - BtnSize - 12f), new Vector2(BtnSize, BtnSize));
            SetRect(leftButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(dpadCx - BtnSize - 12f, dpadCy - (BtnSize + 12f) * 0.5f), new Vector2(BtnSize, BtnSize));
            SetRect(rightButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(dpadCx + BtnSize + 12f, dpadCy - (BtnSize + 12f) * 0.5f), new Vector2(BtnSize, BtnSize));

            // 确认键在十字键下方
            SetRect(confirmButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(dpadCx, dpadCy - BtnSize - 12f - 24f - 52f), new Vector2(BtnSize * 2f + 12f, 52f));

            // WebGL 提示标签（在确认键下方）
            var webglHint = CreateText("WebGLHint", onScreenButtons.transform,
                "WebGL 焦点丢失时可用屏幕按钮", 18, TextAlignmentOptions.Center, FontStyles.Normal);
            webglHint.color = new Color(0.6f, 0.65f, 0.72f, 1f);
            SetRect(webglHint.rectTransform,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 4f), new Vector2(DpadW, 28f));

            // ── 底部状态栏 ───────────────────────────────────────
            float footerY = -PanelH * 0.5f + FooterH * 0.5f + 8f;

            var statusText = CreateText("StatusText", gameRoot.transform,
                "序列进度：0/0", 26, TextAlignmentOptions.Left, FontStyles.Normal);
            statusText.color = new Color(0.80f, 0.84f, 0.92f, 1f);
            SetRect(statusText.rectTransform,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0.5f),
                new Vector2(40f, footerY),
                new Vector2(500f, 40f));

            var errorCountText = CreateText("ErrorCountText", gameRoot.transform,
                "失误次数：0", 26, TextAlignmentOptions.Left, FontStyles.Bold);
            errorCountText.color = new Color(0.95f, 0.62f, 0.40f, 1f);
            SetRect(errorCountText.rectTransform,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0.5f),
                new Vector2(560f, footerY),
                new Vector2(400f, 40f));

            // ── 倒计时文本（覆盖在格子上方）────────────────────
            var countdownText = CreateText("CountdownText", gameRoot.transform,
                "3", 110, TextAlignmentOptions.Center, FontStyles.Bold);
            countdownText.color = new Color(1f, 0.92f, 0.3f, 1f);
            SetAnchorCenter(countdownText.rectTransform,
                new Vector2(gridCenterX, gridCenterY),
                new Vector2(gridActualSize, gridActualSize));

            // ── 说明面板（紧凑，可关闭）─────────────────────────
            // 用一个统一气泡放在格子左下方，而不是分散在各处
            CreateGuideBubble(gameRoot.transform, "Guide_总说明",
                "1. 查看顶部\"当前目标\"符文\n2. WASD/方向键移动光标（黄色高亮格子）\n3. Space/Enter 或点\"确认\"提交\n4. 右上角可切换说明显示",
                new Vector2(gridCenterX, footerY + 48f + 8f),
                new Vector2(gridActualSize, 84f));

            // ── 保存 Prefab ──────────────────────────────────────
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
            Debug.Log($"[RuneTypingPanelPrefabBuilder] 已重建（无CursorFrame版）: {PrefabPath}");
        }

        // ── Helpers ──────────────────────────────────────────────

        /// <summary>以 GameRoot 中心为基准，anchoredPosition 是相对中心偏移</summary>
        private static void SetAnchorCenter(RectTransform rt, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            SetRect(rt,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                anchoredPosition, sizeDelta);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            var parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            var folder = System.IO.Path.GetFileName(folderPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(folder)) return;
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }

        private static GameObject CreateCanvas(Transform parent)
        {
            var canvas = new GameObject("Canvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.transform.SetParent(parent, false);
            StretchRect(canvas.GetComponent<RectTransform>());

            var c = canvas.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.pixelPerfect = false;

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
            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
            return go;
        }

        private static GameObject CreateButton(
            string name, Transform parent,
            string label, float labelSize, Color color,
            out Button button)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.color = color;

            button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
            colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            button.colors = colors;

            var text = CreateText("Label", go.transform, label, labelSize, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchRect(text.rectTransform);
            text.raycastTarget = false;

            return go;
        }

        private static TMP_Text CreateText(
            string name, Transform parent,
            string content, float fontSize,
            TextAlignmentOptions alignment, FontStyles styles)
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
            if (fontAsset != null) text.font = fontAsset;

            return text;
        }

        private static void CreateGuideBubble(
            Transform target, string name,
            string content, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var bubble = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(RuneTypingGuideTag));
            bubble.transform.SetParent(target, false);
            var bubbleRect = bubble.GetComponent<RectTransform>();
            SetAnchorCenter(bubbleRect, anchoredPosition, sizeDelta);

            var image = bubble.GetComponent<Image>();
            image.color = new Color(0.06f, 0.06f, 0.08f, 0.90f);
            image.raycastTarget = false;

            var text = CreateText("Text", bubble.transform,
                content, 20, TextAlignmentOptions.Left, FontStyles.Normal);
            StretchRect(text.rectTransform);
            text.margin = new Vector4(14f, 8f, 14f, 8f);
            text.color = new Color(1f, 0.94f, 0.72f, 1f);
            text.raycastTarget = false;
        }

        private static void StretchRect(RectTransform rt)
        {
            SetRect(rt, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        }

        private static void SetRect(
            RectTransform rt,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }
    }
}
#endif
