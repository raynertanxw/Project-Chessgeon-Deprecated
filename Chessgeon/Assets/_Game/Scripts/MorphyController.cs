﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DaburuTools;

public class MorphyController : MonoBehaviour
{
	[SerializeField] private GameObject _prefabMorphy = null;
	[SerializeField] private Dungeon _dungeon = null;
	public Dungeon Dungeon { get { return _dungeon; } }
	public Vector2Int MorphyPos { get { return _morphy.Pos; } }
	public Vector2Int TargetPos { get; private set; }

	private Morphy _morphy = null;

	private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }
	private const int START_HEALTH = 1;
	private int _health = -1;
	public int Health { get { return _health; } }
	private int _numMovesLeft = -1;

	private void Awake()
	{
		Debug.Assert(_prefabMorphy != null, "_prefabMorphy is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_morphy = GameObject.Instantiate(_prefabMorphy).GetComponent<Morphy>();
		_morphy.transform.SetParent(this.transform);
		_morphy.transform.position = Vector3.zero;
		_morphy.transform.rotation = Quaternion.identity;
		_morphy.Initialise(this);

		_morphy.Hide();
	}

	public void ResetForNewGame()
	{
		SetHealth(START_HEALTH);
		_isDead = false;
	}

	public void ResetFromPrevRunData(RunData inPrevRunData)
	{
		SetHealth(inPrevRunData.Health);
		_isDead = false;
	}

	public void SetUpPlayer()
	{
		_morphy.SpawnAt(_dungeon.CurrentFloor.MorphyPos);
	}

	public void MorphTo(eMoveType inType, int inNumMoves)
	{
		_numMovesLeft = inNumMoves;
		_morphy.SetType(inType);
		_morphy.PlayMorphAnimation();

		ShowPossibleMoves();
	}

	private void TransformBackToMorphy()
	{
		_morphy.TransformBackToMorphy();
		_morphy.PlayMorphAnimation();
		_dungeon.TileManager.HideAllSelectableTiles();
		_dungeon.CardManager.SignalCardUsed();
	}

	private void SetHealth(int inHealth)
	{
		_health = inHealth;

		if (_health < 1)
		{
			_isDead = true;
			Dungeon.EndGame();
		}
	}

	private void ShowPossibleMoves()
	{
		_dungeon.TileManager.HideAllSelectableTiles();
		// TODO: Get nothing if type is morphy.
		Vector2Int[] possibleMoves = null;
		if (_morphy.IsInMorphyForm) possibleMoves = new Vector2Int[0];
		else possibleMoves = Dungeon.CurrentFloor.GridStratergyForMoveType(_morphy.CurrentType).CalcPossibleMoves(_morphy.Pos, GridStratergy.eMoveEntity.Morphy);
		if (possibleMoves.Length > 0)
		{
			_dungeon.TileManager.ShowPossibleMoves(possibleMoves, MoveTo);
		}
		else
		{
			_numMovesLeft = 0;
			TransformBackToMorphy();
			DungeonPopup.PopMiddlePopup("No Possible " + _morphy.CurrentType.ToString() + " Moves");
			Debug.LogWarning("No Possible " + _morphy.CurrentType.ToString() + " Moves");
		}
	}

	private void MoveTo(Vector2Int inTargetPos)
	{
		Debug.Assert(_dungeon.CurrentFloor.IsValidMorphyMove(inTargetPos), inTargetPos + " is not a valid Morphy move!");
		TargetPos = inTargetPos;

		_numMovesLeft--;
		Utils.GenericVoidDelegate onFinishMove;
		if (_numMovesLeft > 0)
		{
			onFinishMove = () =>
			{
				if (_dungeon.CheckClearFloorConditions())
				{
					_numMovesLeft = 0;
				}
				else ShowPossibleMoves();
			};
		}
		else
		{
			onFinishMove = () =>
			{
				TransformBackToMorphy();
			};
		}

		if (_dungeon.TryClearFloor())
		{
			Debug.Assert(_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Stairs), "Floor cleared but inTargetPos is not stairs.");
		}
		else if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Enemy))
		{
			Enemy targetEnemy = _dungeon.CurrentFloor.GetEnemyAt(inTargetPos);
			Debug.Assert(targetEnemy != null, "There is no enemy at " + inTargetPos);
			_morphy.MoveAndAttack(inTargetPos, targetEnemy, onFinishMove);
		}
		else
		{
			_morphy.MoveTo(inTargetPos, onFinishMove);
		}

		_dungeon.CurrentFloor.MoveMorphyTo(inTargetPos);
	}

	public void Smash(eCardTier inCardTier, Utils.GenericVoidDelegate inOnComplete = null)
	{
		List<Vector2Int> allTilesInRange = new List<Vector2Int>();
		List<Vector2Int> targetTiles = new List<Vector2Int>();

		int range = -1;
		switch (inCardTier)
		{
			case eCardTier.Normal: range = 1; break;
			case eCardTier.Silver: range = 2; break;
			case eCardTier.Gold: range = 3; break;
			default: Debug.LogError(inCardTier.ToString() + " eCardTier was not implemented in Smash."); break;
		}

		Vector2Int curPos;
		for (int y = -range; y <= range; y++)
		{
			for (int x = -range; x <= range; x++)
			{
				curPos = MorphyPos + new Vector2Int(x, y);
				if (curPos == MorphyPos) continue;
				if (curPos == Dungeon.CurrentFloor.StairsPos) continue;
				if (!Dungeon.CurrentFloor.IsValidPos(curPos)) continue;

				allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			}
		}

		Enemy[] enemies = new Enemy[targetTiles.Count];
		for (int iTarget = 0; iTarget < targetTiles.Count; iTarget++)
		{
			enemies[iTarget] = Dungeon.CurrentFloor.GetEnemyAt(targetTiles[iTarget]);
			Debug.Assert(enemies[iTarget] != null);
		}

		_morphy.SmashAttack(enemies, allTilesInRange.ToArray(), inOnComplete);
	}

	public void TakeDamage(int inDamage)
	{
	    int newHealth = _health - inDamage;
		SetHealth(newHealth);
		DungeonDisplay.PlayDamageFrameAnimation();

		DungeonCamera.CameraShake(15, 0.5f, 0.2f);
	}
}
