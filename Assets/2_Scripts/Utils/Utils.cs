using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool Contains(LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask.value ) != 0;
    }

    public const float Epsilon = 0.01f;
}
