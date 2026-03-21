using UnityEngine;
using DG.Tweening;

/// <summary>
/// 检测挂载对象上的第一个DOTweenAnimation组件，
/// 在游戏开始后等待3秒播放，播放完成后等待3秒再次播放，循环往复
/// </summary>
public class DOTweenAnimationLoopDriver : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("初始等待时间（秒）")]
    public float initialDelay = 3f;
    
    [Tooltip("播放完成后等待时间（秒）")]
    public float repeatDelay = 3f;

    private DOTweenAnimation _targetAnimation;

    private void Start()
    {
        // 获取第一个DOTweenAnimation组件
        _targetAnimation = GetComponent<DOTweenAnimation>();
        
        if (_targetAnimation == null)
        {
            Debug.LogError("[DOTweenAnimationLoopDriver] 未找到DOTweenAnimation组件！", this);
            return;
        }

        // 禁用自动播放，由本脚本控制
        _targetAnimation.autoPlay = false;

        // 开始延迟播放循环
        StartCoroutine(PlayLoop());
    }

    private System.Collections.IEnumerator PlayLoop()
    {
        // 初始等待
        Debug.Log($"[DOTweenAnimationLoopDriver] 等待 {initialDelay} 秒后开始第一次播放...");
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // 播放动画
            Debug.Log("[DOTweenAnimationLoopDriver] 开始播放动画");
            
            // 使用DORestart确保从头播放
            _targetAnimation.DORestart();
            
            // 等待动画完成
            // 获取tween的实际持续时间
            float duration = _targetAnimation.duration;
            int loops = _targetAnimation.loops;
            
            // 计算总时长（考虑循环）
            float totalDuration = loops > 0 ? duration * loops : duration;
            
            yield return new WaitForSeconds(totalDuration);
            
            Debug.Log($"[DOTweenAnimationLoopDriver] 动画播放完成，等待 {repeatDelay} 秒后再次播放...");
            
            // 等待指定时间后再次播放
            yield return new WaitForSeconds(repeatDelay);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
