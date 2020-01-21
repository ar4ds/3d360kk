using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using DG.Tweening;

public class MiniLoginDialogBoxMgr : MonoBehaviour {

    static MiniLoginDialogBoxMgr _instance;
    public static MiniLoginDialogBoxMgr Instance{
        get{return _instance;}
    }
    // 遮罩层，防止点击到下一层
    public GameObject BlockLayer;
    public RectTransform MainLoginPage, RegisterPage, ProvisionPage, ChangePasswordPage;
    public InputField NameInputField, PwdInputField;
    public Button LoginBtn, ForgetPwdBtn, RegisterBtn;
    public Button RegisterSubmitBtn;
    public Button ChangePwdSubmit;
	public Button ProvisionBtn;
    public List<InputField> RegisterContent = new List<InputField>();
    public List<Button> ExitBtnList = new List<Button>();
    public List<InputField> ChangePwdContent = new List<InputField>();
    // 打开的页面
    List<RectTransform> pageStack = new List<RectTransform>();
    string guid, token;
	void Start () {
        _instance = this;
        MainLoginPage.gameObject.SetActive(false);
        BlockLayer.SetActive(false);
        InitBtnListener();
	}

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            OnClickExitBtn();
        }
    }

    void InitBtnListener(){
        LoginBtn.onClick.AddListener(OnClickLoginBtn);
        ForgetPwdBtn.onClick.AddListener(OnClickForgetPwdBtn);

        RegisterBtn.onClick.AddListener(OnClickRegisterBtn);
        RegisterSubmitBtn.onClick.AddListener(OnClickRegisterSubmitBtn);
        
        ProvisionBtn.onClick.AddListener(OnClickProvisionBtn);
        ChangePwdSubmit.onClick.AddListener(OnClickChangePwdSubmitBtn);

        foreach(Button btn in ExitBtnList){
            btn.onClick.AddListener(OnClickExitBtn);
        }
    }

    public void ShowPage(){
        pageStack = new List<RectTransform>();
        HidePage2RightImmediate(RegisterPage);
        HidePage2RightImmediate(ProvisionPage);
        NameInputField.text = "";
        PwdInputField.text = "";
        pageStack.Add(MainLoginPage);
        RegisterPage.gameObject.SetActive(true);
        ProvisionPage.gameObject.SetActive(true);
        BlockLayer.SetActive(true);
        PopMainPage();
    }
	
    #region 看客后台
    void OnClickLoginBtn()
    {
        StartCoroutine(CheckLogin(NameInputField.text, PwdInputField.text));
    }
    IEnumerator LoadContentItems(float itemHeight, string url, ItemPrefabObj prefab, Transform itemParent){
        itemParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width, itemHeight);
        // 清空列表
        ClearAllChilds(itemParent);
        WWW www = new WWW(url);
        yield return www;
        List<JSONNode> jsList = new List<JSONNode>(){};
        jsList.AddRange(JSON.Parse(www.text).Children);
        for(int i = 0; i < jsList.Count; i++){
            ItemPrefabObj tmpItem = Instantiate(prefab, itemParent);
            tmpItem.Init(jsList[i]);
        }
        RectTransform contentRectTran = itemParent.GetComponent<RectTransform>();
        contentRectTran.sizeDelta = new Vector2(0, itemHeight * jsList.Count);
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
            RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "用户名和密码不能为空");
            yield return 0;
        }else{
            string tmpUrl = string.Format("http://www.3d360kk.com/mobile/Login?username={0}&password={1}", name, pwd);
            WWW www = new WWW(tmpUrl);
            yield return www;
            Debug.Log(JSON.Parse(www.text).ToString());
            if(JSON.Parse(www.text)["flag"].AsBool){
                PlayerPrefs.SetString("UserName", name);
                PlayerPrefs.SetString("Password", pwd);
                token = JSON.Parse(www.text)["token"];
                    Debug.LogError(token);
                Debug.Log("token = " + token);
                PlayerPrefs.SetString("token", token);
                HideMainPage();
                RyanShowLoadOptions.Instance.CollectScene();
            }else{
                RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "用户名或密码错误");
            }
        }
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
            RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "用户名和密码不能为空");
        } else if(!RyanGlobalProps.regex.IsMatch(pwd)){
            RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "密码必须是8-16位的数字、字符组合（不能是纯数字）");
        }else if(!pwd.Equals(pwd2)){
            RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "两次输入的密码不一致");
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
            yield return www;
            if(JSON.Parse(www.text)["flag"].AsBool){
                RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "注册成功", 
                ()=>{
                    // 跳转到起始页
                    StartCoroutine(CheckLogin(RegisterContent[0].text, RegisterContent[1].text));
                });
            }else{
                RyanShowLoadOptions.Instance.PopMessageDialogue("提示", "注册失败");
            }
        }
    }

    void OnClickForgetPwdBtn()
    {
        OnClickChangePwdBtn();
    }
    void OnClickChangePwdBtn(){
        PopPageFromRight(ChangePasswordPage);
        foreach(InputField input in ChangePwdContent){
            input.text = "";
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

    void OnClickRegisterBtn()
    {
        foreach(InputField myInputFld in RegisterContent){
            myInputFld.text = "";
        }
        PopPageFromRight(RegisterPage);
    }
    void OnClickProvisionBtn(){
        PopPageFromRight(ProvisionPage);
    }
    void OnClickExitBtn(){
        if(pageStack.Count > 0){
            RectTransform tmpPage = pageStack[pageStack.Count-1];
            pageStack.Remove(tmpPage);
            if(pageStack.Count > 0){
                HidePage2Left(tmpPage);
            }else{
                HideMainPage();
            }
        }else{
            RyanShowLoadOptions.Instance.Btn_BackToMain();
        }
    }

    void PopPageFromRight(RectTransform page){
        HidePage2RightImmediate(page);
        float delta = .15f;
        CanvasGroup tmpA = page.GetComponent<CanvasGroup>();
        tmpA.alpha = 0;
        tmpA.DOFade(1f, delta);
        page.DOLocalMoveX(0, delta);
        pageStack.Add(page);
    }

    void HidePage2Left(RectTransform page){
        float delta = .2f;
        CanvasGroup tmpA = page.GetComponent<CanvasGroup>();
        tmpA.DOFade(0, delta);
        page.DOLocalMoveX(-MainLoginPage.sizeDelta.x, delta);
    }

    void HidePage2RightImmediate(Transform page){
        page.localPosition = Vector3.right * MainLoginPage.sizeDelta.x;
    }

    void HideMainPage(){
        CanvasGroup page = MainLoginPage.GetComponent<CanvasGroup>();
        float delta = .1f;
        page.alpha = .5f;
        MainLoginPage.DOScale(.9f, delta);
        page.DOFade(0, delta).OnComplete(()=>{MainLoginPage.gameObject.SetActive(false);});
        BlockLayer.SetActive(false);
    }

    void PopMainPage(){
        CanvasGroup page = MainLoginPage.GetComponent<CanvasGroup>();
        float delta = .1f;
        MainLoginPage.gameObject.SetActive(true);
        page.alpha = .5f;
        MainLoginPage.DOScale(1f, delta);
        page.DOFade(1, delta);
    }

    #endregion
}
