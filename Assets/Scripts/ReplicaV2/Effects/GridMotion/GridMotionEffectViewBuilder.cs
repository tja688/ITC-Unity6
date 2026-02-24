using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GridMotionEffectTileView
{
    public RectTransform Root;
    public Image TileImage;
    public Image SpriteImage;
    public Text Text;
}

public sealed class GridMotionEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;

    public RectTransform Backdrop;
    public Image BackdropImage;

    public RectTransform Container;
    public RectTransform RowsRoot;

    public List<RectTransform> RowContents = new List<RectTransform>();
    public List<GridLayoutGroup> RowGridLayouts = new List<GridLayoutGroup>();
    public List<GridMotionEffectTileView> Tiles = new List<GridMotionEffectTileView>();
}

public static class GridMotionEffectViewBuilder
{
    private static Sprite sRadialSprite;

    public static GridMotionEffectView Build(RectTransform mountRoot, GridMotionEffectConfig config)
    {
        var view = new GridMotionEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("GridMotionEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Root.gameObject);

        view.Backdrop = ReplicaUIFactoryV2.CreateRect("Backdrop", view.Root);
        ReplicaUIFactoryV2.Stretch(view.Backdrop);
        view.BackdropImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Backdrop.gameObject);
        view.BackdropImage.sprite = GetOrCreateRadialSprite();
        view.BackdropImage.type = Image.Type.Simple;
        view.BackdropImage.preserveAspect = false;
        view.BackdropImage.color = config != null ? config.GradientColor : Color.black;
        view.BackdropImage.raycastTarget = false;

        view.Container = ReplicaUIFactoryV2.CreateRect("Container", view.Root);
        ReplicaUIFactoryV2.Stretch(view.Container);
        view.Container.localRotation = Quaternion.Euler(0f, 0f, config != null ? config.ContainerRotationDeg : -15f);
        var scale = config != null ? Mathf.Max(0.01f, config.ContainerScale) : 1.5f;
        view.Container.localScale = new Vector3(scale, scale, 1f);

        view.RowsRoot = ReplicaUIFactoryV2.CreateRect("RowsRoot", view.Container);
        ReplicaUIFactoryV2.Stretch(view.RowsRoot);

        var vLayout = ReplicaUIFactoryV2.EnsureComponent<VerticalLayoutGroup>(view.RowsRoot.gameObject);
        vLayout.spacing = config != null ? Mathf.Max(0f, config.Gap) : 16f;
        vLayout.padding = new RectOffset(0, 0, 0, 0);
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;
        vLayout.childForceExpandWidth = true;
        vLayout.childForceExpandHeight = true;

        view.RowContents.Clear();
        view.RowGridLayouts.Clear();
        view.Tiles.Clear();

        var rows = config != null ? Mathf.Max(1, config.RowCount) : 4;
        var cols = config != null ? Mathf.Max(1, config.ColumnCount) : 7;
        var gap = config != null ? Mathf.Max(0f, config.Gap) : 16f;

        for (var row = 0; row < rows; row++)
        {
            var rowSlot = ReplicaUIFactoryV2.CreateRect($"RowSlot_{row}", view.RowsRoot);
            var slotLayout = ReplicaUIFactoryV2.EnsureComponent<LayoutElement>(rowSlot.gameObject);
            slotLayout.flexibleWidth = 1f;
            slotLayout.flexibleHeight = 1f;

            var rowContent = ReplicaUIFactoryV2.CreateRect($"Row_{row}", rowSlot);
            ReplicaUIFactoryV2.Stretch(rowContent);
            rowContent.pivot = new Vector2(0.5f, 0.5f);
            rowContent.anchoredPosition = Vector2.zero;

            var grid = ReplicaUIFactoryV2.EnsureComponent<GridLayoutGroup>(rowContent.gameObject);
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = cols;
            grid.spacing = new Vector2(gap, gap);
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.cellSize = new Vector2(100f, 100f);

            view.RowContents.Add(rowContent);
            view.RowGridLayouts.Add(grid);

            for (var col = 0; col < cols; col++)
            {
                var tile = new GridMotionEffectTileView();

                tile.Root = ReplicaUIFactoryV2.CreateRect($"Tile_{row}_{col}", rowContent);
                tile.TileImage = ReplicaUIFactoryV2.EnsureComponent<Image>(tile.Root.gameObject);
                tile.TileImage.color = config != null ? config.TileColor : new Color(0.067f, 0.067f, 0.067f, 1f);
                tile.TileImage.raycastTarget = false;

                var spriteRect = ReplicaUIFactoryV2.CreateRect("Sprite", tile.Root);
                ReplicaUIFactoryV2.Stretch(spriteRect);
                tile.SpriteImage = ReplicaUIFactoryV2.EnsureComponent<Image>(spriteRect.gameObject);
                tile.SpriteImage.color = Color.white;
                tile.SpriteImage.preserveAspect = true;
                tile.SpriteImage.raycastTarget = false;

                tile.Text = ReplicaUIFactoryV2.CreateText(
                    "Text",
                    tile.Root,
                    "",
                    config != null ? config.FontSize : 28,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    config != null ? config.TextColor : Color.white);
                ReplicaUIFactoryV2.Stretch((RectTransform)tile.Text.transform);

                view.Tiles.Add(tile);
            }
        }

        return view;
    }

    private static Sprite GetOrCreateRadialSprite()
    {
        if (sRadialSprite != null)
        {
            return sRadialSprite;
        }

        const int size = 256;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "ReplicaRadialSprite_GridMotion_V2"
        };

        var center = (size - 1) * 0.5f;
        var maxDistance = center;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var distance = Mathf.Sqrt((dx * dx) + (dy * dy)) / maxDistance;
                var alpha = Mathf.Clamp01(1f - distance);
                alpha = alpha * alpha * (3f - 2f * alpha);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sRadialSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sRadialSprite;
    }
}
