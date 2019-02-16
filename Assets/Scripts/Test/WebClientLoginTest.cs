using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

public class WebClientLoginTest : MonoBehaviour {

    string url = "http://www.3d360kk.com/mobile/Login1?username=demo&password=1";
    string res = "no result";
    // Use this for initialization
    void Start()
    {
        StartCoroutine(GetWebData());
        //WebClient myWebClient = new WebClient();
        //myWebClient.Credentials = CredentialCache.DefaultCredentials;
        //byte[] pageData = myWebClient.DownloadData("http://www.3d360kk.com/mobile/Login1?username=demo&password=1");
        //string pageHtml = Encoding.UTF8.GetString(pageData);
        //Debug.Log("pageHtml" + pageHtml);
    }

    IEnumerator GetWebData()
    {
        WWW www = new WWW(url);
        yield return www;
        res = www.text;
        Debug.Log(www.text);
    }

    void OnGUI()
    {
        Debug.Log(res);
        GUILayout.Label(res);
    }

}
