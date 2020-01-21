using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using SimpleJSON;

public class InfoBarItem : ItemPrefabObj
{
    public Text ContentTxt, TimeTxt;
    Image barImage;
    RectTransform rectTran;
    float waitT;
    bool isPressed = false;
    string id;
    string context;
    Vector3 lastMousePosition;

    void Update()
    {
        if (isPressed && RyanUIController.curM_xOffset == 0)
        {
            waitT += Time.deltaTime;
            if (waitT > 1f)
            {
                OnPressedSucceed();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // click按下，非长按操作
                if (Input.mousePosition == lastMousePosition)
                {
                    OnBarClick();
                }
                OnCancelPressedMode();
            }
        }
    }
    public override void Init(JSONNode jn)
    {
        barImage = transform.GetComponent<Image>();
        this.id = jn["id"];
        this.context = jn["Text"];
        ContentTxt.text = context;
        TimeTxt.text = System.DateTime.Parse(jn["Date"]).ToString("yyyy.MM.dd");
        //判断是否已读
        if (PlayerPrefs.GetString(string.Format("letter:{0},{1}", PlayerPrefs.GetString("token"), id)) != "")
        {
            SetReadedState();
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        waitT = 0;
        isPressed = true;
        barImage.color = new Color(1f, 1f, 1f, .5f);
        lastMousePosition = Input.mousePosition;
    }
    void OnBarClick()
    {
        Debug.Log("OnBarClick");
        // 弹出站内信
        RyanUIController.Instance.PopMailPage(ContentTxt.text);
        PlayerPrefs.SetString(string.Format(string.Format("letter:{0},{1}", PlayerPrefs.GetString("token"), id)), "readed");
        SetReadedState();
    }
    void OnCancelPressedMode()
    {
        isPressed = false;
        barImage.color = new Color(1f, 1f, 1f, 0f);
    }

    void SetReadedState()
    {
        TimeTxt.color = ContentTxt.color = Color.grey * .25f;
    }

    void OnPressedSucceed()
    {
        PopMailPage();
        OnCancelPressedMode();
    }

    void PopMailPage()
    {
        Debug.Log("Press Succeed.");
        UnityAction tmpAction = null;
        tmpAction += RemoveLetter;
        RyanUIController.Instance.PopOptionDialogue("提示", "确认删除？", tmpAction);
    }

    void RemoveLetter()
    {
        StartCoroutine(DeleteLetter());
    }
    //删除站内信
    IEnumerator DeleteLetter()
    {
        string url = string.Format("http://www.3d360kk.com/api/letters/revoke?token={0}&id={1}",PlayerPrefs.GetString("token"), id);
        UnityWebRequest req = new UnityWebRequest(url);
        yield return req.SendWebRequest();
        Debug.Log("delete!");
        Destroy(gameObject);
    }
    IEnumerator DeleteLetterxxx()
    {
        string url = "http://www.3d360kk.com/api/letters/revoke";
        UnityWebRequest req = new UnityWebRequest(url);
        req.downloadHandler = new DownloadHandlerBuffer();
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerPrefs.GetString("token"));
        form.AddField("id", id);
        UnityWebRequest.Post(url, form);
        yield return req.SendWebRequest();
        Debug.Log("delete!");
        Destroy(gameObject);
    }
}