using System;
using UnityEngine;

namespace RippleGen.Extends
{
    public static class RectExtends
    {
        public static Rect Inset(this Rect rect, float x, float y)
        {
            return new Rect(rect.x - x, rect.y - y, rect.width + 2*x, rect.height + 2*y);
        }
        public static Rect Inset(this Rect rect, Vector2 vec)
        {
            return rect.Inset(vec.x, vec.y);
        }

        public static Rect Multiply(this Rect rect, float number) {
            return Multiply(rect, number, number);
        }

        public static Rect Multiply(this Rect rect, float x, float y) {
            return new Rect(rect.x * x, rect.y * y, rect.width * x, rect.height * y);
        }


    }
}

