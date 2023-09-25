using UnityEngine;

namespace BFG.Avocado {
internal static class MathfUtils {
    public static float RecalculateMovement(float from, float to, float vel) {
        var dx = to - from;
        if (Mathf.Abs(dx) <= vel) {
            return to;
        }

        if (dx > 0) {
            return from + vel;
        }

        return from - vel;
    }
}
}
