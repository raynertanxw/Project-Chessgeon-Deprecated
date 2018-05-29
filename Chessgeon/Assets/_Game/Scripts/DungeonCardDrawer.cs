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
	[SerializeField] private Button _endTurnBtn = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() != null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			Debug.Assert(_cardManager != null, "_cardManager is not assigned.");

			Debug.Assert(_endTurnBtn != null, "_endTurnBtn is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Constants.DESIGN_WIDTH, Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH));
			_endTurnBtn.onClick.AddListener(() => { TryInvokeOnPlayerEndTurn(); });
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

	string _endTurnBlockedReason = string.Empty;
	private void TryInvokeOnPlayerEndTurn()
	{
		if (!_endTurnBtn.interactable)
		{
			// TODO: Isn't this kind of redundant???
			DungeonPopup.PopText(_endTurnBlockedReason);
		}
		else
		{
			DisableEndTurnBtn("Enemy Phase in Progress!");
			_dungeon.OnEndPlayerTurn();
		}
	}

	public static void DisableEndTurnBtn(string inBlockReason)
	{
		_instance._endTurnBtn.interactable = false;
		_instance._endTurnBlockedReason = inBlockReason;
	}

	public static void EnableEndTurnBtn()
	{
		_instance._endTurnBtn.interactable = true;
		_instance._endTurnBlockedReason = string.Empty;
	}
}
