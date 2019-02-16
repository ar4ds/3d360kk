using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
	public GameObject BtnParent, PauseBtn, PlayBtn;
	public GameObject LoadingGO;
	public RectTransform LoadingBarFloatingTran;
	bool isPause = false;
	bool calLoading = true;
	public Image LoadingImg;
	float origLoadingLength;
	VideoPlayer video;
	bool isShowBtn = false;

	// Use this for initialization
	void Start ()
	{
		video = GetComponent<VideoPlayer> ();
		video.loopPointReached += (source) => {
			SendMessage ("BackToMainScene");
		};
		video.Prepare();
		video.prepareCompleted += Prepared;
		InitBtns();
		origLoadingLength = LoadingImg.rectTransform.sizeDelta.x;
		StartCoroutine(CalLoadingEffect());
	}
	void InitBtns(){
		isPause = false;
		BtnParent.SetActive (false);
		PlayBtn.SetActive (isPause);
		PauseBtn.SetActive (!isPause);
		PlayBtn.GetComponent<Button>().onClick.AddListener(SwitchPlayMode);
		PauseBtn.GetComponent<Button>().onClick.AddListener(SwitchPlayMode);
	}
	// Update is called once per frame
	void Update ()
	{
		if(calLoading){
			//CalLoadingEffect();
			//CalFloatingEffect();
		}else{
			CalClickAction();
		}
	}
	void Prepared(VideoPlayer vPlayer)
    {
		calLoading = false;
		LoadingGO.SetActive(false);
        vPlayer.Play();
    }

	float curT = 0;
	bool hasClickOnce = false;
	IEnumerator CalLoadingEffect(){
		float second = 20f;
		while(true){
			LoadingImg.rectTransform.sizeDelta += Vector2.right * Time.deltaTime * (origLoadingLength/second);
			if(LoadingImg.rectTransform.sizeDelta.x >= origLoadingLength){
				LoadingImg.rectTransform.sizeDelta = new Vector2(0, LoadingImg.rectTransform.sizeDelta.y);
			}
			yield return 0;
		}
	}

	void CalLoadingEffect0(){
		LoadingImg.fillAmount = ((int)(Time.time * 4.5f) % 12f + 1f) / 12f;
	}
	void CalFloatingEffect(){
		float delta = Time.time - (int)Time.time;
		float speed = 2f;
		delta *= speed;
		delta = delta - (int)delta;
		LoadingBarFloatingTran.localPosition = Vector3.Lerp(Vector3.zero, Vector3.left * 10, delta);
	}

	void CalClickAction(){
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
			if(hasClickOnce){
				// 双击
				SwitchPlayMode();
			}else{
				// 第一次单击
				curT = 0;
				hasClickOnce = true;
			}
		}
		if(hasClickOnce){
			curT += Time.deltaTime;
			if(curT >= .5f){
				// 单击
				isShowBtn = !isShowBtn;
				BtnParent.SetActive(isShowBtn);
				hasClickOnce = false;
			}
		}
	}

	void SwitchPlayMode(){
		hasClickOnce = false;
		isPause = !isPause;
		PauseBtn.SetActive (!isPause);
		PlayBtn.SetActive (isPause);
		if (isPause) {
			video.Pause ();
		} else {
			video.Play ();
		}
	}
}
