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

	// Game Data
	private static int _health = -1;
	public static int Health { get { return _health; } }
	private static int _shield = -1;
	public static int Shield { get { return _shield; } }
	private static int _floorNum = -1;
	public static int FloorNum { get { return _floorNum; } }

	private const string GAMEDATA_FILENAME = "gamedata.txt";
	private const string FLOORDATA_FILENAME = "floordata.txt";
	private const string CARDDATA_FILENAME = "carddata.txt";
	// Keys
	private const string HEALTH_KEY = "HEALTH";
	private const string SHIELD_KEY = "SHIELD";
	private const string FLOOR_NUM_KEY = "FLOOR_NUM";

	public struct GameData
	{
		public int health;
		public int shield;
		public int floorNum;

		public GameData(Dungeon inDungeon)
		{
			health = inDungeon.MorphyController.Health;
			shield = inDungeon.MorphyController.Shield;
			floorNum = inDungeon.FloorNum;
		}
	}

	public static void TryLoadGameData()
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

	public static void SaveData(GameData inGameData)
	{
		using (ES2Writer writer = ES2Writer.Create(GAMEDATA_FILENAME))
		{
			writer.Write(inGameData.health, HEALTH_KEY);
			writer.Write(inGameData.shield, SHIELD_KEY);
			writer.Write(inGameData.floorNum, FLOOR_NUM_KEY);

			writer.Save();
		}
	}

	private static void LoadSavedData(DTJob.OnCompleteCallback inOnComplete)
	{
		if (ES2.Exists(GAMEDATA_FILENAME))
		{
			ES2Data gameData = ES2.LoadAll(GAMEDATA_FILENAME);
			if (gameData.TagExists(HEALTH_KEY)) _health = gameData.Load<int>(HEALTH_KEY);
			if (gameData.TagExists(SHIELD_KEY)) _shield = gameData.Load<int>(SHIELD_KEY);
			if (gameData.TagExists(FLOOR_NUM_KEY)) _floorNum = gameData.Load<int>(FLOOR_NUM_KEY);
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
