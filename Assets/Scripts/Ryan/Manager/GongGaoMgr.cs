using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class GongGaoMgr : MonoBehaviour {
	public Transform GongGaoListParent;
	public GongGaoBarItem GongGaoBarItemPrefab;
	public GameObject GongGaoNumberTag;
	public Text GongGaoNumberTxt;
	public static GongGaoMgr Instance{
		get{return _instance;}
	}
	static GongGaoMgr _instance;
	void Awake(){
		_instance=this;
		GongGaoNumberTag.SetActive(false);
	}
	public void UpdateGongGaoList(){
		StartCoroutine(_UpdateGongGaoList());
	}
    IEnumerator _UpdateGongGaoList(){
		//重置
		ResetList();
        float tmpH = 180f;
        GongGaoListParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width, tmpH);
		string tmpGUID = PlayerPrefs.GetString("token");
		bool isGuest = false;
		if(string.IsNullOrEmpty(tmpGUID)){
			//使用游客身份登录
			tmpGUID = RyanGlobalProps.GuestGUID;
			isGuest = true;
		}
        string tmpUrl = string.Format("http://www.3d360kk.com/mobile/queryannounces?token={0}","00000000-0000-0000-0000-000000000000");
		Debug.Log(tmpUrl);
        WWW www = new WWW(tmpUrl);
        yield return www;
		if(!string.IsNullOrEmpty(www.text)){
			Debug.Log(www.text.Replace("\\\"","\"").Replace("\"[", "[").Replace("]\"", "]"));
			JSONNode jn = JSON.Parse(www.text.Replace("\\\"","\"").Replace("\"[", "[").Replace("]\"", "]"));
			Debug.Log(jn);
			if(jn["flag"].AsBool){
				List<JSONNode> jsList = new List<JSONNode>(){};
				jsList.AddRange(jn["announces"].Children);
				for(int i = 0; i < jsList.Count; i++){
					// if(isGuest){
					// 	Debug.Log("游客身份");
					// 	if(!PlayerPrefs.HasKey("gonggaodelete"+jsList[i]["id"])){
					// 		GongGaoBarItem tmpItem = Instantiate(GongGaoBarItemPrefab, GongGaoListParent);
					// 		tmpItem.Init(jsList[i]);
					// 	}
					// }else
					{
						GongGaoBarItem tmpItem = Instantiate(GongGaoBarItemPrefab, GongGaoListParent);
						print(jsList[i].ToString());
						Debug.Log(jsList[i]["Date"]);
						tmpItem.Init(jsList[i]);
					}
				}
				RectTransform contentRectTran = GongGaoListParent.GetComponent<RectTransform>();
				contentRectTran.sizeDelta = new Vector2(0, tmpH * jsList.Count);
			}
		}
    }
    // 更新公告数字标签上的数字
    public void UpdateGongGaoNumberTag(){
		return;
		StartCoroutine(_UpdateGongGaoNumberTag());
    }
	IEnumerator _UpdateGongGaoNumberTag(){
		string tmpGUID = PlayerPrefs.GetString("token");
		if(string.IsNullOrEmpty(tmpGUID)){
			//使用游客身份登录
			tmpGUID = RyanGlobalProps.GuestGUID;
		}
        string tmpUrl = string.Format("http://www.3d360kk.com/mobile/queryannounces?token={0}",tmpGUID);
        WWW www = new WWW(tmpUrl);
        yield return www;
		if(!string.IsNullOrEmpty(www.text)){
			JSONNode jn = JSON.Parse(www.text.Replace("\\\"","\"").Replace("\"[", "[").Replace("]\"", "]"));
			int tmpCount = 0;
			if(jn["flag"].AsBool){
				tmpCount = GetReadedCount(jn);
			}
			GongGaoNumberTag.SetActive(tmpCount>0);
			GongGaoNumberTxt.text = tmpCount.ToString();
		}
	}

	int GetReadedCount(JSONNode jn){
		return 0;
		string tmpGUID = PlayerPrefs.GetString("token");
		bool isGuest = false;
		if(string.IsNullOrEmpty(tmpGUID)){
			//使用游客身份登录
			isGuest = true;
		}
		int tmpC = 0;
		List<JSONNode> jsList = new List<JSONNode>(){};
		jsList.AddRange(jn["announces"].Children);
		for(int i = 0; i < jsList.Count; i++){
			if(PlayerPrefs.GetString(string.Format("gonggao:{0}", jsList[i]["id"])) == ""){
				++tmpC;
				if(isGuest && PlayerPrefs.HasKey("gonggaodelete" + jsList[i]["id"])){
					--tmpC;
				}
			}
		}
		return tmpC;
	}

	void ResetList(){
		int childCount = GongGaoListParent.childCount;
		for(int i = 0; i < childCount; i++){
			Destroy(GongGaoListParent.GetChild(i).gameObject);
		}
	}
}
