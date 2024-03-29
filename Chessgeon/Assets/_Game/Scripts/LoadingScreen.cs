﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private AudioListener _loadingScreenAudioListener = null;
    [SerializeField] private RectTransform _loadingBarFill = null;

	private const float MIN_LOADING_TIME = 1.0f;

	void Awake()
	{
		Application.targetFrameRate = 60;
		Input.multiTouchEnabled = false;

		Debug.Assert(_loadingScreenAudioListener != null, "_loadingScreenAudioListener is not assigned.");
        Debug.Assert(_loadingBarFill != null, "_loadingBarFill is not assigned.");

        UpdateLoadingBarProgress(0.0f);
	}

	void Start()
	{
		StartCoroutine(LoadDataAndMainSceneAsync());
	}

    // TODO: Make this a weighted function and add array of tasks with weights on progress.
    private void UpdateLoadingBarProgress(float inProgress)
    {
        _loadingBarFill.anchorMax = new Vector2(inProgress, 1.0f);
    }

	IEnumerator LoadDataAndMainSceneAsync()
	{
		float loadStartTime = Time.time;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.SCENE_DUNGEON, LoadSceneMode.Additive);
		asyncLoad.allowSceneActivation = false; // NOTE: Stops scene load at 0.9 and prevents it from being activated immediately when ready.

		GameData.TryLoadAllData();

		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f) // If ready to be activated.
			{
                UpdateLoadingBarProgress(asyncLoad.progress);
				_loadingScreenAudioListener.enabled = false; // NOTE: Prevent double audio listener.
				asyncLoad.allowSceneActivation = true;
			}
			yield return null;
		}

		while (!GameData.HasLoadedAllData)
		{
			yield return null;
		}

		Scene menuScene = SceneManager.GetSceneByBuildIndex(Constants.SCENE_DUNGEON);
		SceneManager.MoveGameObjectToScene(this.gameObject, menuScene);

		UpdateLoadingBarProgress(0.95f);
		float loadEndTime = Time.time;
        while (true)
        {
			if (loadEndTime - loadStartTime > MIN_LOADING_TIME)
				break;

            loadEndTime = Time.time;
			yield return null;
        }
		UpdateLoadingBarProgress(1.0f);
        // NOTE: Wait for a short while so that we can see the fully loaded bar.
        yield return new WaitForSeconds(0.1f);
		StartCoroutine(UnloadLoadingScreenSceneAsync());
	}

	IEnumerator UnloadLoadingScreenSceneAsync()
	{
		Scene loadingScreenScene = SceneManager.GetSceneByBuildIndex(Constants.SCENE_LOADINGSCREEN);
		AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(loadingScreenScene);

		while (!asyncUnload.isDone)
		{
			yield return null;
		}

		// TODO: Insert GC.Collect() force here?

		// Once unloading is done, can destroy itself.
		GameObject.Destroy(this.gameObject);
	}
}
