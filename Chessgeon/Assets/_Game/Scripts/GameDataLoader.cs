using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public static class GameDataLoader
{
	private static bool _hasStartedLoadingData = false;
	public static bool HasStartedLoadingData { get { return _hasStartedLoadingData; } }
	private static bool _hasLoadedAllData = false;
	public static bool HasLoadedAllData { get { return _hasLoadedAllData; } }

	// Data structs
	private static GameData _gameData;
	public static GameData SavedGameData { get { return _gameData; } }
	private static FloorData _floorData;
	public static FloorData SavedFloorData { get { return _floorData; } }

	// Data file names
	private const string GAMEDATA_FILENAME = "gamedata.txt";
	private const string FLOORDATA_FILENAME = "floordata.txt";
	private const string CARDDATA_FILENAME = "carddata.txt";

	// Keys
	private const string GAMEDATA_HEALTH_KEY = "GAMEDATA_HEALTH";
	private const string GAMEDATA_SHIELD_KEY = "GAMEDATA_SHIELD";
	private const string GAMEDATA_NUMCOINS_KEY = "GAMEDATA_NUM_COINS";

	private const string FLOORDATA_FLOORNUM_KEY = "FLOORDATA_FLOORNUM";
	private const string FLOORDATA_SIZE_KEY = "FLOORDATA_SIZE";
	private const string FLOORDATA_STAIR_POS_KEY = "FLOORDATA_STAIR_POS";
	private const string FLOORDATA_MORPHY_POS_KEY = "FLOORDATA_MORPHY_POS";
	private const string FLOORDATA_ENEMY_POS_X_KEY = "FLOORDATA_ENEMY_POS_X";
	private const string FLOORDATA_ENEMY_POS_Y_KEY = "FLOORDATA_ENEMY_POS_Y";
	private const string FLOORDATA_ENEMY_MOVE_TYPE_KEY = "FLOORDATA_ENEMY_MOVE_TYPE";
	private const string FLOORDATA_ENEMY_ELEMENT_KEY = "FLOORDATA_ENEMY_ELEMENT";

	public struct GameData
	{
		private int _health;
		private int _shield;
		private int _numCoins;
		// TODO: Score.

		public int Health { get { return _health; } }
		public int Shield { get { return _shield; } }
		public int NumCoins { get { return _numCoins; } }

		public GameData(Dungeon inDungeon)
		{
			_health = inDungeon.MorphyController.Health;
			_shield = inDungeon.MorphyController.Shield;
			_numCoins = inDungeon.NumCoins;
		}

		public GameData(int inHealth, int inShield, int inNumCoins)
		{
			_health = inHealth;
			_shield = inShield;
			_numCoins = inNumCoins;
		}
	}

	public struct FloorData
	{
		private int _floorNum;
		private Vector2Int _size;
		private Vector2Int _stairPos;
		private Vector2Int _morphyPos;
		private Vector2Int[] _enemyPos;
		private eMoveType[] _enemyMoveType;
		private Enemy.eElement[] _enemyElement;

		public int FloorNum { get { return _floorNum; } }
		public Vector2Int Size { get { return _size; } }
		public Vector2Int StairPos { get { return _stairPos; } }
		public Vector2Int MorphyPos { get { return _morphyPos; } }
		public Vector2Int[] EnemyPos { get { return _enemyPos; } }
		public eMoveType[] EnemyMoveType { get { return _enemyMoveType; } }
		public Enemy.eElement[] EnemyElement { get { return _enemyElement; } }

		public FloorData(Floor inFloor, Enemy[] inEnemies)
		{
			_floorNum = inFloor.FloorNum;
			_size = inFloor.Size;
			_stairPos = inFloor.StairsPos;
			_morphyPos = inFloor.MorphyPos;

			int numEnemies = inEnemies.Length;
			_enemyPos = new Vector2Int[numEnemies];
			_enemyMoveType = new eMoveType[numEnemies];
			_enemyElement = new Enemy.eElement[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				Enemy curEnemy = inEnemies[iEnemy];
				_enemyPos[iEnemy] = curEnemy.Pos;
				_enemyMoveType[iEnemy] = curEnemy.Type;
				_enemyElement[iEnemy] = curEnemy.Element;
			}
		}

		public FloorData(
			int inFloorNum,
			Vector2Int inSize,
			Vector2Int inStairPos,
			Vector2Int inMorphyPos,
			Vector2Int[] inEnemyPos,
			eMoveType[] inEnemyMoveType,
			Enemy.eElement[] inEnemyElement)
		{
			_floorNum = inFloorNum;
			_size = inSize;
			_stairPos = inStairPos;
			_morphyPos = inMorphyPos;
			_enemyPos = inEnemyPos;
			_enemyMoveType = inEnemyMoveType;
			_enemyElement = inEnemyElement;
		}
	}

	public static void TryLoadSaveData()
	{
		if (!HasLoadedAllData && !HasStartedLoadingData)
		{
			Debug.Log("Started TryLoadGameData");
			_hasStartedLoadingData = true;

			DTJob loadSavedDataJob = new DTJob(
				(OnComplete) =>
				{
					LoadSavedData(OnComplete);
				});

			DTJob loadJSONJob = new DTJob(
				(OnComplete) =>
				{
					CoroutineUtility.StartCoroutineImmediate(LoadJSON(OnComplete));
				});

			// DEBUG job
			DTJob artificialTimeDelayJob = new DTJob(
				(OnComplete) =>
				{
					CoroutineUtility.StartCoroutineImmediate(ArtificialTimeDelay(OnComplete));
				});

			DTJobList loadDataJobList = new DTJobList(
				() =>
				{
					_hasLoadedAllData = true;
					Debug.Log("ALL GAME DATA LOADED");
				},
				loadSavedDataJob,
				loadJSONJob,
				artificialTimeDelayJob);


			loadDataJobList.ExecuteAllJobs();
		}
		else
		{
			Debug.LogWarning("Data has already been loaded.");
		}
	}

	public static void SaveData(GameData inGameData, FloorData inFloorData)
	{
		using (ES2Writer writer = ES2Writer.Create(GAMEDATA_FILENAME))
		{
			writer.Write(inGameData.Health, GAMEDATA_HEALTH_KEY);
			writer.Write(inGameData.Shield, GAMEDATA_SHIELD_KEY);
			writer.Write(inGameData.NumCoins, GAMEDATA_NUMCOINS_KEY);

			writer.Save();
		}

		using (ES2Writer writer = ES2Writer.Create(FLOORDATA_FILENAME))
		{
			writer.Write(inFloorData.FloorNum, FLOORDATA_FLOORNUM_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.Size), FLOORDATA_SIZE_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.StairPos), FLOORDATA_STAIR_POS_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.MorphyPos), FLOORDATA_MORPHY_POS_KEY);

			int numEnemies = inFloorData.EnemyPos.Length;
			int[] enemyPosX = new int[numEnemies];
			int[] enemyPosY = new int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPosX[iEnemy] = inFloorData.EnemyPos[iEnemy].x;
				enemyPosY[iEnemy] = inFloorData.EnemyPos[iEnemy].y;
			}
			writer.Write(enemyPosX, FLOORDATA_ENEMY_POS_X_KEY);
			writer.Write(enemyPosY, FLOORDATA_ENEMY_POS_Y_KEY);

			writer.Write(inFloorData.EnemyMoveType, FLOORDATA_ENEMY_MOVE_TYPE_KEY);
			writer.Write(inFloorData.EnemyElement, FLOORDATA_ENEMY_ELEMENT_KEY);

			writer.Save();
		}
	}

	private static void LoadSavedData(DTJob.OnCompleteCallback inOnComplete)
	{
		if (ES2.Exists(GAMEDATA_FILENAME)
			&& ES2.Exists(FLOORDATA_FILENAME))
		{
			ES2Data gameData = ES2.LoadAll(GAMEDATA_FILENAME);
			ES2Data floorData = ES2.LoadAll(FLOORDATA_FILENAME);

			_gameData = new GameData(
				gameData.Load<int>(GAMEDATA_HEALTH_KEY),
				gameData.Load<int>(GAMEDATA_SHIELD_KEY),
				gameData.Load<int>(GAMEDATA_NUMCOINS_KEY));

			Vector2Int size = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOORDATA_SIZE_KEY));
			Vector2Int stairPos = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOORDATA_STAIR_POS_KEY));
			Vector2Int morphyPos = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOORDATA_MORPHY_POS_KEY));
			int[] enemyPosX = floorData.LoadArray<int>(FLOORDATA_ENEMY_POS_X_KEY);
			int[] enemyPosY = floorData.LoadArray<int>(FLOORDATA_ENEMY_POS_Y_KEY);
			int numEnemies = enemyPosX.Length;
			Vector2Int[] enemyPos = new Vector2Int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPos[iEnemy] = new Vector2Int(enemyPosX[iEnemy], enemyPosY[iEnemy]);
			}
			_floorData = new FloorData(
				floorData.Load<int>(FLOORDATA_FLOORNUM_KEY),
				size,
				stairPos,
				morphyPos,
				enemyPos,
				floorData.LoadArray<eMoveType>(FLOORDATA_ENEMY_MOVE_TYPE_KEY),
				floorData.LoadArray<Enemy.eElement>(FLOORDATA_ENEMY_ELEMENT_KEY));
		}
		else
		{
			Debug.Log("There is no save data found.");
		}

		inOnComplete();
    }

	private static IEnumerator LoadJSON(DTJob.OnCompleteCallback inOnComplete)
	{
		// TODO: Implement this when I actually get the proper CDN up and running.

		Debug.Log("Started LoadJSON job");
		using (WWW www = new WWW("http://www.rainblastgames.com"))
		{
			yield return www;
			Debug.Log(www.text);
			inOnComplete();
		}
	}


	// TODO: Remove this debug.
	// DEBUG
	private static IEnumerator ArtificialTimeDelay(DTJob.OnCompleteCallback inOnComplete)
	{
		Debug.Log("Started ArtificialTimeDelayJob");
		float timer = 0.0f;
		while (true)
		{
			timer += Time.deltaTime;
			if (timer < 1.5f) yield return null;
			else break;
		}

		inOnComplete();
	}
}
