using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MuseumItem : MonoBehaviour {

	public Button button;
	public Text text;
	public RawImage image;
	public string museumName;
	public string url;
	// Use this for initialization
	public void Init (string name, string picUrl, string url) {
		museumName = name;
		transform.name = name;
		text.text = name;
		this.url = url;
		if(picUrl != null){
			StartCoroutine(SetPicUrl(picUrl));
		}
		button.onClick.AddListener(OnClickMuseum);
	}

	IEnumerator SetPicUrl(string url){
		WWW www = new WWW(url);
		yield return www;
		image.texture = www.texture;
	}

	void OnClickMuseum(){
        RyanUIController.Cur_PageIndex = RyanUIController.PageList.Count -1;
		LoadLevel(url);
	}
	
	void LoadLevel(string str)
	{
		RyanGlobalProps.SetLoadURL(str);
		SceneManager.LoadScene("VRMain");
	}
}
