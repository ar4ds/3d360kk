using UnityEngine;
using SimpleJSON;
using UnityEngine.EventSystems;

public abstract class ItemPrefabObj : MonoBehaviour, IPointerDownHandler{
	public abstract void Init(JSONNode jn);

    public abstract void OnPointerDown(PointerEventData eventData);
}