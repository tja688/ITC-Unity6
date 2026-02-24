using System;

[Serializable]
public enum RotatingTextSplitBy
{
    Characters,
    Words,
    Lines
}

[Serializable]
public enum RotatingTextStaggerFrom
{
    First,
    Last,
    Center,
    Random
}

[Serializable]
public sealed class RotatingTextEffectModel
{
    public string[] Texts = { "Fast", "Clean", "Reusable" };
    public int CurrentIndex;
    public float RotationInterval = 2f;
    public float StaggerDuration;
    public RotatingTextStaggerFrom StaggerFrom = RotatingTextStaggerFrom.First;
    public bool Loop = true;
    public bool Auto = true;
    public RotatingTextSplitBy SplitBy = RotatingTextSplitBy.Characters;

    public static RotatingTextEffectModel CreateDefault()
    {
        return new RotatingTextEffectModel
        {
            Texts = new[] { "Fast", "Clean", "Reusable" },
            CurrentIndex = 0,
            RotationInterval = 2f,
            StaggerDuration = 0.03f,
            StaggerFrom = RotatingTextStaggerFrom.First,
            Loop = true,
            Auto = true,
            SplitBy = RotatingTextSplitBy.Characters
        };
    }

    public RotatingTextEffectModel Clone()
    {
        return new RotatingTextEffectModel
        {
            Texts = Texts != null ? (string[])Texts.Clone() : null,
            CurrentIndex = CurrentIndex,
            RotationInterval = RotationInterval,
            StaggerDuration = StaggerDuration,
            StaggerFrom = StaggerFrom,
            Loop = Loop,
            Auto = Auto,
            SplitBy = SplitBy
        };
    }
}
