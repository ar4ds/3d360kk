using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RyanAR2Home : MonoBehaviour {
	public Button home;
	public List<RectTransform> Titles = new List<RectTransform>();

	void Start(){
		foreach(RectTransform rt in Titles){
			rt.sizeDelta = new Vector2(0, 100+RyanGlobalProps.StatusBarHeight);
		}
		home.onClick.AddListener(ReturnHomepage);
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			ReturnHomepage();
		}
	}

	public void ReturnHomepage(){
		SceneManager.LoadScene(RyanGlobalProps.mainSceneName);
	}
}
