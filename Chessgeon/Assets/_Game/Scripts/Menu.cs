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
		if (!GameDataLoader.HasStartedLoadingData) GameDataLoader.TryLoadGameData();
#endif
	}

	private void StartGame()
	{
		_dungeon.ResetAndStartGame();
		_menuCanvas.SetVisible(false);
	}

	public void StartNewGame()
	{
		// TODO: Create new set of data.
		StartGame();
	}

	public void ContinueGame()
	{
		// TODO: Load back old set of data.
		StartGame();
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
