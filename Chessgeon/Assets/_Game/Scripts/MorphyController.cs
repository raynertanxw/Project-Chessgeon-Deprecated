using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyController : MonoBehaviour
{
	[SerializeField] private GameObject _prefabMorphy = null;
	[SerializeField] private Dungeon _dungeon = null;

	private Morphy _morphy = null;
	private Floor _floor = null;
	private Vector2Int _pos;
	public Vector2Int Pos { get { return _pos; } }

	private void Awake()
	{
		Debug.Assert(_prefabMorphy != null, "_prefabMorphy is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_morphy = GameObject.Instantiate(_prefabMorphy).GetComponent<Morphy>();
		_morphy.transform.SetParent(this.transform);
		_morphy.transform.position = Vector3.zero;
		_morphy.transform.rotation = Quaternion.identity;

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
				_pos = spawnPos;
				_morphy.SpawnAt(_dungeon.GetTileTransformPosition(Pos));
				Debug.Assert(_floor.TileStates[Pos.x, Pos.y] == Floor.eTileState.Empty);
				_floor.TileStates[Pos.x, Pos.y] = Floor.eTileState.Morphy;
				break;
			}
		}
	}
}
