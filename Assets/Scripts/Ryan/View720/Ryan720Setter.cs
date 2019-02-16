using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using RippleGen;

public class Ryan720Setter : RippleGen.Core.MonoBehaviour
{
	public Transform SpherePrefab;
	public LoadingUI myLoadingUI;
	public Transform MapOptionPanel;
	public Texture blackTex;
	RyanScrollRectCell scrollCtrl;
	GameObject curSphere;
	AssetBundleCreateRequest request;

	GoTweenConfig _mapOptionTweenCfg;
	GoTweenChain goChain;
	GoTween _mapOptionTween;
	string lastFilePath = "";

	void Awake ()
	{
		SceneManager.sceneLoaded += InitScene;
	}

	void Start ()
	{
		scrollCtrl = MapOptionPanel.GetComponent<RyanScrollRectCell> ();
		InitMapPanelTween ();
	}

	void OnDestroy ()
	{
		SceneManager.sceneLoaded -= InitScene;
	}

	void InitScene (Scene arg0, LoadSceneMode arg1)
	{
		//RoteCube.SetActive(false);
		lastFilePath = "";
		curSphere = Instantiate (SpherePrefab).gameObject;
		curSphere.gameObject.SetActive (false);
		curSphere.transform.localScale = new Vector3 (-1f, 1f, 1f);
		DownloadTxtAndThumbnails ();
		loadedThumbnailCount = 0;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			gameObject.AddComponent<LaucherGyroCtrl> ();
		} else {
			gameObject.AddComponent<LaucherGyroCtrl> ();
			//curSphere.AddComponent<FingerCtrl720>();
		}
	}

	void LateUpdate ()
	{
		if (loadedThumbnailCount > 0) {
			if (thumbnailList != null && loadedThumbnailCount == thumbnailList.Count) {
				thumbnailList = null;
			}
		}
	}

	void InitMapPanelTween ()
	{
		goChain = new GoTweenChain ();
		_mapOptionTweenCfg = new GoTweenConfig ();
		_mapOptionTweenCfg.position (new Vector3 (MapOptionPanel.position.x, 0, 0));
		_mapOptionTweenCfg.easeType = GoEaseType.CubicInOut;
		_mapOptionTween = new GoTween (MapOptionPanel, .51f, _mapOptionTweenCfg);
		goChain.append (_mapOptionTween);

	}

	public void CleanAsset ()
	{
		if (request != null && request.assetBundle) {
			request.assetBundle.Unload (true);
		}
	}

	public void SwitchMapPannel (bool b)
	{
		if (b) {
			goChain.playForward ();
		} else {
			goChain.playBackwards ();
		}
	}

	#region Thumbnails

	void DownloadTxtAndThumbnails ()
	{
		Request myRequest;
		myRequest = new Request (RyanGlobalProps.VIEW720Index_URL + "order.txt");
		myRequest.OnComplete.Add ((r) => {
			string tmpTxt = File.ReadAllText (myRequest.responseFile);
			GetThumbnailsFromTxt (tmpTxt);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	int loadedThumbnailCount = 0;
	List<string> thumbnailList;

	void GetThumbnailsFromTxt (string content)
	{
		thumbnailList = new List<string> ();
		List<string> tmpList = new List<string> (content.Split ('#'));
		loadedThumbnailCount = 0;
		for (int i = 0; i < tmpList.Count; i++) {
			//处理缩略图名称
			tmpList [i] = tmpList [i].Trim ();
			if (tmpList [i] != "") {
				thumbnailList.Add (tmpList [i]);
			}
		}
		// 加载默认图片
		if (thumbnailList.Count > 0) {
			string url720 = RyanGlobalProps.VIEW720Index_URL + thumbnailList [0].Insert(thumbnailList [0].LastIndexOf('.'), "s");
			// url720 = url720.Remove (url720.LastIndexOf ('.')) + ".unity3d";
			Debug.Log(url720);
			LoadFirstTex (url720);
		}
		for (int i = 0; i < thumbnailList.Count; i++) {
			DownLoadThumbnails (tmpList [i]);
		}
	}

	void DownLoadThumbnails (string url)
	{
		Request myRequest;
		Debug.Log ("load thumbnails.." + url);
		myRequest = new Request (RyanGlobalProps.VIEW720Index_URL + url);
		string url720 = RyanGlobalProps.VIEW720Index_URL + url;
		// url720 = url720.Remove (url720.LastIndexOf ('.')) + ".unity3d";
		myRequest.OnComplete.Add ((r) => {
			StartCoroutine (AddThumbnail2Bottom (myRequest.responseFile, url720));
		});
		myRequest.OnProgress.Add ((r, p) => {
			//						Debug.Log (p);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	IEnumerator AddThumbnail2Bottom (string filePath, string url720)
	{
		url720 = url720.Insert(url720.Length - 4, "s");
		string tmpUrl = "file://" + filePath;
		WWW www = new WWW (tmpUrl);
		yield return www;
		if (www.isDone) {
			//add thumbnail to bottom menu list
			scrollCtrl.InsertScroll (www.texture, url720);
			++loadedThumbnailCount;
		}
	}

	#endregion

	string curSphereTex = "";

	public void Load720Texture (string tmpStr)
	{
		if (curSphereTex == tmpStr) {
			return;
		}
		myLoadingUI.SetActive (true);
		curSphereTex = tmpStr;
		Request myRequest;
		myRequest = new Request (tmpStr);
		curSphere.GetComponent<Renderer> ().material.mainTexture = blackTex;
		//RoteCube.SetActive(true);
		myRequest.OnComplete.Add ((r) => {
			myLoadingUI.SetActive (false);
			//RoteCube.SetActive(false);
			if (myRequest.error == null) {
				StartCoroutine(LoadTexture (myRequest.responseFile));
			} else {
				SendMessage ("BackToMainScene");
			}
		});
		myRequest.OnProgress.Add ((r, p) => {
			myLoadingUI.SetLoadingBarValue (p);
			Debug.Log (p + "%");
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	void LoadFirstTex (string url)
	{
		curSphereTex = url;
		Request myRequest;
		myLoadingUI.SetActive (true);
		myRequest = new Request (url);
		myRequest.OnComplete.Add ((r) => {
			Debug.Log ("Loading Complete..." + myRequest.responseFile);
			myLoadingUI.SetActive (false);
			if (myRequest.error == null) {
				StartCoroutine(LoadTexture (myRequest.responseFile));
			} else {
				SendMessage ("BackToMainScene");
			}
		});
		myRequest.OnProgress.Add ((r, p) => {
			myLoadingUI.SetLoadingBarValue (p);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}
	IEnumerator LoadTexture (string filePath)
	{
		string tmpUrl = "file://" + filePath;
		WWW www = new WWW (tmpUrl);
		yield return www;
		if (www.isDone) {
			if(www.texture == null){
				File.Delete (filePath);
				SendMessage ("BackToMainScene");
			}else{
				lastFilePath = filePath;
				curSphere.GetComponent<Renderer> ().material.mainTexture = www.texture;
				curSphere.SetActive (true);
			}
		}
	}

	// void LoadTexture (string filePath)
	// {
	// 	if (lastFilePath == filePath) {
	// 		return;
	// 	}
	// 	Debug.LogError(filePath);
	// 	AssetBundle ab = AssetBundle.LoadFromMemory (File.ReadAllBytes (filePath));
	// 	WWW www = new WWW(filePath);
	// 	if (ab == null) {
	// 		File.Delete (filePath);
	// 		SendMessage ("BackToMainScene");
	// 	} else {
	// 		lastFilePath = filePath;
	// 		curSphere.GetComponent<Renderer> ().material.mainTexture = ab.mainAsset as Texture;
	// 		ab.Unload (false);
	// 		curSphere.SetActive (true);
	// 	}
	// }
}