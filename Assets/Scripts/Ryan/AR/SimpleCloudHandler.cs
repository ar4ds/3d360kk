using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";
	public ImageTargetBehaviour ImageTargetTemplate;
	List<string> targetIds = new List<string>();
	ObjectTracker mImageTracker;
	// Use this for initialization

	void Start()
	{
		// register this event handler at the cloud reco behaviour
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}
	}

	public void OnInitialized()
	{
		Debug.Log(transform + "Cloud Reco initialized");
		mImageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
	}

	public void OnInitError(TargetFinder.InitState initError)
	{
		Debug.Log("Cloud Reco init error " + initError.ToString());
	}

	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
		Debug.Log("Cloud Reco update error " + updateError.ToString());
	}

	public void OnLostTarget()
	{
		if (!mIsScanning && mCloudRecoBehaviour != null)
		{
			mImageTracker.TargetFinder.ClearTrackables(false);
			mCloudRecoBehaviour.CloudRecoEnabled = true;
		}
	}

	public void OnStateChanged(bool scanning)
	{
		mIsScanning = scanning;
		// clear all known trackables
		// ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		if (scanning)
		{
			mImageTracker.TargetFinder.ClearTrackables(false);
		}
	}

	// Here we handle a cloud target recognition event
	// public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
	// {
	// 	// Build augmentation based on target
	// 	if (ImageTargetTemplate)
	// 	{
	// 		Debug.LogWarning("here we are!");
	// 		ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
	// 		ImageTargetBehaviour imageTargetBehaviour = 
	// 			tracker.TargetFinder.EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject);
	// 		// do something with the target metadata
	// 		// mTargetMetadata = targetSearchResult.MetaData;
	// 		if (imageTargetBehaviour != null)
	// 		{
	// 			mCloudRecoBehaviour.CloudRecoEnabled = false;
	// 		}
	// 	}
	// }
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
		Debug.Log("create new targetImage:" + targetSearchResult.TargetName);
		string targetName = targetSearchResult.TargetName;
		if(!targetIds.Contains(targetName)){
			targetIds.Add(targetName);
			// duplicate the referenced image target
			GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
			newImageTarget.name = targetName;
			// enable the new result with the same ImageTargetBehaviour:
			ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);

			if (imageTargetBehaviour != null)
			{
				// stop the target finder
				mCloudRecoBehaviour.CloudRecoEnabled = false;
			}
		}else{
			ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, GameObject.Find(targetName));

			if (imageTargetBehaviour != null)
			{
				// stop the target finder
				mCloudRecoBehaviour.CloudRecoEnabled = false;
			}
		}
    }

	void OnGUIs()
	{
		// Display current 'scanning' status
		GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
		// Display metadata of latest detected cloud-target
		GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
		// If not scanning, show button
		// so that user can restart cloud scanning
		if (!mIsScanning)
		{
			if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
			{
				mImageTracker.TargetFinder.ClearTrackables(false);
				// Restart TargetFinder
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}
	}
}