using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingUI : MonoBehaviour
{
	RectTransform front, background;
	float totalLen = 0;
	GameObject curGO;

	void Awake()
	{
		front = transform.Find("RyanSlider/front").GetComponent<RectTransform>();
		totalLen = front.sizeDelta.x;
		front.sizeDelta = new Vector2(0, 0);
		curGO = gameObject;
	}

    public void SetLoadingBarValue(float v)
    {
		if(front != null){
			front.sizeDelta = new Vector2(totalLen * v, 5f);
		}
    }

	public void SetActive(bool b)
	{
		if(curGO != null){
			curGO.SetActive(b);
		}
	}

	public void DestroyLoadingUI()
	{
		if (curGO != null)
		{
			curGO.SetActive(false);
		}
	}
}
