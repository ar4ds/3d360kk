using UnityEngine;
using System.IO;
using System.Collections;
using RippleGen;

public class RyanLoadScene : RippleGen.Core.MonoBehaviour
{
	string downloadURL = "http://www.3d360kk.com/upload/RyanBlueCar0.unity3d";
	Request myRequest;
		
	// Use this for initialization
	protected override void Start ()
	{
		//DownloadSceneObj ();
	}

	void DownloadSceneObj ()
	{
		myRequest = new Request (downloadURL);
		myRequest.OnComplete.Add ((r) => {
			StartCoroutine (load (myRequest.responseFile));
		});
		myRequest.OnProgress.Add ((r, p) => {
//						Debug.Log (p);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	void DeleteSceneObj ()
	{
		Debug.Log ("DeleteSceneObj:" + myRequest.responseFile);
		File.Delete (myRequest.responseFile);
	}

	IEnumerator load (string filePath)
	{
		AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync (File.ReadAllBytes (filePath));
		yield return request;
		Instantiate (request.assetBundle.mainAsset);
	}

	void OnGUI ()
	{
		if (GUI.Button (new Rect (0, 0, 200, 200), "Download")) {
			DownloadSceneObj ();
		} else if (GUI.Button (new Rect (Screen.width - 200f, Screen.height - 200, 200, 200), "Delete")) {
			DeleteSceneObj ();
		}
	}
}