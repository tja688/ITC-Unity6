using UnityEngine;
using UnityEngine.UI;

public static class UGUIReplicaUIFactory
{
    private static Font sCachedFont;

    public static Font DefaultFont
    {
        get
        {
            if (sCachedFont == null)
            {
                sCachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return sCachedFont;
        }
    }

    public static RectTransform CreateRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    public static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        var rect = CreateRect(name, parent);
        var image = EnsureComponent<Image>(rect.gameObject);
        image.color = color;
        return rect;
    }

    public static Text CreateText(
        string name,
        Transform parent,
        string content,
        int fontSize,
        FontStyle fontStyle,
        TextAnchor alignment,
        Color color)
    {
        var rect = CreateRect(name, parent);
        var text = EnsureComponent<Text>(rect.gameObject);
        text.font = DefaultFont;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    public static Button CreateButton(string name, Transform parent, Color background)
    {
        var rect = CreateRect(name, parent);
        var image = EnsureComponent<Image>(rect.gameObject);
        image.color = background;

        var button = EnsureComponent<Button>(rect.gameObject);
        var colors = button.colors;
        colors.normalColor = background;
        colors.highlightedColor = Color.Lerp(background, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(background, Color.black, 0.14f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.55f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.targetGraphic = image;

        return button;
    }

    public static T EnsureComponent<T>(GameObject go) where T : Component
    {
        if (!go.TryGetComponent(out T component))
        {
            component = go.AddComponent<T>();
        }

        return component;
    }
}
