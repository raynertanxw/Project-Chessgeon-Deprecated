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
	[SerializeField] private Button _continueBtn = null;
	[SerializeField] private Button _newGameBtn = null;
	[SerializeField] private Button _leaderboardBtn = null;
	[SerializeField] private Button _achievementBtn = null;
	[SerializeField] private Button _settingsBtn = null;
	[SerializeField] private Button _infoBtn = null;
	[SerializeField] private Text _gemText = null;

	[Header("Information Panel")]
	[SerializeField] private GameObject _informationPanelObject = null;
	[SerializeField] private Text _informationTitleText = null;
	[SerializeField] private Text _informationInfoText = null;
	[SerializeField] private Button _informationDismissBtn = null;

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
		Debug.Assert(_continueBtn != null, "_continueBtn is not assigned.");
		Debug.Assert(_newGameBtn != null, "_newGameBtn is not assigned.");
		Debug.Assert(_leaderboardBtn != null, "_leaderboardBtn is not assigned.");
		Debug.Assert(_achievementBtn != null, "_achievementBtn is not assigned.");
		Debug.Assert(_settingsBtn != null, "_settingsBtn is not assigned.");
		Debug.Assert(_infoBtn != null, "_infoBtn is not assigned.");
		Debug.Assert(_gemText != null, "_gemText is not assigned.");

		Debug.Assert(_informationPanelObject != null, "_informationPanelObject is not assigned.");
		Debug.Assert(_informationTitleText != null, "_informationTitleText is not assigned.");
		Debug.Assert(_informationInfoText != null, "_informationInfoText is not assigned.");
		Debug.Assert(_informationDismissBtn != null, "_informationDismissBtn is not assigned.");

		Debug.Assert(_confirmationPanelObject != null, "_confirmationPanelObject is not assigned.");
		Debug.Assert(_confirmationTitleText != null, "_confirmationTitleText is not assigned.");
		Debug.Assert(_confirmationInfoText != null, "_confirmationInfoText is not assigned.");
		Debug.Assert(_confirmationConfirmBtnText != null, "_confirmationConfirmBtnText is not assigned.");
		Debug.Assert(_confirmationCancelBtnText != null, "_confirmationCancelBtnText is not assigned.");
		Debug.Assert(_confirmationConfirmBtn != null, "_confirmationConfirmBtn is not assigned.");
		Debug.Assert(_confirmationCancelBtn != null, "_confirmationCancelBtn is not assigned.");

		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Constants.DESIGN_WIDTH, Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH));
		_continueBtn.onClick.AddListener(_menu.ContinueGame);
		_newGameBtn.onClick.AddListener(_menu.TryStartNewGame);

		_informationDismissBtn.onClick.AddListener(DismissInformationPanel);
		_confirmationCancelBtn.onClick.AddListener(DismissConfirmationPanel);

		DataLoader.OnAllDataLoaded += CheckBtnAvailability;
		DataLoader.OnAllDataLoaded += UpdateGemText;

		DismissInformationPanel();
		DismissConfirmationPanel();
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		gameObject.SetActive(_isVisible);
		_menuUICamera.enabled = _isVisible;
	}

	public void PopupInformation(string inTitleText, string inInfoText)
	{
		_informationTitleText.text = inTitleText;
		_informationInfoText.text = inInfoText;

		_informationPanelObject.SetActive(true);
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

		_confirmationPanelObject.SetActive(true);
	}

	public void CheckBtnAvailability()
	{
		_continueBtnObject.SetActive(DataLoader.HasPreviousRunData);
	}

	public void UpdateGemText()
	{
		_gemText.text = ChessgeonUtils.FormatGemString(DataLoader.SavedPlayerData.NumGems);
	}

	private void DismissInformationPanel()
	{
		_informationPanelObject.SetActive(false);
	}

	private void DismissConfirmationPanel()
	{
		_confirmationPanelObject.SetActive(false);
	}
}
