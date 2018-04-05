using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private MenuCanvas _menuCanvas = null;

	private void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
		Debug.Assert(_menuCanvas != null, "_menuCanvas is not assigned.");

#if UNITY_EDITOR
		if (!SaveDataLoader.HasStartedLoadingData) SaveDataLoader.TryLoadAllSaveData();
#endif
	}


	public void TryStartNewGame()
	{
		if (SaveDataLoader.HasPreviousRunData)
		{
			_menuCanvas.PromptConfirmation("NEW GAME?",
				"Starting a new game will result in loss of current progress.\n\nAre you sure ?",
				StartNewGame);
		}
		else
		{
			StartNewGame();
		}
	}

	private void StartNewGame()
	{
		SaveDataLoader.DeletePreviousRunData();
		_dungeon.ResetAndStartGame();
		_menuCanvas.SetVisible(false);
	}

	public void ContinueGame()
	{
		_dungeon.StartGameFromSavedData(SaveDataLoader.SavedGameData, SaveDataLoader.SavedFloorData, SaveDataLoader.SavedCardHandData);
		_menuCanvas.SetVisible(false);
	}

	public void UpgradesMenu()
	{
		// TODO: Implement this.
		Debug.LogWarning("NOT YET IMPLEMENTED.");
	}

	public void ReturnToMainMenu()
	{
		_menuCanvas.SetVisible(true);
	}
}
