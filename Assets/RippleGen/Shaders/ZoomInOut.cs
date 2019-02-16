using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoomInOut {

	private Material material;
	public Material Material {get { return material;}}
	private float percent;
	public float Percent{ 
		get{ return percent;}
		set { 
			percent = value;
			material.SetFloat ("percent", percent);
		} 
	}
	private Texture2D textureOut;
	public Texture2D TextureOut{ 
		get{return textureOut;}
		set { 
			textureOut = value;
			material.SetTexture ("_Tex1", textureOut);
		}
	}
	private Texture2D textureIn;
	public Texture2D TextureIn{ 
		get{ return textureIn;} 
		set { 
			textureIn = value;
			material.SetTexture ("_Tex2", textureIn);
		}
	}
	private float centerX;
	public float CenterX{ 
		get{ return centerX;}
		set { centerX = value;
			material.SetVector ("_Center", new Vector4(CenterX, CenterY));
		}
	}
	private float centerY;
	public float CenterY{ 
		get{ return centerY;} 
		set{ centerY = value;
			material.SetVector ("_Center", new Vector4(CenterX, CenterY));
		}
	}

	public ZoomInOut() {
		UnityEngine.Debug.Log("Find Hidden/radialBlur " + ((Shader.Find("Hidden/" + "radialBlur") == null)?"null": "exits"));
		UnityEngine.Debug.Log("Find Custom/ZoomInOut " + ((Shader.Find("Custom/" + "ZoomInOut") == null)?"null": "exits") );
		UnityEngine.Debug.Log("Find Custom/ZoomBlur " + ((Shader.Find("Custom/ZoomBlur") == null)?"null": "exits") );
		material = new Material (Shader.Find("Custom/ZoomInOut"));
		UnityEngine.Debug.Log("2");
		CenterX = 0.5f;
		CenterY = 0.5f;
		Percent = 0f;
	}

}
