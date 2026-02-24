using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Dock Magnify Toolbar Effect Config", fileName = "DockMagnifyToolbarEffectConfig")]
public sealed class DockMagnifyToolbarEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 PanelSize = new Vector2(760f, 96f);
    public float PanelBottomOffset = 82f;
    public float ItemGap = 16f;
    public float ItemBaseBottomPadding = 12f;

    [Header("Magnify")]
    public float BaseItemSize = 74f;
    public float MagnifiedItemSize = 114f;
    public float InfluenceDistance = 260f;
    public float SpringSpeed = 12f;
    public float FontSizeMin = 32f;
    public float FontSizeMax = 44f;

    [Header("Panel Height")]
    public float PanelHeightClosed = 96f;
    public float PanelHeightOpen = 136f;
    public float PanelHeightLerpSpeed = 8f;

    [Header("Labels")]
    public float LabelFadeSpeed = 8f;
    public Vector2 LabelSize = new Vector2(128f, 34f);
    public float LabelOffsetMin = 8f;
    public float LabelOffsetMax = 20f;

    [Header("Click Feedback")]
    public Vector3 ClickPunchScale = new Vector3(0.12f, 0.12f, 0.12f);
    public float ClickPunchDuration = 0.22f;
    public int ClickPunchVibrato = 1;
    public float ClickPunchElasticity = 0.4f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.07f, 0.13f, 0.88f);
    public Color PlateAColor = new Color(0.24f, 0.40f, 0.74f, 0.24f);
    public Color PlateBColor = new Color(0.58f, 0.29f, 0.72f, 0.20f);
    public Color PanelColor = new Color(0.04f, 0.06f, 0.12f, 0.95f);
    public Color PanelBorderColor = new Color(1f, 1f, 1f, 0.28f);
    public Color PanelShadowColor = new Color(0f, 0f, 0f, 0.36f);
    public Color ItemOutlineColor = new Color(1f, 1f, 1f, 0.24f);
    public Color LabelRootColor = new Color(0.03f, 0.05f, 0.10f, 0.95f);
    public Color LabelBorderColor = new Color(1f, 1f, 1f, 0.18f);
    public Color LabelTextColor = new Color(0.90f, 0.95f, 1f, 1f);
}
