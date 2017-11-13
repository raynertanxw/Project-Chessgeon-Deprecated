﻿using System.Collections;
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
	public TileManager TileManager { get { return _tileManager; } }
	public EnemyManager EnemyManager { get { return _enemyManager; } }
	public MorphyController MorphyController { get { return _morphyController; } }

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

	private int _floorNum = -1;
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

		// TODO: Remove this temp debug functionality.
		if (Input.GetKeyDown(KeyCode.Alpha0)) _morphyController.MorphTo(Morphy.eType.Morphy);
		else if (Input.GetKeyDown(KeyCode.Alpha1)) _morphyController.MorphTo(Morphy.eType.Pawn);
		else if (Input.GetKeyDown(KeyCode.Alpha2)) _morphyController.MorphTo(Morphy.eType.Rook);
		else if (Input.GetKeyDown(KeyCode.Alpha3)) _morphyController.MorphTo(Morphy.eType.Bishop);
		else if (Input.GetKeyDown(KeyCode.Alpha4)) _morphyController.MorphTo(Morphy.eType.Knight);
		else if (Input.GetKeyDown(KeyCode.Alpha5)) _morphyController.MorphTo(Morphy.eType.King);
	}

	private void GenerateFloor()
	{
		_floor = new Floor(DUNGEON_MIN_X, DUNGEON_MAX_X, DUNGEON_MIN_Y, DUNGEON_MAX_Y, DungeonTile.eZone.Classic, _floorNum);

		_tileManager.GenerateFloorTerrain(_floor);
		_enemyManager.GenerateAndSpawnEnemies(_floor); // TODO: Can pass in difficulty settings here.
		_morphyController.SetUpPlayer(_floor);

		OnFloorGenerated.Invoke(_floor);
	}

	public void StartGame()
	{
		_floorNum = 1;
		// Reset everything and generate the new 1st floor.
		GenerateFloor();
	}

	public void ProgressToNextFloor()
	{
		_floorNum++;
		GenerateFloor();
	}
}
