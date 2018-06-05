using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class GameOverCanvas : MonoBehaviour 
{
	private static GameOverCanvas _instance = null;

	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private MenuCanvas _menu = null;

	[Header("UI Components")]
	[SerializeField] private Canvas _gameOverCanvas = null;
	[SerializeField] private Button _startOverBtn = null;
	[SerializeField] private Button _exitBtn = null;
	[SerializeField] private Text _scoreText = null;
	[SerializeField] private Text _floorText = null;

	void Awake()
	{
        if (_instance == null)
        {
			_instance = this;

            Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() != null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			Debug.Assert(_menu != null, "_menu is not assigned.");

			Debug.Assert(_gameOverCanvas != null, "_gameOverCanvas is not assigned.");
            Debug.Assert(_startOverBtn != null, "_startOverBtn is not assigned.");
            Debug.Assert(_exitBtn != null, "_exitBtn is not assigned.");
			Debug.Assert(_scoreText != null, "_scoreText is not assigned.");
			Debug.Assert(_floorText != null, "_floorText is not assigned.");

			EnableGameOverPanel(false);

			_startOverBtn.onClick.AddListener(StartOver);
			_exitBtn.onClick.AddListener(ExitToMainMenu);
        }
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
    }

	void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void EnableGameOverPanel(bool inEnabled)
	{
		_instance._gameOverCanvas.enabled = inEnabled;
	}

	public static void SetGameOverValues(int inScore, int inFloor)
	{
		// TODO: Format the int to txts!
		_instance._scoreText.text = "SCORE: " + String.Format("{0:n0}", inScore);
		_instance._floorText.text = "FLOOR: " + inFloor.ToString();
	}

	private void ExitToMainMenu()
	{
		EnableGameOverPanel(false);
		_menu.ReturnToMainMenu();
	}

	private void StartOver()
	{
		EnableGameOverPanel(false);
		_dungeon.ResetAndStartGame();
	}
}
