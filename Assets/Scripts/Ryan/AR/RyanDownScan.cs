using UnityEngine;
using System.Collections;

public class RyanDownScan : MonoBehaviour
{
	GameObject curScanedARObj;

	void OnGUI()
	{
		if (GUILayout.Button("A"))
		{
			UnloadAssetBundle();
			StartCoroutine(RyanDownLoadGO("cloudtest_pizzahut"));
		}
		else if (GUILayout.Button("B"))
		{
			UnloadAssetBundle();
			StartCoroutine(RyanDownLoadGO("cloudtest_starbucks"));
		}
		else if (GUILayout.Button("C"))
		{
			UnloadAssetBundle();
			StartCoroutine(RyanDownLoadGO("cloudtest_zgf"));
		}
	}

	AssetBundle curAssetBundle;

	IEnumerator RyanDownLoadGO(string bundleName)
	{
		WWW myW = new WWW(RyanGlobalProps.ARScan_URL + RyanGlobalProps.Platform_URL + "/" + bundleName + ".unity3d");
		yield return myW;
		curAssetBundle = myW.assetBundle;
		curScanedARObj = Instantiate(curAssetBundle.mainAsset) as GameObject;
	}

	void UnloadAssetBundle()
	{
		Destroy(curScanedARObj);
		if (curAssetBundle)
		{
			curAssetBundle.Unload(true);
		}
	}
}