using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public static class DataLoader
{
	private static bool _hasStartedLoadingData = false;
	public static bool HasStartedLoadingData { get { return _hasStartedLoadingData; } }
	private static bool _hasLoadedAllData = false;
	public static bool HasLoadedAllData { get { return _hasLoadedAllData; } }
	private static bool _hasPreviousRunData = false;
	public static bool HasPreviousRunData { get { return _hasPreviousRunData; } }

	public static Utils.GenericVoidDelegate OnAllDataLoaded = null;

	// Data structs
	private static PrevRunData _prevRunData;
	public static PrevRunData SavedPrevRunData { get { return _prevRunData; } }
	private static FloorData _floorData;
	public static FloorData SavedFloorData { get { return _floorData; } }
	private static CardHandData _cardHandData;
	public static CardHandData SavedCardHandData { get { return _cardHandData; } }

	private static PlayerData _playerData;
	public static PlayerData SavedPlayerData { get { return _playerData; } }

	// Data file names
	private const string PREV_RUN_DATA_FILENAME = "PrevRunData.txt";
	private const string FLOOR_DATA_FILENAME = "FloorData.txt";
	private const string CARD_HAND_DATA_FILENAME = "CardhandData.txt";
	private const string PLAYER_DATA_FILENAME = "PlayerData.txt";
	private const string GAME_DATA_JSON_FILENAME = "GameDataJson.txt";

	// Keys
	private const string PREV_RUN_DATA_HEALTH_KEY = "PREV_RUN_DATA_HEALTH";
	private const string PREV_RUN_DATA_SHIELD_KEY = "PREV_RUN_DATA_SHIELD";

	private const string FLOOR_DATA_FLOORNUM_KEY = "FLOOR_DATA_FLOORNUM";
	private const string FLOOR_DATA_SIZE_KEY = "FLOOR_DATA_SIZE";
	private const string FLOOR_DATA_STAIR_POS_KEY = "FLOOR_DATA_STAIR_POS";
	private const string FLOOR_DATA_MORPHY_POS_KEY = "FLOOR_DATA_MORPHY_POS";
	private const string FLOOR_DATA_ENEMY_POS_X_KEY = "FLOOR_DATA_ENEMY_POS_X";
	private const string FLOOR_DATA_ENEMY_POS_Y_KEY = "FLOOR_DATA_ENEMY_POS_Y";
	private const string FLOOR_DATA_ENEMY_MOVE_TYPE_KEY = "FLOOR_DATA_ENEMY_MOVE_TYPE";

    private const string CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME_KEY = "CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME";
    private const string CARD_HAND_DATA_HAS_DONE_FIRST_TURN_DRAW_KEY = "CARD_HAND_DATA_HAS_DONE_FIRST_TURN_DRAW";
	private const string CARD_HAND_DATA_CARD_TIER_KEY = "CARD_HAND_DATA_CARD_TIER";
	private const string CARD_HAND_DATA_CARD_TYPE_KEY = "CARD_HAND_DATA_CARD_TYPE";
	private const string CARD_HAND_DATA_IS_CLONED_KEY = "CARD_HAND_DATA_IS_CLONED";
	private const string CARD_HAND_DATA_CARD_MOVE_TYPE_KEY = "CARD_HAND_DATA_CARD_MOVE_TYPE";

	private const string PLAYER_DATA_NUM_GEMS_KEY = "PLAYER_DATA_NUM_GEMS";

	#region Data Structs
	public struct PrevRunData
	{
		private int _health;
		private int _shield;
		// TODO: Score.

		public int Health { get { return _health; } }
		public int Shield { get { return _shield; } }

		public PrevRunData(Dungeon inDungeon)
		{
			_health = inDungeon.MorphyController.Health;
			_shield = inDungeon.MorphyController.Shield;
		}

		public PrevRunData(int inHealth, int inShield)
		{
			_health = inHealth;
			_shield = inShield;
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

		public int FloorNum { get { return _floorNum; } }
		public Vector2Int Size { get { return _size; } }
		public Vector2Int StairPos { get { return _stairPos; } }
		public Vector2Int MorphyPos { get { return _morphyPos; } }
		public Vector2Int[] EnemyPos { get { return _enemyPos; } }
		public eMoveType[] EnemyMoveType { get { return _enemyMoveType; } }

		public FloorData(Floor inFloor, Enemy[] inEnemies)
		{
			_floorNum = inFloor.FloorNum;
			_size = inFloor.Size;
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

		public FloorData(
			int inFloorNum,
			Vector2Int inSize,
			Vector2Int inStairPos,
			Vector2Int inMorphyPos,
			Vector2Int[] inEnemyPos,
			eMoveType[] inEnemyMoveType)
		{
			_floorNum = inFloorNum;
			_size = inSize;
			_stairPos = inStairPos;
			_morphyPos = inMorphyPos;
			_enemyPos = inEnemyPos;
			_enemyMoveType = inEnemyMoveType;
		}
	}

	public struct CardHandData
	{
        private bool _isFirstDrawOfGame;
        private bool _hasDoneFirstTurnDraw;
		private CardData[] _cardDatas;
		
        public bool IsFirstDrawOfGame { get { return _isFirstDrawOfGame; } }
        public bool HasDoneFirstTurnDraw { get { return _hasDoneFirstTurnDraw; } }
		public CardData[] CardDatas { get { return _cardDatas; } }

		public CardHandData(bool inIsFirstDrawOfGame, bool inHasDoneFirstTurnDraw, params CardData[] inCardDatas)
		{
            _isFirstDrawOfGame = inIsFirstDrawOfGame;
            _hasDoneFirstTurnDraw = inHasDoneFirstTurnDraw;
			_cardDatas = inCardDatas;
		}
	}
	#endregion

	public static void TryLoadData()
	{
		if (!HasLoadedAllData && !HasStartedLoadingData)
		{
			_hasStartedLoadingData = true;

			DTJob loadPreviousRunDataJob = new DTJob(
				(OnComplete) =>
				{
					LoadPreviousRunData(OnComplete);
				});

			DTJob loadPlayerDataJob = new DTJob(
				(OnComplete) =>
				{
					LoadPlayerData(OnComplete);
				});

			DTJob loadJSONJob = new DTJob(
				(OnComplete) =>
				{
					CoroutineUtility.StartCoroutineImmediate(LoadJSON(OnComplete));
				});

			DTJobList loadDataJobList = new DTJobList(
				() =>
				{
					_hasLoadedAllData = true;
					Debug.Log("ALL DATA LOADED");
					if (OnAllDataLoaded != null) OnAllDataLoaded();
				},
				loadPreviousRunDataJob,
				loadPlayerDataJob,
				loadJSONJob);


			loadDataJobList.ExecuteAllJobs();
		}
		else
		{
			Debug.LogWarning("Data has already been loaded.");
		}
	}

	public static void SavePreviousRunData(PrevRunData inPrevRunData, FloorData inFloorData, CardHandData inCardHandData)
	{
		using (ES2Writer writer = ES2Writer.Create(PREV_RUN_DATA_FILENAME))
		{
			writer.Write(inPrevRunData.Health, PREV_RUN_DATA_HEALTH_KEY);
			writer.Write(inPrevRunData.Shield, PREV_RUN_DATA_SHIELD_KEY);

			writer.Save();
		}

		using (ES2Writer writer = ES2Writer.Create(FLOOR_DATA_FILENAME))
		{
			writer.Write(inFloorData.FloorNum, FLOOR_DATA_FLOORNUM_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.Size), FLOOR_DATA_SIZE_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.StairPos), FLOOR_DATA_STAIR_POS_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inFloorData.MorphyPos), FLOOR_DATA_MORPHY_POS_KEY);

			int numEnemies = inFloorData.EnemyPos.Length;
			int[] enemyPosX = new int[numEnemies];
			int[] enemyPosY = new int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPosX[iEnemy] = inFloorData.EnemyPos[iEnemy].x;
				enemyPosY[iEnemy] = inFloorData.EnemyPos[iEnemy].y;
			}
			writer.Write(enemyPosX, FLOOR_DATA_ENEMY_POS_X_KEY);
			writer.Write(enemyPosY, FLOOR_DATA_ENEMY_POS_Y_KEY);

			writer.Write(inFloorData.EnemyMoveType, FLOOR_DATA_ENEMY_MOVE_TYPE_KEY);

			writer.Save();
		}

		using (ES2Writer writer = ES2Writer.Create(CARD_HAND_DATA_FILENAME))
		{
			int numCards = inCardHandData.CardDatas.Length;
			eCardTier[] cardTier = new eCardTier[numCards];
			eCardType[] cardType = new eCardType[numCards];
			bool[] isCloned = new bool[numCards];
			eMoveType[] cardMoveType = new eMoveType[numCards];

			for (int iCard = 0; iCard < numCards; iCard++)
			{
				CardData curCardData = inCardHandData.CardDatas[iCard];
				cardTier[iCard] = curCardData.cardTier;
				cardType[iCard] = curCardData.cardType;
				isCloned[iCard] = curCardData.isCloned;
				cardMoveType[iCard] = curCardData.cardMoveType;
			}

            writer.Write(inCardHandData.IsFirstDrawOfGame, CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME_KEY);
            writer.Write(inCardHandData.HasDoneFirstTurnDraw, CARD_HAND_DATA_HAS_DONE_FIRST_TURN_DRAW_KEY);
			writer.Write(cardTier, CARD_HAND_DATA_CARD_TIER_KEY);
			writer.Write(cardType, CARD_HAND_DATA_CARD_TYPE_KEY);
			writer.Write(isCloned, CARD_HAND_DATA_IS_CLONED_KEY);
			writer.Write(cardMoveType, CARD_HAND_DATA_CARD_MOVE_TYPE_KEY);

			writer.Save();
		}

		// NOTE: Updates the currently loaded data.
		_prevRunData = inPrevRunData;
		_floorData = inFloorData;
		_cardHandData = inCardHandData;
		_hasPreviousRunData = true;
	}

	private static void LoadPreviousRunData(DTJob.OnCompleteCallback inOnComplete)
	{
		if (ES2.Exists(PREV_RUN_DATA_FILENAME)
			&& ES2.Exists(FLOOR_DATA_FILENAME)
			&& ES2.Exists(CARD_HAND_DATA_FILENAME))
		{
			ES2Data prevRunData = ES2.LoadAll(PREV_RUN_DATA_FILENAME);
			ES2Data floorData = ES2.LoadAll(FLOOR_DATA_FILENAME);
			ES2Data cardHandData = ES2.LoadAll(CARD_HAND_DATA_FILENAME);

			// PREV_RUN_DATA
			_prevRunData = new PrevRunData(
				prevRunData.Load<int>(PREV_RUN_DATA_HEALTH_KEY),
				prevRunData.Load<int>(PREV_RUN_DATA_SHIELD_KEY));

			// FLOOR_DATA
			Vector2Int size = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOOR_DATA_SIZE_KEY));
			Vector2Int stairPos = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOOR_DATA_STAIR_POS_KEY));
			Vector2Int morphyPos = Utils.IntArrayToSingleVector2Int(floorData.LoadArray<int>(FLOOR_DATA_MORPHY_POS_KEY));
			int[] enemyPosX = floorData.LoadArray<int>(FLOOR_DATA_ENEMY_POS_X_KEY);
			int[] enemyPosY = floorData.LoadArray<int>(FLOOR_DATA_ENEMY_POS_Y_KEY);
			int numEnemies = enemyPosX.Length;
			Vector2Int[] enemyPos = new Vector2Int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPos[iEnemy] = new Vector2Int(enemyPosX[iEnemy], enemyPosY[iEnemy]);
			}
			_floorData = new FloorData(
				floorData.Load<int>(FLOOR_DATA_FLOORNUM_KEY),
				size,
				stairPos,
				morphyPos,
				enemyPos,
				floorData.LoadArray<eMoveType>(FLOOR_DATA_ENEMY_MOVE_TYPE_KEY));

            // CARD_HAND_DATA
            bool isFirstDrawOfGame = cardHandData.Load<bool>(CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME_KEY);
            bool hasDoneFirstTurnDraw = cardHandData.Load<bool>(CARD_HAND_DATA_HAS_DONE_FIRST_TURN_DRAW_KEY);
			eCardTier[] cardTier = cardHandData.LoadArray<eCardTier>(CARD_HAND_DATA_CARD_TIER_KEY);
			eCardType[] cardType = cardHandData.LoadArray<eCardType>(CARD_HAND_DATA_CARD_TYPE_KEY);
			bool[] isCloned = cardHandData.LoadArray<bool>(CARD_HAND_DATA_IS_CLONED_KEY);
			eMoveType[] cardMoveType = cardHandData.LoadArray<eMoveType>(CARD_HAND_DATA_CARD_MOVE_TYPE_KEY);

			int numCards = cardTier.Length;
			CardData[] cardDatas = new CardData[numCards];
			for (int iCard = 0; iCard < numCards; iCard++)
			{
				cardDatas[iCard] = new CardData(
					cardTier[iCard],
					cardType[iCard],
					isCloned[iCard],
					cardMoveType[iCard]);
			}

			_cardHandData = new CardHandData(isFirstDrawOfGame, hasDoneFirstTurnDraw, cardDatas);

			_hasPreviousRunData = true;
		}
		else
		{
			Debug.Log("There is no save data found.");
			_hasPreviousRunData = false;
		}

		if (inOnComplete != null) inOnComplete();
    }

	public static void DeletePreviousRunData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (ES2.Exists(PREV_RUN_DATA_FILENAME)) ES2.Delete(PREV_RUN_DATA_FILENAME);
		if (ES2.Exists(FLOOR_DATA_FILENAME)) ES2.Delete(FLOOR_DATA_FILENAME);
		if (ES2.Exists(CARD_HAND_DATA_FILENAME)) ES2.Delete(CARD_HAND_DATA_FILENAME);

		_hasPreviousRunData = false;

		if (inOnComplete != null) inOnComplete();
	}

	public static void SavePlayerData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		using (ES2Writer writer = ES2Writer.Create(PLAYER_DATA_FILENAME))
		{
			writer.Write(_playerData.NumGems, PLAYER_DATA_NUM_GEMS_KEY);

			writer.Save();
		}

		if (inOnComplete != null) inOnComplete();
	}

	private static void LoadPlayerData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (ES2.Exists(PLAYER_DATA_FILENAME))
		{
			ES2Data playerData = ES2.LoadAll(PLAYER_DATA_FILENAME);

            // TODO: Create a TryLoad<T> that will return 0 if failed to load. Safer for future updates and all that.
            _playerData = new PlayerData(playerData.Load<int>(PLAYER_DATA_NUM_GEMS_KEY));
		}
		else // If no have, create empty. Basically new player.
		{
			_playerData = new PlayerData(0);
		}

		if (inOnComplete != null) inOnComplete();
	}

	private static IEnumerator LoadJSON(DTJob.OnCompleteCallback inOnComplete)
	{
		// TODO: Caching system to save the JSON.
		const string gameDataJsonURL = "https://storage.googleapis.com/rainblastgames-chessgeon-cdn/CDN/GameDataJson.txt";
		using (WWW www = new WWW(gameDataJsonURL))
		{
			yield return www;

			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
			}
			else
			{
                Debug.Log("DOING NOTHING TO PROCESS GAMEDATAJSON");
                Debug.Log(www.text);
			
				inOnComplete();
			}
		}
	}

	public static void AwardGems(int inNumGemsAwarded) { _playerData.AwardGems(inNumGemsAwarded); }
	public static bool SpendGems(int inNumGemsToSpend) { return _playerData.SpendGems(inNumGemsToSpend); }
}
