using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MorphyController : MonoBehaviour
{
	[SerializeField] private GameObject _prefabMorphy = null;
	[SerializeField] private Dungeon _dungeon = null;
	public Dungeon Dungeon { get { return _dungeon; } }
	public Vector2Int MorphyPos { get { return _morphy.Pos; } }

	public UnityEvent OnMorphyReachStairs = new UnityEvent();
	private Morphy _morphy = null;

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
			// TODO: Deal with case when there are no possible spaces to move.
			Debug.LogWarning("No Possible Moves.");
		}
	}

	private void MoveTo(Vector2Int inTargetPos)
	{
		Debug.Assert(_dungeon.CurrentFloor.IsValidMorphyMove(inTargetPos), inTargetPos + " is not a valid Morphy move!");

		_numMovesLeft--;
		DaburuTools.Action.OnActionEndDelegate onFinishMove;
		if (_numMovesLeft > 0)
		{
			onFinishMove = () => { ShowPossibleMoves(); };
		}
		else
		{
			onFinishMove = () =>
			{
				TransformBackToMorphy();
				DungeonCardDrawer.EnableCardDrawer(true);
			};
		}

		if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Stairs))
		{
			OnMorphyReachStairs.Invoke();
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
}
