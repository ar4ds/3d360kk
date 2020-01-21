using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using SimpleJSON;

public class GongGaoBarItem : ItemPrefabObj {
	public Text TitleTxt, MuseumTxt, TimeTxt;
	Image barImage;
	RectTransform rectTran;
	float waitT;
	bool isPressed = false;
	string content;
	string id;
	Vector3 lastMousePosition;

	void Update () {
		if(isPressed && RyanUIController.curM_xOffset == 0){
			waitT += Time.deltaTime;
			if(waitT > 1f){
				OnPressedSucceed();
			}else if(Input.GetMouseButtonUp(0)){
				// click按下，非长按操作
				if(Input.mousePosition == lastMousePosition){
					OnBarClick();
				}
				OnCancelPressedMode();
			}
		}
	}
	public override void Init(JSONNode jn){
		barImage = transform.GetComponent<Image>();
		this.id = jn["id"];
		this.content = jn["Text"];
		TitleTxt.text = jn["Title"];
		MuseumTxt.text = jn["museum"];
		System.DateTime tmpD = System.DateTime.Parse(jn["Date"]);
		TimeTxt.text = tmpD.ToString("yyyy-MM-dd");
		//判断是否已读
		if(PlayerPrefs.GetString(string.Format("gonggao:{0}", id)) != ""){
			SetReadedState();
		}
	}
	public override void OnPointerDown(PointerEventData eventData)
    {
		//RyanUIController.curM_xOffset = 0;
		waitT = 0;
		isPressed = true;
		barImage.color = new Color(1f, 1f, 1f, .5f);
		lastMousePosition = Input.mousePosition;
    }
	void OnBarClick(){
		Debug.Log("OnBarClick");
		// 弹出站内信
		RyanUIController.Instance.PopGongGaoContentPage(TitleTxt.text, content);
		PlayerPrefs.SetString(string.Format(string.Format("gonggao:{0}", id)), "readed");
		SetReadedState();
	}
	void OnCancelPressedMode () {
		isPressed = false;
		barImage.color = new Color(1f, 1f, 1f, 0f);
	}

	void SetReadedState(){
		TimeTxt.color = TitleTxt.color = Color.grey * .25f;
        GongGaoMgr.Instance.UpdateGongGaoNumberTag();
	}

	void OnPressedSucceed(){
		PopDeleteDialog();
		OnCancelPressedMode();
	}

	void PopDeleteDialog(){
		Debug.Log("Press Succeed.");
		return;
		UnityAction tmpAction = null;
		if(!string.IsNullOrEmpty(PlayerPrefs.GetString("token"))){
			tmpAction += RemoveItem;
		}else{
			tmpAction += RemoveItemLocal;
		}
        RyanUIController.Instance.PopOptionDialogue("提示", "确认删除？", tmpAction);
	}

	void RemoveItem(){
		StartCoroutine(DeleteLetter());
	}
	void RemoveItemLocal(){
		Destroy(gameObject);
        GongGaoMgr.Instance.UpdateGongGaoNumberTag();
		PlayerPrefs.SetString("gonggaodelete"+id, id);
	}
	//删除站内信
	IEnumerator DeleteLetter(){
		string url = string.Format("http://www.3d360kk.com/mobile/RemoveLetter?mid={0}&uid={1}", id, PlayerPrefs.GetString("token"));
		print(url);
		WWW www = new WWW(url);
		yield return www;
		if(JSON.Parse(www.text)["flag"].AsBool){
			Debug.Log("delete letter..");
        	GongGaoMgr.Instance.UpdateGongGaoNumberTag();
			Destroy(gameObject);
		}
	}
}