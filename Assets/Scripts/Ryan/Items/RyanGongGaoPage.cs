using UnityEngine;
using UnityEngine.UI;

public class RyanGongGaoPage : MonoBehaviour {
	public Text Title;
	public Text Content;
	static RyanGongGaoPage _instance;
	public static RyanGongGaoPage Instance{
		get{return _instance;}
	}
	void Start(){
		_instance = this;
	}
	// Use this for initialization
	public void SetContent (string title, string content) {
		Title.text = title;
		Content.text = content;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
