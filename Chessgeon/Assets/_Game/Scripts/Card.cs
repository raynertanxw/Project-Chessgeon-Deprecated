using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
	// TODO: Card types and attributes.

	private Vector3 _originLocalPos;
	private RectTransform _cardRectTransform = null;
	private float _originZ;

	private Vector3 _desiredCardLocalPos;
	private float _lerpSpeed;

	private const float HOLDING_CARD_Z_OFFSET = 1.0f;
	private const float DRAGGING_LERP_SPEED = 20.0f;
	private const float SNAPPING_BACK_LERP_SPEED = 30.0f;

	private void Awake()
	{
		_cardRectTransform = gameObject.GetComponent<RectTransform>();
	}

	private void Start()
	{
		_desiredCardLocalPos = _cardRectTransform.localPosition;
		_originLocalPos = _cardRectTransform.localPosition;
		_originZ = _cardRectTransform.localPosition.z;
	}

	private void Update()
	{
		_cardRectTransform.localPosition = Vector3.Lerp(_cardRectTransform.localPosition, _desiredCardLocalPos, Time.deltaTime * _lerpSpeed);
	}

	public void EventTriggerOnPointerDown(BaseEventData data)
	{
		_cardRectTransform.localPosition += Vector3.forward * -HOLDING_CARD_Z_OFFSET;

		_desiredCardLocalPos = _cardRectTransform.localPosition;
		_lerpSpeed = DRAGGING_LERP_SPEED;
	}

	public void EventTriggerOnPointerUp(BaseEventData data)
	{
		Vector3 originZ = _cardRectTransform.localPosition;
		originZ.z = _originZ;
		_cardRectTransform.localPosition = originZ;

		_desiredCardLocalPos = _originLocalPos;
		_lerpSpeed = SNAPPING_BACK_LERP_SPEED;
	}

	public void EventTriggerOnDrag(BaseEventData data)
	{
		PointerEventData ptrEventData = (PointerEventData)data;
		Vector3 desiredPos = UICamera.Camera.ScreenToWorldPoint(ptrEventData.position);

		// TODO: Figure out why this code needs such arbitrary values. Would be best if the InverseTransformPoint worked.
		Vector3 localVerOfDesiredPos = UICamera.Camera.transform.InverseTransformPoint(desiredPos);
		localVerOfDesiredPos.z = -HOLDING_CARD_Z_OFFSET;
		_desiredCardLocalPos = localVerOfDesiredPos + new Vector3(0.68f, 5.0f);
		//Debug.Log(desiredPos + " : " + localVerOfDesiredPos);
	}
}
