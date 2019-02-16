using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RyanBeaconPicItem : MonoBehaviour {
	public RawImage Texture;
	// Use this for initialization
	void Start () {
		transform.SetSiblingIndex(1);
	}
	
	public void Init(string name){
		string url = string.Format("http://www.3d360kk.com/upload/beacons/{0}", name);
		StartCoroutine(LoadTexture(url));
	}
	// Update is called once per frame
	IEnumerator LoadTexture (string url) {
		WWW www = new WWW(url);
		yield return www;
		Texture2D tex2d = www.texture;
		//屏幕比
		float screenR = (float)Screen.width/Screen.height;
		//图片比
		float texR = (float)tex2d.width/tex2d.height;
		if(screenR > texR){
			// 图片比屏幕更长
			Texture.rectTransform.sizeDelta = new Vector2(Screen.height * texR, Screen.height);
		}else{
			// 图片比屏幕更宽
			Texture.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.width / texR);
		}
		Texture.texture = tex2d;
	}
}
