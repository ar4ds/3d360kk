using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RyanCalGrid : MonoBehaviour
{
	
	void Start ()
	{
		GridLayoutGroup grid = GetComponent<GridLayoutGroup> ();
//		grid.spacing = new Vector2((Screen.width -  grid.cellSize.x * 5) / 5f, 0);
	}
}
