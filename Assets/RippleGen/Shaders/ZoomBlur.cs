using UnityEngine;
using System.Collections;
using System;

public class ZoomBlur {
	private Material material;
	public Material Material{
		get { 
			compileMaterial ();
			return material;
		}
	}
	public float Strength{ get; set; }
	public Texture2D Texture{ get; set;}

	public ZoomBlur() {
		material = new Material (Shader.Find("Custom/ZoomBlur"));
	}

	void compileMaterial() {
		if (this.Texture == null || this.Texture == null) {
			return;
		} else {
			material.SetTexture ("_Tex1", Texture);
			material.SetVector ("center", new Vector4(Texture.width/2, Texture.height/2));
			material.SetFloat ("strength", Strength);
			material.SetVector ("texSize", new Vector4(Texture.width, Texture.height));
		}
	}
}