using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class StepperProgressSlideContentView
{
    public RectTransform Root;
    public Text Heading;
    public Text Body;
}

public sealed class StepperProgressSlideEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Card;

    public Text HeaderTitle;

    public List<Button> StepButtons = new List<Button>();
    public List<Image> StepNodeImages = new List<Image>();
    public List<Text> StepNodeTexts = new List<Text>();
    public List<Text> StepTitleTexts = new List<Text>();
    public List<Image> ConnectorFills = new List<Image>();

    public RectTransform Viewport;
    public List<StepperProgressSlideContentView> StepContents = new List<StepperProgressSlideContentView>();

    public RectTransform CompletionRect;
    public Image CompletionImage;

    public Button BackButton;
    public Button NextButton;
    public Text NextLabel;
}

public static class StepperProgressSlideEffectViewBuilder
{
    public static StepperProgressSlideEffectView Build(RectTransform mountRoot, StepperProgressSlideEffectConfig config)
    {
        var view = new StepperProgressSlideEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("StepperProgressSlideEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = config.RootBackground;
        rootImage.raycastTarget = true;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var ambientA = ReplicaUIFactoryV2.CreatePanel("AmbientA", view.Root, config.AmbientA);
        ambientA.anchorMin = new Vector2(0.5f, 0.5f);
        ambientA.anchorMax = new Vector2(0.5f, 0.5f);
        ambientA.pivot = new Vector2(0.5f, 0.5f);
        ambientA.sizeDelta = new Vector2(1300f, 520f);
        ambientA.anchoredPosition = new Vector2(-120f, 120f);
        ambientA.localRotation = Quaternion.Euler(0f, 0f, 14f);
        ambientA.GetComponent<Image>().raycastTarget = false;

        view.Card = ReplicaUIFactoryV2.CreatePanel("StepperCard", view.Root, config.CardColor);
        view.Card.anchorMin = new Vector2(0.5f, 0.5f);
        view.Card.anchorMax = new Vector2(0.5f, 0.5f);
        view.Card.pivot = new Vector2(0.5f, 0.5f);
        view.Card.sizeDelta = config.CardSize;
        view.Card.anchoredPosition = new Vector2(0f, -20f);

        var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Card.gameObject);
        shadow.effectDistance = new Vector2(0f, -10f);

        view.HeaderTitle = ReplicaUIFactoryV2.CreateText(
            "Title",
            view.Card,
            "Stepper / Multi-stage Form",
            32,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            config.TitleColor);
        var titleRect = (RectTransform)view.HeaderTitle.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 56f);
        titleRect.anchoredPosition = new Vector2(42f, -28f);

        var row = ReplicaUIFactoryV2.CreateRect("StepRow", view.Card);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(42f, -142f);
        row.offsetMax = new Vector2(-42f, -86f);

        view.StepButtons.Clear();
        view.StepNodeImages.Clear();
        view.StepNodeTexts.Clear();
        view.StepTitleTexts.Clear();
        view.ConnectorFills.Clear();

        var stepCount = Mathf.Max(1, config.StepCount);
        for (var i = 0; i < stepCount; i++)
        {
            var step = ReplicaUIFactoryV2.CreateRect($"Step_{i}", row);
            step.anchorMin = new Vector2((float)i / stepCount, 0f);
            step.anchorMax = new Vector2((float)(i + 1) / stepCount, 1f);
            step.offsetMin = Vector2.zero;
            step.offsetMax = Vector2.zero;

            var nodeButton = ReplicaUIFactoryV2.CreateButton("Node", step, new Color(1f, 1f, 1f, 0f));
            var nodeButtonRect = (RectTransform)nodeButton.transform;
            nodeButtonRect.anchorMin = new Vector2(0f, 0.5f);
            nodeButtonRect.anchorMax = new Vector2(0f, 0.5f);
            nodeButtonRect.pivot = new Vector2(0f, 0.5f);
            nodeButtonRect.sizeDelta = new Vector2(38f, 38f);
            nodeButtonRect.anchoredPosition = new Vector2(0f, 0f);

            var node = ReplicaUIFactoryV2.CreatePanel("NodeVisual", nodeButton.transform, config.NodeInactive);
            ReplicaUIFactoryV2.Stretch(node);
            node.pivot = new Vector2(0.5f, 0.5f);

            var nodeLabel = ReplicaUIFactoryV2.CreateText(
                "NodeLabel",
                node,
                (i + 1).ToString(),
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                config.NodeTextInactive);
            ReplicaUIFactoryV2.Stretch((RectTransform)nodeLabel.transform);

            var titleLabel = ReplicaUIFactoryV2.CreateText(
                "Label",
                step,
                $"Step {i + 1}",
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                config.LabelColor);
            var labelRect = (RectTransform)titleLabel.transform;
            labelRect.anchorMin = new Vector2(0f, 0.5f);
            labelRect.anchorMax = new Vector2(1f, 0.5f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(54f, 0f);
            labelRect.sizeDelta = new Vector2(-58f, 34f);

            view.StepButtons.Add(nodeButton);
            view.StepNodeImages.Add(node.GetComponent<Image>());
            view.StepNodeTexts.Add(nodeLabel);
            view.StepTitleTexts.Add(titleLabel);

            if (i < stepCount - 1)
            {
                var connector = ReplicaUIFactoryV2.CreatePanel("Connector", step, config.ConnectorTrack);
                connector.anchorMin = new Vector2(0f, 0.5f);
                connector.anchorMax = new Vector2(1f, 0.5f);
                connector.pivot = new Vector2(0f, 0.5f);
                connector.sizeDelta = new Vector2(-44f, 4f);
                connector.anchoredPosition = new Vector2(40f, 0f);
                connector.GetComponent<Image>().raycastTarget = false;

                var fill = ReplicaUIFactoryV2.CreatePanel("Fill", connector, config.NodeActive).GetComponent<Image>();
                var fillRect = (RectTransform)fill.transform;
                fillRect.anchorMin = new Vector2(0f, 0.5f);
                fillRect.anchorMax = new Vector2(1f, 0.5f);
                fillRect.pivot = new Vector2(0f, 0.5f);
                fillRect.sizeDelta = new Vector2(0f, 4f);
                fillRect.anchoredPosition = Vector2.zero;
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Left;
                fill.fillAmount = 0f;
                fill.raycastTarget = false;
                view.ConnectorFills.Add(fill);
            }
        }

        var viewportFrame = ReplicaUIFactoryV2.CreatePanel("ViewportFrame", view.Card, new Color(0.91f, 0.93f, 0.98f, 1f));
        viewportFrame.anchorMin = new Vector2(0f, 1f);
        viewportFrame.anchorMax = new Vector2(1f, 1f);
        viewportFrame.pivot = new Vector2(0.5f, 1f);
        viewportFrame.offsetMin = config.ViewportFrameOffsetMin;
        viewportFrame.offsetMax = config.ViewportFrameOffsetMax;
        viewportFrame.GetComponent<Image>().raycastTarget = false;

        view.Viewport = ReplicaUIFactoryV2.CreateRect("Viewport", viewportFrame);
        view.Viewport.anchorMin = new Vector2(0f, 1f);
        view.Viewport.anchorMax = new Vector2(1f, 1f);
        view.Viewport.pivot = new Vector2(0.5f, 1f);
        view.Viewport.sizeDelta = new Vector2(0f, 240f);
        view.Viewport.anchoredPosition = Vector2.zero;
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Viewport.gameObject);

        view.StepContents.Clear();
        for (var i = 0; i < stepCount; i++)
        {
            var content = new StepperProgressSlideContentView();
            content.Root = ReplicaUIFactoryV2.CreateRect($"Content_{i}", view.Viewport);
            content.Root.anchorMin = new Vector2(0f, 1f);
            content.Root.anchorMax = new Vector2(1f, 1f);
            content.Root.pivot = new Vector2(0.5f, 1f);
            content.Root.offsetMin = new Vector2(26f, 0f);
            content.Root.offsetMax = new Vector2(-26f, 0f);
            content.Root.anchoredPosition = Vector2.zero;

            content.Heading = ReplicaUIFactoryV2.CreateText(
                "Heading",
                content.Root,
                $"Step {i + 1}",
                30,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                config.TitleColor);
            var headingRect = (RectTransform)content.Heading.transform;
            headingRect.anchorMin = new Vector2(0f, 1f);
            headingRect.anchorMax = new Vector2(1f, 1f);
            headingRect.pivot = new Vector2(0f, 1f);
            headingRect.sizeDelta = new Vector2(0f, 44f);
            headingRect.anchoredPosition = Vector2.zero;

            content.Body = ReplicaUIFactoryV2.CreateText(
                "Body",
                content.Root,
                "-",
                22,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                config.BodyColor);
            var bodyRect = (RectTransform)content.Body.transform;
            bodyRect.anchorMin = new Vector2(0f, 1f);
            bodyRect.anchorMax = new Vector2(1f, 1f);
            bodyRect.pivot = new Vector2(0f, 1f);
            bodyRect.sizeDelta = new Vector2(0f, 120f);
            bodyRect.anchoredPosition = new Vector2(0f, -58f);

            var tagRow = ReplicaUIFactoryV2.CreateRect("Tags", content.Root);
            tagRow.anchorMin = new Vector2(0f, 1f);
            tagRow.anchorMax = new Vector2(1f, 1f);
            tagRow.pivot = new Vector2(0f, 1f);
            tagRow.sizeDelta = new Vector2(0f, 42f);
            tagRow.anchoredPosition = new Vector2(0f, -164f);

            for (var t = 0; t < 2; t++)
            {
                var badge = ReplicaUIFactoryV2.CreatePanel(
                    $"Tag_{t}",
                    tagRow,
                    new Color(0.84f - (t * 0.08f), 0.87f - (t * 0.08f), 0.98f, 1f));
                badge.anchorMin = new Vector2(0f, 0.5f);
                badge.anchorMax = new Vector2(0f, 0.5f);
                badge.pivot = new Vector2(0f, 0.5f);
                badge.sizeDelta = new Vector2(188f, 34f);
                badge.anchoredPosition = new Vector2(t * 198f, 0f);
                badge.GetComponent<Image>().raycastTarget = false;

                var badgeText = ReplicaUIFactoryV2.CreateText(
                    "TagLabel",
                    badge,
                    t == 0 ? "Motion spring" : "State aware",
                    16,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    new Color(0.20f, 0.24f, 0.37f, 1f));
                ReplicaUIFactoryV2.Stretch((RectTransform)badgeText.transform);
            }

            var preferred = 226f + (i * 8f);
            content.Root.sizeDelta = new Vector2(0f, preferred);
            view.StepContents.Add(content);
        }

        view.CompletionRect = ReplicaUIFactoryV2.CreatePanel("Completion", view.Viewport, new Color(0f, 0f, 0f, 0f));
        ReplicaUIFactoryV2.Stretch(view.CompletionRect);
        view.CompletionImage = view.CompletionRect.GetComponent<Image>();
        view.CompletionImage.raycastTarget = false;
        view.CompletionRect.gameObject.SetActive(false);
        var doneText = ReplicaUIFactoryV2.CreateText(
            "DoneText",
            view.CompletionRect,
            "All steps completed",
            34,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        ReplicaUIFactoryV2.Stretch((RectTransform)doneText.transform);

        var footer = ReplicaUIFactoryV2.CreateRect("Footer", view.Card);
        footer.anchorMin = new Vector2(0f, 0f);
        footer.anchorMax = new Vector2(1f, 0f);
        footer.pivot = new Vector2(0.5f, 0f);
        footer.sizeDelta = new Vector2(0f, 84f);
        footer.anchoredPosition = new Vector2(0f, 28f);

        view.BackButton = ReplicaUIFactoryV2.CreateButton("Back", footer, config.BackButtonBg);
        var backRect = (RectTransform)view.BackButton.transform;
        backRect.anchorMin = new Vector2(0f, 0.5f);
        backRect.anchorMax = new Vector2(0f, 0.5f);
        backRect.pivot = new Vector2(0f, 0.5f);
        backRect.sizeDelta = new Vector2(160f, 48f);
        backRect.anchoredPosition = new Vector2(42f, 0f);
        var backText = ReplicaUIFactoryV2.CreateText(
            "BackLabel",
            view.BackButton.transform,
            "Back",
            22,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.BackButtonText);
        ReplicaUIFactoryV2.Stretch((RectTransform)backText.transform);

        view.NextButton = ReplicaUIFactoryV2.CreateButton("Next", footer, config.NextButtonBg);
        var nextRect = (RectTransform)view.NextButton.transform;
        nextRect.anchorMin = new Vector2(1f, 0.5f);
        nextRect.anchorMax = new Vector2(1f, 0.5f);
        nextRect.pivot = new Vector2(1f, 0.5f);
        nextRect.sizeDelta = new Vector2(180f, 50f);
        nextRect.anchoredPosition = new Vector2(-42f, 0f);
        view.NextLabel = ReplicaUIFactoryV2.CreateText(
            "NextLabel",
            view.NextButton.transform,
            "Continue",
            22,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.NextButtonText);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.NextLabel.transform);

        return view;
    }
}
