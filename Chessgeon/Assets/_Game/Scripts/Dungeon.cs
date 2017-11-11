using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dungeon : MonoBehaviour
{
	[SerializeField] private TileManager _tileManager = null;

	private const int DUNGEON_MAX_X = 15;
	private const int DUNGEON_MAX_Y = 15;
	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MIN_Y = 5;

	private int _floorSizeX = -1;
	private int _floorSizeY = -1;
	public int FloorSizeX { get { return _floorSizeX; } }
	public int FloorSizeY { get { return _floorSizeY; } }

	private int _stairsPosX = -1;
	private int _stairsPosY = -1;
	public int StairsPosX { get { return _stairsPosX; } }
	public int StairsPosY { get { return _stairsPosY; } }

	public UnityEvent OnFloorGenerated;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");

		_tileManager.Initialise(DUNGEON_MAX_X, DUNGEON_MAX_Y, this);
	}

	private void GenerateFloor()
	{
		_floorSizeX = Random.Range(DUNGEON_MIN_X, DUNGEON_MAX_X);
		_floorSizeY = Random.Range(DUNGEON_MIN_Y, DUNGEON_MAX_Y);
		_stairsPosX = Random.Range(1, _floorSizeX - 1);
		_stairsPosY = Random.Range(1, FloorSizeY - 1);

		_tileManager.GenerateFloorTerrain(_floorSizeX, _floorSizeY);
		// TODO: Generate Enemy spawns?

		OnFloorGenerated.Invoke();
	}

	public void StartGame()
	{
		// Reset everything and generate the new 1st floor.
		GenerateFloor();
	}
}
