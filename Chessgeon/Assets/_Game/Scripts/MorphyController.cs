using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyController : MonoBehaviour
{
	[SerializeField] private GameObject _prefabMorphy = null;
	[SerializeField] private Dungeon _dungeon = null;

	private Morphy _morphy = null;
	private Floor _floor = null;

	private void Awake()
	{
		Debug.Assert(_prefabMorphy != null, "_prefabMorphy is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_morphy = GameObject.Instantiate(_prefabMorphy).GetComponent<Morphy>();
		_morphy.transform.SetParent(this.transform);
		_morphy.transform.position = Vector3.zero;
		_morphy.transform.rotation = Quaternion.identity;
		_morphy.Initialise(this, _dungeon);

		_morphy.Hide();
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
		_morphy.SetType(inType);

		// TODO: Disable scrolling and other UI stuff.

		Vector2Int[] possibleMoves = _morphy.CalcPossibleMoves(_floor);
		if (possibleMoves.Length > 0)
		{
			// TODO: Show the options of the possible spaces to move to.
		}
		else
		{
			// TODO: Deal with case when there are no possible spaces to move.
		}
	}
}
