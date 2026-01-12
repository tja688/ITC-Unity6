using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class AimTweenExtras : MonoBehaviour
{
    [Header("Move Sync")]
    [SerializeField] private DOTweenAnimation moveTween;
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private Ease moveEase = Ease.OutElastic;

    [Header("Scale")]
    [SerializeField] private float scaleLeadTime = 0.2f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private float scalePunch = 0.08f;
    [SerializeField] private int scaleVibrato = 6;
    [SerializeField] private float scaleElasticity = 1f;

    [Header("Color")]
    [SerializeField] private Color scaleColor = Color.green;
    [SerializeField] private float colorDuration = 0.08f;

    [Header("Rotate")]
    [SerializeField] private float rotateDuration = 0.25f;

    private RectTransform rectTransform;
    private Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        if (moveTween == null)
        {
            moveTween = GetComponent<DOTweenAnimation>();
        }
        if (moveTween != null)
        {
            moveTween.duration = moveDuration;
            moveTween.easeType = moveEase;
        }
    }

    private void Start()
    {
        PlayExtras();
    }

    private void PlayExtras()
    {
        var moveDuration = moveTween != null ? moveTween.duration : this.moveDuration;
        var scaleStart = Mathf.Max(0f, moveDuration - scaleLeadTime);

        if (rectTransform != null)
        {
            rectTransform
                .DOPunchScale(Vector3.one * scalePunch, scaleDuration, scaleVibrato, scaleElasticity)
                .SetDelay(scaleStart);
        }

        if (image != null)
        {
            image
                .DOColor(scaleColor, colorDuration)
                .SetDelay(scaleStart)
                .SetEase(Ease.OutQuad);
        }

        if (rectTransform != null)
        {
            rectTransform
                .DOLocalRotate(new Vector3(0f, 0f, 360f), rotateDuration, RotateMode.FastBeyond360)
                .SetDelay(scaleStart + scaleDuration)
                .SetEase(Ease.OutQuad);
        }
    }
}
