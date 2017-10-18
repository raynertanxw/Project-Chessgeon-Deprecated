﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private AudioListener _loadingScreenAudioListener = null;
	[SerializeField] private Transform _loadingIndicator = null;

	void Awake()
	{
		Debug.Assert(_loadingScreenAudioListener != null, "_loadingScreenAudioListener in LoadingScreen.cs was not assigned.");
		Debug.Assert(_loadingIndicator != null, "_loadingIndicator in LoadingScreen.cs was not assigned.");
	}

	void Start()
	{
		StartCoroutine(LoadMenuSceneAsync());
	}

	void Update()
	{
		// TODO: Just a temporary placeholder for loading indication.
		_loadingIndicator.Rotate(Vector3.forward, -500.0f * Time.deltaTime);
	}

	IEnumerator LoadMenuSceneAsync()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.SCENE_DUNGEON, LoadSceneMode.Additive);
		asyncLoad.allowSceneActivation = false; // Stops scene load at 0.9 and prevents it from being activated immediately when ready.

		yield return new WaitForSeconds(5.0f); // TODO: Artificial wait loading time.
		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f) // If ready to be activated.
			{
				_loadingScreenAudioListener.enabled = false;
				asyncLoad.allowSceneActivation = true;
			}
			yield return null;
		}

		Scene menuScene = SceneManager.GetSceneByBuildIndex(Constants.SCENE_DUNGEON);
		SceneManager.MoveGameObjectToScene(this.gameObject, menuScene);
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

		// Once unloading is done, can destroy itself.
		GameObject.Destroy(this.gameObject);
	}
}
