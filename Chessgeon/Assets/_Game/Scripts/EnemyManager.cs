using System.Collections;
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

		_dungeon.OnFloorCleared += RemoveAllEnemies;

		RemoveAllEnemies();
	}

	public Enemy SpawnEnemyAt(Vector2Int inSpawnPos)
    {
        eMoveType enemyType = (eMoveType)Random.Range(0, 5);

		return SpawnEnemyAt(inSpawnPos, enemyType);
    }
	public Enemy SpawnEnemyAt(Vector2Int inSpawnPos, eMoveType inMoveType)
    {
        Enemy currentEnemy = null;
        for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
        {
            if (!_enemies[iEnemy].IsAlive) currentEnemy = _enemies[iEnemy];
        }
        Debug.Assert(currentEnemy != null, "Could not get a non-alive enemy! Is whole list exhuasted?");

        Debug.Assert(_dungeon.CurrentFloor.IsTileEmpty(inSpawnPos), "Tile " + inSpawnPos + " is not empty!");
        currentEnemy.SetType(inMoveType);
        currentEnemy.SpawnAt(inSpawnPos);

		return currentEnemy;
    }

	private void RemoveAllEnemies()
	{
		for (int iEnemy = 0; iEnemy < _enemies.Length; iEnemy++)
		{
			_enemies[iEnemy].Remove();
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

	public void ResetForNewGame()
	{
		RemoveAllEnemies();
	}
}
