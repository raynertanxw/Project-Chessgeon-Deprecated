using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloorGeneratedEvent : UnityEvent<Floor>
{
}

public class Dungeon : MonoBehaviour
{
	[SerializeField] private TileManager _tileManager = null;
	[SerializeField] private EnemyManager _enemyManager = null;
	[SerializeField] private MorphyController _morphyController = null;

	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MAX_X = 15;
	private const int DUNGEON_MIN_Y = 5;
	private const int DUNGEON_MAX_Y = 15;
	private const int DUNGEON_MAX_ENEMIES = 50;
	public int MinX { get { return DUNGEON_MIN_X; } }
	public int MaxX { get { return DUNGEON_MAX_X; } }
	public int MinY { get { return DUNGEON_MIN_Y; } }
	public int MaxY { get { return DUNGEON_MAX_Y; } }
	public int MaxNumEnemies { get { return DUNGEON_MAX_ENEMIES; } }

	private Floor _floor = null;

	public FloorGeneratedEvent OnFloorGenerated;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");
		Debug.Assert(_enemyManager != null, "_enemyManager is not assigned.");
		Debug.Assert(_morphyController != null, "_morphyController is not assigned.");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) StartGame(); // TODO: Remove this temp debug functionality.
	}

	private void GenerateFloor()
	{
		_floor = new Floor(DUNGEON_MIN_X, DUNGEON_MAX_X, DUNGEON_MIN_Y, DUNGEON_MAX_Y, DungeonTile.eZone.Classic);

		_tileManager.GenerateFloorTerrain(_floor);
		_enemyManager.GenerateAndSpawnEnemies(_floor); // TODO: Can pass in difficulty settings here.
		_morphyController.SetUpPlayer(_floor);

		OnFloorGenerated.Invoke(_floor);
	}

	public void StartGame()
	{
		// Reset everything and generate the new 1st floor.
		GenerateFloor();
	}

	public Vector3 GetTileTransformPosition(int inPosX, int inPosY)
	{
		return _tileManager.GetTileTransformPosition(inPosX, inPosY);
	}

	public Vector3 GetTileTransformPosition(Vector2Int inPos)
	{
		return _tileManager.GetTileTransformPosition(inPos.x, inPos.y);
	}
}
