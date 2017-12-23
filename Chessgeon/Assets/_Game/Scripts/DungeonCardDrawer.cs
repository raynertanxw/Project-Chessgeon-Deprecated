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

	[Header("Canvas UI Elements")]
	[SerializeField] private RectTransform _cardDrawer = null;
	[SerializeField] private Button _cardDrawerBtn = null;

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

			Debug.Assert(_cardDrawer != null, "_cardDrawer is not assigned.");
			Debug.Assert(_cardDrawerBtn != null, "_cardDrawerBtn is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			EnableCardDrawer(false, false);

			OnPlayerEndTurn.AddListener(() =>
			{
				EnableCardDrawer(false);
				_cardDrawerBtn.interactable = false;
			});
			_cardDrawerBtn.onClick.AddListener(() => { OnPlayerEndTurn.Invoke(); });
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
		const float ENABLED_X_POS = -200.0f;
		const float DISABLED_X_POS = -1300.0f;

		Action.OnActionEndDelegate onCompleteAnim = () =>
		{
			_instance._cardDrawerAnimPlaying = false;
			if (inOnComplete != null) inOnComplete();
			_instance._cardDrawerBtn.interactable = inIsEnabled;
		};

		if (!_instance._cardDrawerAnimPlaying)
		{
			Vector2 newAnchorPos = _instance._cardDrawer.anchoredPosition;
			if (inIsEnabled && _instance._cardDrawer.localPosition.x != ENABLED_X_POS)
			{
				newAnchorPos.x = ENABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					MoveToAnchoredPosAction openDrawer = new MoveToAnchoredPosAction(_instance._cardDrawer, newAnchorPos, 0.6f, _instance._cardDrawerBobber);
					openDrawer.OnActionFinish += onCompleteAnim;
					ActionHandler.RunAction(openDrawer);
				}
				else { _instance._cardDrawer.anchoredPosition = newAnchorPos; }
			}
			else if (!inIsEnabled && _instance._cardDrawer.localPosition.x != DISABLED_X_POS)
			{
				newAnchorPos.x = DISABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					MoveToAnchoredPosAction closeDrawer = new MoveToAnchoredPosAction(_instance._cardDrawer, newAnchorPos, 0.6f, _instance._cardDrawerDipper);
					closeDrawer.OnActionFinish += onCompleteAnim;
					ActionHandler.RunAction(closeDrawer);
				}
				else { _instance._cardDrawer.anchoredPosition = newAnchorPos; }
			}
		}
	}
}
