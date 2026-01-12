using Dott;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class DOTweenTimelineSliderPreview : MonoBehaviour
{
    [SerializeField] private DOTweenTimeline timeline;
    [SerializeField] private bool useNormalizedValue = true;
    [SerializeField] private bool pauseBeforePreview = true;

    private Slider slider;

    private void Awake()
    {
        if (!TryGetComponent(out slider))
        {
            Debug.LogError("DOTweenTimelineSliderPreview requires a Slider component.", this);
            enabled = false;
            return;
        }

        if (timeline == null)
        {
            timeline = GetComponentInParent<DOTweenTimeline>();
        }
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        if (useNormalizedValue)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        if (timeline == null)
        {
            return;
        }

        if (pauseBeforePreview)
        {
            timeline.Pause();
        }

        if (useNormalizedValue)
        {
            timeline.GotoPercentageAndPause(value);
        }
        else
        {
            float clampedTime = value;
            float duration = timeline.Duration;
            if (duration > 0f)
            {
                clampedTime = Mathf.Clamp(value, 0f, duration);
            }
            timeline.GotoAndPause(clampedTime);
        }
    }
}
