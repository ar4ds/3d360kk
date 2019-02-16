using System;

namespace RippleGen.Utils
{
    public static class DefaultScreen
    {
        public static float width = Config.Instance["defaultWidth"].AsFloat;
        public static float height = Config.Instance["defaultHeight"].AsFloat;
    }
}

