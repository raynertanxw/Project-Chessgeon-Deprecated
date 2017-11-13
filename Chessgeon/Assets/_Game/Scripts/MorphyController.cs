using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyController : MonoBehaviour
{
	[SerializeField] private GameObject _prefabMorphy = null;
	[SerializeField] private Dungeon _dungeon = null;
	public Dungeon Dungeon { get { return _dungeon; } }

	private Morphy _morphy = null;
	private Floor _floor = null;

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

	public void SetUpPlayer(Floor inFloor)
	{
		_floor = inFloor;

		// TODO: Determine position of morphy.
		while (true)
		{
			Vector2Int spawnPos = new Vector2Int(Random.Range(0, inFloor.Size.x), Random.Range(0, inFloor.Size.y));
			if (inFloor.IsTileEmpty(spawnPos))
			{
				_morphy.SpawnAt(spawnPos);
				Debug.Assert(_floor.IsTileEmpty(spawnPos), "Tile " + spawnPos + " is not empty!");
				_floor.SetTileState(spawnPos, Floor.eTileState.Morphy);
				break;
			}
		}
	}

	public void MorphTo(Morphy.eType inType)
	{
		_currentStratergy = _stratergies[(int)inType];
		_morphy.SetType(inType);

		Vector2Int[] possibleMoves = _currentStratergy.CalcPossibleMoves(_morphy.Pos, _floor);
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
		Debug.Assert(_floor.IsValidMorphyMove(inTargetPos), inTargetPos + " is not a valid Morphy move!");
		_floor.SetTileState(_morphy.Pos, Floor.eTileState.Empty);
		_floor.SetTileState(inTargetPos, Floor.eTileState.Morphy);
		_morphy.MoveTo(inTargetPos);
	}
}
