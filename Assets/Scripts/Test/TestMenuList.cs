using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TestMenuList : MonoBehaviour
{

	void OnGUI()
	{
		//		if (GUILayout.Button (menuList [0])) {
		//			LoadLevel (menuList [0]);
		//		}
		//		if (GUILayout.Button (menuList [1])) {
		//			LoadLevel (menuList [1]);
		//		}
		//		if (GUILayout.Button (menuList [2])) {
		//			LoadLevel (menuList [2]);
		//		} 
		//		if (GUILayout.Button (menuList [3])) {
		//			SceneManager.LoadScene (menuList [3]);
		//		} 

		//if (GUI.Button (new Rect (0, 0, Screen.width, Screen.height >> 1), "shanghai")) {
		//	LoadLevel (menuList [0]);
		//} else if (GUI.Button (new Rect (0, Screen.height >> 1, Screen.width, Screen.height >> 1), "manager")) {
		//	SceneManager.LoadScene ("SourceMgr");
		//}
		float btnCount = 3;
		if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height /btnCount), "Vuforia"))
		{
			SceneManager.LoadScene("ARCloud");
		}
		else if (GUI.Button(new Rect(0, Screen.height /btnCount, Screen.width, Screen.height/btnCount), "ARKit"))
		{
			SceneManager.LoadScene("ARKit");
		}
		else if (GUI.Button(new Rect(0, Screen.height /btnCount * 2f, Screen.width, Screen.height /btnCount), "shanghai"))
		{
			LoadLevel("shanghai");
		}
	}

	void LoadLevel(string str)
	{
		RyanGlobalProps.SetLoadURL(str);
		SceneManager.LoadScene("VRMain");
	}
}
