using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public static class MathHelper {

        public static float ToRadians(float value) {
            return (MathF.PI / 180.0f) * value;
        }

        public static bool RangeTest(int a, int b, int range) {
            return Math.Min(Math.Abs(a - b), Math.Abs(Math.Min(a, b) + (256 - Math.Max(a, b)))) <= range;
        }

        public static int FloorMod(int a, int b) {
            return (Math.Abs(a * b) + a) % b;
        }
    }
}
