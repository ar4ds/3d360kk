// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace RippleGen.Utils
{
    public class CameraController : RippleGen.Core.Plugin
    {
        public Transform Tran = null;
        float deltaX, deltaY;
        float distance;
        public float MaxY = Mathf.PI / 2;
        public float MinY = - Mathf.PI / 2;
        Vector3 source, attention;
        bool willUpdate;

        public float DeltaX{
            get {
                return deltaX;
            }
            set {
                if (deltaX != value) {
                    deltaX = value;
                    willUpdate = true;
                }
            }
        }

        public float DeltaY {
            get {
                return deltaY;
            }
            set {
                if (value >= MaxY ||
                    value <= MinY)
                    return;
                if (deltaY != value) {
                    deltaY = value;
                    willUpdate = true;
                }
            }
        }

        public float Distance {
            set {
                if (distance != value) {
                    distance = value;
                    willUpdate = true;
                }
            }
            get {
                return distance;
            }
        }
        public Vector3 Source {
            get {
                return source;
            }
            set {
                if (source != value) {
                    source = value;
                    willUpdate = true;
                }
            }
        }
        public Vector3 Attention {
            get {
                return attention;
            }
            set {
                if (attention != value) {
                    Debug.Log("Set Attention!");
                    attention = value;
                    willUpdate = true;
                }
            }
        }

        public CameraController(Transform tran) : base() {
            Tran = tran;
        }

        public override void Update()
        {
            base.Update();

            if (Tran != null && willUpdate) {
                float lY = Mathf.Sin(deltaY) * distance;
                float lX = Mathf.Cos(deltaY) * distance;
                Vector3 pos = new Vector3(Mathf.Cos(deltaX) * lX, lY, Mathf.Sin(deltaX) * lX) + source;
                Tran.position = pos;
                Tran.LookAt(Attention);
                willUpdate = false;
            }
        }
    }
}

