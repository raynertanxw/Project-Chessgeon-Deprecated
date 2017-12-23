using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
	// TODO: Card types and attributes.

	private Vector2 _originAnchorPos;
	private RectTransform _cardRectTransform = null;
	private float _originZ;

	private const float HOLDING_CARD_Z_OFFSET = 1.0f;

	private void Awake()
	{
		_cardRectTransform = gameObject.GetComponent<RectTransform>();
		_originAnchorPos = _cardRectTransform.anchoredPosition;
		_originZ = _cardRectTransform.position.z;
	}

	public void EventTriggerOnPointerDown(BaseEventData data)
	{
		_cardRectTransform.position += Vector3.forward * -HOLDING_CARD_Z_OFFSET;
	}

	public void EventTriggerOnPointerUp(BaseEventData data)
	{
		_cardRectTransform.anchoredPosition = _originAnchorPos;
		Vector3 originZ = _cardRectTransform.position;
		originZ.z = _originZ;
		_cardRectTransform.position = originZ;
	}

	public void EventTriggerOnDrag(BaseEventData data)
	{
		PointerEventData ptrEventData = (PointerEventData)data;
		Vector3 desiredPos = UICamera.Camera.ScreenToWorldPoint(ptrEventData.position);
		desiredPos.z = _cardRectTransform.position.z;
		_cardRectTransform.position = desiredPos;
	}
}
