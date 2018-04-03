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
	[SerializeField] private RectTransform _endTurnBtnMesh = null;
	[SerializeField] private RectTransform _hideDrawerBtnMesh = null;
	[SerializeField] private RectTransform _showDrawerBtnMesh = null;
	[SerializeField] private Button _endTurnBtn = null;
	[SerializeField] private Button _hideDrawerBtn = null;
	[SerializeField] private Button _showDrawerBtn = null;

	[Header("Animation Graphs")]
	[SerializeField] private AnimationCurve _cardDrawerBobber = null;
	[SerializeField] private AnimationCurve _cardDrawerDipper = null;
	[SerializeField] private AnimationCurve _bigBobber = null;
	[SerializeField] private AnimationCurve _bigDipper = null;

	public static Utils.GenericVoidDelegate OnPlayerEndTurn;

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
			Debug.Assert(_endTurnBtnMesh != null, "_endTurnBtnMesh is not assigned.");
			Debug.Assert(_hideDrawerBtnMesh != null, "_hideDrawerBtnMesh is not assigned.");
			Debug.Assert(_showDrawerBtnMesh != null, "_showDrawerBtnMesh is not assigned.");
			Debug.Assert(_endTurnBtn != null, "_endTurnBtn is not assigned.");
			Debug.Assert(_hideDrawerBtn != null, "_hideDrawerBtn is not assigned.");
			Debug.Assert(_showDrawerBtn != null, "_showDrawerBtn is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			EnableCardDrawer(false, false);
			_showDrawerBtn.interactable = false;

			// Set up drawer btn Listeners.
			OnPlayerEndTurn += () =>
			{
				EnableCardDrawer(false, true);
			};
			_endTurnBtn.onClick.AddListener(() => { TryInvokeOnPlayerEndTurn(); });

			_hideDrawerBtn.onClick.AddListener(() =>
			{
				EnableCardDrawer(false, true, false);
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

	bool _isEndTurnBlocked = false;
	string _endTurnBlockedReason = string.Empty;
	private void TryInvokeOnPlayerEndTurn()
	{
		if (_isEndTurnBlocked)
		{
			DungeonPopup.PopText(_endTurnBlockedReason);
		}
		else
		{
			OnPlayerEndTurn.Invoke();
		}
	}

	private bool _cardDrawerAnimPlaying = false;
	public static void EnableCardDrawer(bool inIsEnabled, bool inIsAnimated = true, bool inHideShowDrawerBtn = true, DTJob.OnCompleteCallback inOnComplete = null)
	{
		const float ENABLED_X_POS = -100.0f;
		const float DISABLED_X_POS = -1250.0f;

		// Disable all btns on drawer for safety reasons.
		_instance._endTurnBtn.interactable = false;
		_instance._hideDrawerBtn.interactable = false;
		_instance._showDrawerBtn.interactable = false;

		Utils.GenericVoidDelegate onCompleteAnim = () =>
		{
			_instance._cardDrawerAnimPlaying = false;
			if (inOnComplete != null) inOnComplete();

			if (inIsEnabled)
			{
				_instance._endTurnBtn.interactable = true;
				_instance._hideDrawerBtn.interactable = true;
			}
			else if (!inHideShowDrawerBtn)
			{
				_instance._showDrawerBtn.interactable = true;
			}
		};

		if (!_instance._cardDrawerAnimPlaying)
		{
			Vector2 newAnchorPos = _instance._cardDrawerRectTransform.anchoredPosition;
			Vector3 btnOpenScale = Vector3.one;
			Vector3 btnCloseScale = new Vector3(1.0f, 0.0f, 1.0f);
			bool isAlreadyInPos = false;
			if (inIsEnabled && _instance._cardDrawerRectTransform.localPosition.x != ENABLED_X_POS)
			{
				newAnchorPos.x = ENABLED_X_POS;
			}
			else if (!inIsEnabled && _instance._cardDrawerRectTransform.localPosition.x != DISABLED_X_POS)
			{
				newAnchorPos.x = DISABLED_X_POS;
			}
			else
			{
				isAlreadyInPos = true;
			}

			if (inIsAnimated && !isAlreadyInPos)
			{
				_instance._cardDrawerAnimPlaying = true;
				MoveToAnchoredPosAction animateDrawerMove = new MoveToAnchoredPosAction(
					_instance._cardDrawerRectTransform,
					newAnchorPos,
					inIsEnabled ? 0.6f : 0.3f,
					inIsEnabled ? _instance._cardDrawerBobber : _instance._cardDrawerDipper);

				const float BTN_SCALE_ANIM_TIME = 0.25f;
				ActionParallel animateButtonScales = new ActionParallel(
					new ScaleToAction(_instance._endTurnBtnMesh, inIsEnabled ? btnOpenScale : btnCloseScale, BTN_SCALE_ANIM_TIME, inIsEnabled ? _instance._bigBobber : _instance._bigDipper),
					new ScaleToAction(_instance._hideDrawerBtnMesh, inIsEnabled ? btnOpenScale : btnCloseScale,  BTN_SCALE_ANIM_TIME, inIsEnabled ? _instance._bigBobber : _instance._bigDipper)
				);
				ScaleToAction showDrawerBtnScale;
				if (inHideShowDrawerBtn)
				{
					showDrawerBtnScale = new ScaleToAction(_instance._showDrawerBtnMesh, btnCloseScale, BTN_SCALE_ANIM_TIME, _instance._bigDipper);
				}
				else
				{
					showDrawerBtnScale = new ScaleToAction(_instance._showDrawerBtnMesh, inIsEnabled ? btnCloseScale : btnOpenScale, BTN_SCALE_ANIM_TIME, inIsEnabled ? _instance._bigDipper : _instance._bigBobber);
				}
				animateButtonScales.Add(showDrawerBtnScale);

				ActionSequence animateDrawerAction = null;
				if (inIsEnabled)
				{
					animateDrawerAction = new ActionSequence(animateDrawerMove, animateButtonScales);
				}
				else
				{
					animateDrawerAction = new ActionSequence(animateButtonScales, animateDrawerMove);
				}
				animateDrawerAction.OnActionFinish += onCompleteAnim;

				ActionHandler.RunAction(animateDrawerAction);
			}
			else
			{
				_instance._cardDrawerRectTransform.anchoredPosition = newAnchorPos;
				_instance._endTurnBtnMesh.localScale = inIsEnabled ? btnOpenScale : btnCloseScale;
				_instance._hideDrawerBtnMesh.localScale = inIsEnabled ? btnOpenScale : btnCloseScale;
				if (inHideShowDrawerBtn)
				{
					_instance._showDrawerBtnMesh.localScale = btnCloseScale;
				}
				else
				{
					_instance._showDrawerBtnMesh.localScale = inIsEnabled ? btnCloseScale : btnOpenScale;
				}
				onCompleteAnim();
			}
		}
	}

	public static void DisableEndTurnBtn(string inBlockReason)
	{
		_instance._isEndTurnBlocked = true;
		_instance._endTurnBlockedReason = inBlockReason;
	}

	public static void EnableEndTurnBtn()
	{
		_instance._isEndTurnBlocked = false;
		_instance._endTurnBlockedReason = string.Empty;
	}
}
