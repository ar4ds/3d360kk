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
using System.Collections.Generic;
using UnityEngine;

namespace RippleGen.Behaviour
{
    public class LineManager : MonoBehaviour
    {
        public List<Vector3> Points = new List<Vector3>();
        public Color Color = Color.white;
        static Material lineMaterial;

        static void CreateLineMaterial() { 
            if( !lineMaterial ) { 
                lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + " Blend SrcAlpha OneMinusSrcAlpha " + " ZWrite Off Cull Off Fog { Mode Off } " + " BindChannels {" + " Bind \"vertex\", vertex Bind \"color\", color }" + "} } }" ); 
                lineMaterial.hideFlags = HideFlags.HideAndDontSave; 
                lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave; 
            } 
        }
        
        void OnPostRender() { 
            if (Points.Count == 0) 
                return;
            CreateLineMaterial();       // set the current material x
            lineMaterial.SetPass( 0 ); 
            GL.InvalidateState();
            GL.Begin( GL.LINES ); 
            GL.Color(Color); 
            for(int n = 0, t = Points.Count - 1; n < t; n++) {
                GL.Vertex(Points[n]);
                GL.Vertex(Points[n + 1]);
            }
            GL.End(); 
        } 
    }
}
