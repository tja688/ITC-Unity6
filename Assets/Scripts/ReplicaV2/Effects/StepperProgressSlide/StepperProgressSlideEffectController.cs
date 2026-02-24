using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class StepperProgressSlideEffectController : IReplicaEffect<StepperProgressSlideEffectConfig, StepperProgressSlideEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<IReplicaTweenHandle> mRuntimeTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private StepperProgressSlideEffectConfig mConfig;
    private StepperProgressSlideEffectView mView;
    private bool mInitialized;

    private int mCurrentStep;
    private bool mCompleted;

    private Vector2 mBaseCardPos;

    public string EffectId => "stepper-progress-slide-v2";

    public void Initialize(ReplicaHostContext context, StepperProgressSlideEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<StepperProgressSlideEffectConfig>();
        mView = StepperProgressSlideEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mBaseCardPos = mView.Card.anchoredPosition;
        WireEvents();
        mInitialized = true;
        SwitchStep(0, 0, false);
    }

    public void SetModel(StepperProgressSlideEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model != null ? model : StepperProgressSlideEffectModel.CreateDefault();
        if (mView.HeaderTitle != null)
        {
            mView.HeaderTitle.text = string.IsNullOrWhiteSpace(safe.HeaderTitle) ? "Stepper / Multi-stage Form" : safe.HeaderTitle;
        }

        var stepCount = safe.Steps != null ? safe.Steps.Count : 0;
        stepCount = Mathf.Clamp(stepCount, 1, mView.StepContents.Count);

        for (var i = 0; i < stepCount; i++)
        {
            var step = safe.Steps[i];
            if (step == null)
            {
                continue;
            }

            if (mView.StepNodeTexts.Count > i && mView.StepNodeTexts[i] != null)
            {
                mView.StepNodeTexts[i].text = (i + 1).ToString();
            }

            if (mView.StepContents.Count > i && mView.StepContents[i] != null)
            {
                var content = mView.StepContents[i];
                if (content.Heading != null)
                {
                    content.Heading.text = $"Step {i + 1}: {step.Title}";
                }

                if (content.Body != null)
                {
                    content.Body.text = step.Body;
                }
            }

            if (mView.StepTitleTexts.Count > i && mView.StepTitleTexts[i] != null)
            {
                mView.StepTitleTexts[i].text = step.Title;
            }
        }

        UpdateStepVisuals(animated);
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        mView.Card.anchoredPosition = mBaseCardPos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, mBaseCardPos, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, mBaseCardPos + offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        if (mContext.Pointer != null && mView != null)
        {
            mContext.Pointer.TryGetLocalPoint(mView.Card, out _, mContext.UICamera);
        }
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillRuntimeTweens();
        UnwireEvents();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void WireEvents()
    {
        for (var i = 0; i < mView.StepButtons.Count; i++)
        {
            var capture = i;
            mView.StepButtons[i].onClick.AddListener(() => OnStepClicked(capture));
        }

        mView.BackButton.onClick.AddListener(OnBackClicked);
        mView.NextButton.onClick.AddListener(OnNextClicked);
    }

    private void UnwireEvents()
    {
        for (var i = 0; i < mView.StepButtons.Count; i++)
        {
            if (mView.StepButtons[i] != null)
            {
                mView.StepButtons[i].onClick.RemoveAllListeners();
            }
        }

        if (mView.BackButton != null)
        {
            mView.BackButton.onClick.RemoveAllListeners();
        }

        if (mView.NextButton != null)
        {
            mView.NextButton.onClick.RemoveAllListeners();
        }
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

        if (mCurrentStep < mView.StepContents.Count - 1)
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
        target = Mathf.Clamp(target, 0, mView.StepContents.Count - 1);

        mCompleted = false;
        if (mView.CompletionRect != null)
        {
            mView.CompletionRect.gameObject.SetActive(false);
        }

        var previous = mCurrentStep;
        mCurrentStep = target;

        for (var i = 0; i < mView.StepContents.Count; i++)
        {
            var shouldActive = i == previous || i == mCurrentStep;
            mView.StepContents[i].Root.gameObject.SetActive(shouldActive);
        }

        var incoming = mView.StepContents[mCurrentStep].Root;
        var outgoing = mView.StepContents[previous].Root;
        incoming.SetAsLastSibling();
        UpdateStepVisuals(true);

        var targetHeight = incoming.sizeDelta.y + mConfig.ViewportHeightPadding;
        TweenViewportHeight(targetHeight, animate ? mConfig.HeightDuration : 0f);

        if (!animate || previous == mCurrentStep)
        {
            incoming.anchoredPosition = Vector2.zero;
            outgoing.anchoredPosition = Vector2.zero;
            outgoing.gameObject.SetActive(previous == mCurrentStep);
            return;
        }

        KillSlideTweens();

        var width = Mathf.Max(280f, mView.Viewport.rect.width);
        var enterFrom = direction > 0 ? width : -width;
        var exitTo = direction > 0 ? -width * 0.5f : width * 0.5f;
        incoming.anchoredPosition = new Vector2(enterFrom, 0f);

        mRuntimeTweens.Add(mContext.Tweens
            .AnchoredPosXTo(outgoing, exitTo, mConfig.SlideOutDuration, outgoing)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime));

        mRuntimeTweens.Add(mContext.Tweens
            .AnchoredPosXTo(incoming, 0f, mConfig.SlideInDuration, incoming)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime)
            .OnComplete(() => outgoing.gameObject.SetActive(false)));
    }

    private void TweenViewportHeight(float targetHeight, float duration)
    {
        if (mView.Viewport == null)
        {
            return;
        }

        mContext.Tweens.Kill(mView.Viewport);

        if (duration <= 0.0001f)
        {
            mView.Viewport.sizeDelta = new Vector2(mView.Viewport.sizeDelta.x, targetHeight);
            return;
        }

        var start = mView.Viewport.sizeDelta.y;
        mRuntimeTweens.Add(mContext.Tweens
            .ToFloat(
                () => start,
                value =>
                {
                    start = value;
                    mView.Viewport.sizeDelta = new Vector2(mView.Viewport.sizeDelta.x, value);
                },
                targetHeight,
                duration,
                mView.Viewport)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime));
    }

    private void CompleteFlow()
    {
        mCompleted = true;
        KillSlideTweens();

        for (var i = 0; i < mView.StepContents.Count; i++)
        {
            mView.StepContents[i].Root.gameObject.SetActive(false);
        }

        UpdateStepVisuals(true);

        mView.BackButton.interactable = false;
        mView.NextButton.interactable = true;
        mView.NextLabel.text = "Restart";
        mView.NextButton.onClick.RemoveAllListeners();
        mView.NextButton.onClick.AddListener(RestartFlow);

        TweenViewportHeight(0f, 0.30f);

        mRuntimeTweens.Add(mContext.Tweens
            .DelayedCall(0.30f, () =>
            {
                if (mView.CompletionRect == null || mView.CompletionImage == null)
                {
                    return;
                }

                mView.CompletionRect.gameObject.SetActive(true);
                var color = mConfig.CompletionColor;
                color.a = 0f;
                mView.CompletionImage.color = color;

                mRuntimeTweens.Add(mContext.Tweens
                    .FadeImageTo(mView.CompletionImage, mConfig.CompletionColor.a, 0.28f, mView.CompletionImage)
                    .SetEase(ReplicaEase.OutQuad)
                    .SetUpdate(mContext.UseUnscaledTime));
            }, mView.Root)
            .SetUpdate(mContext.UseUnscaledTime));
    }

    private void RestartFlow()
    {
        mView.NextButton.onClick.RemoveAllListeners();
        mView.NextButton.onClick.AddListener(OnNextClicked);
        mView.NextLabel.text = "Continue";
        mView.BackButton.interactable = true;
        SwitchStep(0, 1, true);
    }

    private void UpdateStepVisuals(bool animate)
    {
        for (var i = 0; i < mView.StepNodeImages.Count; i++)
        {
            var state = i < mCurrentStep ? 2 : i == mCurrentStep ? 1 : 0;
            if (mCompleted)
            {
                state = 2;
            }

            var node = mView.StepNodeImages[i];
            var label = mView.StepNodeTexts[i];
            var targetColor = state == 0 ? mConfig.NodeInactive : state == 1 ? mConfig.NodeActive : mConfig.NodeComplete;
            var textColor = state == 0 ? mConfig.NodeTextInactive : Color.white;

            if (animate)
            {
                mContext.Tweens.Kill(node);
                mRuntimeTweens.Add(mContext.Tweens
                    .ColorImageTo(node, targetColor, 0.24f, node)
                    .SetEase(ReplicaEase.OutQuad)
                    .SetUpdate(mContext.UseUnscaledTime));
                TweenTextColor(label, textColor, 0.24f);
            }
            else
            {
                node.color = targetColor;
                label.color = textColor;
            }

            label.text = state == 2 ? "\u2713" : state == 1 ? "\u2022" : (i + 1).ToString();
        }

        for (var i = 0; i < mView.ConnectorFills.Count; i++)
        {
            var fill = mCompleted || mCurrentStep > i ? 1f : 0f;
            if (animate)
            {
                TweenFillAmount(mView.ConnectorFills[i], fill, mConfig.ConnectorFillDuration);
            }
            else
            {
                mView.ConnectorFills[i].fillAmount = fill;
            }
        }

        mView.BackButton.interactable = !mCompleted && mCurrentStep > 0;
        if (!mCompleted)
        {
            mView.NextLabel.text = mCurrentStep >= mView.StepContents.Count - 1 ? "Complete" : "Continue";
        }
    }

    private void TweenTextColor(UnityEngine.UI.Text label, Color target, float duration)
    {
        if (label == null)
        {
            return;
        }

        mContext.Tweens.Kill(label);

        if (duration <= 0.0001f)
        {
            label.color = target;
            return;
        }

        var start = label.color;
        var t = 0f;
        mRuntimeTweens.Add(mContext.Tweens
            .ToFloat(
                () => t,
                value =>
                {
                    t = value;
                    label.color = Color.Lerp(start, target, value);
                },
                1f,
                duration,
                label)
            .SetEase(ReplicaEase.OutQuad)
            .SetUpdate(mContext.UseUnscaledTime));
    }

    private void TweenFillAmount(UnityEngine.UI.Image image, float target, float duration)
    {
        if (image == null)
        {
            return;
        }

        mContext.Tweens.Kill(image);

        if (duration <= 0.0001f)
        {
            image.fillAmount = target;
            return;
        }

        var start = image.fillAmount;
        mRuntimeTweens.Add(mContext.Tweens
            .ToFloat(
                () => start,
                value =>
                {
                    start = value;
                    image.fillAmount = value;
                },
                target,
                duration,
                image)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime));
    }

    private IReplicaTweenHandle FadeCanvasGroup(UnityEngine.CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
    {
        var alpha = group.alpha;
        return mContext.Tweens
            .ToFloat(
                () => alpha,
                value =>
                {
                    alpha = value;
                    group.alpha = value;
                },
                targetAlpha,
                duration,
                owner)
            .SetEase(ease);
    }

    private void KillTransitionTweens()
    {
        for (var i = 0; i < mTransitionTweens.Count; i++)
        {
            mTransitionTweens[i]?.Kill();
        }

        mTransitionTweens.Clear();
    }

    private void KillRuntimeTweens()
    {
        for (var i = 0; i < mRuntimeTweens.Count; i++)
        {
            mRuntimeTweens[i]?.Kill();
        }

        mRuntimeTweens.Clear();
    }

    private void KillSlideTweens()
    {
        for (var i = 0; i < mView.StepContents.Count; i++)
        {
            var root = mView.StepContents[i].Root;
            if (root != null)
            {
                mContext.Tweens.Kill(root);
            }
        }
    }

    private static Vector2 EnterOffset(ReplicaEnterDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaEnterDirection.FromTop: return new Vector2(0f, distance);
            case ReplicaEnterDirection.FromBottom: return new Vector2(0f, -distance);
            case ReplicaEnterDirection.FromLeft: return new Vector2(-distance, 0f);
            case ReplicaEnterDirection.FromRight: return new Vector2(distance, 0f);
            default: return Vector2.zero;
        }
    }

    private static Vector2 ExitOffset(ReplicaExitDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaExitDirection.ToTop: return new Vector2(0f, distance);
            case ReplicaExitDirection.ToBottom: return new Vector2(0f, -distance);
            case ReplicaExitDirection.ToLeft: return new Vector2(-distance, 0f);
            case ReplicaExitDirection.ToRight: return new Vector2(distance, 0f);
            case ReplicaExitDirection.FadeOnly:
            default:
                return Vector2.zero;
        }
    }
}
