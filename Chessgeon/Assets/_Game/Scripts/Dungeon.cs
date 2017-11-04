using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
	[SerializeField] private TileManager _tileManager = null;

	private const int DUNGEON_MAX_X = 15;
	private const int DUNGEON_MAX_Y = 15;
	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MIN_Y = 5;

	private int _floorSizeX = -1;
	private int _floorSizeY = -1;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");

		_tileManager.Initialise(DUNGEON_MAX_X, DUNGEON_MAX_Y);
	}

	private void GenerateFloor()
	{
		_floorSizeX = Random.Range(DUNGEON_MIN_X, DUNGEON_MAX_X);
		_floorSizeY = Random.Range(DUNGEON_MIN_Y, DUNGEON_MAX_Y);

		_tileManager.GenerateFloorTerrain(_floorSizeX, _floorSizeY);
		// TODO: Generate Enemy spawns?
	}

	public void StartGame()
	{
		// Reset everything and generate the new 1st floor.
		GenerateFloor();
	}
}
