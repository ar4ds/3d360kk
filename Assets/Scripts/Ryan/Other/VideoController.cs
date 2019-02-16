using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
	VideoPlayer videoCtrl;
	// Use this for initialization
	void Awake ()
	{
		videoCtrl = GetComponent<VideoPlayer> ();
	}
	
	// Update is called once per frame
	public void PlayURL (string url)
	{
		StartCoroutine (PlayVideo (url));
	}

	IEnumerator PlayVideo (string url)
	{
		videoCtrl.url = url;
		videoCtrl.Prepare ();
		while (!videoCtrl.isPrepared) {
			yield return 0;
		}
		videoCtrl.Play ();
	}

	public void StopVideo ()
	{
		videoCtrl.Stop ();
	}
}
