using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CollectionItemBar : ItemPrefabObj {

	string cmid;
	float waitT;
	bool isPressed = false;
	public Text NameTxt;
	public Text SizeTxt;
	public RawImage IconImg;
	string museum;
	string guid;
	Vector3 lastMousePosition;
	// Use this for initialization
	public override void Init (JSONNode jn) {
		NameTxt.text = jn["name"];
		cmid = jn["id"];
		SizeTxt.text = cmid + "MB";
		museum = jn["museum"];
		StartCoroutine(LoadImg(jn["guid"]));
	}
	void Update(){
		if(isPressed && RyanUIController.curM_xOffset == 0){
			waitT += Time.deltaTime;
			if(waitT > 1f){
				OnPressedSucceed();
			}else if(Input.GetMouseButtonUp(0)){
				// click按下，非长按操作
				if(Input.mousePosition == lastMousePosition){
					OnClickMuseum();
				}
				OnCancelPressedMode();
			}
		}
	}

	IEnumerator LoadImg(string guid){
        string tmpUrl = string.Format("http://www.3d360kk.com/upload/halls/{0}/panorama.jpg", guid);
		WWW www = new WWW(tmpUrl);
		yield return www;
		IconImg.texture = www.texture;
	}

	void OnClickMuseum(){
		LoadLevel(museum);
	}
	
	void LoadLevel(string str)
	{
		RyanGlobalProps.SetLoadURL(str);
		SceneManager.LoadScene("VRMain");
	}

	void OnPressedSucceed(){
		PopMailPage();
		OnCancelPressedMode();
	}

	void PopMailPage(){
		Debug.Log("Press Succeed.");
		UnityAction tmpAction = null;
		tmpAction += RemoveItem;
        RyanUIController.Instance.PopOptionDialogue("提示", "取消收藏？", tmpAction);
	}

	void RemoveItem(){
		StartCoroutine(DeleteLetter());
	}
	//删除站内信
	IEnumerator DeleteLetter(){
		string url = string.Format("http://www.3d360kk.com/mobile/revoke_collection?id={0}", cmid);
		Debug.Log(url);
		WWW www = new WWW(url);
		yield return www;
		if(JSON.Parse(www.text)["flag"].AsBool){
			Debug.Log("delete letter..");
			Destroy(gameObject);
		}
	}
	public override void OnPointerDown(PointerEventData eventData)
    {
		waitT = 0;
		isPressed = true;
		lastMousePosition = Input.mousePosition;
    }
	void OnCancelPressedMode () {
		isPressed = false;
	}
}
