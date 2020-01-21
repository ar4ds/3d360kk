using UnityEngine;
using UnityEngine.UI;

public class FenLeiItemA : MonoBehaviour {

	RectTransform rectTran;
	GameObject fenleiGroupParent;
	Button button;
	Transform grid;
	Text text;
	int index;
	bool isOpen;
	void Awake(){
		button = GetComponent<Button>();
		rectTran = GetComponent<RectTransform>();
		text = button.GetComponentInChildren<Text>();
		grid = button.GetComponentInChildren<GridLayoutGroup>().transform;
	}
	// Use this for initialization
	public void Init (int index, string name, string[] itmes, Button childPrefab) {
		this.index = index;
		button.name = name;
		text.text = name;
		float c = 1f;
		text.color = new Color(c,c,c,1f);
		foreach(string str in itmes){
			Button btn = Instantiate(childPrefab, grid);
			btn.onClick.AddListener(OnClickChildBtn);
			btn.GetComponentInChildren<Text>().text = str;
			btn.name = str;
		}
		button.onClick.AddListener(OnButtonClicked);
	}

	public RectTransform GetRectTransform(){
		return rectTran;
	}

	void OnButtonClicked(){
		//	开合分类项时，重置UI布局
		if(isOpen){
			CloseGrid();
			RyanUIController.Instance.RecalSearchLayer(index,0);
		}else{
			OpenGrid();
			RyanUIController.Instance.RecalSearchLayer(index,grid.childCount);
		}
		RyanUIController.Instance.CloseOtherGrid(index);
	}

	void OnClickChildBtn(){
		RyanUIController.Instance.PopMuseumListPage();
	}

	public void CloseGrid(){
			float c = .15f;
			text.color = new Color(c,c,c,1f);
			Go.to(grid, .1f, new GoTweenConfig().scale(new Vector2(1f, 0)).setEaseType(GoEaseType.CircIn));
			isOpen = false;
	}
	public void OpenGrid(){
			float c = 1f;
			text.color = new Color(c,c,c,1f);
			Go.to(grid, .1f, new GoTweenConfig().scale(new Vector2(1f, 1)).setEaseType(GoEaseType.CircIn));
			isOpen = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
