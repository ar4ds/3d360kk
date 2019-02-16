using UnityEngine;
using UnityEditor;

public class RyanSceneMgr : MonoBehaviour
{
    /// <summary>
    /// android原生代码对象
    /// </summary>
    AndroidJavaObject _ajc;
	public static int StatusBarHeight = 0;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
        //通过该API来实例化导入的arr中对应的类
        _ajc = new AndroidJavaObject("com.ar4ds.statusbar.Unity2Android");
        //请求成功
		StatusBarHeight = _ajc.Call<int>("getStatusBarHeight");
		
		AndroidStatusBar.dimmed=false;
		AndroidStatusBar.statusBarState=AndroidStatusBar.States.Visible;
		#endif
	}
}