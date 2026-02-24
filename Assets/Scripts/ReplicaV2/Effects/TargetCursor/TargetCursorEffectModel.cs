using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class TargetCursorEffectModel
{
    public List<RectTransform> HoverTargets = new List<RectTransform>();

    public TargetCursorEffectModel Clone() => new TargetCursorEffectModel
    {
        HoverTargets = HoverTargets != null ? new List<RectTransform>(HoverTargets) : new List<RectTransform>()
    };

    public static TargetCursorEffectModel CreateDefault() => new TargetCursorEffectModel();
}
