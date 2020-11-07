using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask layerMask, LayerMask layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
