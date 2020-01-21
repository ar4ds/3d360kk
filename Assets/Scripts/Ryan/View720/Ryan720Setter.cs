using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using RippleGen;
using UnityEngine.Networking;

public class Ryan720Setter : RippleGen.Core.MonoBehaviour
{
    public Transform SpherePrefab;
    public LoadingUI myLoadingUI;
    public Transform MapOptionPanel;
    public Texture blackTex;
    RyanScrollRectCell scrollCtrl;
    GameObject curSphere;
    AssetBundleCreateRequest request;

    GoTweenConfig _mapOptionTweenCfg;
    GoTweenChain goChain;
    GoTween _mapOptionTween;
    string lastFilePath = "";

    void Awake()
    {
        SceneManager.sceneLoaded += InitScene;
    }
    protected override void Start()
    {
        base.Start();
        scrollCtrl = MapOptionPanel.GetComponent<RyanScrollRectCell>();
        InitMapPanelTween();
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= InitScene;
        base.OnDestroy();
    }

    void InitScene(Scene arg0, LoadSceneMode arg1)
    {
        //RoteCube.SetActive(false);
        lastFilePath = "";
        curSphere = Instantiate(SpherePrefab).gameObject;
        curSphere.gameObject.SetActive(false);
        curSphere.transform.localScale = new Vector3(-1f, 1f, 1f);
        StartCoroutine(DownloadTxtAndThumbnails());
        // DownloadTxtAndThumbnails();
        loadedThumbnailCount = 0;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameObject.AddComponent<GyroTest>();
        }
        else
        {
            gameObject.AddComponent<GyroTest>();
            //curSphere.AddComponent<FingerCtrl720>();
        }
    }

    void LateUpdate()
    {
        if (loadedThumbnailCount > 0)
        {
            if (thumbnailList != null && loadedThumbnailCount == thumbnailList.Count)
            {
                thumbnailList = null;
            }
        }
    }

    void InitMapPanelTween()
    {
        goChain = new GoTweenChain();
        _mapOptionTweenCfg = new GoTweenConfig();
        _mapOptionTweenCfg.position(new Vector3(MapOptionPanel.position.x, 0, 0));
        _mapOptionTweenCfg.easeType = GoEaseType.CubicInOut;
        _mapOptionTween = new GoTween(MapOptionPanel, .51f, _mapOptionTweenCfg);
        goChain.append(_mapOptionTween);

    }

    public void CleanAsset()
    {
        if (request != null && request.assetBundle)
        {
            request.assetBundle.Unload(true);
        }
    }

    public void SwitchMapPannel(bool b)
    {
        if (b)
        {
            goChain.playForward();
        }
        else
        {
            goChain.playBackwards();
        }
    }

    #region Thumbnails

    IEnumerator DownloadTxtAndThumbnails()
    {
        string serverPath = RyanGlobalProps.VIEW720Index_URL;
        Debug.LogError(serverPath);
        string dirPath = RyanGlobalProps.cacheDir + RyanGlobalProps.CurrentMuseumName;
        string localPath = Path.Combine(dirPath, "order.txt");
        Debug.LogError(localPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        UnityWebRequest req = new UnityWebRequest(serverPath);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();
        if (!string.IsNullOrEmpty(req.error))
        {
            Debug.LogError(req.error);
        }
        if (!string.IsNullOrEmpty(req.error))
        {
            Debug.LogError(req.error);
            // 联网失败，调用本地文件
            if (File.Exists(localPath))
            {
                GetThumbnailsFromTxt(File.ReadAllText(localPath));
            }
            else
            {
                Debug.LogError("文件不存在！");
            }
        }
        else
        {
            // 联网成功，更新本地文件
            File.WriteAllText(localPath, req.downloadHandler.text);
            GetThumbnailsFromTxt(req.downloadHandler.text);
        }
    }

    int loadedThumbnailCount = 0;
    List<string> thumbnailList;

    void GetThumbnailsFromTxt(string content)
    {
        thumbnailList = new List<string>();
        Pics720Json pics = JsonUtility.FromJson<Pics720Json>(content);
        // 封面第一张自动加载
        bool isFirstLoaded = false;
        foreach(var item in pics.pics){
            if(!isFirstLoaded){
                isFirstLoaded = true;
                LoadFirstTex(item.large);
            }
            DownLoadThumbnails(item.icon, item.large);
        }
    }

    void DownLoadThumbnails(string icon, string large)
    {
        Request myRequest;
        myRequest = new Request(icon);
        myRequest.OnComplete.Add((r) =>
        {
            StartCoroutine(AddThumbnail2Bottom(myRequest.responseFile, large));
        });
        myRequest.OnProgress.Add((r, p) =>
        {
            Debug.Log (p);
        });
        myRequest.ReadCache = true;
        myRequest.CheckExpire = false;
        addOperation(myRequest);
    }

    IEnumerator AddThumbnail2Bottom(string filePath, string url720)
    {
        string tmpUrl = "file://" + filePath;
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(tmpUrl);
        req.downloadHandler = new DownloadHandlerTexture();
        yield return req.SendWebRequest();
        if (req.isDone)
        {
            //add thumbnail to bottom menu list
            scrollCtrl.InsertScroll(DownloadHandlerTexture.GetContent(req), url720);
            ++loadedThumbnailCount;
        }
    }

    #endregion

    string curSphereTex = "";

    Request myRequest;
    public void Load720Texture(string tmpStr)
    {
        if (curSphereTex == tmpStr)
        {
            return;
        }
        myLoadingUI.SetActive(true);
        curSphereTex = tmpStr;
        myRequest = new Request(tmpStr);
        curSphere.GetComponent<Renderer>().material.mainTexture = blackTex;
        //RoteCube.SetActive(true);
        myRequest.OnComplete.Add((r) =>
        {
            myLoadingUI.SetActive(false);
            //RoteCube.SetActive(false);
            if (myRequest.error == null)
            {
                StartCoroutine(LoadTexture(myRequest.responseFile));
            }
            else
            {
                SendMessage("BackToMainScene");
            }
        });
        myRequest.OnProgress.Add((r, p) =>
        {
            myLoadingUI.SetLoadingBarValue(p);
            Debug.Log(p + "%");
        });
        myRequest.ReadCache = true;
        addOperation(myRequest);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if(myRequest != null){
            myRequest.Cancel();
        }
    }

    void LoadFirstTex(string url)
    {
        Request myRequest;
        curSphereTex = url;
        myLoadingUI.SetActive(true);
        myRequest = new Request(url);
        myRequest.OnComplete.Add((r) =>
        {
            Debug.Log("Loading Complete..." + myRequest.responseFile);
            myLoadingUI.SetActive(false);
            if (myRequest.error == null)
            {
                StartCoroutine(LoadTexture(myRequest.responseFile));
            }
            else
            {
                SendMessage("BackToMainScene");
            }
        });
        myRequest.OnProgress.Add((r, p) =>
        {
            myLoadingUI.SetLoadingBarValue(p);
        });
        myRequest.ReadCache = true;
        myRequest.CheckExpire = false;
        addOperation(myRequest);
    }
    IEnumerator LoadTexture(string filePath)
    {
        string tmpUrl = "file://" + filePath;
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(tmpUrl);
        req.downloadHandler = new DownloadHandlerTexture();
        yield return req.SendWebRequest();
        if (req.isDone)
        {
			Texture2D t2d = DownloadHandlerTexture.GetContent(req);
            if (t2d == null)
            {
                File.Delete(filePath);
                SendMessage("BackToMainScene");
            }
            else
            {
                lastFilePath = filePath;
                curSphere.GetComponent<Renderer>().material.mainTexture = t2d;
                curSphere.SetActive(true);
            }
        }
    }
}