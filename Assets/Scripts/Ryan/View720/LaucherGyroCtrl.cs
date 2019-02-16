using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaucherGyroCtrl : MonoBehaviour
{
	List<Quaternion> _quatList = new List<Quaternion>();
	int _buffCount = 2;
	float x0 = 0;
	float y0 = 0;
	float z0 = 0;
	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Input.gyro.enabled = true;
		for (int i = 0; i < _buffCount; i++)
		{
			_quatList.Add(new Quaternion());
		}
	}

	//void ViewMoving()
	//{
	//	//获取陀螺仪旋转度数
	//	transform.rotation = GetBufferAttitude();
	//	//相机向上旋转90度
	//	transform.Rotate(Vector3.right, -90f, Space.World);
	//	//反转陀螺仪XY参数
	//	Quaternion tmpQ = transform.rotation;
	//	transform.rotation = new Quaternion(-tmpQ.x, -tmpQ.y, tmpQ.z, tmpQ.w);
	//}

	void ViewMoving()
	{
		//获取陀螺仪旋转度数
		transform.rotation = GetBufferAttitude();
		//相机向上旋转90度
		transform.Rotate(Vector3.right, -90f, Space.World);
		//反转陀螺仪XY参数
		Quaternion tmpQ = transform.rotation;
		transform.rotation = new Quaternion(-tmpQ.x, -tmpQ.y, tmpQ.z, tmpQ.w);
	}

	Quaternion GetBufferAttitude()
	{
		Quaternion tmpQ = Input.gyro.attitude;
		float x1 = Input.acceleration.x;
		float xOffset = Mathf.Abs(x0 - x1);
		x0 = x1;

		float y1 = Input.acceleration.y;
		float yOffset = Mathf.Abs(y0 - y1);
		y0 = y1;

		float z1 = Input.acceleration.z;
		float zOffset = Mathf.Abs(z0 - z1);
		z0 = z1;

		float x = tmpQ.x;
		float y = tmpQ.y;
		float z = tmpQ.z;
		float w = tmpQ.w;

		//flip calculate progress edge
		float v0 = _quatList[_buffCount - 1].w;
		float v1 = tmpQ.w;
		if ((v0 < 0 && v1 >= 0)
			|| (v0 > 0 && v1 <= 0)
			|| (v0 == 0 && v1 != 0))
		{
			for (int i = 0; i < _buffCount; i++)
			{
				_quatList[i] = tmpQ;
			}
			return tmpQ;
		}

		if (xOffset + yOffset + zOffset > 0.01f)
		{
			for (int i = 1; i < _quatList.Count; i++)
			{
				x += _quatList[i].x;
				y += _quatList[i].y;
				z += _quatList[i].z;
			}
			x /= _buffCount;
			y /= _buffCount;
			z /= _buffCount;
		}
		else {
			x = _quatList[_quatList.Count - 1].x;
			y = _quatList[_quatList.Count - 1].y;
			z = _quatList[_quatList.Count - 1].z;
		}
		for (int i = 1; i < _quatList.Count; i++)
		{
			w += _quatList[i].w;
		}
		tmpQ = new Quaternion(x, y, z, w / _buffCount);
		_quatList.RemoveAt(0);
		_quatList.Add(tmpQ);
		return tmpQ;
	}

	void Update()
	{
		ViewMoving();
		//RotateOperate();
	}

	#region rotate Operation
	float curAngleX = 0;
	float curAngleY = 0;
	float deltaX = 0;
	void RotateOperate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			curAngleX = 0;
			curAngleY = 0;
		}
		else if (Input.GetMouseButton(0))
		{
			//参考角度的位置
			curAngleX += Input.GetAxis("Mouse X") * 1.5f;
			curAngleY += Input.GetAxis("Mouse Y") * 1.5f;
		}
		if (curAngleX > 0.001f || curAngleX < -0.001f)
		{
			curAngleX = Mathf.Lerp(curAngleX, 0, Time.deltaTime * 15f);
			//transform.Rotate(Vector3.up, curAngleX);
			deltaX += curAngleX;
		}
		//if (curAngleY > 0.001f || curAngleY < -0.001f)
		//{
		//	curAngleY = Mathf.Lerp(curAngleY, 0, Time.deltaTime * 25f);
		//	Vector3 tmpY = camera.eulerAngles + Vector3.right * curAngleY;
		//	if (tmpY.x < 270f && tmpY.x >= 180f)
		//	{
		//		tmpY = Vector3.right * 270f;
		//	}
		//	else if (tmpY.x > 90f && tmpY.x < 180f)
		//	{
		//		tmpY = Vector3.right * 90;
		//	}
		//	camera.eulerAngles = tmpY;
		//}
	}
	#endregion
}