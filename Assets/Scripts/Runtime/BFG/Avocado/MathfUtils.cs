namespace BFG.Avocado {
internal static class MathfUtils {
    public static float RecalculateMovement(float from, float to, float vel) {
        if (to > 0) {
            from += vel;
            if (from > to) {
                from = to;
            }
        }
        else if (to < 0) {
            from -= vel;
            if (from < to) {
                from = to;
            }
        }
        else {
            if (from > 0) {
                if (from - vel < 0) {
                    from = 0;
                }
                else {
                    from -= vel;
                }
            }
            else if (from < 0) {
                if (from + vel > 0) {
                    from = 0;
                }
                else {
                    from += vel;
                }
            }
        }

        return from;
    }
}
}
