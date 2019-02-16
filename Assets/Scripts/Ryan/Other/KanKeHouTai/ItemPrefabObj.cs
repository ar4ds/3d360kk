using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class ItemPrefabObj : MonoBehaviour, IPointerDownHandler{
	public abstract void Init(JSONNode jn);

    public abstract void OnPointerDown(PointerEventData eventData);
}
