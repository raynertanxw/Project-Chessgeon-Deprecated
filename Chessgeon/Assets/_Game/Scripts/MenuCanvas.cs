using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class MenuCanvas : MonoBehaviour
{
	[SerializeField] private Menu _menu = null;
	[SerializeField] private Camera _menuUICamera = null;

	[Header("Main Menu UI")]
	[SerializeField] private GameObject _continueBtnObject = null;
	[SerializeField] private GameObject _upgradesBtnObject = null;
	[SerializeField] private Button _continueBtn = null;
	[SerializeField] private Button _newGameBtn = null;
	[SerializeField] private Button _upgradesBtn = null;

	[Header("Confirmation Panel")]
	[SerializeField] private GameObject _confirmationPanelObject = null;
	[SerializeField] private Text _confirmationTitleText = null;
	[SerializeField] private Text _confirmationInfoText = null;
	[SerializeField] private Text _confirmationConfirmBtnText = null;
	[SerializeField] private Text _confirmationCancelBtnText = null;
	[SerializeField] private Button _confirmationConfirmBtn = null;
	[SerializeField] private Button _confirmationCancelBtn = null;


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

		Debug.Assert(_confirmationPanelObject != null, "_confirmationPanelObject is not assigned.");
		Debug.Assert(_confirmationTitleText != null, "_confirmationTitleText is not assigned.");
		Debug.Assert(_confirmationInfoText != null, "_confirmationInfoText is not assigned.");
		Debug.Assert(_confirmationConfirmBtnText != null, "_confirmationConfirmBtnText is not assigned.");
		Debug.Assert(_confirmationCancelBtnText != null, "_confirmationCancelBtnText is not assigned.");
		Debug.Assert(_confirmationConfirmBtn != null, "_confirmationConfirmBtn is not assigned.");
		Debug.Assert(_confirmationCancelBtn != null, "_confirmationCancelBtn is not assigned.");

		_continueBtn.onClick.AddListener(_menu.ContinueGame);
		_newGameBtn.onClick.AddListener(_menu.TryStartNewGame);
		_upgradesBtn.onClick.AddListener(_menu.UpgradesMenu);

		SaveDataLoader.OnAllDataLoaded += CheckBtnAvailability;

		DismissConfirmationPanel();
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		gameObject.SetActive(_isVisible);
		_menuUICamera.enabled = _isVisible;
	}

	public void PromptConfirmation(string inTitleText, string inInfoText, Utils.GenericVoidDelegate inOnConfirm, string confirmBtnText = "CONFIRM", string cancelBtnText = "CANCEL")
	{
		_confirmationTitleText.text = inTitleText;
		_confirmationInfoText.text = inInfoText;
		_confirmationConfirmBtnText.text = confirmBtnText;
		_confirmationCancelBtnText.text = cancelBtnText;

		_confirmationConfirmBtn.onClick.RemoveAllListeners();
		_confirmationConfirmBtn.onClick.AddListener(() =>
		{
			inOnConfirm();
			DismissConfirmationPanel();
		});

		_confirmationCancelBtn.onClick.RemoveAllListeners();
		_confirmationCancelBtn.onClick.AddListener(() => { DismissConfirmationPanel(); });

		_confirmationPanelObject.SetActive(true);
	}

	private void CheckBtnAvailability()
	{
		_continueBtnObject.SetActive(SaveDataLoader.HasPreviousRunData);
	}

	private void DismissConfirmationPanel()
	{
		_confirmationPanelObject.SetActive(false);
	}
}
