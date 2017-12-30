using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DaburuTools;

public class DungeonCardDrawer : MonoBehaviour
{
	private static DungeonCardDrawer _instance = null;

	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private CardManager _cardManager = null;

	[Header("Canvas UI Elements")]
	[SerializeField] private RectTransform _cardDrawerRectTransform = null;
	[SerializeField] private Button _endTurnBtn = null;
	[SerializeField] private Button _hideDrawerBtn = null;
	[SerializeField] private Button _showDrawerBtn = null;

	[Header("Animation Graphs")]
	[SerializeField] private AnimationCurve _cardDrawerBobber = null;
	[SerializeField] private AnimationCurve _cardDrawerDipper = null;

	public static UnityEvent OnPlayerEndTurn = new UnityEvent();

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() != null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");
			Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			Debug.Assert(_cardManager != null, "_cardManager is not assigned.");

			Debug.Assert(_cardDrawerRectTransform != null, "_cardDrawer is not assigned.");
			Debug.Assert(_endTurnBtn != null, "_endTurnBtn is not assigned.");
			Debug.Assert(_hideDrawerBtn != null, "_hideDrawerBtn is not assigned.");
			Debug.Assert(_showDrawerBtn != null, "_showDrawerBtn is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			EnableCardDrawer(false, false);
			_showDrawerBtn.interactable = false;

			// Set up drawer btn Listeners.
			OnPlayerEndTurn.AddListener(() =>
			{
				EnableCardDrawer(false);
			});
			_endTurnBtn.onClick.AddListener(() => { OnPlayerEndTurn.Invoke(); });

			_hideDrawerBtn.onClick.AddListener(() =>
			{
				EnableCardDrawer(false);
			});

			_showDrawerBtn.onClick.AddListener(() =>
			{
				EnableCardDrawer(true);
			});
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

	private bool _cardDrawerAnimPlaying = false;
	public static void EnableCardDrawer(bool inIsEnabled, bool inIsAnimated = true, OnJobComplete inOnComplete = null)
	{
		const float ENABLED_X_POS = -100.0f;
		const float DISABLED_X_POS = -1300.0f;

		// Disable all btns on drawer for safety reasons.
		_instance._endTurnBtn.interactable = false;
		_instance._hideDrawerBtn.interactable = false;
		_instance._showDrawerBtn.interactable = false;

		Action.OnActionEndDelegate onCompleteAnim = () =>
		{
			_instance._cardDrawerAnimPlaying = false;
			if (inOnComplete != null) inOnComplete();

			if (inIsEnabled)
			{
				_instance._endTurnBtn.interactable = true;
				_instance._hideDrawerBtn.interactable = true;
			}
			else
			{
				_instance._showDrawerBtn.interactable = true;
			}
		};

		if (!_instance._cardDrawerAnimPlaying)
		{
			Vector2 newAnchorPos = _instance._cardDrawerRectTransform.anchoredPosition;
			MoveToAnchoredPosAction animateDrawerAction = null;
			if (inIsEnabled && _instance._cardDrawerRectTransform.localPosition.x != ENABLED_X_POS)
			{
				newAnchorPos.x = ENABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					animateDrawerAction = new MoveToAnchoredPosAction(_instance._cardDrawerRectTransform, newAnchorPos, 0.6f, _instance._cardDrawerBobber);
				}
			}
			else if (!inIsEnabled && _instance._cardDrawerRectTransform.localPosition.x != DISABLED_X_POS)
			{
				newAnchorPos.x = DISABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					animateDrawerAction = new MoveToAnchoredPosAction(_instance._cardDrawerRectTransform, newAnchorPos, 0.6f, _instance._cardDrawerDipper);
				}
			}

			if (inIsAnimated && animateDrawerAction != null)
			{
				_instance._cardDrawerAnimPlaying = true;
				animateDrawerAction.OnActionFinish += onCompleteAnim;
				ActionHandler.RunAction(animateDrawerAction);
			}
			else
			{
				_instance._cardDrawerRectTransform.anchoredPosition = newAnchorPos;
				onCompleteAnim.Invoke();
			}
		}
	}
}
