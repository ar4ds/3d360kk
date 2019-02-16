using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using DG.Tweening;

public class RyanBeaconManager : MonoBehaviour
{
    public GameObject BeaconTitle;
    public Button HelloBeacon;
    public Text BeaconDataText;
    public Button QuitPageBtn;
    BroadcastState bs_State;
    private List<Beacon> mybeacons = new List<Beacon>();
    float mouseClickLastT = 0;
    float titleHeight;
    float mouseXPose;
    bool hasMouseMoved = false;  //鼠标是否移动过
    RyanBeaconManager _instance;
    public RyanBeaconManager Instance {
        get { return _instance; }
    }
    string beaconUUIDStr;
    public static bool Show_Title = false;

    void Start()
    {
        // StartCoroutine(InsertBeaconPages(""));
        _instance = this;
        titleHeight = BeaconTitle.GetComponent<RectTransform>().sizeDelta.y;
        bs_State = BroadcastState.inactive;
        HelloBeacon.onClick.AddListener(() => {
            SwitchBeaconState();
        });
		QuitPageBtn.onClick.AddListener(HidePage);
        // 隐藏标题栏
        BeaconTitle.transform.position += Vector3.up * titleHeight;
        On();
    }
    void Update(){
        if(RyanUIController.Instance.CanCalPage()){
            CalBeaconPage();
        }
    }

	void HidePage(){
		RyanUIController.Instance.RemoveBeaconPage();
        //检查下一页是否Beacon页
        if(!IsBeaconPage(RyanUIController.Cur_PageIndex)){
            HideBeaconTitle();
        }
	}

    void CalBeaconPage(){
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
#else
		if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Began)
#endif
        {
            hasMouseMoved = false;
            mouseClickLastT = Time.time;
            mouseXPose = Input.mousePosition.x;
        } 
#if UNITY_EDITOR
        else if(Input.GetMouseButtonUp(0))
#else
		else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Ended)
#endif
        {
            if(mouseClickLastT + .5f > Time.time){
                // 没有移动手指，并且是Beacon页，就能显隐标题栏
                if(!hasMouseMoved && IsBeaconPage(RyanUIController.Cur_PageIndex)){
                    SwitchBeaconTitle();
                }
            }
        }
#if UNITY_EDITOR
		else if (Input.GetMouseButton(0))
#else
		else if(Input.touchCount == 1 && Input.touches [0].phase == TouchPhase.Moved)
#endif
		{
            if(mouseXPose > Input.mousePosition.x){
                hasMouseMoved = true;
                if(!IsBeaconPage(RyanUIController.Cur_PageIndex + 1)){
                    HideBeaconTitle();
                }
            }else if(mouseXPose < Input.mousePosition.x){
                hasMouseMoved = true;
                if(!IsBeaconPage(RyanUIController.Cur_PageIndex - 1)){
                   HideBeaconTitle();
                }
            }
        }
    }
    bool isBeaconTitleHide = true;
    void SwitchBeaconTitle(){
        if(isBeaconTitleHide){
            ShowBeaconTitle();
        }else{
            HideBeaconTitle();
        }
    }
    void HideBeaconTitle(){
        isBeaconTitleHide = true;
        BeaconTitle.transform.DOMoveY(titleHeight + Screen.height, .2f).SetEase(Ease.InOutSine);
        BeaconTitle.GetComponent<Image>().CrossFadeAlpha(0, .5f, true);
    }
    void ShowBeaconTitle(){
        isBeaconTitleHide = false;
        BeaconTitle.transform.DOMoveY(Screen.height, .2f).SetEase(Ease.InOutSine);
        BeaconTitle.GetComponent<Image>().CrossFadeAlpha(0.5f, .5f, true);
    }
    public bool IsBeaconPage(int index){
        if(index < RyanUIController.PageList.Count - 2  && index > 0){
            return true;
        }else{
            return false;
        }
    }

    // BroadcastState
    void SwitchBeaconState()
    {
        /*** Beacon will start ***/
        if (bs_State == BroadcastState.inactive)
        {
            On();
        }
        else
        {
            Off();
        }
    }
    void On(){
        #region ReceiveMode
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        iBeaconReceiver.regions = new iBeaconRegion[] { new iBeaconRegion("", new Beacon()) };
        // !!! Bluetooth has to be turned on !!! TODO
        iBeaconReceiver.Scan();
        #endregion
        bs_State = BroadcastState.active;
        BeaconDataText.text = "开始寻找目标...";
    }
    void Off(){
        iBeaconReceiver.Stop();
        iBeaconReceiver.BeaconRangeChangedEvent -= OnBeaconRangeChanged;
        removeFoundBeacons();
        bs_State = BroadcastState.inactive;
    }

    private void OnBeaconRangeChanged(Beacon[] beacons)
    {
        foreach (Beacon b in beacons)
        {
            var index = mybeacons.IndexOf(b);
            if (index == -1)
            {
                // 添加beacon页
                mybeacons.Add(b);
                StartCoroutine(InsertBeaconPages(b.UUID));
            }
            else
            {
                mybeacons[index] = b;
            }
        }
        for (int i = mybeacons.Count - 1; i >= 0; --i)
        {
            if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now)
            {
                mybeacons.RemoveAt(i);
            }
        }
        // DisplayOnBeaconFound();
    }

    IEnumerator InsertBeaconPages(string uuid){
        string url = string.Format("http://www.3d360kk.com/mobile/getbeacon?guid={0}", uuid);
        // url = "http://www.3d360kk.com/mobile/getbeacon?guid=2F234454-CF6D-4A0F-ADF2-F4911BA9FFA6";
        WWW www = new WWW(url);
        yield return www;
        if(JSON.Parse(www.text)["flag"].AsBool){
            foreach(JSONNode jn in JSON.Parse(JSON.Parse(www.text)["photos"]).Children){
                RyanUIController.Instance.InsertBeaconPage(jn["photo_name"].Value);
            }
        }
    }

    private void removeFoundBeacons()
    {
        Debug.Log("removing all found Beacons");
        BeaconDataText.text = "关闭寻找Beacon设备";
    }

    private void DisplayOnBeaconFound()
    {
        removeFoundBeacons();
        string tmpTxt = Time.time + "\ncount = " + mybeacons.Count + "\n";
        foreach (Beacon b in mybeacons)
        {
            tmpTxt += "#######################";
            switch (b.type)
            {
                case BeaconType.iBeacon:
                    tmpTxt += "\nUUID:";
                    tmpTxt += b.UUID.ToString();
                    break;
                default:
                    break;
            }
        }
        BeaconDataText.text = tmpTxt;
    }
}