using UnityEngine;
using System.Collections;

public class RyanScreenOrientation : MonoBehaviour
{
	public ScreenOrientation InitialOrient = ScreenOrientation.Portrait;

	void Awake()
	{
		Screen.orientation = InitialOrient;
	}
}
