using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class RyanSceneLoader : MonoBehaviour
{
	Button btn;
	public string SceneName;
	void Start()
	{
		btn = GetComponent<Button>();
		btn.onClick.AddListener(LoadClick);
	}

	void OnDisable()
	{
		btn.onClick.RemoveListener(LoadClick);
	}

	public void LoadClick()
	{
		SceneManager.LoadScene(SceneName);
	}
}
