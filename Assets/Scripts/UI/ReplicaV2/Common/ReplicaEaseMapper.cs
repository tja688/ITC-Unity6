using DG.Tweening;

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
}
