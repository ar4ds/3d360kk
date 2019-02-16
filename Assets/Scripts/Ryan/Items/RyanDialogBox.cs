using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RyanDialogBox : MonoBehaviour {
	public Text Title, Content;
	public Button OkButton, CancelButton;
	public void Init (string title, string content) {
		Title.text = title;
		Content.text = content;
		// OkButton.onClick.AddListener(()=>{RyanUIController.Instance.ConfirmMessageDialogBox();});
	}
}
