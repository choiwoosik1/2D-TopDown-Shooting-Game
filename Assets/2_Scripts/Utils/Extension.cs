using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return Utils.Contains(layerMask, layer);
    }
}
