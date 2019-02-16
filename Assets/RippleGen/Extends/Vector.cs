using System;
using UnityEngine;

namespace RippleGen.Extends
{
    public static class Vector
    {
        public static Vector2 GUITo3D(this Vector2 vec) {
            return new Vector2(vec.x, Screen.height - vec.y);
        }
        public static Vector3 GUITo3D(this Vector3 vec) {
            return new Vector3(vec.x, Screen.height - vec.y, vec.z);
        }
        public static Vector2 GUITo2D(this Vector3 vec) {
            return new Vector2(vec.x, vec.y);
        }


        public static Vector2 NGUITo3D(this Vector2 vec) {
            return new Vector2(vec.x - Screen.width / 2, vec.y - Screen.height / 2);
        }
        public static Vector2 NGUITo2D(this Vector2 vec) {
            return new Vector2(vec.x + Screen.width / 2, vec.y + Screen.height / 2);
        }
    }
}

