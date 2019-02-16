using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RyanScrollRectCell : MonoBehaviour
{
	public Button BtnPrefab;
	List<Button> btnList = new List<Button> ();
	Transform floorRoot;
	RectTransform tmpRTran;
	Vector2 cellSize;
	Ryan720Setter my720setter;

	void Awake ()
	{
		floorRoot = transform.Find ("FloorRoot");
		tmpRTran = floorRoot.GetComponent<RectTransform> ();
		cellSize = floorRoot.GetComponent<GridLayoutGroup> ().cellSize;
		if (Camera.main) {
			my720setter = Camera.main.GetComponent<Ryan720Setter> ();
		}
	}

	//wandar
	public List<Button> CreateScroll (int cellCount)
	{
		for (int i = 0; i < cellCount; i++) {
			btnList.Add (Instantiate (BtnPrefab, floorRoot));
		}
		// float rectWidth = cellSize.x * cellCount;
		// Rect tmpR = tmpRTran.rect;
		// tmpRTran.sizeDelta = new Vector2 (rectWidth, tmpR.height);
		return btnList;
	}

	public List<Button> CreateScroll (List<Texture> thumbnailList)
	{
		for (int i = 0; i < thumbnailList.Count; i++) {
			btnList.Add (Instantiate (BtnPrefab, floorRoot));
		}
		// float rectWidth = cellSize.x * thumbnailList.Count;
		// Rect tmpR = tmpRTran.rect;
		// tmpRTran.sizeDelta = new Vector2 (rectWidth, tmpR.height);
		return btnList;
	}

	public List<Button> InsertScroll (Texture2D thumbnail, string url)
	{
		Button tmpBtn = Instantiate (BtnPrefab, floorRoot);
		Destroy (tmpBtn.GetComponentInChildren<Text> ().gameObject);
		Sprite tmpP = Sprite.Create (thumbnail, new Rect (0, 0, thumbnail.width, thumbnail.height), Vector2.zero);
		tmpBtn.image.sprite = tmpP;
		// tmpBtn.transform.parent = floorRoot;
		btnList.Add (tmpBtn);
		Rect tmpR = tmpRTran.rect;
		tmpRTran.sizeDelta += new Vector2 (cellSize.x, 0);
		tmpRTran.sizeDelta = new Vector2 (tmpRTran.sizeDelta.x, tmpR.height);
		tmpBtn.onClick.AddListener (delegate () {
			Load720Texture (url);
		});
		return btnList;
	}

	void Load720Texture (string url)
	{
		if(my720setter != null){
			my720setter.Load720Texture (url);
		}
	}
}
