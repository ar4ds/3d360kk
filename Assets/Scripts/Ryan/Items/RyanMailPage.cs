using UnityEngine;
using UnityEngine.UI;

public class RyanMailPage : MonoBehaviour {
	public Text Content;
	static RyanMailPage _instance;
	public static RyanMailPage Instance{
		get{return _instance;}
	}
	void Start(){
		_instance = this;
	}
	// Use this for initialization
	public void SetContent (string content) {
		Content.text = content;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
