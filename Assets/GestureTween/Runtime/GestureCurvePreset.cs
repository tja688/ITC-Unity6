using DG.Tweening;
using UnityEngine;

namespace GestureTween
{
    /// <summary>
    /// 手绘曲线预设资产，存储 AnimationCurve 并可应用到 DOTweenAnimation
    /// </summary>
    [CreateAssetMenu(fileName = "GestureCurve", menuName = "GestureTween/Curve Preset")]
    public class GestureCurvePreset : ScriptableObject
    {
        [Tooltip("手绘生成的缓动曲线")]
        public AnimationCurve easeCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f)
        );

        [Tooltip("推荐时长（秒）")]
        public float recommendedDuration = 1f;

        [Tooltip("曲线描述")]
        [TextArea(2, 4)]
        public string description;

        /// <summary>
        /// 应用曲线到 DOTweenAnimation 组件
        /// </summary>
        public void ApplyTo(DOTweenAnimation anim)
        {
            if (anim == null) return;

            anim.easeType = Ease.INTERNAL_Custom;
            anim.easeCurve = new AnimationCurve(easeCurve.keys);
            anim.duration = recommendedDuration;
        }

        /// <summary>
        /// 使用此曲线创建一个 virtual tween (用于预览)
        /// </summary>
        public Tween CreatePreviewTween(TweenCallback<float> onUpdate, float duration = -1f)
        {
            float dur = duration > 0 ? duration : recommendedDuration;
            return DOVirtual.Float(0f, 1f, dur, onUpdate).SetEase(easeCurve);
        }
    }
}
