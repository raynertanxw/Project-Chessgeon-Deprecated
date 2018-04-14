using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private AudioListener _loadingScreenAudioListener = null;
    // TODO: Proper loading bar

	void Awake()
	{
		Debug.Assert(_loadingScreenAudioListener != null, "_loadingScreenAudioListener in LoadingScreen.cs was not assigned.");
	}

	void Start()
	{
		StartCoroutine(LoadDataAndMainSceneAsync());
	}

	IEnumerator LoadDataAndMainSceneAsync()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.SCENE_DUNGEON, LoadSceneMode.Additive);
		asyncLoad.allowSceneActivation = false; // NOTE: Stops scene load at 0.9 and prevents it from being activated immediately when ready.

		DataLoader.TryLoadData();

		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f) // If ready to be activated.
			{
				_loadingScreenAudioListener.enabled = false; // NOTE: Prevent double audio listener.
				asyncLoad.allowSceneActivation = true;
			}
			yield return null;
		}

		while (!DataLoader.HasLoadedAllData)
		{
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

		// TODO: Insert GC.Collect() force here?

		// Once unloading is done, can destroy itself.
		GameObject.Destroy(this.gameObject);
	}
}
