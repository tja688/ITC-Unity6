using DG.Tweening;
using SevenStrikeModules.XTween;

public static class ReplicaEaseMapper
{
    public static Ease ToDotween(ReplicaEase ease)
    {
        switch (ease)
        {
            case ReplicaEase.Linear: return Ease.Linear;
            case ReplicaEase.InSine: return Ease.InSine;
            case ReplicaEase.OutSine: return Ease.OutSine;
            case ReplicaEase.InOutSine: return Ease.InOutSine;
            case ReplicaEase.InQuad: return Ease.InQuad;
            case ReplicaEase.OutQuad: return Ease.OutQuad;
            case ReplicaEase.InOutQuad: return Ease.InOutQuad;
            case ReplicaEase.InCubic: return Ease.InCubic;
            case ReplicaEase.OutCubic: return Ease.OutCubic;
            case ReplicaEase.InOutCubic: return Ease.InOutCubic;
            case ReplicaEase.InQuart: return Ease.InQuart;
            case ReplicaEase.OutQuart: return Ease.OutQuart;
            case ReplicaEase.InOutQuart: return Ease.InOutQuart;
            case ReplicaEase.InQuint: return Ease.InQuint;
            case ReplicaEase.OutQuint: return Ease.OutQuint;
            case ReplicaEase.InOutQuint: return Ease.InOutQuint;
            case ReplicaEase.InExpo: return Ease.InExpo;
            case ReplicaEase.OutExpo: return Ease.OutExpo;
            case ReplicaEase.InOutExpo: return Ease.InOutExpo;
            case ReplicaEase.InCirc: return Ease.InCirc;
            case ReplicaEase.OutCirc: return Ease.OutCirc;
            case ReplicaEase.InOutCirc: return Ease.InOutCirc;
            case ReplicaEase.InElastic: return Ease.InElastic;
            case ReplicaEase.OutElastic: return Ease.OutElastic;
            case ReplicaEase.InOutElastic: return Ease.InOutElastic;
            case ReplicaEase.InBack: return Ease.InBack;
            case ReplicaEase.OutBack: return Ease.OutBack;
            case ReplicaEase.InOutBack: return Ease.InOutBack;
            case ReplicaEase.InBounce: return Ease.InBounce;
            case ReplicaEase.OutBounce: return Ease.OutBounce;
            case ReplicaEase.InOutBounce: return Ease.InOutBounce;
            default: return Ease.Linear;
        }
    }

    public static EaseMode ToXTween(ReplicaEase ease)
    {
        switch (ease)
        {
            case ReplicaEase.Linear: return EaseMode.Linear;
            case ReplicaEase.InSine: return EaseMode.InSine;
            case ReplicaEase.OutSine: return EaseMode.OutSine;
            case ReplicaEase.InOutSine: return EaseMode.InOutSine;
            case ReplicaEase.InQuad: return EaseMode.InQuad;
            case ReplicaEase.OutQuad: return EaseMode.OutQuad;
            case ReplicaEase.InOutQuad: return EaseMode.InOutQuad;
            case ReplicaEase.InCubic: return EaseMode.InCubic;
            case ReplicaEase.OutCubic: return EaseMode.OutCubic;
            case ReplicaEase.InOutCubic: return EaseMode.InOutCubic;
            case ReplicaEase.InQuart: return EaseMode.InQuart;
            case ReplicaEase.OutQuart: return EaseMode.OutQuart;
            case ReplicaEase.InOutQuart: return EaseMode.InOutQuart;
            case ReplicaEase.InQuint: return EaseMode.InQuint;
            case ReplicaEase.OutQuint: return EaseMode.OutQuint;
            case ReplicaEase.InOutQuint: return EaseMode.InOutQuint;
            case ReplicaEase.InExpo: return EaseMode.InExpo;
            case ReplicaEase.OutExpo: return EaseMode.OutExpo;
            case ReplicaEase.InOutExpo: return EaseMode.InOutExpo;
            case ReplicaEase.InCirc: return EaseMode.InCirc;
            case ReplicaEase.OutCirc: return EaseMode.OutCirc;
            case ReplicaEase.InOutCirc: return EaseMode.InOutCirc;
            case ReplicaEase.InElastic: return EaseMode.InElastic;
            case ReplicaEase.OutElastic: return EaseMode.OutElastic;
            case ReplicaEase.InOutElastic: return EaseMode.InOutElastic;
            case ReplicaEase.InBack: return EaseMode.InBack;
            case ReplicaEase.OutBack: return EaseMode.OutBack;
            case ReplicaEase.InOutBack: return EaseMode.InOutBack;
            case ReplicaEase.InBounce: return EaseMode.InBounce;
            case ReplicaEase.OutBounce: return EaseMode.OutBounce;
            case ReplicaEase.InOutBounce: return EaseMode.InOutBounce;
            default: return EaseMode.Linear;
        }
    }
}
