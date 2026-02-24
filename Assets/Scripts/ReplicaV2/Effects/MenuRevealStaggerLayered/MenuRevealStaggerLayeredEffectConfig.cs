using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Menu Reveal Stagger Layered Effect Config", fileName = "MenuRevealStaggerLayeredEffectConfig")]
public sealed class MenuRevealStaggerLayeredEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public float PanelWidth = 470f;
    public float PanelPaddingLeft = 38f;
    public float PanelPaddingTop = 132f;
    public float PanelPaddingRight = 36f;
    public float PanelPaddingBottom = 36f;
    public float MenuRowHeight = 72f;
    public float MenuRowStep = 74f;

    [Header("Animation")]
    public float LayerStagger = 0.04f;
    public float LayerSlideDuration = 0.35f;
    public float PanelSlideDuration = 0.42f;
    public float ItemSlideDuration = 0.55f;
    public float ItemStagger = 0.06f;
    public float SocialFadeDuration = 0.3f;
    public float SocialLinkDuration = 0.35f;
    public float IconRotateDurationOpen = 0.45f;
    public float IconRotateDurationClose = 0.25f;
    public float ToggleTextMoveDuration = 0.3f;
    public float ToggleTextMoveCloseDuration = 0.25f;
    public float CloseSlideDuration = 0.22f;

    [Header("Hidden Pose")]
    public float ItemHiddenY = -98f;
    public float ItemHiddenRotZ = -10f;
    public float SocialHiddenYOffset = 25f;

    [Header("Visual")]
    public Color RootBackground = new Color(0.05f, 0.07f, 0.14f, 0.92f);
    public Color AmbientAColor = new Color(0.15f, 0.22f, 0.40f, 0.52f);
    public Color AmbientBColor = new Color(0.08f, 0.35f, 0.56f, 0.42f);
    public Color AccentColor = new Color(0.32f, 0.15f, 1f, 1f);
    public Color PanelColor = new Color(0.96f, 0.97f, 1f, 0.98f);
    public Color ToggleClosedColor = new Color(0.93f, 0.95f, 1f, 1f);
    public Color ToggleOpenColor = new Color(0.12f, 0.14f, 0.22f, 1f);
    public Color Layer0Color = new Color(0.63f, 0.53f, 0.95f, 0.86f);
    public Color Layer1Color = new Color(0.32f, 0.15f, 1.00f, 0.72f);
    public Color Layer2Color = new Color(0.16f, 0.09f, 0.55f, 0.58f);
    public Color MenuLabelColor = new Color(0.09f, 0.10f, 0.14f, 1f);
    public Color SocialLinkColor = new Color(0.12f, 0.13f, 0.20f, 1f);
    public Color PanelShadowColor = new Color(0f, 0f, 0f, 0.28f);
}
