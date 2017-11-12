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

	private const int DUNGEON_MAX_X = 15;
	private const int DUNGEON_MAX_Y = 15;
	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MIN_Y = 5;
	private const int DUNGEON_MAX_ENEMIES = 50;

	private Floor _floor = null;

	public FloorGeneratedEvent OnFloorGenerated;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");
		Debug.Assert(_enemyManager != null, "_enemyManager is not assigned.");

		_tileManager.Initialise(DUNGEON_MAX_X, DUNGEON_MAX_Y, this);
		_enemyManager.Initialise(DUNGEON_MAX_ENEMIES, this);
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

		OnFloorGenerated.Invoke(_floor);
	}

	public void StartGame()
	{
		// Reset everything and generate the new 1st floor.
		GenerateFloor();
	}
}
