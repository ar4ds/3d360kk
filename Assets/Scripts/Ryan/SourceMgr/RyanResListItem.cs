using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RyanResListItem : MonoBehaviour
{
	Text displayName;
	string _name;
	List<string> _idList = new List<string> ();
	Button delBtn;
	RyanSourceMgr _smgr;
	RyanSourceMgr.Type _type;

	public void InitItem (string name, string fileSize, List<string> idList, RyanSourceMgr smgr, RyanSourceMgr.Type type)
	{
		_name = name;
		_idList = idList;
		_smgr = smgr;
		_type = type;
		transform.Find ("DisplayName").GetComponent<Text> ().text = name;
		string sizeStr = (float.Parse (fileSize) / 1024f / 1024f).ToString ("F2") + "M";
		transform.Find ("FileSize").GetComponent<Text> ().text = sizeStr;
		delBtn = transform.Find ("DelBtn").GetComponent<Button> ();
		delBtn.onClick.AddListener (() => DeleteItem ());
	}

	void DeleteItem ()
	{
		_smgr.DeleteCaches (_name, _idList, _type);
		Destroy (gameObject);
	}
}
