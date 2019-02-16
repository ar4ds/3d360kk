using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadingCubeSque : MonoBehaviour
{

	float curT;
	float frameT;
	int frameCount = 27;
	int curF = 1;
	Image curImage;
	string _prefix;
	public List<Sprite> LoadSpritList = new List<Sprite> ();

	void Start ()
	{
		frameT = 1f / 25f;
		curImage = GetComponent<Image> ();
		frameCount = LoadSpritList.Count;
	}
	// Update is called once per frame
	void Update ()
	{
		curT += Time.deltaTime;
		if (curT >= frameT) {
			curT = 0;
			curImage.sprite = LoadSpritList [GetNextIndex ()];
		}
	}

	int GetNextIndex ()
	{
		if (++curF > frameCount - 1) {
			curF = 0;
		}
		return curF;
	}
}