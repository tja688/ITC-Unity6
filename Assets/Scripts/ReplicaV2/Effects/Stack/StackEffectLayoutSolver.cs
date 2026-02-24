using System.Collections.Generic;
using UnityEngine;

public static class StackEffectLayoutSolver
{
    public static StackLayoutState[] Solve(int count, StackEffectConfig config, IReadOnlyList<float> randomOffsets)
    {
        if (count <= 0)
        {
            return new StackLayoutState[0];
        }

        var states = new StackLayoutState[count];
        for (var i = 0; i < count; i++)
        {
            var depth = count - i - 1;
            var random = (randomOffsets != null && i < randomOffsets.Count) ? randomOffsets[i] : 0f;
            var rotation = (depth * config.RotationStep) + random;
            var scale = 1f + (i * config.ScaleStep) - (count * config.ScaleStep);
            scale = Mathf.Clamp(scale, 0.72f, 1.08f);

            states[i] = new StackLayoutState(Vector2.zero, rotation, scale, i);
        }

        return states;
    }
}
