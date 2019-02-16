using UnityEngine;
using System.Collections;

// Get the latest webcam shot from outside "Friday's" in Times Square
public class ExampleClass : MonoBehaviour
{
	string url = "https://ss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo/logo_white_fe6da1ec.png";

	IEnumerator Start ()
	{
		// Start a download of the given URL
		WWW www = new WWW (url);

		// Wait for download to complete
		yield return www;
		Debug.Log ("Hello World!");

		// assign texture
		Renderer renderer = GetComponent<Renderer> ();
		renderer.material.mainTexture = www.texture;
	}
}