using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using RippleGen;

namespace RyanDownload
{
	public class RyanDownloadUtility : RippleGen.Core.MonoBehaviour
	{
		List<Texture2D> texList = new List<Texture2D> ();
		Request myRequest;

		public Texture2D GetTexture (string tmpURL)
		{
			StartCoroutine ("IGetTexture", tmpURL);
			return texList [0];
		}

		IEnumerator IGetTexture (string tmpURL)
		{
			WWW myW = new WWW (tmpURL);
			yield return myW;
			Debug.LogError (myW.texture.texelSize);
			texList.Add (myW.texture);
		}

		void DownloadSceneObj (string tmpURL)
		{
			myRequest = new Request (tmpURL);
			myRequest.OnComplete.Add ((r) => {
				Debug.Log ("Hello:" + myRequest.responseFile);
				StartCoroutine (load (myRequest.responseFile));
			});
			myRequest.OnProgress.Add ((r, p) => {
				Debug.Log (p);
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
	}
}