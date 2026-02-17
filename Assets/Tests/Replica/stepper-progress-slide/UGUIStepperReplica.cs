using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIStepperReplica : MonoBehaviour
{
    private readonly List<Button> mStepButtons = new();
    private readonly List<Image> mStepNodeImages = new();
    private readonly List<Text> mStepNodeTexts = new();
    private readonly List<Image> mConnectorFills = new();
    private readonly List<RectTransform> mStepContents = new();

    private RectTransform mViewport;
    private RectTransform mCompletionRect;
    private Button mBackButton;
    private Button mNextButton;
    private Text mNextLabel;
    private int mCurrentStep;
    private bool mCompleted;
    private Tween mHeightTween;
    private Tween mOutTween;
    private Tween mInTween;

    private static readonly string[] sTitles = { "Identity", "Profile", "Preferences", "Review" };
    private static readonly string[] sBodies =
    {
        "Start with your account details and choose a secure sign-in method.",
        "Add public profile metadata and a short intro for collaborators.",
        "Tune notifications, update cadence, and workspace defaults.",
        "Confirm summary, then publish settings to all linked projects."
    };

    private static readonly Color sNodeInactive = new(0.12f, 0.14f, 0.19f, 1f);
    private static readonly Color sNodeActive = new(0.32f, 0.15f, 1f, 1f);
    private static readonly Color sNodeComplete = new(0.32f, 0.15f, 1f, 1f);
    private static readonly Color sNodeTextInactive = new(0.63f, 0.66f, 0.74f, 1f);

    private void Awake()
    {
        BuildView();
        SwitchStep(0, 0, false);
    }

    private void OnDisable()
    {
        mHeightTween?.Kill();
        mOutTween?.Kill();
        mInTween?.Kill();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.05f, 0.07f, 0.12f, 0.90f);
        rootImage.raycastTarget = true;

        var ambientA = UGUIReplicaUIFactory.CreatePanel("AmbientA", root, new Color(0.22f, 0.34f, 0.56f, 0.26f));
        ambientA.anchorMin = new Vector2(0.5f, 0.5f);
        ambientA.anchorMax = new Vector2(0.5f, 0.5f);
        ambientA.pivot = new Vector2(0.5f, 0.5f);
        ambientA.sizeDelta = new Vector2(1300f, 520f);
        ambientA.anchoredPosition = new Vector2(-120f, 120f);
        ambientA.localRotation = Quaternion.Euler(0f, 0f, 14f);

        var card = UGUIReplicaUIFactory.CreatePanel("StepperCard", root, new Color(0.95f, 0.97f, 1f, 0.98f));
        card.anchorMin = new Vector2(0.5f, 0.5f);
        card.anchorMax = new Vector2(0.5f, 0.5f);
        card.pivot = new Vector2(0.5f, 0.5f);
        card.sizeDelta = new Vector2(860f, 620f);
        card.anchoredPosition = new Vector2(0f, -20f);
        UGUIReplicaUIFactory.EnsureComponent<Shadow>(card.gameObject).effectDistance = new Vector2(0f, -10f);

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            card,
            "Stepper / Multi-stage Form",
            32,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.08f, 0.11f, 0.18f, 1f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 56f);
        titleRect.anchoredPosition = new Vector2(42f, -28f);

        var row = UGUIReplicaUIFactory.CreateRect("StepRow", card);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(42f, -142f);
        row.offsetMax = new Vector2(-42f, -86f);

        mStepButtons.Clear();
        mStepNodeImages.Clear();
        mStepNodeTexts.Clear();
        mConnectorFills.Clear();
        for (var i = 0; i < sTitles.Length; i++)
        {
            var step = UGUIReplicaUIFactory.CreateRect($"Step_{i}", row);
            step.anchorMin = new Vector2((float)i / sTitles.Length, 0f);
            step.anchorMax = new Vector2((float)(i + 1) / sTitles.Length, 1f);
            step.offsetMin = Vector2.zero;
            step.offsetMax = Vector2.zero;

            var nodeButton = UGUIReplicaUIFactory.CreateButton("Node", step, new Color(1f, 1f, 1f, 0f));
            var nodeButtonRect = (RectTransform)nodeButton.transform;
            nodeButtonRect.anchorMin = new Vector2(0f, 0.5f);
            nodeButtonRect.anchorMax = new Vector2(0f, 0.5f);
            nodeButtonRect.pivot = new Vector2(0f, 0.5f);
            nodeButtonRect.sizeDelta = new Vector2(38f, 38f);
            nodeButtonRect.anchoredPosition = new Vector2(0f, 0f);
            var capture = i;
            nodeButton.onClick.AddListener(() => OnStepClicked(capture));

            var node = UGUIReplicaUIFactory.CreatePanel("NodeVisual", nodeButton.transform, sNodeInactive);
            Stretch(node);
            node.pivot = new Vector2(0.5f, 0.5f);
            var nodeLabel = UGUIReplicaUIFactory.CreateText(
                "NodeLabel",
                node,
                (i + 1).ToString(),
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                sNodeTextInactive);
            Stretch((RectTransform)nodeLabel.transform);

            var titleLabel = UGUIReplicaUIFactory.CreateText(
                "Label",
                step,
                sTitles[i],
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.08f, 0.11f, 0.18f, 0.95f));
            var labelRect = (RectTransform)titleLabel.transform;
            labelRect.anchorMin = new Vector2(0f, 0.5f);
            labelRect.anchorMax = new Vector2(1f, 0.5f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(54f, 0f);
            labelRect.sizeDelta = new Vector2(-58f, 34f);

            mStepButtons.Add(nodeButton);
            mStepNodeImages.Add(node.GetComponent<Image>());
            mStepNodeTexts.Add(nodeLabel);

            if (i < sTitles.Length - 1)
            {
                var connector = UGUIReplicaUIFactory.CreatePanel("Connector", step, new Color(0.82f, 0.84f, 0.90f, 0.9f));
                connector.anchorMin = new Vector2(0f, 0.5f);
                connector.anchorMax = new Vector2(1f, 0.5f);
                connector.pivot = new Vector2(0f, 0.5f);
                connector.sizeDelta = new Vector2(-44f, 4f);
                connector.anchoredPosition = new Vector2(40f, 0f);

                var fill = UGUIReplicaUIFactory.CreatePanel("Fill", connector, sNodeActive).GetComponent<Image>();
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
                mConnectorFills.Add(fill);
            }
        }

        var viewportFrame = UGUIReplicaUIFactory.CreatePanel("ViewportFrame", card, new Color(0.91f, 0.93f, 0.98f, 1f));
        viewportFrame.anchorMin = new Vector2(0f, 1f);
        viewportFrame.anchorMax = new Vector2(1f, 1f);
        viewportFrame.pivot = new Vector2(0.5f, 1f);
        viewportFrame.offsetMin = new Vector2(42f, -460f);
        viewportFrame.offsetMax = new Vector2(-42f, -156f);

        mViewport = UGUIReplicaUIFactory.CreateRect("Viewport", viewportFrame);
        mViewport.anchorMin = new Vector2(0f, 1f);
        mViewport.anchorMax = new Vector2(1f, 1f);
        mViewport.pivot = new Vector2(0.5f, 1f);
        mViewport.sizeDelta = new Vector2(0f, 240f);
        mViewport.anchoredPosition = Vector2.zero;
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(mViewport.gameObject);

        mStepContents.Clear();
        for (var i = 0; i < sTitles.Length; i++)
        {
            var content = UGUIReplicaUIFactory.CreateRect($"Content_{i}", mViewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = new Vector2(26f, 0f);
            content.offsetMax = new Vector2(-26f, 0f);
            content.anchoredPosition = Vector2.zero;

            var heading = UGUIReplicaUIFactory.CreateText(
                "Heading",
                content,
                $"Step {i + 1}: {sTitles[i]}",
                30,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.08f, 0.10f, 0.18f, 1f));
            var headingRect = (RectTransform)heading.transform;
            headingRect.anchorMin = new Vector2(0f, 1f);
            headingRect.anchorMax = new Vector2(1f, 1f);
            headingRect.pivot = new Vector2(0f, 1f);
            headingRect.sizeDelta = new Vector2(0f, 44f);
            headingRect.anchoredPosition = Vector2.zero;

            var body = UGUIReplicaUIFactory.CreateText(
                "Body",
                content,
                sBodies[i],
                22,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.22f, 0.25f, 0.35f, 1f));
            var bodyRect = (RectTransform)body.transform;
            bodyRect.anchorMin = new Vector2(0f, 1f);
            bodyRect.anchorMax = new Vector2(1f, 1f);
            bodyRect.pivot = new Vector2(0f, 1f);
            bodyRect.sizeDelta = new Vector2(0f, 120f);
            bodyRect.anchoredPosition = new Vector2(0f, -58f);

            var tagRow = UGUIReplicaUIFactory.CreateRect("Tags", content);
            tagRow.anchorMin = new Vector2(0f, 1f);
            tagRow.anchorMax = new Vector2(1f, 1f);
            tagRow.pivot = new Vector2(0f, 1f);
            tagRow.sizeDelta = new Vector2(0f, 42f);
            tagRow.anchoredPosition = new Vector2(0f, -164f);

            for (var t = 0; t < 2; t++)
            {
                var badge = UGUIReplicaUIFactory.CreatePanel(
                    $"Tag_{t}",
                    tagRow,
                    new Color(0.84f - (t * 0.08f), 0.87f - (t * 0.08f), 0.98f, 1f));
                badge.anchorMin = new Vector2(0f, 0.5f);
                badge.anchorMax = new Vector2(0f, 0.5f);
                badge.pivot = new Vector2(0f, 0.5f);
                badge.sizeDelta = new Vector2(188f, 34f);
                badge.anchoredPosition = new Vector2(t * 198f, 0f);

                var badgeText = UGUIReplicaUIFactory.CreateText(
                    "TagLabel",
                    badge,
                    t == 0 ? "Motion spring" : "State aware",
                    16,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    new Color(0.20f, 0.24f, 0.37f, 1f));
                Stretch((RectTransform)badgeText.transform);
            }

            var preferred = 226f + (i * 8f);
            content.sizeDelta = new Vector2(0f, preferred);
            mStepContents.Add(content);
        }

        mCompletionRect = UGUIReplicaUIFactory.CreatePanel("Completion", mViewport, new Color(0.18f, 0.52f, 0.34f, 0f));
        Stretch(mCompletionRect);
        mCompletionRect.gameObject.SetActive(false);
        var doneText = UGUIReplicaUIFactory.CreateText(
            "DoneText",
            mCompletionRect,
            "All steps completed",
            34,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        Stretch((RectTransform)doneText.transform);

        var footer = UGUIReplicaUIFactory.CreateRect("Footer", card);
        footer.anchorMin = new Vector2(0f, 0f);
        footer.anchorMax = new Vector2(1f, 0f);
        footer.pivot = new Vector2(0.5f, 0f);
        footer.sizeDelta = new Vector2(0f, 84f);
        footer.anchoredPosition = new Vector2(0f, 28f);

        mBackButton = UGUIReplicaUIFactory.CreateButton("Back", footer, new Color(0.91f, 0.93f, 0.97f, 1f));
        var backRect = (RectTransform)mBackButton.transform;
        backRect.anchorMin = new Vector2(0f, 0.5f);
        backRect.anchorMax = new Vector2(0f, 0.5f);
        backRect.pivot = new Vector2(0f, 0.5f);
        backRect.sizeDelta = new Vector2(160f, 48f);
        backRect.anchoredPosition = new Vector2(42f, 0f);
        var backText = UGUIReplicaUIFactory.CreateText(
            "BackLabel",
            mBackButton.transform,
            "Back",
            22,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.34f, 0.37f, 0.45f, 1f));
        Stretch((RectTransform)backText.transform);
        mBackButton.onClick.AddListener(OnBackClicked);

        mNextButton = UGUIReplicaUIFactory.CreateButton("Next", footer, sNodeActive);
        var nextRect = (RectTransform)mNextButton.transform;
        nextRect.anchorMin = new Vector2(1f, 0.5f);
        nextRect.anchorMax = new Vector2(1f, 0.5f);
        nextRect.pivot = new Vector2(1f, 0.5f);
        nextRect.sizeDelta = new Vector2(180f, 50f);
        nextRect.anchoredPosition = new Vector2(-42f, 0f);
        mNextLabel = UGUIReplicaUIFactory.CreateText(
            "NextLabel",
            mNextButton.transform,
            "Continue",
            22,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        Stretch((RectTransform)mNextLabel.transform);
        mNextButton.onClick.AddListener(OnNextClicked);
    }

    private void OnStepClicked(int target)
    {
        if (mCompleted || target == mCurrentStep)
        {
            return;
        }

        var direction = target > mCurrentStep ? 1 : -1;
        SwitchStep(target, direction, true);
    }

    private void OnBackClicked()
    {
        if (mCompleted || mCurrentStep <= 0)
        {
            return;
        }

        SwitchStep(mCurrentStep - 1, -1, true);
    }

    private void OnNextClicked()
    {
        if (mCompleted)
        {
            return;
        }

        if (mCurrentStep < sTitles.Length - 1)
        {
            SwitchStep(mCurrentStep + 1, 1, true);
        }
        else
        {
            CompleteFlow();
        }
    }

    private void SwitchStep(int target, int direction, bool animate)
    {
        target = Mathf.Clamp(target, 0, sTitles.Length - 1);
        mCompleted = false;
        mCompletionRect.gameObject.SetActive(false);
        mCompletionRect.GetComponent<Image>().color = new Color(0.18f, 0.52f, 0.34f, 0f);

        var previous = mCurrentStep;
        mCurrentStep = target;

        for (var i = 0; i < mStepContents.Count; i++)
        {
            mStepContents[i].gameObject.SetActive(i == previous || i == mCurrentStep);
        }

        var incoming = mStepContents[mCurrentStep];
        var outgoing = mStepContents[previous];
        incoming.SetAsLastSibling();
        UpdateStepVisuals(true);

        var targetHeight = incoming.sizeDelta.y + 12f;
        mHeightTween?.Kill();
        mHeightTween = mViewport.DOSizeDelta(new Vector2(mViewport.sizeDelta.x, targetHeight), animate ? 0.35f : 0f).SetEase(Ease.OutCubic).SetUpdate(true);

        if (!animate || previous == mCurrentStep)
        {
            incoming.anchoredPosition = Vector2.zero;
            outgoing.anchoredPosition = Vector2.zero;
            outgoing.gameObject.SetActive(previous == mCurrentStep);
            return;
        }

        mOutTween?.Kill();
        mInTween?.Kill();

        var width = Mathf.Max(280f, mViewport.rect.width);
        var enterFrom = direction > 0 ? width : -width;
        var exitTo = direction > 0 ? -width * 0.5f : width * 0.5f;
        incoming.anchoredPosition = new Vector2(enterFrom, 0f);

        mOutTween = outgoing.DOAnchorPosX(exitTo, 0.36f).SetEase(Ease.OutCubic).SetUpdate(true);
        mInTween = incoming.DOAnchorPosX(0f, 0.38f).SetEase(Ease.OutCubic).SetUpdate(true);
        mInTween.OnComplete(() => outgoing.gameObject.SetActive(false));
    }

    private void CompleteFlow()
    {
        mCompleted = true;
        mOutTween?.Kill();
        mInTween?.Kill();

        for (var i = 0; i < mStepContents.Count; i++)
        {
            mStepContents[i].gameObject.SetActive(false);
        }

        UpdateStepVisuals(true);
        mBackButton.interactable = false;
        mNextButton.interactable = true;
        mNextLabel.text = "Restart";
        mNextButton.onClick.RemoveAllListeners();
        mNextButton.onClick.AddListener(RestartFlow);

        mHeightTween?.Kill();
        mHeightTween = DOTween.Sequence()
            .SetUpdate(true)
            .Append(mViewport.DOSizeDelta(new Vector2(mViewport.sizeDelta.x, 0f), 0.3f).SetEase(Ease.InCubic))
            .AppendCallback(() =>
            {
                mCompletionRect.gameObject.SetActive(true);
                var color = mCompletionRect.GetComponent<Image>().color;
                color.a = 0f;
                mCompletionRect.GetComponent<Image>().color = color;
            })
            .Append(mCompletionRect.GetComponent<Image>().DOFade(0.92f, 0.28f).SetEase(Ease.OutQuad));
    }

    private void RestartFlow()
    {
        mNextButton.onClick.RemoveAllListeners();
        mNextButton.onClick.AddListener(OnNextClicked);
        mNextLabel.text = "Continue";
        mBackButton.interactable = true;
        SwitchStep(0, 1, true);
    }

    private void UpdateStepVisuals(bool animate)
    {
        for (var i = 0; i < mStepNodeImages.Count; i++)
        {
            var state = i < mCurrentStep ? 2 : i == mCurrentStep ? 1 : 0;
            if (mCompleted)
            {
                state = 2;
            }

            var node = mStepNodeImages[i];
            var label = mStepNodeTexts[i];
            var targetColor = state == 0 ? sNodeInactive : state == 1 ? sNodeActive : sNodeComplete;
            var textColor = state == 0 ? sNodeTextInactive : Color.white;

            if (animate)
            {
                node.DOColor(targetColor, 0.24f).SetEase(Ease.OutQuad).SetUpdate(true);
                label.DOColor(textColor, 0.24f).SetEase(Ease.OutQuad).SetUpdate(true);
            }
            else
            {
                node.color = targetColor;
                label.color = textColor;
            }

            label.text = state == 2 ? "\u2713" : state == 1 ? "\u2022" : (i + 1).ToString();
        }

        for (var i = 0; i < mConnectorFills.Count; i++)
        {
            var fill = mCompleted || mCurrentStep > i ? 1f : 0f;
            if (animate)
            {
                mConnectorFills[i].DOFillAmount(fill, 0.30f).SetEase(Ease.OutCubic).SetUpdate(true);
            }
            else
            {
                mConnectorFills[i].fillAmount = fill;
            }
        }

        mBackButton.interactable = !mCompleted && mCurrentStep > 0;
        if (!mCompleted)
        {
            mNextLabel.text = mCurrentStep >= sTitles.Length - 1 ? "Complete" : "Continue";
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
