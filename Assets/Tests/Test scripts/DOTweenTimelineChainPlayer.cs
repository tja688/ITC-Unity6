using DG.Tweening;
using Dott;
using UnityEngine;

/// <summary>
/// 串联播放两个 DOTweenTimeline 的测试脚本。
/// 在第一个时间轴播完后自动播放第二个。
/// </summary>
public class DOTweenTimelineChainPlayer : MonoBehaviour
{
    [Header("Timelines")]
    [SerializeField] private DOTweenTimeline firstTimeline;
    [SerializeField] private DOTweenTimeline secondTimeline;

    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart)
        {
            PlayChain();
        }
    }

    /// <summary>
    /// 开始执行链式播放
    /// </summary>
    [ContextMenu("Play Chain")]
    public void PlayChain()
    {
        if (firstTimeline == null)
        {
            Debug.LogWarning("[DOTweenTimelineChainPlayer] First timeline is not assigned!", this);
            if (secondTimeline != null)
            {
                secondTimeline.Play();
            }
            return;
        }

        // 获取第一个时间轴的 Sequence 并注册完成回调
        var sequence = firstTimeline.Play();

        if (sequence != null && sequence.IsActive())
        {
            sequence.OnComplete(() =>
            {
                if (secondTimeline != null)
                {
                    Debug.Log("[DOTweenTimelineChainPlayer] First timeline completed. Playing second timeline.", this);
                    secondTimeline.Play();
                }
                else
                {
                    Debug.LogWarning("[DOTweenTimelineChainPlayer] Second timeline is not assigned!", this);
                }
            });

            Debug.Log("[DOTweenTimelineChainPlayer] Playing first timeline...", this);
        }
        else
        {
            // 如果第一个时间轴无法播放或没有动画，直接播放第二个
            Debug.LogWarning("[DOTweenTimelineChainPlayer] First timeline sequence is null or inactive. Skipping to second timeline.", this);
            if (secondTimeline != null)
            {
                secondTimeline.Play();
            }
        }
    }
}
