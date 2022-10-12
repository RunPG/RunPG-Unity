using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingFunc
{
    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
            return n1 * t * t;
        else if (t < 2f / d1)
            return n1 * (t - 1.5f / d1) * (t - 1.5f / d1) + 0.75f;
        else if (t < 2.5f / d1)
            return n1 * (t - 2.25f / d1) * (t - 2.25f / d1) + 0.9375f;
        else
            return 1;
    }

    public static float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }
}
