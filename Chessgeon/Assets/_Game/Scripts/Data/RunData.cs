using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RunData
{
	public const string FILENAME = "PrevRunData";

	public const string HEALTH_KEY = "HEALTH";
	public const string SHIELD_KEY = "SHIELD";

	public const string FLOOR_NUM_KEY = "FLOOR_NUM";
	public const string FLOOR_SIZE_KEY = "FLOOR_SIZE";
	public const string STAIR_POS_KEY = "STAIR_POS";
	public const string MORPHY_POS_KEY = "MORPHY_POS";
	public const string ENEMY_POS_X_KEY = "ENEMY_POS_X";
	public const string ENEMY_POS_Y_KEY = "ENEMY_POS_Y";
	public const string ENEMY_MOVE_TYPE_KEY = "ENEMY_MOVE_TYPE";



	private int _health;
	private int _shield;
	// TODO: Score.

	private int _floorNum;
	private Vector2Int _floorSize;
	private Vector2Int _stairPos;
	private Vector2Int _morphyPos;
	private Vector2Int[] _enemyPos;
	private eMoveType[] _enemyMoveType;



	public int Health { get { return _health; } }
	public int Shield { get { return _shield; } }

	public int FloorNum { get { return _floorNum; } }
	public Vector2Int FloorSize { get { return _floorSize; } }
	public Vector2Int StairPos { get { return _stairPos; } }
	public Vector2Int MorphyPos { get { return _morphyPos; } }
	public Vector2Int[] EnemyPos { get { return _enemyPos; } }
	public eMoveType[] EnemyMoveType { get { return _enemyMoveType; } }

	public RunData(Dungeon inDungeon,
		Floor inFloor, Enemy[] inEnemies)
	{
		_health = inDungeon.MorphyController.Health;
		_shield = inDungeon.MorphyController.Shield;

		// Floor
		_floorNum = inFloor.FloorNum;
		_floorSize = inFloor.Size;
		_stairPos = inFloor.StairsPos;
		_morphyPos = inFloor.MorphyPos;

		int numEnemies = inEnemies.Length;
		_enemyPos = new Vector2Int[numEnemies];
		_enemyMoveType = new eMoveType[numEnemies];
		for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
		{
			Enemy curEnemy = inEnemies[iEnemy];
			_enemyPos[iEnemy] = curEnemy.Pos;
			_enemyMoveType[iEnemy] = curEnemy.Type;
		}
	}

	public RunData(
		int inHealth,
		int inShield,

		int inFloorNum,
		Vector2Int inSize,
		Vector2Int inStairPos,
		Vector2Int inMorphyPos,
		Vector2Int[] inEnemyPos,
		eMoveType[] inEnemyMoveType)
	{
		_health = inHealth;
		_shield = inShield;

		_floorNum = inFloorNum;
		_floorSize = inSize;
		_stairPos = inStairPos;
		_morphyPos = inMorphyPos;
		_enemyPos = inEnemyPos;
		_enemyMoveType = inEnemyMoveType;
	}
}