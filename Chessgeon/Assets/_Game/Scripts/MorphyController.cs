using System.Collections;
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

	private Morphy _morphy = null;

	private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }
	private const int START_HEALTH = 1;
	private int _health = -1;
	public int Health { get { return _health; } }
	private const int MAX_SHIELD = 1;
	private int _shield = -1;
	public int Shield { get { return _shield; } }
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
		SetShield(0, null);
		_isDead = false;
	}

	public void ResetFromPrevRunData(RunData inPrevRunData)
	{
		SetHealth(inPrevRunData.Health);
		SetShield(inPrevRunData.Shield, null);
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

		ShowPossibleMoves();
	}

	private void TransformBackToMorphy()
	{
		_morphy.TransformBackToMorphy();
		_dungeon.TileManager.HideAllSelectableTiles();
		_dungeon.CardManager.ToggleControlBlocker(false);
		if (_dungeon.EnemyManager.CheckIfFloorCleared()) DungeonCardDrawer.DisableEndTurnBtn("Floor cleared: All enemies on floor defeated.");
		else DungeonCardDrawer.EnableEndTurnBtn();
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

	private void SetShield(int inShield, DTJob.OnCompleteCallback inOnComplete)
	{
		_shield = inShield;
		if (_shield > 0) _morphy.ToggleShieldVisibility(true, true, inOnComplete);
		else _morphy.ToggleShieldVisibility(false, true, inOnComplete);
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

		_numMovesLeft--;
		Utils.GenericVoidDelegate onFinishMove;
		if (_numMovesLeft > 0)
		{
			onFinishMove = () =>
			{
				ShowPossibleMoves();
			};
		}
		else
		{
			onFinishMove = () =>
			{
				TransformBackToMorphy();
			};
		}

		if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Stairs))
		{
			_dungeon.ClearFloor();
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

		Vector2Int curPos;

		curPos = MorphyPos + new Vector2Int(1, 1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(0, 1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(-1, 1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(-1, 0); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(-1, -1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(0, -1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(1, -1); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
		curPos = MorphyPos + new Vector2Int(1, 0); allTilesInRange.Add(curPos);
		if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);

		if (inCardTier == eCardTier.Silver
			|| inCardTier == eCardTier.Gold)
		{
			curPos = MorphyPos + new Vector2Int(2, 2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(1, 2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(0, 2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-1, 2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-2, 2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-2, 1); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-2, 0); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-2, -1); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-2, -2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(-1, -2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(0, -2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(1, -2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(2, -2); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(2, -1); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(2, 0); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
			curPos = MorphyPos + new Vector2Int(2, 1); allTilesInRange.Add(curPos);
			if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);

			if (inCardTier == eCardTier.Gold)
			{
				curPos = MorphyPos + new Vector2Int(3, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(2, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(1, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(0, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-1, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-2, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, 3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, 2); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, 1); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, 0); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, -1); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, -2); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-3, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-2, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(-1, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(0, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(1, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(2, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, -3); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, -2); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, -1); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, 0); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, 1); allTilesInRange.Add(curPos);
				if (Dungeon.CurrentFloor.IsTileOfState(curPos, Floor.eTileState.Enemy)) targetTiles.Add(curPos);
				curPos = MorphyPos + new Vector2Int(3, 2); allTilesInRange.Add(curPos);
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
		// NOTE: Either take shield damage or health damage.
		if (_shield > 0)
		{
			int newShield = Mathf.Max(_shield - inDamage, 0);
			SetShield(newShield, null);
		}
		else
		{
			int newHealth = _health - inDamage;
			SetHealth(newHealth);
			DungeonDisplay.PlayDamageFrameAnimation();
		}

		DungeonCamera.CameraShake(15, 0.5f, 0.2f);
	}

	public void AwardShield(int inShield, DTJob.OnCompleteCallback inOnComplete)
	{
		int newShield = Mathf.Min(_shield + inShield, MAX_SHIELD);
		SetShield(newShield, inOnComplete);

		if (newShield == MAX_SHIELD) DungeonPopup.PopMiddlePopup("Shields at MAX!");
	}
}
