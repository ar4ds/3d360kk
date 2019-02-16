using System;

namespace RippleGen.Utils
{
    public class BonderException : Exception {

    }

    public class Curve
    {
        public double Y(float x) {
            if (x > 1 || x < 0)
            {
                throw new BonderException();
            }
            return progress(x);
        }
        public float middle(float max, float min, float x) {
            return (max - min) * x + min;
        } 

        protected virtual float progress(float x) {
            return x;
        }
    }

    public class Parabola  : Curve
    {
        public float A = 0;
        public float B = 0;
        public float Power = 2;

        public Parabola(float a, float b, float power) {
            A = a;
            B = b;
            Power = power;
        }
        public Parabola(): this(0,0,2) {}

        protected override float progress(float x)
        {
            return (float)(Math.Pow(x + A, Power) - B);
        }
    }
}

