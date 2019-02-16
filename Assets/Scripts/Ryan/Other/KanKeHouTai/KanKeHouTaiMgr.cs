using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;

public class KanKeHouTaiMgr : MonoBehaviour {

    public Transform ChangePasswordPage;
    public Transform ProvisionPage;
    public Transform RegisterPage;
    public InputField NameInputField, PwdInputField;
    public Button LoginBtn, ForgetPwdBtn, RegisterBtn, ChangePwdBtn;
    public Button ChangeUserInfoSubmit, ChangePwdSubmit, RegisterSubmitBtn;
    public GameObject HasLogin, NotLogin;
    public Button LogoutBtn;
    public List<GameObject> TabContents = new List<GameObject>();
    public List<Button> BaseInfoTab = new List<Button>();
    public Button BaseInfoSubmitBtn;
    //公共信，站内信
    public InfoBarItem InfoBarItemPrefab;
    public CollectionItemBar CollectionItemBarPrefab;
    public Transform InfoBarItemParent, CollectionItemBarParent;
    public Text UserNameTxt;
    public Button ProvisionBtn;
    public GridLayoutGroup UserStageTabs;
    public List<InputField> RegisterContent = new List<InputField>();
    public List<InputField> ChangePwdContent = new List<InputField>();
    public List<InputField> ChangeUserInfoContent = new List<InputField>();
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    Button[] userTabs;
    InfoBarItem curInfoBarItem;
    GongGaoBarItem curGongGaoItem;
    CollectionItemBar curCollectionItem;
    string guid, token;
    Coroutine LoadItemsCoroutine;
	// Use this for initialization
    void Awake(){
        userTabs = UserStageTabs.GetComponentsInChildren<Button>();
        m_EventSystem = FindObjectOfType<EventSystem>();
    }
	void Start () {
        InitBtnListener();
        InitUserCheckState();
        InitUserStage();
	}
	void Update(){
        // //Check if the left Mouse button is clicked
        // if (Input.GetMouseButtonDown(0))
        // {
        //     GameObject tmpSltGo = m_EventSystem.currentSelectedGameObject;
        //     if(tmpSltGo){
        //         InfoBarItem tmpLetterItem = tmpSltGo.GetComponent<InfoBarItem>();
        //         GongGaoBarItem tmpGongGaoItem = tmpSltGo.GetComponent<GongGaoBarItem>();
        //         CollectionItemBar tmpCollectItem = tmpSltGo.GetComponent<CollectionItemBar>();
        //         if(tmpLetterItem){
        //             // tmpLetterItem.OnPointerDown();
        //             // curInfoBarItem = tmpLetterItem;
        //         }else if(tmpGongGaoItem){
        //             // tmpGongGaoItem.OnPointerDown();
        //             // curGongGaoItem = tmpGongGaoItem;
        //         }else if(tmpCollectItem){
        //             // tmpCollectItem.OnPointerDown();
        //             // curCollectionItem = tmpCollectItem;
        //         }
        //     }
        // }
    }

    void InitBtnListener(){
        LoginBtn.onClick.AddListener(OnClickLoginBtn);
        ForgetPwdBtn.onClick.AddListener(OnClickForgetPwdBtn);

        RegisterBtn.onClick.AddListener(OnClickRegisterBtn);
        RegisterSubmitBtn.onClick.AddListener(OnClickRegisterSubmitBtn);
        
        LogoutBtn.onClick.AddListener(OnClickLogoutBtn);
        BaseInfoSubmitBtn.onClick.AddListener(OnClickSubmitBaseInfoBtn);

        BaseInfoTab[0].onClick.AddListener(OnClickInfoTab0Tab);
        BaseInfoTab[1].onClick.AddListener(OnClickInfoTab1Tab);
        userTabs[0].onClick.AddListener(OnClickUserStageTab0);
        userTabs[1].onClick.AddListener(OnClickUserStageTab1);
        userTabs[2].onClick.AddListener(OnClickUserStageTab2);
        userTabs[3].onClick.AddListener(OnClickUserStageTab3);
        ChangePwdBtn.onClick.AddListener(OnClickChangePwdBtn);
        ChangePwdSubmit.onClick.AddListener(OnClickChangePwdSubmitBtn);
        ChangeUserInfoSubmit.onClick.AddListener(OnClickeChangeUserInfoBtn);
        ProvisionBtn.onClick.AddListener(OnClickProvisionBtn);
    }

    void InitUserStage(){
        float tmpW = (Screen.width - 60)*.25f;
        UserStageTabs.cellSize = new Vector2(tmpW, 80f);
    }
	
    #region 看客后台
    void OnClickSubmitBaseInfoBtn(){
    }
    void InitUserCheckState(){
        string name = PlayerPrefs.GetString("UserName");
        string pwd = PlayerPrefs.GetString("Password");
        StartCoroutine(CheckLogin(name, pwd));
    }
    void OnClickLoginBtn()
    {
        string name = NameInputField.text;
        string pwd = PwdInputField.text;
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd)){
            RyanUIController.Instance.PopMessageDialogue("提示", "用户名和密码不能为空");
        }else{
            StartCoroutine(CheckLogin(name, pwd));
        }
    }
    void OnClickInfoTab0Tab(){
        ChangeKanKeHouTai_JiBenXinXi_Tab(0);
    }
    void OnClickInfoTab1Tab(){
        ChangeKanKeHouTai_JiBenXinXi_Tab(1);
    }
    void ChangeKanKeHouTai_JiBenXinXi_Tab(int index){
        for(int i = 0; i < BaseInfoTab.Count;i++){
            if(index == i){
                BaseInfoTab[i].GetComponentInChildren<Text>().color = Color.black;
            }else{
                BaseInfoTab[i].GetComponentInChildren<Text>().color = Color.grey;
            }
        }
    }
    void InitializeUI(int tabIndex){
        if(tabIndex == 1){
            OnClickUserStageTab1();
        }else{
            ChangeUserTabContent(tabIndex);
        }
    }
    IEnumerator UpdateMailContentItem(){
        //清空所有站内信
        ClearAllChilds(InfoBarItemParent);
        float tmpH = 100f;
        InfoBarItemParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width, tmpH);
        string tmpUrl = string.Format("http://3d360kk.com/mobile/getletters?uid={0}", guid);
        WWW www = new WWW(tmpUrl);
        yield return www;
        List<JSONNode> jsList = new List<JSONNode>(){};
        jsList.AddRange(JSON.Parse(www.text).Children);
        for(int i = 0; i < jsList.Count; i++){
            InfoBarItem tmpItem = Instantiate(InfoBarItemPrefab, InfoBarItemParent);
            tmpItem.Init(jsList[i]);
        }
        RectTransform contentRectTran = InfoBarItemParent.GetComponent<RectTransform>();
        contentRectTran.sizeDelta = new Vector2(0, tmpH * jsList.Count);
    }

    IEnumerator LoadContentItems(float itemHeight, string url, ItemPrefabObj prefab, Transform itemParent){
        itemParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width, itemHeight);
        // 清空列表
        ClearAllChilds(itemParent);
        WWW www = new WWW(url);
        yield return www;
        if(!string.IsNullOrEmpty(www.text)){
            List<JSONNode> jsList = new List<JSONNode>(){};
            jsList.AddRange(JSON.Parse(www.text)["museums"].Children);
            for(int i = 0; i < jsList.Count; i++){
                ItemPrefabObj tmpItem = Instantiate(prefab, itemParent);
                tmpItem.Init(jsList[i]);
            }
            RectTransform contentRectTran = itemParent.GetComponent<RectTransform>();
            contentRectTran.sizeDelta = new Vector2(0, itemHeight * jsList.Count);
        }
    }

    void ClearAllChilds(Transform parent){
        int tmpCount = parent.childCount;
        for(int i = 0; i < tmpCount; i++){
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    
    #endregion
    #region 登陆页
    IEnumerator CheckLogin(string name, string pwd)
    {
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd)){
            Debug.Log("Is Empty");
            UserNameTxt.text = "游客";
            HasLogin.SetActive(false);
            NotLogin.SetActive(true);
            ResetUserInfo();
            yield return 0;
        }else{
            string tmpUrl = string.Format("http://www.3d360kk.com/mobile/Login1?username={0}&password={1}", name, pwd);            
            WWW www = new WWW(tmpUrl);
            yield return www;
            if(!string.IsNullOrEmpty(www.text)){
                JSONNode jn = JSON.Parse(www.text.Replace("\\\"", "\""));
                Debug.Log(jn);
                if(JSON.Parse(www.text)["flag"].AsBool){
                    // RyanUIController.Instance.PopMessageDialogue("提示", "登录成功");
                    HasLogin.SetActive(true);
                    NotLogin.SetActive(false);
                    PlayerPrefs.SetString("UserName", name);
                    PlayerPrefs.SetString("Password", pwd);
                    token = jn["token"];
                    Debug.Log("token = " + token);
                    token = name;
                    JSONNode user = JSON.Parse(jn["user"].Value);
                    guid = user["UserId"];
                    InitializeUI(1);
                    PlayerPrefs.SetString("GUID", guid);
                    UserNameTxt.text = name;
                    ChangeUserInfoContent[0].text = user["MP"];
                    ChangeUserInfoContent[1].text = user["Email"];
                    // 更新站内信列表
                    StartCoroutine(UpdateMailContentItem());
                }else{
                    RyanUIController.Instance.PopMessageDialogue("提示", "用户名或密码错误");
                    HasLogin.SetActive(false);
                    NotLogin.SetActive(true);
                }
            }
        }
        GongGaoMgr.Instance.UpdateGongGaoNumberTag();
        PwdInputField.text = "";
    }

    void ResetUserInfo(){
        foreach(InputField inFld in ChangeUserInfoContent){
            inFld.text = "";
        }
    }

    void OnClickLogoutBtn(){
        HasLogin.SetActive(false);
        NotLogin.SetActive(true);
        PlayerPrefs.DeleteKey("UserName");
        PlayerPrefs.DeleteKey("Password");
        PlayerPrefs.DeleteKey("GUID");
        UserNameTxt.text = "游客";
        GongGaoMgr.Instance.UpdateGongGaoNumberTag();
    }

    void OnClickForgetPwdBtn()
    {
        OnClickChangePwdBtn();
    }

    void OnClickRegisterBtn()
    {
        // 重置输入框
        foreach(InputField myInputFld in RegisterContent){
            myInputFld.text = "";
        }
        RyanUIController.Instance.PopOutPage(RegisterPage);
    }
    // 提交注册资料时
    void OnClickRegisterSubmitBtn(){
        StartCoroutine(TriggerRegist());
    }
    IEnumerator TriggerRegist(){
        string name = RegisterContent[0].text;
        string pwd = RegisterContent[1].text;
        string pwd2 = RegisterContent[2].text;
        if(name == "" || pwd == ""){
            RyanUIController.Instance.PopMessageDialogue("提示", "用户名和密码不能为空");
        }else if(!RyanGlobalProps.regex.IsMatch(pwd)){
            RyanUIController.Instance.PopMessageDialogue("提示", "密码必须是8-16位的数字、字符组合（不能是纯数字）");
        }else if(!pwd.Equals(pwd2)){
            RyanUIController.Instance.PopMessageDialogue("提示", "两次输入的密码不一致");
        }else{
            string tmpUrl = string.Format("http://www.3d360kk.com/mobile/Regist?data={{"
                    +"Username:'{0}',"
                    +"Password:'{1}',"
                    +"MP:'{2}',"
                    +"Email:'{3}'}}",
                    RegisterContent[0].text,
                    RegisterContent[1].text,
                    RegisterContent[4].text,
                    RegisterContent[3].text);
            WWW www = new WWW(tmpUrl);
            Debug.Log(tmpUrl);
            yield return www;
            if(JSON.Parse(www.text)["flag"].AsBool){
                RyanUIController.Instance.PopMessageDialogue("提示", "注册成功");
                Debug.Log("Hello?");
                RyanUIController.Instance.HidePopPage();
                yield return new WaitForSeconds(2);
                // 跳转到起始页
                StartCoroutine(CheckLogin(RegisterContent[0].text, RegisterContent[1].text));
            }else{
                RyanUIController.Instance.PopMessageDialogue("提示", "注册失败");
            }
        }
    }
    void OnClickProvisionBtn(){
        RyanUIController.Instance.PopRightPage(ProvisionPage);
    }
    void OnClickChangePwdBtn(){
        RyanUIController.Instance.PopRightPage(ChangePasswordPage);
        foreach(InputField input in ChangePwdContent){
            input.text = "";
        }
    }
    void OnClickeChangeUserInfoBtn(){
        StartCoroutine(ChangeUserInfo());
    }

    IEnumerator ChangeUserInfo(){

        Regex telRegex = new Regex(@"^[1]+[3,5]+\d{9}");
        Regex emailRegex = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;");
        if(!telRegex.IsMatch(ChangeUserInfoContent[0].text)){
            RyanUIController.Instance.PopMessageDialogue("提示", "电话格式不正确");
        } else if(!emailRegex.IsMatch(ChangeUserInfoContent[0].text)){
            RyanUIController.Instance.PopMessageDialogue("提示", "邮箱格式不正确");
        }else{
            string tmpUrl = string.Format("http://www.3d360kk.com/mobile/changeUserInfo?data={{"
                    +"UserId:'{0}',"
                    +"MP:'{1}',"
                    +"Email:'{2}'}}",
                    guid,
                    ChangeUserInfoContent[0].text,
                    ChangeUserInfoContent[1].text);
            WWW www = new WWW(tmpUrl);
            Debug.Log(tmpUrl);
            yield return www;
            if(JSON.Parse(www.text)["flag"].AsBool){
                RyanUIController.Instance.PopMessageDialogue("提示", "修改成功");
            }else{
                RyanUIController.Instance.PopMessageDialogue("提示", "修改失败");
            }
        }
    }
    void OnClickChangePwdSubmitBtn(){
        StartCoroutine(ChangePwdCheck());
    }
    IEnumerator ChangePwdCheck(){
        string _oldPwd = ChangePwdContent[0].text;
        string _newPwd = ChangePwdContent[1].text;
        string _repeatPwd = ChangePwdContent[2].text;
        Regex regex = new Regex(@"
            (?=.*[0-9])                     #必须包含数字
            (?=.*[a-zA-Z])                  #必须包含小写或大写字母
            .{8,16}                         #至少8个字符，最多16个字符
            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        if(PlayerPrefs.GetString("Password") != _oldPwd){
            RyanUIController.Instance.PopMessageDialogue("提示", "原密码不正确");
        }
        else if(!regex.IsMatch(_newPwd)){
            RyanUIController.Instance.PopMessageDialogue("提示", "密码必须是8-16位的数字、字符组合（不能是纯数字）");
        }
        else if(_newPwd != _repeatPwd){
            RyanUIController.Instance.PopMessageDialogue("提示", "两次填写的密码不一致");
        }else{
            string tmpUrl = string.Format("http://3d360kk.com/mobile/changePWD?token={0}&pwd0={1}&pwd1={2}",
            token, _oldPwd, _newPwd);
            WWW www = new WWW(tmpUrl);
            yield return www;
            Debug.Log(www.text);
            RyanUIController.Instance.PopMessageDialogue("提示", "密码修改成功");
            RyanUIController.Instance.HidePopPage();
            PlayerPrefs.SetString("Password", _newPwd);
        }
    }
    void OnClickUserStageTab0(){
        ChangeUserTabContent(0);
    }
    void OnClickUserStageTab1(){
        ChangeUserTabContent(1);
        if(LoadItemsCoroutine != null){
            StopCoroutine(LoadItemsCoroutine);
        }
        // 加载列表内容
        LoadItemsCoroutine = StartCoroutine(LoadContentItems(180f,
            string.Format("http://www.3d360kk.com/mobile/query_museum?token={0}", guid),
            //"http://www.3d360kk.com/mobile/load_collection",
            CollectionItemBarPrefab, CollectionItemBarParent));
    }
    void OnClickUserStageTab2(){
        ChangeUserTabContent(2);
    }
    void OnClickUserStageTab3(){
        ChangeUserTabContent(3);
    }

    void ChangeUserTabContent(int bntIndex){//Button btn){
        for(int i = 0; i < userTabs.Length; i++){
            if(bntIndex == i){
                userTabs[i].GetComponent<Image>().color = new Color(254f/256f, 123f/256f, 0f);
                userTabs[i].GetComponentInChildren<Text>().color = Color.white;
                TabContents[i].SetActive(true);
            }else{
                userTabs[i].GetComponent<Image>().color = Color.white;
                userTabs[i].GetComponentInChildren<Text>().color = new Color(254f/256f, 123f/256f, 0f);
                TabContents[i].SetActive(false);
            }
        }
    }

    #endregion
}
