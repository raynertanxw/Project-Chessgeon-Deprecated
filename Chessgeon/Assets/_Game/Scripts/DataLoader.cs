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

	private static PersistentData _persistentData;
	public static PersistentData SavedPersistentData { get { return _persistentData; } }

	private static UpgradesData _upgradesData;
	public static UpgradesData LoadedUpgradesData { get { return _upgradesData; } }

	// Data file names
	private const string PREV_RUN_DATA_FILENAME = "PrevRunData.txt";
	private const string FLOOR_DATA_FILENAME = "FloorData.txt";
	private const string CARD_HAND_DATA_FILENAME = "CardhandData.txt";
	private const string PERSISTENT_DATA_FILENAME = "PersistentData.txt";
	private const string GAME_DATA_JSON_FILENAME = "GameDataJson.txt";

	// Keys
	private const string PREV_RUN_DATA_HEALTH_KEY = "PREV_RUN_DATA_HEALTH";
	private const string PREV_RUN_DATA_SHIELD_KEY = "PREV_RUN_DATA_SHIELD";
	private const string PREV_RUN_DATA_NUMCOINS_KEY = "PREV_RUN_DATA_NUM_COINS";

	private const string FLOOR_DATA_FLOORNUM_KEY = "FLOOR_DATA_FLOORNUM";
	private const string FLOOR_DATA_SIZE_KEY = "FLOOR_DATA_SIZE";
	private const string FLOOR_DATA_STAIR_POS_KEY = "FLOOR_DATA_STAIR_POS";
	private const string FLOOR_DATA_MORPHY_POS_KEY = "FLOOR_DATA_MORPHY_POS";
	private const string FLOOR_DATA_ENEMY_POS_X_KEY = "FLOOR_DATA_ENEMY_POS_X";
	private const string FLOOR_DATA_ENEMY_POS_Y_KEY = "FLOOR_DATA_ENEMY_POS_Y";
	private const string FLOOR_DATA_ENEMY_MOVE_TYPE_KEY = "FLOOR_DATA_ENEMY_MOVE_TYPE";
	private const string FLOOR_DATA_ENEMY_ELEMENT_KEY = "FLOOR_DATA_ENEMY_ELEMENT";

	private const string CARD_HAND_DATA_CARD_TIER_KEY = "CARD_HAND_DATA_CARD_TIER";
	private const string CARD_HAND_DATA_CARD_TYPE_KEY = "CARD_HAND_DATA_CARD_TYPE";
	private const string CARD_HAND_DATA_IS_CLONED_KEY = "CARD_HAND_DATA_IS_CLONED";
	private const string CARD_HAND_DATA_CARD_MOVE_TYPE_KEY = "CARD_HAND_DATA_CARD_MOVE_TYPE";

	private const string PERSISTENT_DATA_NUM_GEMS_KEY = "PERSISTENT_DATA_NUM_GEMS";
	private const string PERSISTENT_DATA_UPGRADE_LEVEL_HEALTH_KEY = "PERSISTENT_DATA_UPGRADE_LEVEL_HEALTH";
	private const string PERSISTENT_DATA_UPGRADE_LEVEL_COIN_DROP_KEY = "PERSISTENT_DATA_UPGRADE_LEVEL_COIN_DROP";
	private const string PERSISTENT_DATA_UPGRADE_LEVEL_SHOP_PRICE_KEY = "PERSISTENT_DATA_UPGRADE_LEVEL_SHOP_PRICE";
	private const string PERSISTENT_DATA_UPGRADE_LEVEL_CARD_TIER_KEY = "PERSISTENT_DATA_UPGRADE_LEVEL_CARD_TIER";

	#region Data Structs
	public struct PersistentData
	{
		private int _numGems;
		private int _upgradeLevelHealth;
		private int _upgradeLevelCoinDrop;
		private int _upgradeLevelShopPrice;
		private int _upgradeLevelCardTier;
		
		public int NumGems { get { return _numGems; } }
		public int UpgradeLevelHealth { get { return _upgradeLevelHealth; } }
		public int UpgradeLevelCoinDrop { get { return _upgradeLevelCoinDrop; } }
		public int UpgradeLevelShopPrice { get { return _upgradeLevelShopPrice; } }
		public int UpgradeLevelCardTier { get { return _upgradeLevelCardTier; } }

		public PersistentData(
			int inNumGems,
			int inUpgradeLevelHealth,
			int inUpgradeLevelCoinDrop,
			int inUpgradeLevelShopPrice,
			int inUpgradeLevelCardTier)
		{
			_numGems = inNumGems;
			_upgradeLevelHealth = inUpgradeLevelHealth;
			_upgradeLevelCoinDrop = inUpgradeLevelCoinDrop;
			_upgradeLevelShopPrice = inUpgradeLevelShopPrice;
			_upgradeLevelCardTier = inUpgradeLevelCardTier;
		}

		public void AwardGems(int inNumGemsAwarded) { _numGems += inNumGemsAwarded; }
		public bool SpendGems(int inNumGemsToSpend)
		{
			int numGemsAfterSpending = _persistentData.NumGems - inNumGemsToSpend;
			if (numGemsAfterSpending < 0)
			{
				return false;
			}
			else
			{
				_numGems = numGemsAfterSpending;
				return true;
			}
		}
		public void UpgradeHealth() { _upgradeLevelHealth++; }
		public void UpgradeCoinDrop() { _upgradeLevelCoinDrop++; }
		public void UpgradeShopPrice() { _upgradeLevelShopPrice++; }
		public void UpgradeCardTier() { _upgradeLevelCardTier++; }
	}

	public struct UpgradesData
	{
		private int _numHealthUpgradeLevels;
		private int _numCoinDropUpgradeLevels;
		private int _numShopPriceUpgradeLevels;
		private int _numCardTierUpgradeLevels;

		private int[] _healthUpgradeCosts;
		private int[] _coinDropUpgradeCosts;
		private int[] _shopPriceUpgradeCosts;
		private int[] _cardTierUpgradeCosts;

		public int NumHealthUpgradeLevels { get { return _numHealthUpgradeLevels; } }
		public int NumCoinDropUpgradeLevels { get { return _numCoinDropUpgradeLevels; } }
		public int NumShopPriceUpgradeLevels { get { return _numShopPriceUpgradeLevels; } }
		public int NumCardTierUpgradeLevels { get { return _numCardTierUpgradeLevels; } }

		public int[] HealthUpgradeCosts { get { return _healthUpgradeCosts; } }
		public int[] CoinDropUpgradeCosts { get { return _coinDropUpgradeCosts; } }
		public int[] ShopPriceUpgradeCosts { get { return _shopPriceUpgradeCosts; } }
		public int[] CardTierUpgradeCosts { get { return _cardTierUpgradeCosts; } }

		public UpgradesData(
			int inNumHealthUpgradeLevels,
			int inNumCoinDropUpgradeLevels,
			int inNumShopPriceUpgradeLevels,
			int inNumCardTierUpgradeLevels,
			int[] inHealthUpgradeCosts,
			int[] inCoinDropUpgradeCosts,
			int[] inShopPriceUpgradeCosts,
			int[] inCardTierUpgradeCosts)
		{
			_numHealthUpgradeLevels = inNumHealthUpgradeLevels;
			_numCoinDropUpgradeLevels = inNumCoinDropUpgradeLevels;
			_numShopPriceUpgradeLevels = inNumShopPriceUpgradeLevels;
			_numCardTierUpgradeLevels = inNumCardTierUpgradeLevels;

			_healthUpgradeCosts = inHealthUpgradeCosts;
			_coinDropUpgradeCosts = inCoinDropUpgradeCosts;
			_shopPriceUpgradeCosts = inShopPriceUpgradeCosts;
			_cardTierUpgradeCosts = inCardTierUpgradeCosts;
		}
	}

	public struct PrevRunData
	{
		private int _health;
		private int _shield;
		private int _numCoins;
		// TODO: Score.

		public int Health { get { return _health; } }
		public int Shield { get { return _shield; } }
		public int NumCoins { get { return _numCoins; } }

		public PrevRunData(Dungeon inDungeon)
		{
			_health = inDungeon.MorphyController.Health;
			_shield = inDungeon.MorphyController.Shield;
			_numCoins = inDungeon.NumCoins;
		}

		public PrevRunData(int inHealth, int inShield, int inNumCoins)
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

	public struct CardHandData
	{
		private CardData[] _cardDatas;
		
		public CardData[] CardDatas { get { return _cardDatas; } }

		public CardHandData(params CardData[] inCardDatas)
		{
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

			DTJob loadPersistentDataJob = new DTJob(
				(OnComplete) =>
				{
					LoadPersistentData(OnComplete);
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
					Debug.Log("ALL DATA LOADED");
					if (OnAllDataLoaded != null) OnAllDataLoaded();
				},
				loadPreviousRunDataJob,
				loadPersistentDataJob,
				loadJSONJob,
				artificialTimeDelayJob);


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
			writer.Write(inPrevRunData.NumCoins, PREV_RUN_DATA_NUMCOINS_KEY);

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
			writer.Write(inFloorData.EnemyElement, FLOOR_DATA_ENEMY_ELEMENT_KEY);

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
				prevRunData.Load<int>(PREV_RUN_DATA_SHIELD_KEY),
				prevRunData.Load<int>(PREV_RUN_DATA_NUMCOINS_KEY));

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
				floorData.LoadArray<eMoveType>(FLOOR_DATA_ENEMY_MOVE_TYPE_KEY),
				floorData.LoadArray<Enemy.eElement>(FLOOR_DATA_ENEMY_ELEMENT_KEY));

			// CARD_HAND_DATA
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

			_cardHandData = new CardHandData(cardDatas);

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

	public static void SavePersistentData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		using (ES2Writer writer = ES2Writer.Create(PERSISTENT_DATA_FILENAME))
		{
			writer.Write(_persistentData.NumGems, PERSISTENT_DATA_NUM_GEMS_KEY);
			writer.Write(_persistentData.UpgradeLevelHealth, PERSISTENT_DATA_UPGRADE_LEVEL_HEALTH_KEY);
			writer.Write(_persistentData.UpgradeLevelCoinDrop, PERSISTENT_DATA_UPGRADE_LEVEL_COIN_DROP_KEY);
			writer.Write(_persistentData.UpgradeLevelShopPrice, PERSISTENT_DATA_UPGRADE_LEVEL_SHOP_PRICE_KEY);
			writer.Write(_persistentData.UpgradeLevelCardTier, PERSISTENT_DATA_UPGRADE_LEVEL_CARD_TIER_KEY);

			writer.Save();
		}

		if (inOnComplete != null) inOnComplete();
	}

	private static void LoadPersistentData(DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (ES2.Exists(PERSISTENT_DATA_FILENAME))
		{
			ES2Data persistentData = ES2.LoadAll(PERSISTENT_DATA_FILENAME);

			// TODO: Create a TryLoad<T> that will return 0 if failed to load. Safer for future updates and all that.
			_persistentData = new PersistentData(
				persistentData.Load<int>(PERSISTENT_DATA_NUM_GEMS_KEY),
				persistentData.Load<int>(PERSISTENT_DATA_UPGRADE_LEVEL_HEALTH_KEY),
				persistentData.Load<int>(PERSISTENT_DATA_UPGRADE_LEVEL_COIN_DROP_KEY),
				persistentData.Load<int>(PERSISTENT_DATA_UPGRADE_LEVEL_SHOP_PRICE_KEY),
				persistentData.Load<int>(PERSISTENT_DATA_UPGRADE_LEVEL_CARD_TIER_KEY));
		}
		else // If no have, create empty. Basically new player.
		{
			_persistentData = new PersistentData(
				0,
				0,
				0,
				0,
				0);
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
				const string UPGRADES_KEY = "upgrades";
				const string UPGRADES_NUM_LEVELS_KEY = "num levels";
				const string UPGRADES_COST_KEY = "cost";
				const string UPGRADES_HEALTH_KEY = "health";
				const string UPGRADES_COIN_DROP_KEY = "coin drop";
				const string UPGRADES_SHOP_PRICE_KEY = "shop price";
				const string UPGRADES_CARD_TIER_KEY = "card tier";
				JSONNode gameDataRootNode = JSON.Parse(www.text);
				JSONNode upgradesNode = gameDataRootNode[UPGRADES_KEY];

				JSONNode healthUpgradeNode = upgradesNode[UPGRADES_HEALTH_KEY];
				JSONNode coinDropUpgradeNode = upgradesNode[UPGRADES_COIN_DROP_KEY];
				JSONNode shopPriceUpgradeNode = upgradesNode[UPGRADES_SHOP_PRICE_KEY];
				JSONNode cardTierUpgradeNode = upgradesNode[UPGRADES_CARD_TIER_KEY];

				JSONArray healthCostsJSONArr = healthUpgradeNode[UPGRADES_COST_KEY].AsArray;
				JSONArray coinDropCostsJSONArr = coinDropUpgradeNode[UPGRADES_COST_KEY].AsArray;
				JSONArray shopPriceCostsJSONArr = shopPriceUpgradeNode[UPGRADES_COST_KEY].AsArray;
				JSONArray cardTierCostsJSONArr = cardTierUpgradeNode[UPGRADES_COST_KEY].AsArray;
				int[] healthCosts = new int[healthCostsJSONArr.Count];
				int[] coinDropCosts = new int[coinDropCostsJSONArr.Count];
				int[] shopPriceCosts = new int[shopPriceCostsJSONArr.Count];
				int[] cardTierCosts = new int[cardTierCostsJSONArr.Count];
				for (int iCost = 0; iCost < healthCosts.Length; iCost++) healthCosts[iCost] = healthCostsJSONArr[iCost].AsInt;
				for (int iCost = 0; iCost < coinDropCosts.Length; iCost++) coinDropCosts[iCost] = coinDropCostsJSONArr[iCost].AsInt;
				for (int iCost = 0; iCost < shopPriceCosts.Length; iCost++) shopPriceCosts[iCost] = shopPriceCostsJSONArr[iCost].AsInt;
				for (int iCost = 0; iCost < cardTierCosts.Length; iCost++) cardTierCosts[iCost] = cardTierCostsJSONArr[iCost].AsInt;

				_upgradesData = new UpgradesData(
					healthUpgradeNode[UPGRADES_NUM_LEVELS_KEY].AsInt,
					coinDropUpgradeNode[UPGRADES_NUM_LEVELS_KEY].AsInt,
					shopPriceUpgradeNode[UPGRADES_NUM_LEVELS_KEY].AsInt,
					cardTierUpgradeNode[UPGRADES_NUM_LEVELS_KEY].AsInt,
					healthCosts,
					coinDropCosts,
					shopPriceCosts,
					cardTierCosts);
				
				inOnComplete();
			}
		}
	}

	// TODO: Remove this debug.
	// DEBUG
	private static IEnumerator ArtificialTimeDelay(DTJob.OnCompleteCallback inOnComplete)
	{
		float timer = 0.0f;
		while (true)
		{
			timer += Time.deltaTime;
			if (timer < 1.5f) yield return null;
			else break;
		}

		inOnComplete();
	}

	public static void AwardGems(int inNumGemsAwarded) { _persistentData.AwardGems(inNumGemsAwarded); }
	public static bool SpendGems(int inNumGemsToSpend) { return _persistentData.SpendGems(inNumGemsToSpend); }
	public static void UpgradeHealth() { _persistentData.UpgradeHealth(); }
	public static void UpgradeCoinDrop() { _persistentData.UpgradeCoinDrop(); }
	public static void UpgradeShopPrice() { _persistentData.UpgradeShopPrice(); }
	public static void UpgradeCardTier() { _persistentData.UpgradeCardTier(); }
}
