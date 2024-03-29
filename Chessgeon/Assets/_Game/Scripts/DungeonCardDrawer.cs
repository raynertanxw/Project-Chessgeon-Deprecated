﻿using System.Collections;
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
	[SerializeField] private Image _endTurnBtnImage = null;
	[SerializeField] private Text _endTurnBtnText = null;

	[Header("End Turn Btn Settings")]
	[SerializeField] private Color _endTurnColor;
    [SerializeField] private Color _enemyTurnColor;
	[SerializeField] private Color _loadingColor;
	[Space]
	[Header("End Turn Btn Color Block")]
	[SerializeField] private ColorBlock _endTurnColorBlock;
	[Space]
	[Header("Enemy Turn Btn Color Block")]
	[SerializeField] private ColorBlock _enemyTurnColorBlock;
	[Space]
	[Header("Loading Color Block")]
	[SerializeField] private ColorBlock _loadingColorBlock;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() != null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			Debug.Assert(_cardManager != null, "_cardManager is not assigned.");

			Debug.Assert(_endTurnBtn != null, "_endTurnBtn is not assigned.");
			Debug.Assert(_endTurnBtnImage != null, "_endTurnBtnImg is not assigned.");
			Debug.Assert(_endTurnBtnText != null, "_endTurnBtnText is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Constants.DESIGN_WIDTH, Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH));
			_endTurnBtn.onClick.AddListener(_dungeon.EndPlayerTurn);
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

	private void Update()
	{
		bool cannotEndTurn =
			(!_dungeon.IsPlayersTurn ||
			_cardManager.IsCardInUse ||
			_cardManager.IsCloneMode ||
			_dungeon.IsPlayerTurnStartAnimPlaying ||
			_dungeon.FloorCleared ||
			_dungeon.CheckClearFloorConditions());
		bool canEndTurn = !cannotEndTurn;

		if (_endTurnBtn.interactable != canEndTurn) _endTurnBtn.interactable = canEndTurn;
	}

	public static void SetEndTurnBtnForPlayerPhase()
	{
		RotateByAction rotateDown = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(-90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
		rotateDown.OnActionFinish += () =>
		{
			_instance._endTurnBtn.colors = _instance._endTurnColorBlock;
			_instance._endTurnBtnImage.color = _instance._endTurnColor;
			_instance._endTurnBtnText.text = "END TURN";
		};
		RotateByAction rotateBack = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
		ActionHandler.RunAction(new ActionSequence(rotateDown, rotateBack));
	}

	public static void SetEndTurnBtnForEnemyPhase()
	{
		RotateByAction rotateDown = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(-90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
        rotateDown.OnActionFinish += () =>
        {
			_instance._endTurnBtn.colors = _instance._enemyTurnColorBlock;
			_instance._endTurnBtnImage.color = _instance._enemyTurnColor;
            _instance._endTurnBtnText.text = "ENEMY TURN";
        };
		RotateByAction rotateBack = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
        ActionHandler.RunAction(new ActionSequence(rotateDown, rotateBack));
	}

	public static void SetEndTurnBtnForLoading(bool inIsAnimated = true)
	{
		if (inIsAnimated)
		{
			RotateByAction rotateDown = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(-90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
            rotateDown.OnActionFinish += () =>
            {
				_instance._endTurnBtn.colors = _instance._loadingColorBlock;
                _instance._endTurnBtnImage.color = _instance._loadingColor;
                _instance._endTurnBtnText.text = "LOADING...";
            };
            RotateByAction rotateBack = new RotateByAction(_instance._endTurnBtn.transform, new Vector3(90.0f, 0.0f, 0.0f), 0.15f, Utils.CurveSmoothStep);
            ActionHandler.RunAction(new ActionSequence(rotateDown, rotateBack));
		}
		else
		{
			_instance._endTurnBtn.colors = _instance._loadingColorBlock;
			_instance._endTurnBtnImage.color = _instance._loadingColor;
			_instance._endTurnBtnText.text = "LOADING...";
		}
	}
}
