﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DaburuTools;

public enum eCardTier { Normal, Silver, Gold }
public enum eCardType { Joker, Clone, Smash, Draw, Movement } // Movement needs to be last for calculations.

public struct CardData
{
	public eCardTier cardTier;
	public eCardType cardType;
	public eMoveType cardMoveType;
	public bool isCloned;

	public CardData(eCardTier inCardTier, eCardType inCardType, bool inIsCloned, eMoveType inMoveType = eMoveType.Pawn)
	{
		cardTier = inCardTier;
		cardType = inCardType;
		isCloned = inIsCloned;
		cardMoveType = inMoveType;
	}
}

public class Card : MonoBehaviour
{
	private CardData _cardData;
	public CardData CardData { get { return _cardData; } }

	private Canvas _cardCanvas = null;
	private RectTransform _cardRectTransform = null;
	private Image _cardFrontImage = null;
	private Image _cardBackImage = null;
	private Image _clonedIconImage = null;
	private CardManager _cardManager = null;
	private int _cardIndex = -1;
	public int CardIndex { get { return _cardIndex; } }
	private bool _isEnabled = false;
	public bool IsEnabled { get { return _isEnabled; } }

	private Vector3 _originLocalPos;
	public Vector3 OriginLocalPos { get { return _originLocalPos; } }
	private float _originZ;

	private bool _isDragging = false;
	private Vector3 _desiredCardLocalPos;
	private Vector3 _desiredCardWorldPos;
	private float _lerpSpeed;
	private Vector2 _prevFrameLocalPos;
	private Vector2 _tiltIntertia;

	private const float DRAGGING_LERP_SPEED = 20.0f;
	private const float SNAPPING_BACK_LERP_SPEED = 30.0f;
	private readonly Vector2 TILT_INTERTIA_FACTOR = new Vector2(0.25f * Screen.width / 1080.0f, 0.25f * Screen.height / 1920.0f);
	private readonly Vector2 TILT_INTERTIA_DRAG = new Vector2(5.0f * Screen.width / 1080.0f, 5.0f * Screen.height / 1920.0f); 
	private const float MAX_TILT = 35.0f;
	private const float MAX_TILT_DELTA = 5.0f;
	private readonly Vector3 DRAG_SCALE = new Vector3(2.5f, 2.5f, 1.0f);
	private const float SCALE_LERP_SPEED = 20.0f;
	private readonly Vector3 DRAG_HOLDING_POINT_OFFSET = new Vector3(0.0f, 250.0f * Screen.height / 1920.0f, 0.0f);
	private const int CARD_CANVAS_NORMAL_SORT_ORDER = 3;
	private const int CARD_CANVAS_DRAGGING_SORT_ORDER = 4;

	public delegate void CardExecutionAction(int inCardIndex);
	public CardExecutionAction OnCardExecute = null;

	public bool IsAnimating
	{
		get
		{
			return
				_isAnimatingCardDraw ||
				_isAnimatingCardExecute ||
				_isAnimatingMoveToOtherCardPos;
		}
	}

	private void Awake()
	{
		_cardCanvas = gameObject.GetComponent<Canvas>();
		_cardRectTransform = gameObject.GetComponent<RectTransform>();
		_cardBackImage = transform.GetChild(0).GetComponent<Image>();
		_cardFrontImage = transform.GetChild(1).GetComponent<Image>();
		_clonedIconImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
		_cardManager = transform.parent.parent.GetComponent<CardManager>();

		Debug.Assert(_cardCanvas.overrideSorting, "Card Canvas should override sorting");
		_cardCanvas.sortingOrder = CARD_CANVAS_NORMAL_SORT_ORDER;

		SetEnabled(true);
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
		if (!IsAnimating)
		{
			if (_isDragging)
			{
				_cardRectTransform.position = Vector3.Lerp(_cardRectTransform.position, _desiredCardWorldPos, Time.deltaTime * _lerpSpeed);
				_cardRectTransform.localScale = Vector3.Lerp(_cardRectTransform.localScale, DRAG_SCALE, Time.deltaTime * SCALE_LERP_SPEED);
			}
			else
			{
				_cardRectTransform.localPosition = Vector3.Lerp(_cardRectTransform.localPosition, _desiredCardLocalPos, Time.deltaTime * _lerpSpeed);
				_cardRectTransform.localScale = Vector3.Lerp(_cardRectTransform.localScale, Vector3.one, Time.deltaTime * SCALE_LERP_SPEED);
			}

            _cardRectTransform.localRotation = Quaternion.Euler(_tiltIntertia.y, _tiltIntertia.x, 0.0f);
			_tiltIntertia.x = Mathf.Lerp(_tiltIntertia.x, 0.0f, Time.deltaTime * TILT_INTERTIA_DRAG.x);
			_tiltIntertia.y = Mathf.Lerp(_tiltIntertia.y, 0.0f, Time.deltaTime * TILT_INTERTIA_DRAG.y);
		}
	}

	public void EventTriggerOnPointerDown(BaseEventData data)
	{
		if (!_isAnimatingCardExecute)
		{
			_isDragging = true;
			_cardCanvas.sortingOrder = CARD_CANVAS_DRAGGING_SORT_ORDER;

			_desiredCardWorldPos = _cardRectTransform.position + DRAG_HOLDING_POINT_OFFSET;
			_desiredCardLocalPos = _cardRectTransform.localPosition;
			_tiltIntertia = Vector2.zero;
			_prevFrameLocalPos = _cardRectTransform.localPosition;
			_lerpSpeed = DRAGGING_LERP_SPEED;
		}
	}

	public void EventTriggerOnPointerUp(BaseEventData data)
	{
		if (_isDragging)
		{
			_isDragging = false;
			_cardCanvas.sortingOrder = CARD_CANVAS_NORMAL_SORT_ORDER;

			Vector3 originZ = _cardRectTransform.localPosition;
			originZ.z = _originZ;
			_cardRectTransform.localPosition = originZ;

			_desiredCardLocalPos = _originLocalPos;
			_lerpSpeed = SNAPPING_BACK_LERP_SPEED;

			PointerEventData ptrEventData = (PointerEventData)data;
			if (ptrEventData.position.y > _cardManager.CardUseYThreshold
				&& !_isAnimatingCardExecute) // NOTE: Prevents double execution from clicking while execute anim is running.
			{
				if (OnCardExecute != null) OnCardExecute(_cardIndex);
			}
		}
	}

	public void EventTriggerOnDrag(BaseEventData data)
	{
		if (_isDragging)
		{
			PointerEventData ptrEventData = (PointerEventData)data;
			_desiredCardWorldPos = ((Vector3)ptrEventData.position) + DRAG_HOLDING_POINT_OFFSET;
			_desiredCardWorldPos.z = _cardRectTransform.position.z;

			Vector2 tiltIntertiaDelta = _prevFrameLocalPos - (Vector2)_cardRectTransform.localPosition;
			tiltIntertiaDelta.Scale(TILT_INTERTIA_FACTOR);
			tiltIntertiaDelta = Vector2.ClampMagnitude(tiltIntertiaDelta, MAX_TILT_DELTA);
			_tiltIntertia += tiltIntertiaDelta;
			_tiltIntertia = Vector2.ClampMagnitude(_tiltIntertia, MAX_TILT);
			_prevFrameLocalPos = _cardRectTransform.localPosition;
		}
	}

	public void SetCard(eCardTier inCardTier, eCardType inCardType, bool inIsCloned = false, eMoveType inMoveType = eMoveType.Pawn)
	{
		SetCard(new CardData(inCardTier, inCardType, inIsCloned, inMoveType));
	}
	public void SetCard(CardData inCardData)
	{
		_cardData = inCardData;
		_clonedIconImage.enabled = _cardData.isCloned;

		SetCardSprite();
	}

	private void SetCardSprite()
	{
		_cardBackImage.sprite = _cardManager.GetCardBackSprite(_cardData.cardTier);
		_cardFrontImage.sprite = _cardManager.GetCardFrontSprite(_cardData.cardTier, _cardData.cardType, _cardData.cardMoveType);
	}

	bool _isAnimatingCardDraw = false;
	public void AnimateDrawCard(float inDelay = 0.0f, DTJob.OnCompleteCallback inOnComplete = null)
	{
		_isAnimatingCardDraw = true;
		_cardRectTransform.localPosition = new Vector3(700.0f, 700.0f, 0.0f);
		_cardRectTransform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

		LocalMoveToAction moveToHand = new LocalMoveToAction(_cardRectTransform, _originLocalPos, 0.6f, Utils.CurveInverseExponential);
		LocalRotateToAction rotateCard = new LocalRotateToAction(_cardRectTransform, Vector3.zero, 0.6f, Utils.CurveSmoothStep);
		ActionSequence revealCard = new ActionSequence(moveToHand, rotateCard);
		revealCard.OnActionStart += () =>
		{
			_cardBackImage.enabled = true;
			_cardFrontImage.enabled = false;
			_clonedIconImage.enabled = false;
		};
		revealCard.OnActionUpdate += () =>
		{
			if (!_cardBackImage.enabled) return;
			else if (Mathf.Abs(_cardRectTransform.localEulerAngles.y) < 90.0f)
			{
				_cardBackImage.enabled = false;
				_cardFrontImage.enabled = true;
				if (CardData.isCloned) _clonedIconImage.enabled = true;
			}
		};
		revealCard.OnActionFinish += () =>
		{
			_isAnimatingCardDraw = false;
			_cardRectTransform.localPosition = _originLocalPos;
		};
		if (inOnComplete != null) revealCard.OnActionFinish += () => { inOnComplete(); };

		ActionHandler.RunAction(new ActionAfterDelay(revealCard, inDelay));
	}

	bool _isAnimatingMoveToOtherCardPos = false;
	public void AnimateMoveToOriginFrom(Vector3 inOtherCardLocalPos, float inMoveDuration, float inZOffset)
	{
		_isAnimatingMoveToOtherCardPos = true;
		_cardRectTransform.localPosition = inOtherCardLocalPos + Vector3.forward * -inZOffset;
		LocalMoveToAction moveToOriginPos = new LocalMoveToAction(_cardRectTransform, _originLocalPos, inMoveDuration, Utils.CurveSmoothStep);
		moveToOriginPos.OnActionFinish += () =>
		{
			_isAnimatingMoveToOtherCardPos = false;
		};

        ActionHandler.RunAction(moveToOriginPos);
	}

	bool _isAnimatingCardExecute = false;
	public void AnimateCardExecuteAndDisable(DTJob.OnCompleteCallback inOnComplete = null)
	{
		_isAnimatingCardExecute = true;
		AxisLocalRotateByAction spinCard = new AxisLocalRotateByAction(
			_cardRectTransform,
			Vector3.up,
			270.0f,
			0.4f,
			Utils.CurveExponential);
		ScaleToAction flattenCard = new ScaleToAction(
			_cardRectTransform,
			Vector3.zero,
			0.5f,
			Utils.CurveExponential);
		ActionParallel executeCardAnim = new ActionParallel(spinCard, flattenCard);
		executeCardAnim.OnActionFinish += () =>
		{
			_isAnimatingCardExecute = false;
			SetEnabled(false);
			_cardRectTransform.localScale = Vector3.one;
			_tiltIntertia = Vector2.zero;
			_cardRectTransform.localRotation = Quaternion.identity;
			if (inOnComplete != null) inOnComplete();
		};
		ActionHandler.RunAction(executeCardAnim);
	}

	public void ReturnCardAndUnexecute(string inReturnReason)
	{
		SetEnabled(true);
		DungeonPopup.PopMiddlePopup(inReturnReason);
		Debug.LogWarning("Card Returned: " + inReturnReason);
	}

	public void SetEnabled(bool inIsEnabled)
	{
		_isEnabled = inIsEnabled;
		_cardCanvas.enabled = inIsEnabled;

		if (inIsEnabled && CardData.isCloned)
		{
			_clonedIconImage.enabled = true;
		}
		else
		{
			_clonedIconImage.enabled = false;
		}
	}
}
