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
	private static RunData _prevRunData;
	public static RunData PrevRunData { get { return _prevRunData; } }
	private static CardHandData _cardHandData;
	public static CardHandData SavedCardHandData { get { return _cardHandData; } }

	private static PlayerData _playerData;
	public static PlayerData SavedPlayerData { get { return _playerData; } }

	// Data file names
	private const string CARD_HAND_DATA_FILENAME = "CardhandData.txt";
	private const string GAME_DATA_JSON_FILENAME = "GameDataJson.txt";

	// Keys
    private const string CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME_KEY = "CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME";
	private const string CARD_HAND_DATA_CARD_TIER_KEY = "CARD_HAND_DATA_CARD_TIER";
	private const string CARD_HAND_DATA_CARD_TYPE_KEY = "CARD_HAND_DATA_CARD_TYPE";
	private const string CARD_HAND_DATA_IS_CLONED_KEY = "CARD_HAND_DATA_IS_CLONED";
	private const string CARD_HAND_DATA_CARD_MOVE_TYPE_KEY = "CARD_HAND_DATA_CARD_MOVE_TYPE";

	#region Data Structs

	public struct CardHandData
	{
        private bool _isFirstDrawOfGame;
		private CardData[] _cardDatas;
		
        public bool IsFirstDrawOfGame { get { return _isFirstDrawOfGame; } }
		public CardData[] CardDatas { get { return _cardDatas; } }

		public CardHandData(bool inIsFirstDrawOfGame, params CardData[] inCardDatas)
		{
            _isFirstDrawOfGame = inIsFirstDrawOfGame;
			_cardDatas = inCardDatas;
		}
	}
	#endregion

	public static void TryLoadAllData()
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

	public static void SavePreviousRunData(RunData inPrevRunData, CardHandData inCardHandData)
	{
		using (ES2Writer writer = ES2Writer.Create(RunData.FILENAME))
		{
			writer.Write(inPrevRunData.Health, RunData.HEALTH_KEY);
			writer.Write(inPrevRunData.Shield, RunData.SHIELD_KEY);

			writer.Write(inPrevRunData.FloorNum, RunData.FLOOR_NUM_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inPrevRunData.FloorSize), RunData.FLOOR_SIZE_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inPrevRunData.StairPos), RunData.STAIR_POS_KEY);
			writer.Write(Utils.Vector2IntToIntArray(inPrevRunData.MorphyPos), RunData.MORPHY_POS_KEY);

			int numEnemies = inPrevRunData.EnemyPos.Length;
			int[] enemyPosX = new int[numEnemies];
			int[] enemyPosY = new int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPosX[iEnemy] = inPrevRunData.EnemyPos[iEnemy].x;
				enemyPosY[iEnemy] = inPrevRunData.EnemyPos[iEnemy].y;
			}
			writer.Write(enemyPosX, RunData.ENEMY_POS_X_KEY);
			writer.Write(enemyPosY, RunData.ENEMY_POS_Y_KEY);

			writer.Write(inPrevRunData.EnemyMoveType, RunData.ENEMY_MOVE_TYPE_KEY);

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
			writer.Write(cardTier, CARD_HAND_DATA_CARD_TIER_KEY);
			writer.Write(cardType, CARD_HAND_DATA_CARD_TYPE_KEY);
			writer.Write(isCloned, CARD_HAND_DATA_IS_CLONED_KEY);
			writer.Write(cardMoveType, CARD_HAND_DATA_CARD_MOVE_TYPE_KEY);

			writer.Save();
		}

		// NOTE: Updates the currently loaded data.
		_prevRunData = inPrevRunData;
		_cardHandData = inCardHandData;
		_hasPreviousRunData = true;
	}

	private static void LoadPreviousRunData(DTJob.OnCompleteCallback inOnComplete)
	{
		if (ES2.Exists(RunData.FILENAME)
			&& ES2.Exists(CARD_HAND_DATA_FILENAME))
		{
			ES2Data prevRunData = ES2.LoadAll(RunData.FILENAME);
			ES2Data cardHandData = ES2.LoadAll(CARD_HAND_DATA_FILENAME);

			Vector2Int size = Utils.IntArrayToSingleVector2Int(TryLoadArray<int>(prevRunData, RunData.FLOOR_SIZE_KEY));
			Vector2Int stairPos = Utils.IntArrayToSingleVector2Int(TryLoadArray<int>(prevRunData, RunData.STAIR_POS_KEY));
			Vector2Int morphyPos = Utils.IntArrayToSingleVector2Int(TryLoadArray<int>(prevRunData, RunData.MORPHY_POS_KEY));
			int[] enemyPosX = TryLoadArray<int>(prevRunData, RunData.ENEMY_POS_X_KEY);
			int[] enemyPosY = TryLoadArray<int>(prevRunData, RunData.ENEMY_POS_Y_KEY);
			int numEnemies = enemyPosX.Length;
			Vector2Int[] enemyPos = new Vector2Int[numEnemies];
			for (int iEnemy = 0; iEnemy < numEnemies; iEnemy++)
			{
				enemyPos[iEnemy] = new Vector2Int(enemyPosX[iEnemy], enemyPosY[iEnemy]);
			}

			// PREV_RUN_DATA
			_prevRunData = new RunData(
				TryLoad<int>(prevRunData, RunData.HEALTH_KEY),
				TryLoad<int>(prevRunData, RunData.SHIELD_KEY),

				TryLoad<int>(prevRunData, RunData.FLOOR_NUM_KEY),
				size,
				stairPos,
				morphyPos,
				enemyPos,
				TryLoadArray<eMoveType>(prevRunData, RunData.ENEMY_MOVE_TYPE_KEY));

			// CARD_HAND_DATA
			bool isFirstDrawOfGame = cardHandData.Load<bool>(CARD_HAND_DATA_IS_FIRST_DRAW_OF_GAME_KEY);
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

			_cardHandData = new CardHandData(isFirstDrawOfGame, cardDatas);

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
		if (ES2.Exists(RunData.FILENAME)) ES2.Delete(RunData.FILENAME);
		if (ES2.Exists(CARD_HAND_DATA_FILENAME)) ES2.Delete(CARD_HAND_DATA_FILENAME);

		_hasPreviousRunData = false;

		if (inOnComplete != null) inOnComplete();
	}

	public static void SavePlayerData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		using (ES2Writer writer = ES2Writer.Create(PlayerData.FILENAME))
		{
			writer.Write(_playerData.NumGems, PlayerData.NUM_GEMS_KEY);

			writer.Save();
		}

		if (inOnComplete != null) inOnComplete();
	}

	private static void LoadPlayerData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (ES2.Exists(PlayerData.FILENAME))
		{
			ES2Data playerData = ES2.LoadAll(PlayerData.FILENAME);

			_playerData = new PlayerData(TryLoad<int>(playerData, PlayerData.NUM_GEMS_KEY));
		}
		else // If no have, create empty. Basically new player.
		{
			_playerData = new PlayerData(0);
		}

		if (inOnComplete != null) inOnComplete();
	}

	// TODO: Save immediately when calling these gem functions.
	public static void AwardGems(int inNumGemsAwarded) { _playerData.AwardGems(inNumGemsAwarded); }
	public static bool SpendGems(int inNumGemsToSpend) { return _playerData.SpendGems(inNumGemsToSpend); }

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

	#region Helper Functions
	private static T TryLoad<T>(ES2Data inData, string inKey)
	{
		if (inData.TagExists(inKey))
			return inData.Load<T>(inKey);
		else
		{
			Debug.LogWarning("Key " + inKey + " does not exist!");
			return default(T);
		}
	}

	private static T[] TryLoadArray<T>(ES2Data inData, string inKey)
	{
		if (inData.TagExists(inKey))
			return inData.LoadArray<T>(inKey);
		else
		{
			Debug.LogWarning("Key " + inKey + " does not exist!");
			return new T[0];
		}
	}
	#endregion
}
