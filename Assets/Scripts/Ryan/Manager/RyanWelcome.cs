using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RyanWelcome : MonoBehaviour {
	public CanvasGroup BlackLayer;
	public static bool IsLoading = true;
	// Use this for initialization
	void Start () {
		LoadScene();
	}
	
	// Update is called once per frame
	void LoadScene () {
		AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(RyanGlobalProps.mainSceneName, LoadSceneMode.Additive);
		sceneAsync.completed +=(a)=>{
			StartCoroutine(FadeOut());
		};
	}
	IEnumerator FadeOut(){
		// 总共淡入的时长
		float loadBlack = 1.5f;
		while(BlackLayer.alpha > 0){
			BlackLayer.alpha -= Time.deltaTime / loadBlack;
			yield return 0;
		}
		IsLoading = false;
		SceneManager.UnloadSceneAsync("Welcome");
	}
}
