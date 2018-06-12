using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RunData
{
	public const string FILENAME = "PrevRunData";

	public const string HEALTH_KEY = "HEALTH";

	public const string FLOOR_NUM_KEY = "FLOOR_NUM";
	public const string FLOOR_SIZE_KEY = "FLOOR_SIZE";
	public const string STAIR_POS_KEY = "STAIR_POS";
	public const string MORPHY_POS_KEY = "MORPHY_POS";
	public const string ENEMY_POS_X_KEY = "ENEMY_POS_X";
	public const string ENEMY_POS_Y_KEY = "ENEMY_POS_Y";
	public const string ENEMY_MOVE_TYPE_KEY = "ENEMY_MOVE_TYPE";

	public const string IS_FIRST_DRAW_OF_GAME_KEY = "IS_FIRST_DRAW_OF_GAME";
	public const string IS_FIRST_DRAW_OF_FLOOR_KEY = "IS_FIRST_DRAW_OF_FLOOR";
	public const string CARD_TIER_KEY = "CARD_TIER";
	public const string CARD_TYPE_KEY = "CARD_TYPE";
	public const string CARD_IS_CLONED_KEY = "CARD_IS_CLONED";
	public const string CARD_MOVE_TYPE_KEY = "CARD_MOVE_TYPE";



	private int _health;
	// TODO: Score.

	private int _floorNum;
	private Vector2Int _floorSize;
	private Vector2Int _stairPos;
	private Vector2Int _morphyPos;
	private Vector2Int[] _enemyPos;
	private eMoveType[] _enemyMoveType;

	private bool _isFirstDrawOfGame;
	private bool _isFirstDrawOfFloor;
	private CardData[] _cardDatas;



	public int Health { get { return _health; } }

	public int FloorNum { get { return _floorNum; } }
	public Vector2Int FloorSize { get { return _floorSize; } }
	public Vector2Int StairPos { get { return _stairPos; } }
	public Vector2Int MorphyPos { get { return _morphyPos; } }
	public Vector2Int[] EnemyPos { get { return _enemyPos; } }
	public eMoveType[] EnemyMoveType { get { return _enemyMoveType; } }

	public bool IsFirstDrawOfGame { get { return _isFirstDrawOfGame; } }
	public bool IsFirstDrawOfFloor { get { return _isFirstDrawOfFloor; } }
	public CardData[] CardDatas { get { return _cardDatas; } }

	public RunData(Dungeon inDungeon)
	{
		_health = inDungeon.MorphyController.Health;

		// Floor
		_floorNum = inDungeon.CurrentFloor.FloorNum;
		_floorSize = inDungeon.CurrentFloor.Size;
		_stairPos = inDungeon.CurrentFloor.StairsPos;
		_morphyPos = inDungeon.CurrentFloor.MorphyPos;

		Enemy[] enemiesAlive = inDungeon.EnemyManager.GetArrayOfAliveEnemies();
		int numEnemies = enemiesAlive.Length;
		_enemyPos = new Vector2Int[numEnemies];
		_enemyMoveType = new eMoveType[numEnemies];
		for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
		{
			Enemy curEnemy = enemiesAlive[iEnemy];
			_enemyPos[iEnemy] = curEnemy.Pos;
			_enemyMoveType[iEnemy] = curEnemy.Type;
		}

		// Cards
		_isFirstDrawOfGame = inDungeon.CardManager.IsFirstDrawOfGame;
		_isFirstDrawOfFloor = inDungeon.CardManager.IsFirstDrawOfFloor;
		_cardDatas = inDungeon.CardManager.GenerateCardHandData();
	}

	public RunData(
		int inHealth,

		int inFloorNum,
		Vector2Int inSize,
		Vector2Int inStairPos,
		Vector2Int inMorphyPos,
		Vector2Int[] inEnemyPos,
		eMoveType[] inEnemyMoveType,
		
		bool inIsFirstDrawOfGame,
        bool inIsFirstDrawOfFloor,
		CardData[] inCardDatas)
	{
		_health = inHealth;

		_floorNum = inFloorNum;
		_floorSize = inSize;
		_stairPos = inStairPos;
		_morphyPos = inMorphyPos;
		_enemyPos = inEnemyPos;
		_enemyMoveType = inEnemyMoveType;

		_isFirstDrawOfGame = inIsFirstDrawOfGame;
		_isFirstDrawOfFloor = inIsFirstDrawOfFloor;
		_cardDatas = inCardDatas;
	}
}