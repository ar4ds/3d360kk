using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using RippleGen;
using SimpleJSON;
using System.IO;

public class RyanSourceMgr : MonoBehaviour
{
    public RyanResListItem ResItemPrefab;
    public Transform ResListRoot;
    GridLayoutGroup resGrid;
    public List<Button> BtnList = new List<Button>();

    RyanResTab bgTab = new RyanResTab();
    RyanResTab movieTab = new RyanResTab();
    RyanResTab view720Tab = new RyanResTab();
    RyanResTab wandarTab = new RyanResTab();
    Dictionary<Type, RyanResTab> typeDic = new Dictionary<Type, RyanResTab>();

    public enum Type
    {
        bg,
        movie,
        view720,
        wandar,
        other
    }
    ;
    Vector2 itemSize = new Vector2(0, 50f);
    Request myRequest;
    Rect itemRect;

    internal void Init()
    {
        Debug.Log("init resource");
        typeDic.Clear();
        itemRect = ResItemPrefab.GetComponent<RectTransform>().rect;
        resGrid = ResListRoot.GetComponent<GridLayoutGroup>();
        resGrid.cellSize = new Vector2(Screen.width, 100);
        Screen.orientation = ScreenOrientation.Portrait;
        typeDic.Add(Type.bg, bgTab);
        typeDic.Add(Type.movie, movieTab);
        typeDic.Add(Type.view720, view720Tab);
        typeDic.Add(Type.wandar, wandarTab);
        GetJSONList();
        ShowBgSourceList();
        InitBtnListener();
    }

    void InitBtnListener()
    {
        BtnList[0].onClick.AddListener(ShowBgSourceList);
        BtnList[1].onClick.AddListener(ShowMovieSourceList);
        BtnList[2].onClick.AddListener(ShowView720SourceList);
        BtnList[3].onClick.AddListener(ShowWandarSourceList);
    }

    //Ryan
    void GetJSONList()
    {
        JSONNode jn = _readIndex();
        if (jn == null || jn.AsObject == null)
        {
            return;
        }
        foreach (string k in jn.Keys)
        {
            if (jn[k]["status"].Value == "over" && !string.IsNullOrEmpty(jn[k]["type"].Value))
            {
                string tmpTypeStr = jn[k]["type"].Value;
                Type tmpType = (Type)Enum.Parse(typeof(Type), tmpTypeStr);
                typeDic[tmpType].AddItem(jn[k]["name"].Value, k, jn[k]["size"].Value, tmpType);
            }
        }
    }

    void ShowBgSourceList()
    {
        InitResList(bgTab);
    }

    void ShowMovieSourceList()
    {
        InitResList(movieTab);
    }

    void ShowView720SourceList()
    {
        InitResList(view720Tab);
    }

    void ShowWandarSourceList()
    {
        InitResList(wandarTab);
    }

    void InitResList(RyanResTab tab)
    {
        //清空所有子对象
        List<Transform> tmpCList = new List<Transform>();
        for (int i = 0; i < ResListRoot.childCount; i++)
        {
            tmpCList.Add(ResListRoot.GetChild(i));
        }
        for (int i = 0; i < tmpCList.Count; i++)
        {
            Destroy(tmpCList[i].gameObject);
        }
        tmpCList.Clear();
        List<string> keys = new List<string>(tab.curDic.Keys);
        
        GameObject scrollViewGO = ResListRoot.parent.gameObject;
        scrollViewGO.transform.localPosition = Vector3.zero;

        ResListRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (resGrid.cellSize.y + resGrid.spacing.y) * keys.Count);
        for (int i = 0; i < keys.Count; i++)
        {
            RyanResListItem tmpResItem = Instantiate(ResItemPrefab);
            Transform tmpResTran = tmpResItem.transform;
            tmpResTran.parent = ResListRoot;
            tmpResTran.GetComponent<RectTransform>().sizeDelta = new Vector2(0, resGrid.cellSize.y);
            tmpResItem.InitItem(keys[i], tab.curSize[keys[i]], tab.curDic[keys[i]], this, tab._type);
        }

        //		int _count = 100;
        //		ResListRoot.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, (resGrid.cellSize.y + resGrid.spacing.y) * _count);
        //		for (int i = 0; i < _count; i++) {
        //			RyanResListItem tmpResItem = Instantiate (ResItemPrefab);
        //			tmpResItem.transform.Find ("DisplayName").GetComponent<Text> ().text = i + 1 + ". name";
        //			Transform tmpResTran = tmpResItem.transform;
        //			tmpResTran.parent = ResListRoot;
        //			tmpResTran.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, resGrid.cellSize.y);
        //		}
    }

    string GetDisplayName(string url)
    {
        string display = url.Replace(RyanGlobalProps.PREFIX_URL, "");
        display = display.Remove(display.LastIndexOf('/'));
        return display;
    }

    public void DeleteCaches(string name, List<string> filenames, RyanSourceMgr.Type type)
    {
        // http://www.3d360kk.com/images/2/1.jpg
        for (int i = 0; i < filenames.Count; i++)
        {
            //string url = "http://www.3d360kk.com/images/2/1.jpg";
            //Request.Delete(url);
            // DeleteCache(filenames[i]);
            Request.DeleteLocal(filenames[i]);
        }
        typeDic[type].curDic.Remove(name);
    }

    void DeleteCache(string filename)
    {
        JSONNode jn = _readIndex();
        jn.Remove(filename);
        _writeIndex(jn);
        //Request.Delete(RyanGlobalProps.cacheDir + filename);
        File.Delete(RyanGlobalProps.cacheDir + filename);
    }

    static JSONNode _readIndex()
    {
        if (!File.Exists(RyanGlobalProps.indexPath))
        {
            return null;
        }
        return JSON.Parse(File.ReadAllText(RyanGlobalProps.indexPath));
    }

    static void _writeIndex(JSONNode node)
    {
        File.WriteAllText(RyanGlobalProps.indexPath, node.ToString());
    }
}