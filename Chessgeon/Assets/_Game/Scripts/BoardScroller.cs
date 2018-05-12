using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DaburuTools;

public class BoardScroller : MonoBehaviour
{
	private static BoardScroller _instance = null;

	public delegate void BeginDrag(Vector2 inPointerPos);
	public delegate void Drag(Vector2 inPointerPos);

	public static BeginDrag OnBeginDrag = null;
	public static Drag OnDrag = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Constants.DESIGN_WIDTH, Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH));
		}
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public void EventTriggerOnBeginDrag(BaseEventData data)
	{
		if (OnBeginDrag != null) OnBeginDrag(((PointerEventData)data).position);
	}

	public void EventTriggerOnDrag(BaseEventData data)
	{
		if (OnDrag != null) OnDrag(((PointerEventData)data).position);
	}
}
