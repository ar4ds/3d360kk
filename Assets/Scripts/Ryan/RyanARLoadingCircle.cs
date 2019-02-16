using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RyanARLoadingCircle : MonoBehaviour
{
	SpriteRenderer circleImage;
	bool isCalculate = false;

	void Awake ()
	{
		circleImage = GetComponent<SpriteRenderer> ();
	}

	public void Show (bool b)
	{
		isCalculate = b;
		circleImage.enabled = b;
	}

	void Update ()
	{
		if (isCalculate) {
			CalCircleRotation ();
		}
	}

	void CalCircleRotation ()
	{
		transform.Rotate (Vector3.back * Time.deltaTime * 222f);
	}
}
