using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using RippleGen;

public class RyanTravelSetter : RippleGen.Core.MonoBehaviour
{
	List<Texture2D> texList = new List<Texture2D> ();
	RyanScrollRectCell scrollCtrl;
	public Transform FirstPersonPrefab;
	Transform curPerson;
	AssetBundleCreateRequest request;
	public GameObject MapPanel;
	public Transform MapOptionPanel;
	static RawImage mapImage;
	public GameObject MapCloseBtn;
	public Transform JoystickTran;
	Request myRequest;
	bool isMapOpen = true;
	public LoadingUI myLoadingUI;

	GoTweenConfig _mapOptionTweenCfg, _joystickTweenCfg;
	GoTweenChain goChain;
	GoTween _mapOptionTween;
	GoTween _joystickTween;

	void Awake ()
	{
		SceneManager.sceneLoaded += InitScene;
	}

	void Start ()
	{
		MapOptionPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, 50f);
		scrollCtrl = MapOptionPanel.GetComponent<RyanScrollRectCell> ();
		InitMapPanelTween ();
	}

	void OnDestroy ()
	{
		SceneManager.sceneLoaded -= InitScene;
	}

	void InitScene (Scene arg0, LoadSceneMode arg1)
	{
		mapImage = MapPanel.GetComponent<RawImage> ();
		curPerson = Instantiate (FirstPersonPrefab) as Transform;
		curPerson.gameObject.SetActive (false);
		MapPanel.SetActive (false);
		MapCloseBtn.SetActive (false);
		JoystickTran.gameObject.SetActive (false);
		DownloadGameObject (RyanGlobalProps.TRAVEL_URL);
	}

	void InitMapPanelTween ()
	{
		goChain = new GoTweenChain ();
		_mapOptionTweenCfg = new GoTweenConfig ();
		_mapOptionTweenCfg.position (new Vector3 (MapOptionPanel.position.x, 0, 0));
		_mapOptionTweenCfg.easeType = GoEaseType.CubicInOut;
		_mapOptionTween = new GoTween (MapOptionPanel, .51f, _mapOptionTweenCfg);

		_joystickTweenCfg = new GoTweenConfig ();
		_joystickTweenCfg.position (new Vector3 (JoystickTran.position.x, -JoystickTran.position.y));
		_joystickTweenCfg.easeType = GoEaseType.CircIn;
		_joystickTween = new GoTween (JoystickTran, .51f, _joystickTweenCfg);
		goChain.append (_joystickTween);
		goChain.append (_mapOptionTween);

	}

	public void CleanAsset ()
	{
		if (request != null && request.assetBundle != null) {
			request.assetBundle.Unload (true);
		}
	}

	#region switch map panel

	public void OpenMapView ()
	{
		SwitchMap (true);
	}

	public void CloseMapView ()
	{
		SwitchMap (false);
	}

	void SwitchMap (bool _isOpen)
	{
		if (_isOpen != isMapOpen) {
			MapPanel.SetActive (_isOpen);
			MapCloseBtn.SetActive (!_isOpen);
			isMapOpen = !isMapOpen;
		}
	}

	#endregion

	void DownloadGameObject (string tmpStr)
	{
		myRequest = new Request (tmpStr);
		myRequest.OnComplete.Add ((r) => {
			myLoadingUI.DestroyLoadingUI ();
			if (myRequest.error == null) {
				StartCoroutine (LoadGameObject (myRequest.responseFile));
			} else {
				SendMessage ("BackToMainScene");
			}
		});
		myRequest.OnProgress.Add ((r, p) => {
			Debug.Log (p);
			myLoadingUI.SetLoadingBarValue (p);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	void DeleteSceneObj ()
	{
		File.Delete (myRequest.responseFile);
	}

	IEnumerator LoadGameObject (string filePath)
	{
		//request = AssetBundle.LoadFromMemoryAsync (File.ReadAllBytes (filePath));
		AssetBundle ab = AssetBundle.LoadFromMemory (File.ReadAllBytes (filePath));

		if (ab == null) {
			File.Delete (filePath);
			SendMessage ("BackToMainScene");
		} else {
			Transform tmpTran = (Instantiate (ab.mainAsset) as GameObject).transform;
			CreateFloorUI (tmpTran);
			ab.Unload (false);
		}
		yield return null;

		//if (request == null) {
		//	File.Delete (filePath);
		//	SendMessage ("BackToMainScene");
		//}
		//yield return request;
		//Transform tmpTran = (Instantiate (request.assetBundle.mainAsset) as GameObject).transform;
		//CreateFloorUI (tmpTran);
		//CreateMapPictureTrigger (tmpTran);
		curPerson.gameObject.SetActive (true);
		JoystickTran.gameObject.SetActive (true);
		CloseMapView ();
	}

	void CreateMapPictureTrigger (Transform tran)
	{
		List<Transform> tmpList = new List<Transform> ();
		Transform mapRoot = tran.Find ("MapRoot");
		int mapCount = mapRoot.childCount;
		for (int i = 0; i < mapCount; i++) {
			tmpList.Add (mapRoot.GetChild (i));
		}
		for (int i = 0; i < tmpList.Count; i++) {
			Collider tmpCol = tmpList [i].GetComponent<Collider> ();
			tmpCol.isTrigger = true;
			tmpList [i].gameObject.GetComponent<RyanMapTrigger> ();
		}
	}

	public static void SetMapPicture (Texture tex)
	{
		mapImage.texture = tex;
	}

	public void SwitchMapPannel (bool b)
	{
		if (b) {
			goChain.playForward ();
		} else {
			goChain.playBackwards ();
		}
	}

	void CreateFloorUI (Transform tran)
	{
		List<Transform> tmpList = new List<Transform> ();
		Transform floorRoot = tran.Find ("FloorRoot");
		if (floorRoot) {
			int floorCount = floorRoot.childCount;
			for (int i = 0; i < floorCount; i++) {
				tmpList.Add (floorRoot.GetChild (i));
			}
			List<Button> btnList = scrollCtrl.CreateScroll (tmpList.Count);
			for (int i = 0; i < btnList.Count; i++) {
				Button tmpBtn = btnList [i];
				Transform tmpLocation = tmpList [i];
				tmpBtn.name = tmpList [i].name;
				tmpBtn.GetComponentInChildren<Text> ().text = tmpList [i].name;
				tmpBtn.onClick.AddListener (delegate () {
					curPerson.position = tmpLocation.position;
					curPerson.rotation = tmpLocation.rotation;
				});
			}
		}
	}
}
