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
	[SerializeField] private Button _endTurnBtn = null;

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
			Debug.Assert(_endTurnBtn != null, "_endTurnBtn is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

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
			DungeonPopup.PopText(_endTurnBlockedReason);
		}
		else
		{
			DisableEndTurnBtn("Enemy Phase in Progress!");
			OnPlayerEndTurn.Invoke();
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
