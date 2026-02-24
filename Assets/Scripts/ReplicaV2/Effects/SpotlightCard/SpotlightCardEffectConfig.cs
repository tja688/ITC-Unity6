using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Spotlight Card Effect Config", fileName = "SpotlightCardEffectConfig")]
public sealed class SpotlightCardEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 CardSize = new Vector2(900f, 500f);
    public Vector2 SpotlightSize = new Vector2(700f, 700f);
    public Vector2 CardOffset = new Vector2(0f, -24f);
    public float InnerInset = 2f;

    [Header("Motion")]
    public float SpotlightFollowSpeed = 6f;
    public float SpotlightAlphaFollowSpeed = 4f;
    public float BorderColorFollowSpeed = 4f;
    [Range(0f, 1f)] public float HoverSpotlightAlpha = 0.6f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.07f, 0.13f, 0.92f);
    public Color CardColor = new Color(0.07f, 0.09f, 0.16f, 0.96f);
    public Color BorderIdleColor = new Color(0.22f, 0.28f, 0.38f, 0.70f);
    public Color BorderHoverColor = new Color(0.44f, 0.86f, 1f, 0.72f);
    public Color InnerColor = new Color(0.05f, 0.07f, 0.13f, 0.98f);
    public Color SpotlightColor = new Color(0.48f, 0.84f, 1f, 0.5f);
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.95f);
    public Color TitleColor = new Color(0.94f, 0.97f, 1f, 0.98f);
    public Color BodyColor = new Color(0.84f, 0.90f, 1f, 0.72f);

    [Header("Animation")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;
}

