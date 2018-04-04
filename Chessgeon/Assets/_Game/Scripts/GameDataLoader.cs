using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class GameDataLoader
{
	private static bool _hasStartedLoadingData = false;
	public static bool HasStartedLoadingData { get { return _hasStartedLoadingData; } }
	private static bool _hasLoadedAllData = false;
	public static bool HasLoadedAllData { get { return _hasLoadedAllData; } }

	public static void TryLoadGameData()
	{
		if (!HasLoadedAllData && !HasStartedLoadingData)
		{
			Debug.Log("Started TryLoadGameData");
			_hasStartedLoadingData = true;

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
				OnFinishLoadingData,
				loadJSONJob,
				artificialTimeDelayJob);


			loadDataJobList.ExecuteAllJobs();
		}
		else
		{
			Debug.LogWarning("Data has already been loaded.");
		}
	}

	private static void OnFinishLoadingData()
	{
		_hasLoadedAllData = true;
		Debug.Log("ALL GAME DATA LOADED");
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
