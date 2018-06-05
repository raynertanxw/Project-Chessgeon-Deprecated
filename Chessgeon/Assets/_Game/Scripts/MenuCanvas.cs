using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class MenuCanvas : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;

	[Header("Main Menu UI")]
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
	[SerializeField] private Canvas _confirmationPanelCanvas = null;
	[SerializeField] private Text _confirmationTitleText = null;
	[SerializeField] private Text _confirmationInfoText = null;
	[SerializeField] private Text _confirmationConfirmBtnText = null;
	[SerializeField] private Text _confirmationCancelBtnText = null;
	[SerializeField] private Button _confirmationConfirmBtn = null;
	[SerializeField] private Button _confirmationCancelBtn = null;

	private bool _isVisible = true;

	void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

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

		Debug.Assert(_confirmationPanelCanvas != null, "_confirmationPanelCanvas is not assigned.");
		Debug.Assert(_confirmationTitleText != null, "_confirmationTitleText is not assigned.");
		Debug.Assert(_confirmationInfoText != null, "_confirmationInfoText is not assigned.");
		Debug.Assert(_confirmationConfirmBtnText != null, "_confirmationConfirmBtnText is not assigned.");
		Debug.Assert(_confirmationCancelBtnText != null, "_confirmationCancelBtnText is not assigned.");
		Debug.Assert(_confirmationConfirmBtn != null, "_confirmationConfirmBtn is not assigned.");
		Debug.Assert(_confirmationCancelBtn != null, "_confirmationCancelBtn is not assigned.");

#if UNITY_EDITOR
		if (!GameData.HasStartedLoadingData) GameData.TryLoadAllData();
#endif

		_continueBtn.onClick.AddListener(ContinueGame);
		_newGameBtn.onClick.AddListener(TryStartNewGame);

		_informationDismissBtn.onClick.AddListener(DismissInformationPanel);
		_confirmationCancelBtn.onClick.AddListener(DismissConfirmationPanel);

		GameData.OnAllDataLoaded += CheckBtnAvailability;
		GameData.OnAllDataLoaded += UpdateGemText;

		DismissInformationPanel();
		DismissConfirmationPanel();
	}

	private void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		gameObject.SetActive(_isVisible);
		gameObject.SetActive(_isVisible);
	}

	public void PopupInformation(string inTitleText, string inInfoText)
	{
		_informationTitleText.text = inTitleText;
		_informationInfoText.text = inInfoText;

		_informationPanelObject.SetActive(true);
	}

	private void PromptConfirmation(string inTitleText, string inInfoText, Utils.GenericVoidDelegate inOnConfirm, string confirmBtnText = "CONFIRM", string cancelBtnText = "CANCEL")
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

		_confirmationPanelCanvas.enabled = true;
	}

	public void CheckBtnAvailability()
	{
		_continueBtn.gameObject.SetActive(GameData.HasPreviousRunData);
	}

	public void UpdateGemText()
	{
		_gemText.text = ChessgeonUtils.FormatGemString(GameData.SavedPlayerData.NumGems);
	}

	private void DismissInformationPanel()
	{
		_informationPanelObject.SetActive(false);
	}

	private void DismissConfirmationPanel()
	{
		_confirmationPanelCanvas.enabled = false;
	}

	private void TryStartNewGame()
	{
		if (GameData.HasPreviousRunData)
		{
			PromptConfirmation("NEW GAME?",
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
		GameData.DeletePreviousRunData();
		_dungeon.ResetAndStartGame();
		SetVisible(false);
	}

	private void ContinueGame()
	{
		_dungeon.StartGameFromPrevRun(GameData.PrevRunData);
		SetVisible(false);
	}

	public void ReturnToMainMenu()
	{
		CheckBtnAvailability();
		UpdateGemText();
		SetVisible(true);
	}
}
