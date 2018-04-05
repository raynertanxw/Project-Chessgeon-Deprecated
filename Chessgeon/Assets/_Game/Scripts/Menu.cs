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
		if (!DataLoader.HasStartedLoadingData) DataLoader.TryLoadData();
#endif
	}


	public void TryStartNewGame()
	{
		if (DataLoader.HasPreviousRunData)
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
		DataLoader.DeletePreviousRunData();
		_dungeon.ResetAndStartGame();
		_menuCanvas.SetVisible(false);
	}

	public void ContinueGame()
	{
		_dungeon.StartGameFromPrevRun(DataLoader.SavedPrevRunData, DataLoader.SavedFloorData, DataLoader.SavedCardHandData);
		_menuCanvas.SetVisible(false);
	}

	public void ReturnToMainMenu()
	{
		_menuCanvas.SetVisible(true);
	}
}
