using UnityEngine;
using UnityEngine.SceneManagement;

//ARKit&Vuforia的关闭按钮，临时测试用
public class TestReturn : MonoBehaviour
{

	public void ReturnMain()
	{
		SceneManager.LoadScene("ScenePages");
	}
}
