using UnityEngine;
using System.Collections;

public class FingerCtrl720 : MonoBehaviour
{
	float curAngleX = 0;
	float curAngleY = 0;
	float curAddDepth;
	Camera myCamera;
	Transform cameraTran;
	float scrollSpeedZ = -3f;
	float curDepth;
	float clampNear = 30f;
	float clampFar = 80f;

	void Start ()
	{
		myCamera = Camera.main;
		curDepth = myCamera.fieldOfView = clampFar;
		cameraTran = myCamera.transform;
	}

	void Update ()
	{
		RotateOperate ();
		MouseScroll ();
	}

	#region rotate Operation
	void RotateOperate ()
	{
		if (Input.GetMouseButtonDown (0)) {
			curAngleX = 0;
			curAngleY = 0;
		} else if (Input.GetMouseButton (0)) {
			//参考角度的位置
			curAngleX += Input.GetAxis ("Mouse X") * 1.5f;
			curAngleY += Input.GetAxis ("Mouse Y") * 1.5f;
		}
		if (curAngleX > 0.001f || curAngleX < -0.001f) {
			curAngleX = Mathf.Lerp (curAngleX, 0, Time.deltaTime * 15f);
			transform.Rotate (Vector3.up, curAngleX);
		}
		if (curAngleY > 0.001f || curAngleY < -0.001f) {
			curAngleY = Mathf.Lerp (curAngleY, 0, Time.deltaTime * 25f);
			Vector3 tmpY = cameraTran.eulerAngles + Vector3.right * curAngleY;
			if (tmpY.x < 270f && tmpY.x >= 180f) {
				tmpY = Vector3.right * 270f;
			} else if (tmpY.x > 90f && tmpY.x < 180f) {
				tmpY = Vector3.right * 90;
			}
			cameraTran.eulerAngles = tmpY;
		}
	}
	#endregion

	public void MouseScroll ()
	{
		float curMScroll = Input.GetAxis ("Mouse ScrollWheel");
		if (curMScroll != 0) {
			curAddDepth += curMScroll * scrollSpeedZ;
		}
		if (curAddDepth != 0) {
			curAddDepth = Mathf.Lerp (curAddDepth, 0, Time.deltaTime * 5f);
			curDepth += curAddDepth;
			curDepth = Mathf.Clamp (curDepth, clampNear, clampFar);
			myCamera.fieldOfView = curDepth;
		}
	}

	public void Reset ()
	{
		curAngleX = 0;
		transform.eulerAngles = new Vector3 (0, 90f, 0);
	}
}