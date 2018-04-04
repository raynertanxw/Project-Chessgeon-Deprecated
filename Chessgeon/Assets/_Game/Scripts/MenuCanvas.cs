using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
	[SerializeField] private Menu _menu = null;
	[SerializeField] private Camera _menuUICamera = null;
	[SerializeField] private GameObject _continueBtnObject = null;
	[SerializeField] private GameObject _upgradesBtnObject = null;
	[SerializeField] private Button _continueBtn = null;
	[SerializeField] private Button _newGameBtn = null;
	[SerializeField] private Button _upgradesBtn = null;

	private bool _isVisible = true;

	void Awake()
	{
		Debug.Assert(_menu != null, "_menu is not assigned.");
		Debug.Assert(_menuUICamera != null, "_menuUICamera is not assigned.");
		Debug.Assert(_continueBtnObject != null, "_continueBtnObject is not assigned.");
		Debug.Assert(_upgradesBtnObject != null, "_upgradesBtnObject is not assigned.");
		Debug.Assert(_continueBtn != null, "_continueBtn is not assigned.");
		Debug.Assert(_newGameBtn != null, "_newGameBtn is not assigned.");
		Debug.Assert(_upgradesBtn != null, "_upgradesBtn is not assigned.");

		_continueBtn.onClick.AddListener(_menu.ContinueGame);
		_newGameBtn.onClick.AddListener(_menu.TryStartNewGame);
		_upgradesBtn.onClick.AddListener(_menu.UpgradesMenu);

		SaveDataLoader.OnAllDataLoaded += CheckBtnAvailability;
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		gameObject.SetActive(_isVisible);
		_menuUICamera.enabled = _isVisible;
	}

	private void CheckBtnAvailability()
	{
		_continueBtnObject.SetActive(SaveDataLoader.HasPreviousRunData);
	}
}
