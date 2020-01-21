/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using UnityEngine;
using Vuforia;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class CloudRecoTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
	#region PRIVATE_MEMBER_VARIABLES

	private TrackableBehaviour mTrackableBehaviour;
	GameObject curScanedARObj;
	AssetBundle curAssetBundle;
	Transform videoPlaneTran;
	string videoPrefix = "video_";
	RyanARLoadingCircle loadingCircle;
	string lastTrackName = "";
	SimpleCloudHandler RyanHandler;
	CloudRecoBehaviour cloudReco;

	#endregion // PRIVATE_MEMBER_VARIABLES



	#region UNTIY_MONOBEHAVIOUR_METHODS

	VideoController ryanVideoCtrl;

	void Start()
	{
		Debug.LogError(transform);
		cloudReco = FindObjectOfType<CloudRecoBehaviour>();
		ryanVideoCtrl = FindObjectOfType<VideoController>();
		loadingCircle = FindObjectOfType<RyanARLoadingCircle>();
		RyanHandler = FindObjectOfType<SimpleCloudHandler>();
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
		videoPlaneTran = transform.Find("VideoPlane");
		curAssetBundle = null;
		// StartCoroutine(Test("cloudtest_pizzahut"));
	}

	#endregion // UNTIY_MONOBEHAVIOUR_METHODS



	#region PUBLIC_METHODS

	/// <summary>
	/// Implementation of the ITrackableEventHandler function called when the
	/// tracking state changes.
	/// </summary>
	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}

	#endregion // PUBLIC_METHODS



	#region PRIVATE_METHODS


	private void OnTrackingFound()
	{
		string trackName = mTrackableBehaviour.TrackableName;
		if (lastTrackName != trackName)
		{
			UnloadAssetBundle(trackName);
		}

		if (trackName.StartsWith(videoPrefix, System.StringComparison.CurrentCultureIgnoreCase))
		{
			string url = RyanGlobalProps.ARScan_URL + "/video/" + trackName.Substring(videoPrefix.Length, trackName.Length - videoPrefix.Length) + ".mp4";
			ryanVideoCtrl.PlayURL(url);
			Debug.Log("play video.." + url);
			//显示播放面板
			ShowModel(videoPlaneTran);
		}
		else
		{
			//下载模型
			if (lastTrackName == trackName && curScanedARObj)
			{
				ShowModel(curScanedARObj.transform);
			}
			else
			{
				if (curScanedARObj)
				{
					Destroy(curScanedARObj);
				}
				curDownloadCoroutine = RyanDownLoadGO(trackName);
				StartCoroutine(curDownloadCoroutine);
			}
		}
		// Stop finder since we have now a result, finder will be restarted again when we lose track of the result
		ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		if (objectTracker != null)
		{
			objectTracker.GetTargetFinder<ImageTargetFinder>().Stop();
		}
		Debug.Log("Trackable " + trackName + " found");
	}

	IEnumerator curDownloadCoroutine;

	void ShowModel(Transform tran)
	{
		Renderer[] rendererComponents = tran.GetComponentsInChildren<Renderer>(true);

		// Enable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = true;
		}
	}

	private void OnTrackingLost()
	{
		string trackName = mTrackableBehaviour.TrackableName;

		//停止正在下载的模型
		//if (curDownloadCoroutine != null) {
		//	StopCoroutine (curDownloadCoroutine);
		//	curDownloadCoroutine = null;
		//}
		StopAllCoroutines();
		#region Hide Model
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);

		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}
		#endregion

		if (trackName.StartsWith(videoPrefix, System.StringComparison.CurrentCultureIgnoreCase))
		{
			ryanVideoCtrl.StopVideo();
		}

		// Start finder again if we lost the current trackable
		// ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		// if (objectTracker != null)
		// {
		// 	objectTracker.TargetFinder.ClearTrackables(false);
		// 	objectTracker.TargetFinder.StartRecognition();
		// }
		if (loadingCircle)
		{
			loadingCircle.Show(false);
		}
		lastTrackName = trackName;
		Debug.Log("Trackable " + trackName + " lost");
		RyanHandler.OnLostTarget();
	}
	
	IEnumerator Test(string bundleName){
		// string str = RyanGlobalProps.ARScan_URL + RyanGlobalProps.Platform_URL + "/cloudtest_pizzahut.unity3d";
		string str =  RyanGlobalProps.ARScan_URL + RyanGlobalProps.Platform_URL + bundleName + ".unity3d";
		WWW myW = new WWW(str);
		Debug.LogError(str);
		yield return myW;
		curAssetBundle = myW.assetBundle;
		GameObject go;
		if (curAssetBundle)
		{
			go = Instantiate(curAssetBundle.LoadAsset(bundleName + ".prefab")) as GameObject;
			go.transform.localScale = Vector3.one * 12f;
			Debug.LogError(go);
		}
	}

	IEnumerator RyanDownLoadGO(string bundleName)
	{
		loadingCircle.Show(true);
		WWW myW = new WWW(RyanGlobalProps.ARScan_URL + RyanGlobalProps.Platform_URL + bundleName + ".unity3d");
		Debug.Log(RyanGlobalProps.ARScan_URL + RyanGlobalProps.Platform_URL + bundleName + ".unity3d");
		yield return myW;
		curAssetBundle = myW.assetBundle;
		if (curAssetBundle)
		{
			// curScanedARObj = Instantiate(curAssetBundle.mainAsset) as GameObject;
			curScanedARObj = Instantiate(curAssetBundle.LoadAsset(bundleName + ".prefab")) as GameObject;
			curScanedARObj.layer = LayerMask.NameToLayer("ARVuforiaGO");
			curScanedARObj.transform.parent = transform;
			curScanedARObj.transform.localScale = Vector3.one;
			loadingCircle.Show(false);
		}
	}

	public void PopHomePage()
	{
		if (curAssetBundle)
		{
			curAssetBundle.Unload(true);
		}
		SceneManager.LoadScene(RyanGlobalProps.mainSceneName);
	}

	void UnloadAssetBundle(string trackName)
	{
		//停止正在下载的模型
		//if (curDownloadCoroutine != null) {
		//	StopCoroutine (curDownloadCoroutine);
		//	curDownloadCoroutine = null;
		//}
		StopAllCoroutines();
		//删除模型
		if (curScanedARObj != null)
		{
			Destroy(curScanedARObj);
		}
		if (curAssetBundle)
		{
			curAssetBundle.Unload(true);
		}
	}

	#endregion // PRIVATE_METHODS
}
