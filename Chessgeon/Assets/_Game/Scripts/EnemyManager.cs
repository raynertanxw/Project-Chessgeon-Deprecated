﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabEnemy = null;
	[SerializeField] private Dungeon _dungeon = null;
	public Dungeon Dungeon { get { return _dungeon; } }

	private Enemy[] _enemies = null;

	private void Awake()
	{
		Debug.Assert(_prefabEnemy != null, "_prefabEnemy is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_enemies = new Enemy[_dungeon.MaxNumEnemies];
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			Enemy newEnemy = GameObject.Instantiate(_prefabEnemy).GetComponent<Enemy>();
			newEnemy.transform.SetParent(this.transform);
			newEnemy.Initialise(this);

			_enemies[iEnemy] = newEnemy;
		}

		_dungeon.MorphyController.OnMorphyReachStairs.AddListener(HideAllEnemies);

		HideAllEnemies();
	}

	public Enemy SpawnEnemyAt(Vector2Int inSpawnPos)
    {
        eMoveType enemyType = (eMoveType)Random.Range(0, 5);

        Enemy currentEnemy = null;
        for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
        {
            if (!_enemies[iEnemy].IsAlive) currentEnemy = _enemies[iEnemy];
        }
        Debug.Assert(currentEnemy != null, "Could not get a non-alive enemy! Is whole list exhuasted?");

        Debug.Assert(_dungeon.CurrentFloor.IsTileEmpty(inSpawnPos), "Tile " + inSpawnPos + " is not empty!");
        currentEnemy.SetEnemy(enemyType, Enemy.eElement.Basic);
        currentEnemy.SpawnAt(inSpawnPos);

		return currentEnemy;
    }

	private void HideAllEnemies()
	{
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			_enemies[iEnemy].Hide();
		}
	}
	
	public Enemy[] GetArrayOfAliveEnemies()
	{
		List<Enemy> enemiesAlive = new List<Enemy>();
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			if (_enemies[iEnemy].IsAlive) enemiesAlive.Add(_enemies[iEnemy]);
		}

		return enemiesAlive.ToArray();
	}
}
