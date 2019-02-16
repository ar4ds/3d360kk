using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;
using DG.Tweening;
using UnityEngine.Events;

public class RyanUIController : MonoBehaviour {
    public Transform MainCanvas;
    public static RyanUIController Instance{
        get{return _instance;}
    }
    static RyanUIController _instance;
    public FenLeiItemA FenLeiAPrefab;
    public Button ARBtn;
    public Button GongGaoBtn, FenLeiBtn, ResourceBtn;
    public RectTransform FenLeiScrollContent;
    public Transform FenleiPanel;
    public Button FenLeiBPrefab;
    public GridLayoutGroup MuseumGrid;
    public CanvasGroup ExitLabel;
    public MuseumItem MuseumItemPrefab;
    public RyanDialogBox MsgDialogBoxPrefab, MsgDialogBox2OptionPrefab;
	public Transform GongGaoListPage;
	public Transform GongGaoContentPage;
	public Transform MuseumListPage;
    public Transform ResourceMgrPage;
    public Transform MailPage;
	public Transform BeaconPagePrefab;
	public Transform PageScrollTran;
    public Image LeftBorder, RightBorder;
    bool willSprintLeftBorder = false;
    bool willSprintRightBorder = false;
    // 记录鼠标路径的个数
    int saveMouseXCount = 4;
    List<float> mouseXList = new List<float>();
    public List<Button> PopPageExitBtnList = new List<Button>();
    public List<RectTransform> Titles = new List<RectTransform>();
    public List<RectTransform> UnderTitleUIList = new List<RectTransform>();
    string[] fenleiA = {"社会馆", "企业馆"};
    // string[] fenleiB = {"规划馆", "博物馆", "纪念馆", "文化馆", "科技馆", "车展","教育中心","艺术画廊"};
    // string[] museumList = {"规划馆", "博物馆", "纪念馆", "文化馆", "科技馆", "车展","教育中心","艺术画廊",
    // "规划馆", "博物馆", "纪念馆", "文化馆", "科技馆", "车展","教育中心","艺术画廊",
    // "规划馆", "博物馆", "纪念馆", "文化馆", "科技馆", "车展","教育中心","艺术画廊"};
    float fenleiA_H=0;
    float searchAItemH = 60f;
    string categoriesUrl = "http://www.3d360kk.com/mobile/categories";
    //string museumUrl = "http://www.3d360kk.com/mobile/query_museum";
    string museumUrl = "http://www.3d360kk.com/mobile/Halls";

	GridLayoutGroup PageGrid;
	public static List<RectTransform> PageList = new List<RectTransform>();
    // 正在翻页
    bool isPageTurning = false;
    // 初始化首页为AR页
	public static int Cur_PageIndex = 1;
	float mBeginPos;
    float mouseClickLastT;
    //按退出程序的倒计时
    float exitCountDownT = 0;
	public static float curM_xOffset;
    // 是否在弹出页上（公告、博物馆列表等）
    bool isOnPopPage = false;
    bool isOnDialogBox = false;
    // 用来保存先进后出的弹出页
    List<Transform> popPageStack = new List<Transform>();
    List<GameObject> msgDialogStack = new List<GameObject>();
    // GameObject curMessageBox;
    int beaconPageCount = 0;
    AndroidJavaObject _ajc;
    int statusBarH = 0;
    List<MuseumItem> museumItemList = new List<MuseumItem>();
    void Awake(){
        _instance = this;
        #if UNITY_ANDROID && !UNITY_EDITOR
        _ajc = new AndroidJavaObject("com.ar4ds.statusbar.Unity2Android");
        statusBarH = _ajc.Call<int>("getStatusBarHeight");
        #endif
        RyanGlobalProps.StatusBarHeight = statusBarH;
    }

    // Use this for initialization
    void Start ()
    {
        InitButtonListener();
        InitTitleBarHeight();
        StartCoroutine(InitFenLei());
        StartCoroutine(InitMuseum());
        InitTurnPages();
        InitOther();
    }

    void InitButtonListener(){
        foreach(Button btn in PopPageExitBtnList){
            btn.onClick.AddListener(CallHidePopPage);
        }
        GongGaoBtn.onClick.AddListener(PopGongGaoListPage);
        ResourceBtn.onClick.AddListener(PopResourceMgrPage);
        FenLeiBtn.onClick.AddListener(ShowFenleiPanel);
        ARBtn.onClick.AddListener(LoadARPage);
        QueryBtn.onClick.AddListener(QueryMuseum);
    }

    void LoadARPage(){
        Cur_PageIndex = PageList.Count - 2;
        SceneManager.LoadScene("ARCloud");
    }

    void InitTitleBarHeight(){
        foreach(RectTransform rt in Titles){
            rt.sizeDelta = new Vector2(0, 100+statusBarH);
        }
        foreach(RectTransform rt in UnderTitleUIList){
            Vector2 tmpV = rt.offsetMax;
            rt.offsetMax = new Vector2(tmpV.x, tmpV.y - statusBarH);
        }
    }

    void InitTurnPages(){
        PageList = new List<RectTransform>();
		PageGrid = PageScrollTran.Find("Viewport").GetComponent<GridLayoutGroup>();

		PageGrid.cellSize = new Vector2(Screen.width, Screen.height);
		MuseumListPage.position = new Vector2(Screen.width * 1.5f, Screen.height >> 1);
		for (int i = 0; i < PageGrid.transform.childCount; i++)
		{
			PageList.Add(PageGrid.transform.GetChild(i).GetComponent<RectTransform>());
		}
		PageScrollTran.localPosition = new Vector3(Cur_PageIndex * -Screen.width,0,0);
        if(Cur_PageIndex == PageList.Count - 1 && isOnMuseumList){
            PopMuseumListPage();
        }
        ResetBorder();
    }

    void InitOther(){
        ExitLabel.alpha = 0;
    }

    List<FenLeiItemA> fenleiAList = new List<FenLeiItemA>();
    IEnumerator InitFenLei(){
        WWW www = new WWW(categoriesUrl);
        yield return www;
        if(!string.IsNullOrEmpty(www.text)){
            JSONNode tmpJN = JSON.Parse(www.text);
            string[] tmpFenLeiBs = new string[tmpJN.Count];
            for(int i = 0; i < tmpJN.Count; i++){
                tmpFenLeiBs[i] = tmpJN[i][0].Value;
            }
            for(int i = 0; i < fenleiA.Length; i++){
                FenLeiItemA item = Instantiate(FenLeiAPrefab,FenLeiScrollContent);
                item.GetComponent<RectTransform>().anchoredPosition=Vector2.up * -searchAItemH * i;
                if(i == 0)
                {
                    item.Init(i, fenleiA[i], tmpFenLeiBs, FenLeiBPrefab);
                }else{
                    item.Init(i, fenleiA[i], new string[]{}, FenLeiBPrefab);
                }
                fenleiAList.Add(item);
            }
            fenleiA_H += fenleiA.Length * searchAItemH;
            CloseOtherGrid(-1);
        }
    }
    IEnumerator InitMuseum(){
        museumItemList = new List<MuseumItem>();
        WWW www = new WWW(museumUrl);
        yield return www;
        if(!string.IsNullOrEmpty(www.text)){
            JSONNode tmpJN = JSON.Parse(www.text);
            List<JSONNode> tmpJNList = new List<JSONNode>();
            tmpJNList.AddRange(tmpJN.Children);
            float tmpHRate = 1.335329341317365f;
            float gapW = 5f;
            float tmpW = Screen.width - gapW * 4f;
            float cellSizeW = tmpW / 3f;
            MuseumGrid.cellSize = new Vector2(cellSizeW, cellSizeW * tmpHRate);
            int rowCount = (tmpJNList.Count-1) / 3 + 1;
            MuseumGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  rowCount * (cellSizeW * tmpHRate + gapW) + gapW);
            foreach(JSONNode jn in tmpJNList){
                MuseumItem mi = Instantiate(MuseumItemPrefab, MuseumGrid.transform);
                string tmpUrl = string.Format("http://www.3d360kk.com/upload/halls/{0}/panorama.jpg", jn["guid"].Value);
                Debug.Log(tmpUrl);
                mi.Init(jn["name"], tmpUrl, jn["museum"]);
                museumItemList.Add(mi);
            }
        }
    }

    public Button QueryBtn;
    public InputField QueryBar;
    public Transform QueryListPage;
    public GridLayoutGroup QueryMuseumGrid;
    void QueryMuseum(){
        if(!string.IsNullOrEmpty(QueryBar.text)){
            ClearAllChilds(QueryMuseumGrid.transform);
            LoadQueryMuseumList(QueryBar.text);
            PopRightPage(QueryListPage);
        }
    }
    void LoadQueryMuseumList(string name){
        int tmpC=0;
        float tmpHRate = 1.335329341317365f;
        float gapW = 5f;
        float tmpW = Screen.width - gapW * 4f;
        float cellSizeW = tmpW / 3f;
        QueryMuseumGrid.cellSize = new Vector2(cellSizeW, cellSizeW * tmpHRate);
        foreach (MuseumItem item in museumItemList)
        {
            if(item.museumName.Contains(name)){
                MuseumItem mi = Instantiate(item,QueryMuseumGrid.transform);
                mi.Init(item.museumName, null, item.url);
                ++tmpC;
            }
        }
        QueryMuseumGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  tmpC * (cellSizeW * tmpHRate + gapW) + gapW);
        int rowCount = (tmpC-1) / 3 + 1;
    }

    #region 查找分类页
    // 重置搜索列表布局
    // index是当前被选择的分类，childCount是他的子类
    public void RecalSearchLayer(int index, int childCount){
        float tmpGap = 25f;
        // 子模块展开的高度
        float tmpChildH = 0;
        if(childCount > 1){
            tmpChildH = childCount * 40f + (childCount - 1) * tmpGap;
        }
        for(int i = 0; i < fenleiAList.Count;i++){
            fenleiAList[i].GetRectTransform().anchoredPosition = Vector2.up * -searchAItemH * i;
            // 比被选分类大的，布局会被推到下面
            if(i > index){
                fenleiAList[i].GetRectTransform().anchoredPosition += Vector2.up * -tmpChildH;
            }
        }
        // 计算整块滚动栏的大小
        FenLeiScrollContent.sizeDelta = new Vector2(0, fenleiA_H + tmpChildH + tmpGap);
    }

	//	开合分类项子项时，重置UI布局
    public void CloseOtherGrid(int index){
        for(int i = 0; i < fenleiAList.Count; i++){
            if(index != i){
                fenleiAList[i].CloseGrid();
            }
        }
    }
    #endregion

    bool isExpand = false;
    void ShowFenleiPanel(){
        isExpand = !isExpand;
        if(isExpand){
            FenleiPanel.scaleTo(.1f, Vector3.one);
        }else{
            FenleiPanel.scaleTo(.1f, new Vector2(1f,0));
        }
    }

    // Update is called once per frame
    void Update () {
        if(RyanWelcome.IsLoading){
            return;
        }
        if(CanCalPage()){
		    CalTurnPage();
            //再按一次退出系统
            if(Input.GetKeyUp(KeyCode.Escape)){
                TryExit();
            }
        }else if(Input.GetKeyUp(KeyCode.Escape)){
            if(isOnDialogBox){
                //关闭消息框
                CloseDialogBox();
            }else if(isOnPopPage){
                //关闭弹出页
                HidePopPage();
            }else{
                //再按一次退出系统
                TryExit();
            }
        }
        if(exitCountDownT > 0){
            exitCountDownT -= Time.deltaTime;
            if(exitCountDownT < 0){
                exitCountDownT = 0;
            }
        }
	}
    #region 翻页控制
    public bool CanCalPage(){
        return !isOnPopPage && !isOnDialogBox && !isPageTurning;
    }
    bool readyToTurnPage = false;
	void CalTurnPage(){
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
#else
		if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Began)
#endif
		{
            readyToTurnPage = true;
            mouseClickLastT = Time.time;
			mBeginPos = Input.mousePosition.x;
            if(!isPageTurning){
                //float offset = (PageScrollTran.localPosition-PageList[Cur_PageIndex].localPosition).x%Screen.width;
                //mBeginPos += offset;
            }
            mouseXList = new List<float>(){mBeginPos};
		    curM_xOffset = 0;
		}
#if UNITY_EDITOR
		else if (Input.GetMouseButtonUp(0) && readyToTurnPage)
#else
		else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Ended && readyToTurnPage)
#endif
		{
            readyToTurnPage = false;
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
            curM_xOffset = 0;
		}
#if UNITY_EDITOR
		else if (Input.GetMouseButton(0))
#else
		else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Moved)
#endif
		{
			curM_xOffset = Input.mousePosition.x - mBeginPos;
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
                PageScrollTran.localPosition = -PageList[Cur_PageIndex].localPosition + Vector3.right * curM_xOffset;
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

    void TurnPageLeft(float speed = .5f){
        if(Cur_PageIndex <= 0){
            SpringbackPage();
            return;
        }
        --Cur_PageIndex;
        isPageTurning = true;
        PageScrollTran.DOLocalMove(-PageList[Cur_PageIndex].localPosition, speed).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
    }
    void TurnPageRight(float speed = .5f){
        if(Cur_PageIndex >= PageList.Count - 1){
            SpringbackPage();
            return;
        }
        isPageTurning = true;
		++Cur_PageIndex;
		PageScrollTran.DOLocalMove(-PageList[Cur_PageIndex].localPosition, speed).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
    }
    // 弹回原位
    void SpringbackPage(){
        PageScrollTran.DOLocalMove(-PageList[Cur_PageIndex].localPosition, .2f).SetEase(Ease.OutCubic).OnComplete(()=>{OnTurnPageComplete();});
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
    // 弹出消息提示对话框
    public void PopMessageDialogue(string title, string content){
        isOnDialogBox = true;
        RyanDialogBox tmpDialogue = Instantiate(MsgDialogBoxPrefab, MainCanvas);
        msgDialogStack.Add(tmpDialogue.gameObject);
        tmpDialogue.OkButton.onClick.AddListener(CloseDialogBox);
        tmpDialogue.Init(title, content);
        // 停止翻页计算
		readyToTurnPage = false;
        // curMessageBox = tmpDialogue.gameObject;
    }
    public void CloseDialogBox(){
        Destroy(msgDialogStack[msgDialogStack.Count - 1]);
        msgDialogStack.RemoveAt(msgDialogStack.Count - 1);
        if(msgDialogStack.Count == 0){
            isOnDialogBox = false;
        }
    }
    #region 可选对话框
    
    // 弹出消息提示对话框
    public void PopOptionDialogue(string title, string content, UnityAction action){
        isOnDialogBox = true;
        RyanDialogBox tmpDialogue = Instantiate(MsgDialogBox2OptionPrefab, MainCanvas);
        msgDialogStack.Add(tmpDialogue.gameObject);
        tmpDialogue.OkButton.onClick.AddListener(action);
        tmpDialogue.OkButton.onClick.AddListener(CloseDialogBox);
        tmpDialogue.CancelButton.onClick.AddListener(CloseDialogBox);
        tmpDialogue.Init(title, content);
        // 停止翻页计算
		readyToTurnPage = false;
        // curMessageBox = tmpDialogue.gameObject;
    }

    #endregion
    
    // 从下面弹出页面
    public void PopOutPage(Transform pageTran){
        isOnPopPage = true;
		pageTran.transform.position = new Vector3(Screen.width >> 1, Screen.height >> 1);
	    CanvasGroup tmpPageGroup = pageTran.GetComponent<CanvasGroup>();
		tmpPageGroup.alpha = 0.5f;
        pageTran.localScale = Vector3.one * .8f;
		tmpPageGroup.DOFade(1f, .2f).SetEase(Ease.OutSine);
		pageTran.gameObject.SetActive(true);
		pageTran.DOScale(1f, .2f).SetEase(Ease.OutSine);
        popPageStack.Add(pageTran);
    }
    // 从下面弹出页面
    public void PopUpPage(Transform pageTran){
        isOnPopPage = true;
		pageTran.transform.position = new Vector3(Screen.width >> 1, -Screen.height >> 1);
	    CanvasGroup tmpPageGroup = pageTran.GetComponent<CanvasGroup>();
		tmpPageGroup.alpha = 0.5f;
		tmpPageGroup.DOFade(1f, .3f).SetEase(Ease.OutSine);
		// pageTran.gameObject.SetActive(true);
		pageTran.DOMove(new Vector3(Screen.width >> 1, Screen.height >> 1), .25f).SetEase(Ease.InQuad);
        popPageStack.Add(pageTran);
    }

    // 从右侧弹出页面
	public void PopRightPage(Transform pageTran)
	{
        isOnPopPage = true;
		pageTran.position = new Vector3(Screen.width * 1.5f, Screen.height >> 1);
		// init museumPage
	    CanvasGroup tmpPageGroup = pageTran.GetComponent<CanvasGroup>();
		tmpPageGroup.alpha = 0.5f;
		tmpPageGroup.DOFade(1f, .3f).SetEase(Ease.InOutCubic);
		pageTran.DOMoveX(Screen.width >> 1, .3f).SetEase(Ease.InOutCubic);
        popPageStack.Add(pageTran);
	}
	void PopGongGaoListPage()
	{
        GongGaoMgr.Instance.UpdateGongGaoList();
        PopOutPage(GongGaoListPage);
	}
    public void PopMailPage(string content){
        RyanMailPage.Instance.SetContent(content);
        PopRightPage(MailPage);
    }
    public void PopGongGaoContentPage(string title, string content){
        Debug.Log(RyanGongGaoPage.Instance);
        RyanGongGaoPage.Instance.SetContent(title, content);
        PopRightPage(GongGaoContentPage);
    }
    void PopResourceMgrPage(){
        PopRightPage(ResourceMgrPage);
    }
	public void PopMuseumListPage()
	{
        PopRightPage(MuseumListPage);
        isOnMuseumList = true;
	}
	public void HideGongGaoPage()
	{
		GongGaoListPage.gameObject.SetActive(false);
        isOnPopPage = false;
	}
    // 监听时只能通过实时指定才会在不同页面正常跳转
    void CallHidePopPage(){
        HidePopPage();
    }
    //博物馆列表是否打开着
    static bool isOnMuseumList = false;
	public void HidePopPage()
	{
        if(popPageStack.Count > 0){
            Transform tmpPopTran = popPageStack[popPageStack.Count - 1];
            HidePage2Left(tmpPopTran);
            // tmpPopTran.gameObject.SetActive(false);
            popPageStack.Remove(tmpPopTran);
            if(Cur_PageIndex == PageList.Count - 1){
                isOnMuseumList = false;
            }
        }
	}

    void HidePage2Left(Transform page){
        float delta = .2f;
        CanvasGroup tmpA = page.GetComponent<CanvasGroup>();
        tmpA.DOFade(.8f, delta).SetEase(Ease.InSine);
        page.DOLocalMoveX(-Screen.width, delta).SetEase(Ease.InSine).OnComplete(
            ()=>{
                if(popPageStack.Count == 0){
                    isOnPopPage = false;
        }});
    }

	public void InsertBeaconPage(string name)
	{
		RectTransform tmpPage = Instantiate(BeaconPagePrefab, PageGrid.transform).GetComponent<RectTransform>();
		PageList.Insert(1, tmpPage);
        tmpPage.GetComponent<RyanBeaconPicItem>().Init(name);
		// tmpPage.SetSiblingIndex(1);
		if (Cur_PageIndex > 0)
		{
			PageScrollTran.localPosition = new Vector3(++Cur_PageIndex * -Screen.width,0,0);
		}
        ++beaconPageCount;
	}
    void TryExit(){
        //第一次试图退出
        if(exitCountDownT <= 0){
            exitCountDownT = 2f;
		    #if UNITY_ANDROID && !UNITY_EDITOR
            _ajc.Call<bool>("showToast","再按一次返回键退出应用");
            #else
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(ExitLabel.DOFade(1f, .1f));
            mySequence.AppendInterval(1.6f);
            mySequence.Append(ExitLabel.DOFade(0, .3f));
            #endif
        }else{
            Debug.Log("退出");
            Application.Quit();
        }
    }

	public void RemoveBeaconPage()
	{
		RectTransform tmpPage = PageList[Cur_PageIndex];
		PageList.Remove(tmpPage);
		Destroy(tmpPage.gameObject);
        --beaconPageCount;
	}

    //清空内容
    void ClearAllChilds(Transform parent){
        int tmpCount = parent.childCount;
        for(int i = 0; i < tmpCount; i++){
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    #endregion
}
