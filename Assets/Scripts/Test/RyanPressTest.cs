using UnityEngine;
using UnityEngine.EventSystems;

public class RyanPressTest : MonoBehaviour {
	Vector3 lastMousePose;
    EventSystem m_EventSystem;
	float curT = 0;
	// 是否已经被选择
	bool isPressed = false;
	void Start(){
        m_EventSystem = FindObjectOfType<EventSystem>();
	}
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			lastMousePose = Input.mousePosition;
		}
        if (Input.GetMouseButton(0) && !isPressed && lastMousePose == Input.mousePosition)
        {
			curT += Time.deltaTime;
			// 长按1秒
			if(curT >= 1f){
            	Debug.Log(m_EventSystem.currentSelectedGameObject + " was pressed.");
				isPressed = true;
			}
        }
		if(Input.GetMouseButtonUp(0)){
			isPressed = false;
			curT = 0;
		}
	}
}
