﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DaburuTools;

public enum eCardTier { Normal, Silver, Gold }
public enum eCardType { Joker, Duplicate, Smash, Draw, Shield, Movement } // Movement needs to be last for calculations.

public struct CardData
{
	public eCardTier cardTier;
	public eCardType cardType;
	public eMoveType cardMoveType;

	public CardData(eCardTier inCardTier, eCardType inCardType, eMoveType inMoveType = eMoveType.Pawn)
	{
		cardTier = inCardTier;
		cardType = inCardType;
		cardMoveType = inMoveType;
	}
}

public class Card : MonoBehaviour
{
	private CardData _cardData;
	public CardData CardData { get { return _cardData; } }

	private RectTransform _cardRectTransform = null;
	private MeshRenderer _cardMeshRen = null;
	private CardManager _cardManager = null;
	private int _cardIndex = -1;

	private Vector3 _originLocalPos;
	private float _originZ;

	private Vector3 _desiredCardLocalPos;
	private float _lerpSpeed;
	private Vector2 _prevFrameLocalPos;
	private Vector2 _tiltIntertia;

	private const float HOLDING_CARD_Z_OFFSET = 1.0f;
	private const float DRAGGING_LERP_SPEED = 20.0f;
	private const float SNAPPING_BACK_LERP_SPEED = 30.0f;
	private readonly Vector2 TILT_INTERTIA_FACTOR = new Vector2(25.0f, 12.5f);
	private const float TILT_INTERTIA_DRAG = 5.0f; 
	private const float MAX_TILT = 35.0f;

	public delegate void CardExecutionAction(int inCardIndex);
	public CardExecutionAction OnCardExecute = null;

	private void Awake()
	{
		_cardRectTransform = gameObject.GetComponent<RectTransform>();
		_cardMeshRen = gameObject.GetComponent<MeshRenderer>();
		_cardManager = transform.parent.GetComponent<CardManager>();
	}

	private void Start()
	{
		_desiredCardLocalPos = _cardRectTransform.localPosition;
		_originLocalPos = _cardRectTransform.localPosition;
		_originZ = _cardRectTransform.localPosition.z;
	}

	public void SetCardIndex(int inIndex)
	{
		_cardIndex = inIndex;
	}

	private void Update()
	{
		if (!_isAnimatingCardDraw) // TODO: Include booleans for animating card execution.
		{
			_cardRectTransform.localPosition = Vector3.Lerp(_cardRectTransform.localPosition, _desiredCardLocalPos, Time.deltaTime * _lerpSpeed);
			_cardRectTransform.localRotation = Quaternion.Euler(_tiltIntertia.y, _tiltIntertia.x, 0.0f);
			_tiltIntertia = Vector2.Lerp(_tiltIntertia, Vector2.zero, Time.deltaTime * TILT_INTERTIA_DRAG);
		}
	}

	public void EventTriggerOnPointerDown(BaseEventData data)
	{
		_cardRectTransform.localPosition += Vector3.forward * -HOLDING_CARD_Z_OFFSET;

		_desiredCardLocalPos = _cardRectTransform.localPosition;
		_tiltIntertia = Vector2.zero;
		_prevFrameLocalPos = _cardRectTransform.localPosition;
		_lerpSpeed = DRAGGING_LERP_SPEED;
	}

	public void EventTriggerOnPointerUp(BaseEventData data)
	{
		Vector3 originZ = _cardRectTransform.localPosition;
		originZ.z = _originZ;
		_cardRectTransform.localPosition = originZ;

		_desiredCardLocalPos = _originLocalPos;
		_lerpSpeed = SNAPPING_BACK_LERP_SPEED;

		PointerEventData ptrEventData = (PointerEventData)data;
		if (ptrEventData.position.y > Screen.height / 2.0f) // If released in top half of screen.
		{
			if (OnCardExecute != null) OnCardExecute(_cardIndex);
		}
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

		Vector2 tiltIntertiaDelta = _prevFrameLocalPos - (Vector2)_cardRectTransform.localPosition;
		tiltIntertiaDelta.Scale(TILT_INTERTIA_FACTOR);
		_tiltIntertia += tiltIntertiaDelta;
		_tiltIntertia = Vector2.ClampMagnitude(_tiltIntertia, MAX_TILT);
		_prevFrameLocalPos = _cardRectTransform.localPosition;
	}

	public void SetCard(eCardTier inCardTier, eCardType inCardType, eMoveType inMoveType = eMoveType.Pawn)
	{
		SetCard(new CardData(inCardTier, inCardType, inMoveType));
	}
	public void SetCard(CardData inCardData)
	{
		_cardData = inCardData;

		SetCardTexture();
	}

	private void SetCardTexture()
	{
		_cardMeshRen.material.SetTexture("_MainTex", _cardManager.GetCardTexture(_cardData.cardTier, _cardData.cardType, _cardData.cardMoveType));
	}

	bool _isAnimatingCardDraw = false;
	public void AnimateDrawCard(float inDelay = 0.0f, DTJob.OnCompleteCallback inOnComplete = null)
	{
		_isAnimatingCardDraw = true;
		_cardRectTransform.localPosition = new Vector3(-2.0f, -7.5f);
		_cardRectTransform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

		LocalMoveToAction moveToHand = new LocalMoveToAction(_cardRectTransform, _originLocalPos, 0.4f, Utils.CurveInverseExponential);
		LocalRotateToAction rotateCard = new LocalRotateToAction(_cardRectTransform, Vector3.zero, 0.6f, Utils.CurveSmoothStep);
		ActionSequence revealCard = new ActionSequence(moveToHand, rotateCard);
		revealCard.OnActionFinish += () => { _isAnimatingCardDraw = false; };
		if (inOnComplete != null) revealCard.OnActionFinish += () => { inOnComplete(); };

		ActionHandler.RunAction(new ActionAfterDelay(revealCard, inDelay));
	}
}
