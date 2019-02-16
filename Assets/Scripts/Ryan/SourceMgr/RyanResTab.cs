using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RyanResTab : MonoBehaviour
{
	public RyanSourceMgr.Type _type;

	public enum Type
	{
		bg,
		movie,
		view720,
		wandar
	}
	;
	//key:name	value:ids
	public Dictionary<string, List<string>> curDic = new Dictionary<string, List<string>> ();
	public Dictionary<string, string> curSize = new Dictionary<string, string> ();
	// Use this for initialization
	public void AddItem (string name, string id, string size, RyanSourceMgr.Type type)
	{
		_type = type;
		if (curDic.ContainsKey (name)) {
			curDic [name].Add (id);
			curSize [name] = (int.Parse (curSize [name]) + int.Parse (size)).ToString ();
		} else {
			curDic.Add (name, new List<string> () { id });
			curSize.Add (name, size);
		}
	}
}
