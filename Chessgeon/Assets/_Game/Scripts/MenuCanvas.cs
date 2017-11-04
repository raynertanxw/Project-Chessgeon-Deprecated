using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
	[SerializeField] private Menu _menu = null;
	[SerializeField] private Button _startButton = null;

	private bool _isVisible = true;

	void Awake()
	{
		Debug.Assert(_menu != null, "_menu is not assigned.");
		Debug.Assert(_startButton != null, "_startButton is not assigned.");
		_startButton.onClick.AddListener(_menu.StartGame);
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		_startButton.gameObject.SetActive(_isVisible);
	}
}
