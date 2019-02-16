using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class RyanMainScene : MonoBehaviour
{
	public void RyanLoadScene (string sceneName)
	{
		Debug.Log ("RyanLoadScene.." + sceneName);
		SceneManager.LoadScene (sceneName);
	}

	public void RyanLoadMuseum (string museumName)
	{
		RyanGlobalProps.SetLoadURL (museumName);
		SceneManager.LoadScene ("VRMain");
	}
}