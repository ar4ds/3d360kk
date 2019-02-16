using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using RippleGen;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System.Collections.Generic;

public class RyanShowLoadOptions : MonoBehaviour
{
    public RyanDialogBox MsgDialogBoxPrefab;
	static RyanShowLoadOptions _instance;
	public static RyanShowLoadOptions Instance{
		get{return _instance;}
	}
	public Button MainBtn, VideoBtn, View720Btn, TravelBtn, CollectionBtn;
	public Transform MainCanvas;
	bool isVideoExit = false;
	bool isView720Exit = false;
	bool isTravelExit = false;
	GameObject curMessageBox;
	public List<RectTransform> Titles = new List<RectTransform>();

	void Awake()
	{
		_instance = this;
		SceneManager.sceneLoaded += InitScene;
		InitListener();
		foreach(RectTransform rt in Titles){
			rt.sizeDelta = new Vector2(0, 100+RyanGlobalProps.StatusBarHeight);
		}
	}

	void InitListener(){
		MainBtn.onClick.AddListener(Btn_BackToMain);
		CollectionBtn.onClick.AddListener(OnClickCollectBtn);
		VideoBtn.onClick.AddListener(Btn_VideoScene);
		View720Btn.onClick.AddListener(Btn_View720Scene);
		TravelBtn.onClick.AddListener(Btn_TravelScene);
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= InitScene;
	}

	void InitScene(Scene arg0, LoadSceneMode arg1)
	{
		RyanGlobalProps.CurrentScene = RyanSourceMgr.Type.bg.ToString();
		isVideoExit = Request.Exist(RyanGlobalProps.VIDEO_URL);
		isView720Exit = Request.Exist(RyanGlobalProps.VIEW720Index_URL);
		isTravelExit = Request.Exist(RyanGlobalProps.TRAVEL_URL);
	}
    // public void PopMessageDialogue(string title, string content){
    //     RyanDialogBox tmpDialogue = Instantiate(MsgDialogBoxPrefab, MainCanvas);
	// 	tmpDialogue.OkButton.onClick.AddListener(OnDialogBoxClick);
    //     tmpDialogue.Init(title, content);
    //     curMessageBox = tmpDialogue.gameObject;
    // }
    public void PopMessageDialogue(string title, string content, UnityEngine.Events.UnityAction action = null){
        RyanDialogBox tmpDialogue = Instantiate(MsgDialogBoxPrefab, MainCanvas);
		action += ()=>{Destroy(curMessageBox);};
		tmpDialogue.OkButton.onClick.AddListener(action);
        tmpDialogue.Init(title, content);
        curMessageBox = tmpDialogue.gameObject;
    }
	
	void OnDialogBoxClick(){
		Destroy(curMessageBox);
	}

	void OnClickCollectBtn()
	{
        // 用户已登陆
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("UserName")))
        {
			CollectScene();
        }
        // 用户未登录
        else
        {
			Debug.Log("Not Login");
			MiniLoginDialogBoxMgr.Instance.ShowPage();
        }
	}

	public void CollectScene(){
		StartCoroutine(_CollectScene());
	}

	IEnumerator _CollectScene(){
		string guid = PlayerPrefs.GetString("GUID");
		string url2 = string.Format("http://www.3d360kk.com/mobile/query_museum?token={0}", guid);
		WWW www2 = new WWW(url2);
		yield return www2;
		List<JSONNode> jsList = new List<JSONNode>(){};
		jsList.AddRange(JSON.Parse(www2.text)["museums"].Children);
		bool already = false;
		for(int i = 0; i < jsList.Count; i++){
			if(jsList[i]["museum"]==RyanGlobalProps.CurrentMuseumName){
				already = true;
				PopMessageDialogue("提示","重复收藏");
				break;
			}
		}
		if(!already){
			string url = string.Format("http://www.3d360kk.com/mobile/save_museum?token={0}&museum={1}",
							guid, RyanGlobalProps.CurrentMuseumName);
			Debug.Log(url);
			WWW www = new WWW(url);
			yield return www;
			if(JSON.Parse(www.text)["flag"].AsBool){
				PopMessageDialogue("提示","收藏成功");
			}else{
				PopMessageDialogue("提示","收藏失败");
			}
		}
	}

	public void Btn_BackToMain()
	{
		RyanGlobalProps.CurrentScene = RyanSourceMgr.Type.bg.ToString();
		SceneManager.LoadScene(RyanGlobalProps.mainSceneName);
	}

	public void Btn_VideoScene()
	{
		RyanGlobalProps.CurrentScene = RyanSourceMgr.Type.movie.ToString();
		SceneManager.LoadScene("Scene_Video");
	}

	public void Btn_TravelScene()
	{
		RyanGlobalProps.CurrentScene = RyanSourceMgr.Type.wandar.ToString();
		SceneManager.LoadScene("Scene_Travel");
	}

	public void Btn_View720Scene()
	{
		RyanGlobalProps.CurrentScene = RyanSourceMgr.Type.view720.ToString();
		SceneManager.LoadScene("Scene_720View");
	}
}
