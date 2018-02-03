using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour 
{
	private static GameOverCanvas _instance = null;

	[Header("UI Components")]
	[SerializeField] private Button _startOverBtn = null;
	[SerializeField] private Button _exitBtn = null;
	[SerializeField] private Text _startOverBtnText = null;
	[SerializeField] private Text _exitBtnText = null;
	[SerializeField] private Text _gameOverText = null;
	[SerializeField] private Text _scoreText = null;
	[SerializeField] private Text _floorText = null;
	[SerializeField] private Text _goldText = null;

	[Header("Meshe Renderers")]
	[SerializeField] private MeshRenderer _gameOverPanelMeshRen = null;
	[SerializeField] private MeshRenderer _startOverBtnMeshRen = null;
	[SerializeField] private MeshRenderer _exitBtnMeshRen = null;

	void Awake()
	{
        if (_instance == null)
        {
			_instance = this;

            Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() != null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");
            Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");

            Debug.Assert(_startOverBtn != null, "_startOverBtn is not assigned.");
            Debug.Assert(_exitBtn != null, "_exitBtn is not assigned.");
			Debug.Assert(_startOverBtnText != null, "_startOverBtnText is not assigned.");
			Debug.Assert(_exitBtnText != null, "_exitBtnText is not assigned.");
			Debug.Assert(_gameOverText != null, "_gameOverText is not assigned.");
			Debug.Assert(_scoreText != null, "_scoreText is not assigned.");
			Debug.Assert(_floorText != null, "_floorText is not assigned.");
			Debug.Assert(_goldText != null, "_goldText is not assigned.");

			Debug.Assert(_gameOverPanelMeshRen != null, "_gameOverPanelMeshRen is not assigned.");
			Debug.Assert(_startOverBtnMeshRen != null, "_startOverBtnMeshRen is not assigned.");
			Debug.Assert(_exitBtnMeshRen != null, "_exitBtnMeshRen is not assigned.");

			EnableGameOverPanel(false);
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
		_instance._startOverBtn.interactable = inEnabled;
		_instance._exitBtn.interactable = inEnabled;
		_instance._startOverBtnText.enabled = inEnabled;
		_instance._exitBtnText.enabled = inEnabled;

		_instance._gameOverText.enabled = inEnabled;
		_instance._scoreText.enabled = inEnabled;
		_instance._floorText.enabled = inEnabled;
		_instance._goldText.enabled = inEnabled;

		_instance._gameOverPanelMeshRen.enabled = inEnabled;
		_instance._startOverBtnMeshRen.enabled = inEnabled;
		_instance._exitBtnMeshRen.enabled = inEnabled;
	}

	public static void SetGameOverValues(int inScore, int inFloor, int inGold)
	{
		// TODO: Format the int to txts!
		_instance._scoreText.text = Convert.ToString(inScore);
		_instance._floorText.text = Convert.ToString(inFloor);
		_instance._goldText.text = Convert.ToString(inGold);
	}
}
