﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabEnemy = null;
	[SerializeField] private Dungeon _dungeon = null;

	private Enemy[] _enemies = null;
	private Floor _floor = null;

	private void Awake()
	{
		Debug.Assert(_prefabEnemy != null, "_prefabEnemy is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_enemies = new Enemy[_dungeon.MaxNumEnemies];
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			Enemy newEnemy = GameObject.Instantiate(_prefabEnemy).GetComponent<Enemy>();
			newEnemy.transform.SetParent(this.transform);
			newEnemy.Initialise(this, _dungeon);

			_enemies[iEnemy] = newEnemy;
		}

		HideAllEnemies();
	}

	public void GenerateAndSpawnEnemies(Floor inFloor)
	{
		_floor = inFloor;
		HideAllEnemies();

		int numEnemiesToSpawn = inFloor.Size.x * inFloor.Size.y / 10;
		int numEnemies = 0;

		while (numEnemies < numEnemiesToSpawn)
		{
			Vector2Int newEnemyPos = new Vector2Int(Random.Range(0, inFloor.Size.x), Random.Range(0, inFloor.Size.y));
			if (inFloor.IsTileEmpty(newEnemyPos))
			{
				Enemy.eType enemyType = (Enemy.eType)Random.Range(0, 4 + 1);

				Enemy currentEnemy = null;
				for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
				{
					if (!_enemies[iEnemy].IsAlive) currentEnemy = _enemies[iEnemy];
				}
				Debug.Assert(currentEnemy != null, "Could not get a non-alive enemy! Is whole list exhuasted?");

				Debug.Assert(_floor.TileStates[newEnemyPos.x, newEnemyPos.y] == Floor.eTileState.Empty);
				_floor.TileStates[newEnemyPos.x, newEnemyPos.y] = Floor.eTileState.Enemy;

				currentEnemy.SetEnemy(enemyType, Enemy.eElement.Basic);
				currentEnemy.SpawnAt(newEnemyPos);

				numEnemies++;
			}
		}
	}

	private void HideAllEnemies()
	{
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			_enemies[iEnemy].Hide();
		}
	}
}
