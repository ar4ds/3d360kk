using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RippleGen;
using System.IO;
using DG.Tweening;

public class RyanBGScroll : RippleGen.Core.MonoBehaviour
{
	public Transform PicRoot;
	public LoadingUI myLoadingUI;
    public Image LeftBorder, RightBorder;
    bool willSprintLeftBorder = false;
    bool willSprintRightBorder = false;
    // 记录鼠标路径的个数
    int saveMouseXCount = 4;
	List<RectTransform> rtList = new List<RectTransform> ();
	List<RawImage> bgList = new List<RawImage> ();
	int Cur_PageIndex = 0;
	RectTransform curScrollRectTran;
    List<float> mouseXList = new List<float>();
    float mouseClickLastT;
	List<RectTransform> PageList = new List<RectTransform>();
    // 正在翻页
    bool isPageTurning = false;

	bool isLoadingLeft = false;
	bool isLoadingRight = false;

	float curT = 0;
	float tarT = 1f;
	bool isLerp = false;
	float mBeginPos;
	float curM_xOffset;
	float picLen;

	protected override void Start ()
	{
		base.Start();
		Cur_PageIndex = 0;
		curScrollRectTran = GetComponent<RectTransform> ();
		picLen = curScrollRectTran.rect.width;
		for (int i = 0; i < PicRoot.childCount; i++) {
			rtList.Add (PicRoot.GetChild (i).GetComponent<RectTransform> ());
			rtList [i].localPosition = new Vector3 (picLen * i, 0, 0);
			bgList.Add (rtList [i].GetComponent<RawImage> ());
		}
		curScrollRectTran.localPosition = -rtList [Cur_PageIndex].localPosition;
        InitTurnPages();
		DownLoadPicture (Cur_PageIndex);
	}

    void InitTurnPages(){
        PageList = new List<RectTransform>();
		for (int i = 0; i < PicRoot.childCount; i++)
		{
			PageList.Add(PicRoot.GetChild(i).GetComponent<RectTransform>());
		}
		PicRoot.localPosition = new Vector3(Cur_PageIndex * -Screen.width,0,0);
        ResetBorder();
    }

	protected override void Update ()
	{
		base.Update ();
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown (0))
#else
		if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Began)
#endif
		{
            mouseClickLastT = Time.time;
			mBeginPos = Input.mousePosition.x;
            mouseXList = new List<float>(){mBeginPos};
			curM_xOffset = 0;

			isLoadingLeft = false;
			isLoadingRight = false;
		}
#if UNITY_EDITOR
		else if (Input.GetMouseButtonUp (0))
#else
			else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Ended)
#endif
		{
            float deltaSpeed = mouseXList[0] - Input.mousePosition.x;
            float tmpLen = Screen.width >> 5;
            // 左翻页
            if(deltaSpeed < -tmpLen){
                TurnPageLeft(.25f);
            } else if(deltaSpeed > tmpLen){
                TurnPageRight(.25f);
            }
			else if ((curM_xOffset >= Screen.width >> 1))
			{
                TurnPageLeft();
			}
			else if ((curM_xOffset <= -Screen.width >> 1))
			{
                TurnPageRight();
			}else {
                // 弹回原位
                SpringbackPage();
            }
            if(willSprintLeftBorder){
                LeftBorderSprintBack();
            }else if(willSprintRightBorder){
                RightBorderSprintBack();
            }
		}
#if UNITY_EDITOR
		else if (Input.GetMouseButton (0))
#else
		else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Moved)
#endif
		{
			curM_xOffset = Input.mousePosition.x - mBeginPos;
			Debug.Log(Cur_PageIndex + " >> " + (PageList.Count - 1));
			//第一页或最后一页加特效
            if(Cur_PageIndex <= 0 && curM_xOffset > 0){
                willSprintLeftBorder = true;
                float tmpLimit = Mathf.Sin(.5f * Mathf.PI * curM_xOffset/Screen.width);
                LeftBorder.color = new Color(1,1,1,tmpLimit);
                LeftBorder.transform.localScale = new Vector3(tmpLimit, 1f, 1f);
            }else if(Cur_PageIndex >= PageList.Count - 1 && curM_xOffset < 0){
                willSprintRightBorder = true;
                float tmpLimit = -Mathf.Sin(.5f * Mathf.PI * curM_xOffset/Screen.width);
                RightBorder.color = new Color(1,1,1,tmpLimit);
                RightBorder.transform.localScale = new Vector3(tmpLimit, 1f, 1f);
            }else{
                // 页面拖拽效果
                PicRoot.localPosition = -PageList[Cur_PageIndex].localPosition + Vector3.right * curM_xOffset;
				if (curM_xOffset >= Screen.width * .05f && !isLoadingLeft) {
					isLoadingLeft = true;
					if (Cur_PageIndex > 0) {
						DownLoadPicture (Cur_PageIndex - 1);
					}
				} else if (curM_xOffset <= Screen.width * -.05f && !isLoadingRight) {
					isLoadingRight = true;
					if (Cur_PageIndex < rtList.Count - 1) {
						DownLoadPicture (Cur_PageIndex + 1);
					}
				}
            }
            //记录鼠标x方向序列
            InsertMouseXList(Input.mousePosition.x);
		}
	}

    void InsertMouseXList(float x){
        if(mouseXList.Count > saveMouseXCount){
            mouseXList.RemoveAt(0);
        }
        mouseXList.Add(x);
    }

	void InitLoadingPic (int idx)
	{
		myLoadingUI.transform.parent = rtList [idx];
		myLoadingUI.transform.position = rtList [idx].position;
		myLoadingUI.SetLoadingBarValue (0);
		myLoadingUI.SetActive (true);
	}

	void DownLoadPicture (int picIndex)
	{
		Request myRequest;
		myRequest = new Request (RyanGlobalProps.VRMainBG_URL + picIndex + ".jpg");
		if (bgList [picIndex].texture.name == "Black") {
			//重置加在动态图
			InitLoadingPic (picIndex);
		} else {
			myLoadingUI.SetActive (false);
		}
		myRequest.OnComplete.Add ((r) => {
			myLoadingUI.SetActive (false);
			StartCoroutine (LoadTexture (myRequest.responseFile, picIndex));
		});
		myRequest.OnProgress.Add ((r, p) => {
			myLoadingUI.SetLoadingBarValue (p);
			Debug.Log (p);
		});
		myRequest.ReadCache = true;
		myRequest.CheckExpire = false;
		addOperation (myRequest);
	}

	IEnumerator LoadTexture (string filePath, int picIndex)
	{
		string tmpUrl = "file://" + filePath;
		if (File.Exists (filePath)) {
			WWW www = new WWW (tmpUrl);
			yield return www;
			if (www.isDone) {
				bgList [picIndex].texture = www.texture;
			}
		}
	}

	//	IEnumerator LoadTexture (string filePath, int picIndex)
	//	{
	//		// Start a download of the given URL
	//		WWW www = new WWW ("https://ss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo/logo_white_fe6da1ec.png");
	//
	//		// Wait for download to complete
	//		yield return www;
	//		Debug.LogError ("Hello World!");
	//		bgList [picIndex].texture = www.texture;
	//	}

    void TurnPageLeft(float speed = .5f){
        if(Cur_PageIndex <= 0){
            SpringbackPage();
            return;
        }
        --Cur_PageIndex;
        isPageTurning = true;
        PicRoot.DOLocalMove(-PageList[Cur_PageIndex].localPosition, speed).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
    }
    void TurnPageRight(float speed = .5f){
        if(Cur_PageIndex >= PageList.Count - 1){
            SpringbackPage();
            return;
        }
        isPageTurning = true;
		++Cur_PageIndex;
		PicRoot.DOLocalMove(-PageList[Cur_PageIndex].localPosition, speed).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
    }
    // 弹回原位
    void SpringbackPage(){
        PicRoot.DOLocalMove(-PageList[Cur_PageIndex].localPosition, .2f).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
    }
    void LeftBorderSprintBack(){
        willSprintLeftBorder = false;
        float delta = .3f;
        LeftBorder.DOFade(0, delta);
        LeftBorder.transform.DOScaleX(0, delta);
    }
    void RightBorderSprintBack(){
        willSprintRightBorder = false;
        float delta = .3f;
        RightBorder.DOFade(0, delta);
        RightBorder.transform.DOScaleX(0, delta);
    }

    void ResetBorder(){
        willSprintLeftBorder = false;
        willSprintRightBorder = false;
        LeftBorder.color = new Color(1, 1, 1, 0);
        RightBorder.color = new Color(1, 1, 1, 0);
        LeftBorder.transform.localScale = new Vector3(1, 1, 0);
        RightBorder.transform.localScale = new Vector3(1, 1, 0);
    }
    void OnTurnPageComplete(){
        isPageTurning = false;
    }

	void LerpPos ()
	{
		if (isLerp) {
			if (curT < tarT) {
				curT += Time.deltaTime;
				curScrollRectTran.localPosition = Vector3.Slerp (
					curScrollRectTran.localPosition,
					-rtList [Cur_PageIndex].localPosition,
					curT);
			} else {
				curT = 0;
				curScrollRectTran.localPosition = -rtList [Cur_PageIndex].localPosition;
				isLerp = false;
			}
		}
	}
}