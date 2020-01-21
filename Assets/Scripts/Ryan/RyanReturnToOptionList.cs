using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RyanReturnToOptionList : MonoBehaviour
{
	public List<RectTransform> Titles = new List<RectTransform>();

	void Start(){
		foreach(RectTransform rt in Titles){
			rt.sizeDelta = new Vector2(0, 100+RyanGlobalProps.StatusBarHeight);
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			BackToMainScene();
			Debug.LogError("Escape");
		}
	}
	public void BackToMainScene()
	{
		gameObject.SendMessage("CleanAsset", SendMessageOptions.DontRequireReceiver);
		SceneManager.LoadScene("VRMain");
	}
}
