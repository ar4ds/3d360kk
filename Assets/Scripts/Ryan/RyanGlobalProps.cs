using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class RyanGlobalProps : MonoBehaviour
{
	//public static string MAIN_MUSEUM_PATH;
	public static int StatusBarHeight;
	public static string CurrentMuseumName;
	//当前的场景是720还是漫游等等
	public static string CurrentScene;
	public static string VIDEO_URL;
	public static string VRMainBG_URL;
	public static string VIEW720Index_URL;
	public static string TRAVEL_URL;
	public static string GuestGUID = "920a6b9d-4aaf-410d-9baa-de9b0aa3fe2d";
    // 缓存文件夹得位置
    public static string mainSceneName = "Pages";
	public static string cacheDir = Application.persistentDataPath + "/net_cache/";
	public static string indexPath = cacheDir + "index";
    public static Regex regex = new Regex(@"
        (?=.*[0-9])                     #必须包含数字
        (?=.*[a-zA-Z])                  #必须包含小写或大写字母
        .{8,16}                         #至少8个字符，最多16个字符
        ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

    public static string Platform_URL
	{
		get
		{
			string curPlatform = "";
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				curPlatform = "/iOS/";
			}
			else if (Application.platform == RuntimePlatform.OSXEditor
				|| Application.platform == RuntimePlatform.Android)
			{
				curPlatform = "/Android/";
			}
			return curPlatform;
		}
	}

	// 获取距离当前时间的表示方式(xx年前，1个月前，3天前，5小时前，28分钟，刚刚)
	public static string GetRelativeTimeFormat(DateTime msgDate){
		DateTime nowDate = DateTime.Now;
		int tmpOffset;
		tmpOffset = int.Parse(nowDate.ToString("yyyy")) - int.Parse(msgDate.ToString("yyyy"));
		if(tmpOffset > 0){
			return tmpOffset + "年前";
		}
		tmpOffset = int.Parse(nowDate.ToString("MM")) - int.Parse(msgDate.ToString("MM"));
		if(tmpOffset > 0){
			return tmpOffset + "个月前";
		}
		tmpOffset = int.Parse(nowDate.ToString("dd")) - int.Parse(msgDate.ToString("dd"));
		if(tmpOffset > 1){
			return tmpOffset + "天前";
		}else if(tmpOffset == 1){
			return "昨天";
		}
		tmpOffset = int.Parse(nowDate.ToString("HH")) - int.Parse(msgDate.ToString("HH"));
		if(tmpOffset > 0){
			return tmpOffset + "小时前";
		}
		tmpOffset = int.Parse(nowDate.ToString("mm")) - int.Parse(msgDate.ToString("mm"));
		if(tmpOffset > 0){
			return tmpOffset + "分钟前";
		}
		return "刚刚";
	}

	public static string PREFIX_URL = "http://www.3d360kk.com";
	public static string ARScan_URL = PREFIX_URL + "/upload/ARScan";

	public static void SetLoadURL(string sceneName, string id)
	{
		string curPlatform = "";
		if (Application.platform == RuntimePlatform.OSXEditor
			|| Application.platform == RuntimePlatform.IPhonePlayer)
		{
			curPlatform = "/iOS/";
		}
		CurrentMuseumName = sceneName;
		VIDEO_URL = PREFIX_URL + "/" + sceneName + "/mp4/video.mp4";
		//VIEW720Index_URL = PREFIX_URL + "/mobile/panoramalist?pid=" + id;
		VIEW720Index_URL = PREFIX_URL + "/api/museum/Panoramalist?id=" + id;
		// 背景图地址
		VRMainBG_URL = PREFIX_URL + "/api/museum/list?id=" + id;
		TRAVEL_URL = PREFIX_URL + "/upload/" + sceneName + curPlatform + "travelModel.unity3d";
		Debug.Log("VIDEO_URL: " + VIDEO_URL);
		Debug.Log("VIEW720_URL: " + VIEW720Index_URL);
		Debug.Log("TRAVEL_URL: " + TRAVEL_URL);
	}
}
