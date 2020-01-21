using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MuseumItem : MonoBehaviour {

	public Button button;
	public Text text;
	public RawImage image;
	public MuseumJson jsonObj;
	// Use this for initialization
	public void Init (MuseumJson jsonObj) {
		this.jsonObj = jsonObj;
		transform.name =text.text= jsonObj.name;
		string pictureUrl = string.Format("http://www.3d360kk.com/upload/halls/{0}/panorama.jpg", jsonObj.guid);

		if(!string.IsNullOrEmpty(pictureUrl)){
			StartCoroutine(SetPicUrl(pictureUrl));
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
		RyanGlobalProps.SetLoadURL(jsonObj.museum, jsonObj.id);
		Debug.LogError("##"  + jsonObj.id);
		SceneManager.LoadScene("VRMain");
	}
}
