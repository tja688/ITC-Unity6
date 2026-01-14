using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 瞄准动画扩展脚本
/// 为UI元素提供一系列组合动画效果,包括缩放、颜色变化和旋转
/// 主要用于瞄准指示器或需要强调的UI元素
/// </summary>
public sealed class AimTweenExtras : MonoBehaviour
{
    #region 移动同步配置
    [Header("Move Sync")]
    [Tooltip("DOTween动画组件,用于同步移动动画")]
    [SerializeField] private DOTweenAnimation moveTween;
    [Tooltip("移动动画持续时间")]
    [SerializeField] private float moveDuration = 0.6f;
    [Tooltip("移动动画缓动类型")]
    [SerializeField] private Ease moveEase = Ease.OutElastic;
    #endregion

    #region 缩放配置
    [Header("Scale")]
    [Tooltip("缩放动画提前时间,相对于移动动画结束的时间")]
    [SerializeField] private float scaleLeadTime = 0.2f;
    [Tooltip("缩放动画持续时间")]
    [SerializeField] private float scaleDuration = 0.2f;
    [Tooltip("缩放冲量强度")]
    [SerializeField] private float scalePunch = 0.08f;
    [Tooltip("缩放振动次数")]
    [SerializeField] private int scaleVibrato = 6;
    [Tooltip("缩放弹性系数")]
    [SerializeField] private float scaleElasticity = 1f;
    #endregion

    #region 颜色配置
    [Header("Color")]
    [Tooltip("缩放时的目标颜色")]
    [SerializeField] private Color scaleColor = Color.green;
    [Tooltip("颜色变化持续时间")]
    [SerializeField] private float colorDuration = 0.08f;
    #endregion

    #region 旋转配置
    [Header("Rotate")]
    [Tooltip("旋转动画持续时间")]
    [SerializeField] private float rotateDuration = 0.25f;
    #endregion

    // 组件引用
    private RectTransform rectTransform;
    private Image image;

    /// <summary>
    /// 初始化组件引用并配置移动动画参数
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        // 如果没有手动指定DOTweenAnimation组件,则尝试从当前对象获取
        if (moveTween == null)
        {
            moveTween = GetComponent<DOTweenAnimation>();
        }
        
        // 配置移动动画参数
        if (moveTween != null)
        {
            moveTween.duration = moveDuration;
            moveTween.easeType = moveEase;
        }
    }

    /// <summary>
    /// 启动时播放额外的动画效果
    /// </summary>
    private void Start()
    {
        PlayExtras();
    }

    /// <summary>
    /// 播放所有额外的动画效果(缩放、颜色变化、旋转)
    /// 动画按照特定顺序和延迟执行,以创造连贯的视觉效果
    /// </summary>
    private void PlayExtras()
    {
        // 获取移动动画的实际持续时间
        var moveDuration = moveTween != null ? moveTween.duration : this.moveDuration;
        
        // 计算缩放动画的开始时间(在移动动画结束前scaleLeadTime秒开始)
        var scaleStart = Mathf.Max(0f, moveDuration - scaleLeadTime);

        // 播放缩放冲量动画
        if (rectTransform != null)
        {
            rectTransform
                .DOPunchScale(Vector3.one * scalePunch, scaleDuration, scaleVibrato, scaleElasticity)
                .SetDelay(scaleStart);
        }

        // 播放颜色变化动画
        if (image != null)
        {
            image
                .DOColor(scaleColor, colorDuration)
                .SetDelay(scaleStart)
                .SetEase(Ease.OutQuad);
        }

        // 播放旋转动画(在缩放动画结束后开始)
        if (rectTransform != null)
        {
            rectTransform
                .DOLocalRotate(new Vector3(0f, 0f, 360f), rotateDuration, RotateMode.FastBeyond360)
                .SetDelay(scaleStart + scaleDuration)
                .SetEase(Ease.OutQuad);
        }
    }
}
