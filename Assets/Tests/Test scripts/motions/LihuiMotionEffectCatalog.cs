using System;

public enum LihuiDotweenEffectId
{
    VerticalNodAgree = 0,
    VerticalHappyBounce = 1,
    HorizontalNoShake = 2,
    HorizontalHappySway = 3,
    AngryTremble = 4,
    SurpriseBackLean = 5,
    EmphasisForwardPush = 6,
    EnterSlideIn = 7,
    ExitSlideOut = 8,
    ZoomInApproach = 9,
    ZoomOutLeave = 10,
    TiltConfused = 11,
    RotateYTurnBack = 12,
    RotateXFallBack = 13
}

public enum LihuiShaderEffectId
{
    Breathing = 0,
    HeartbeatPulse = 1,
    MeshWave = 2
}

[Serializable]
public sealed class LihuiDotweenEffectDefinition
{
    public LihuiDotweenEffectDefinition(LihuiDotweenEffectId id, string nodeName, string displayName)
    {
        Id = id;
        NodeName = nodeName;
        DisplayName = displayName;
    }

    public LihuiDotweenEffectId Id { get; }
    public string NodeName { get; }
    public string DisplayName { get; }
}

[Serializable]
public sealed class LihuiShaderEffectDefinition
{
    public LihuiShaderEffectDefinition(LihuiShaderEffectId id, string nodeName, string displayName)
    {
        Id = id;
        NodeName = nodeName;
        DisplayName = displayName;
    }

    public LihuiShaderEffectId Id { get; }
    public string NodeName { get; }
    public string DisplayName { get; }
}

public static class LihuiMotionEffectCatalog
{
    public const string MotionRootName = "MOTION_FX_LIBRARY";
    public const string ShaderName = "Tests/LihuiSpriteMotion";

    public static readonly LihuiDotweenEffectDefinition[] DotweenEffects =
    {
        new(LihuiDotweenEffectId.VerticalNodAgree, "MOTION_DOT_01_VerticalNod", "上下轻移-点头赞同"),
        new(LihuiDotweenEffectId.VerticalHappyBounce, "MOTION_DOT_02_VerticalHappyBounce", "上下轻移-开心弹跳"),
        new(LihuiDotweenEffectId.HorizontalNoShake, "MOTION_DOT_03_HorizontalNoShake", "左右晃动-摇头否定"),
        new(LihuiDotweenEffectId.HorizontalHappySway, "MOTION_DOT_04_HorizontalHappySway", "左右晃动-愉悦慢摇"),
        new(LihuiDotweenEffectId.AngryTremble, "MOTION_DOT_05_AngryTremble", "轻微颤抖-愤怒压抖"),
        new(LihuiDotweenEffectId.SurpriseBackLean, "MOTION_DOT_06_SurpriseBackLean", "后仰后退-震惊回缩"),
        new(LihuiDotweenEffectId.EmphasisForwardPush, "MOTION_DOT_07_EmphasisForwardPush", "前倾前进-强调台词"),
        new(LihuiDotweenEffectId.EnterSlideIn, "MOTION_DOT_08_EnterSlideIn", "入场移动-侧边滑入"),
        new(LihuiDotweenEffectId.ExitSlideOut, "MOTION_DOT_09_ExitSlideOut", "退场移动-侧边滑出"),
        new(LihuiDotweenEffectId.ZoomInApproach, "MOTION_DOT_10_ZoomInApproach", "缩放远近-靠近"),
        new(LihuiDotweenEffectId.ZoomOutLeave, "MOTION_DOT_11_ZoomOutLeave", "缩放远近-远离"),
        new(LihuiDotweenEffectId.TiltConfused, "MOTION_DOT_12_TiltConfused", "小幅倾斜-疑惑歪头"),
        new(LihuiDotweenEffectId.RotateYTurnBack, "MOTION_DOT_13_RotateYTurnBack", "三维旋转-Y轴转身"),
        new(LihuiDotweenEffectId.RotateXFallBack, "MOTION_DOT_14_RotateXFallBack", "三维旋转-X轴仰倒")
    };

    public static readonly LihuiShaderEffectDefinition[] ShaderEffects =
    {
        new(LihuiShaderEffectId.Breathing, "MOTION_MAT_01_Breathing", "网格模拟-呼吸起伏"),
        new(LihuiShaderEffectId.HeartbeatPulse, "MOTION_MAT_02_HeartbeatPulse", "网格模拟-心跳冲击"),
        new(LihuiShaderEffectId.MeshWave, "MOTION_MAT_03_MeshWave", "网格模拟-波浪扭曲")
    };
}

