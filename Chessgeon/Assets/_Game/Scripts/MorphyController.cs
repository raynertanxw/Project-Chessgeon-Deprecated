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

	private MorphyStratergy[] _stratergies = null;
	private MorphyStratergy _currentStratergy = null;

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

		_stratergies = new MorphyStratergy[6];
		_stratergies[0] = new MorphyStratergyPawn();
		_stratergies[1] = new MorphyStratergyRook();
		_stratergies[2] = new MorphyStratergyBishop();
		_stratergies[3] = new MorphyStratergyKnight();
		_stratergies[4] = new MorphyStratergyKing();
		_stratergies[5] = new MorphyStratergyMorphy();
	}

	public void SetUpPlayer()
	{
		_morphy.SpawnAt(_dungeon.CurrentFloor.MorphyPos);
	}

	public void MorphTo(Morphy.eType inType)
	{
		_currentStratergy = _stratergies[(int)inType];
		_morphy.SetType(inType);

		_dungeon.TileManager.HideAllSelectableTiles();
		Vector2Int[] possibleMoves = _currentStratergy.CalcPossibleMoves(_morphy.Pos, _dungeon.CurrentFloor);
		if (possibleMoves.Length > 0)
		{
			_dungeon.TileManager.ShowPossibleMoves(possibleMoves, MoveTo);
		}
		else
		{
			// TODO: Deal with case when there are no possible spaces to move.
		}
	}

	private void MoveTo(Vector2Int inTargetPos)
	{
		Debug.Assert(_dungeon.CurrentFloor.IsValidMorphyMove(inTargetPos), inTargetPos + " is not a valid Morphy move!");

		if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Stairs))
		{
			OnMorphyReachStairs.Invoke();
		}
		else if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Enemy))
		{
			Enemy targetEnemy = _dungeon.CurrentFloor.GetEnemyAt(inTargetPos);
			Debug.Assert(targetEnemy != null, "There is no enemy at " + inTargetPos);
			_morphy.MoveAndAttack(inTargetPos, targetEnemy);
		}
		else
		{
			_morphy.MoveTo(inTargetPos);
		}

		_dungeon.CurrentFloor.MoveMorphyTo(inTargetPos);
	}
}
