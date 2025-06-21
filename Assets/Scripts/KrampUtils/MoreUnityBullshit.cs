using UnityEngine;

namespace KrampUtils {
    public static class MoreUnityBullshit {
        public static bool IsLayerMasked(this LayerMask mask, int layer) => ((1 << layer) & mask.value) != 0;
    }
}