using UnityEngine;
using System.Collections;

public class RyanMapTrigger : MonoBehaviour
{
	public Texture myTexture;

	void OnTriggerEnter (Collider other)
	{
		if (other.GetComponent<CharacterController> ()) {
			SetMapPicture ();
		}
	}

	public void SetMapPicture ()
	{
		RyanTravelSetter.SetMapPicture (myTexture);
	}
}
