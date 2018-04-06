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

	[Header("Upgrades Panel")]
	[SerializeField] private GameObject _upgradesPanelObject = null;
	[SerializeField] private Button _upgradesPanelCloseBtn = null;
	[SerializeField] private Text _healthUpgradeStateText = null;
	[SerializeField] private Text _coinDropUpgradeStateText = null;
	[SerializeField] private Text _shopPriceUpgradeStateText = null;
	[SerializeField] private Text _cardTierUpgradeStateText = null;
	[SerializeField] private Text _healthUpgradeCostText = null;
	[SerializeField] private Text _coinDropUpgradeCostText = null;
	[SerializeField] private Text _shopPriceUpgradeCostText = null;
	[SerializeField] private Text _cardTierUpgradeCostText = null;
	[SerializeField] private Button _healthUpgradeBtn = null;
	[SerializeField] private Button _coinDropUpgradeBtn = null;
	[SerializeField] private Button _shopPriceUpgradeBtn = null;
	[SerializeField] private Button _cardTierUpgradeBtn = null;

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

		Debug.Assert(_upgradesPanelObject != null, "_upgradesPanelObject is not assigned.");
		Debug.Assert(_upgradesPanelCloseBtn != null, "_upgradesPanelCloseBtn is not assigned.");
		Debug.Assert(_healthUpgradeStateText != null, "_healthUpgradeStateText is not assigned.");
		Debug.Assert(_coinDropUpgradeStateText != null, "_coinDropUpgradeStateText is not assigned.");
		Debug.Assert(_shopPriceUpgradeStateText != null, "_shopPriceUpgradeStateText is not assigned.");
		Debug.Assert(_cardTierUpgradeStateText != null, "_cardTierUpgradeStateText is not assigned.");
		Debug.Assert(_healthUpgradeCostText != null, "_healthUpgradeCostText is not assigned.");
		Debug.Assert(_coinDropUpgradeCostText != null, "_coinDropUpgradeCostText is not assigned.");
		Debug.Assert(_shopPriceUpgradeCostText != null, "_shopPriceUpgradeCosttext is not assigned.");
		Debug.Assert(_cardTierUpgradeCostText != null, "_cardTierUpgradeCostText is not assigned.");
		Debug.Assert(_healthUpgradeBtn != null, "_healthUpgradeBtn is not assigned.");
		Debug.Assert(_coinDropUpgradeBtn != null, "_coinDropUpgradeBtn is not assigned.");
		Debug.Assert(_shopPriceUpgradeBtn != null, "_shopPriceUpgradeBtn is not assigned.");
		Debug.Assert(_cardTierUpgradeBtn != null, "_cardTierUpgradeBtn is not assigned.");

		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

		_continueBtn.onClick.AddListener(_menu.ContinueGame);
		_newGameBtn.onClick.AddListener(_menu.TryStartNewGame);
		_upgradesBtn.onClick.AddListener(OpenUpgradesPanel);

		_informationDismissBtn.onClick.AddListener(DismissInformationPanel);
		_confirmationCancelBtn.onClick.AddListener(DismissConfirmationPanel);
		_upgradesPanelCloseBtn.onClick.AddListener(DismissUpgradesPanel);

		_healthUpgradeBtn.onClick.AddListener(TryUpgradeHealth);
		_coinDropUpgradeBtn.onClick.AddListener(TryUpgradeCoinDrop);
		_shopPriceUpgradeBtn.onClick.AddListener(TryUpgradeShopPrice);
		_cardTierUpgradeBtn.onClick.AddListener(TryUpgradeCardTier);

		DataLoader.OnAllDataLoaded += CheckBtnAvailability;
		DataLoader.OnAllDataLoaded += UpdateGemText;
		DataLoader.OnAllDataLoaded += UpdateUpgradesPanelText;

		DismissInformationPanel();
		DismissConfirmationPanel();
		DismissUpgradesPanel();
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
		_gemText.text = ChessgeonUtils.FormatGemString(DataLoader.SavedPersistentData.NumGems);
	}

	private void UpdateUpgradesPanelText()
	{
		DataLoader.PersistentData persistentData = DataLoader.SavedPersistentData;
		DataLoader.UpgradesData upgradesData = DataLoader.LoadedUpgradesData;

		_healthUpgradeStateText.text = persistentData.UpgradeLevelHealth + "/" + upgradesData.NumHealthUpgradeLevels;
		_coinDropUpgradeStateText.text = persistentData.UpgradeLevelCoinDrop + "/" + upgradesData.NumCoinDropUpgradeLevels;
		_shopPriceUpgradeStateText.text = persistentData.UpgradeLevelShopPrice + "/" + upgradesData.NumShopPriceUpgradeLevels;
		_cardTierUpgradeStateText.text = persistentData.UpgradeLevelCardTier + "/" + upgradesData.NumCardTierUpgradeLevels;

		if (persistentData.UpgradeLevelHealth == upgradesData.NumHealthUpgradeLevels) _healthUpgradeCostText.text = "MAX";
		else _healthUpgradeCostText.text = ChessgeonUtils.FormatGemString(upgradesData.HealthUpgradeCosts[persistentData.UpgradeLevelHealth]);

		if (persistentData.UpgradeLevelCoinDrop == upgradesData.NumCoinDropUpgradeLevels) _coinDropUpgradeCostText.text = "MAX";
		else _coinDropUpgradeCostText.text = ChessgeonUtils.FormatGemString(upgradesData.CoinDropUpgradeCosts[persistentData.UpgradeLevelCoinDrop]);

		if (persistentData.UpgradeLevelShopPrice == upgradesData.NumShopPriceUpgradeLevels) _shopPriceUpgradeCostText.text = "MAX";
		else _shopPriceUpgradeCostText.text = ChessgeonUtils.FormatGemString(upgradesData.ShopPriceUpgradeCosts[persistentData.UpgradeLevelShopPrice]);

		if (persistentData.UpgradeLevelCardTier == upgradesData.NumCardTierUpgradeLevels) _cardTierUpgradeCostText.text = "MAX";
		else _cardTierUpgradeCostText.text = ChessgeonUtils.FormatGemString(upgradesData.CardTierUpgradeCosts[persistentData.UpgradeLevelCardTier]);
	}

	private void OpenUpgradesPanel()
	{
		UpdateUpgradesPanelText();
		_upgradesPanelObject.SetActive(true);
	}

	private void DismissInformationPanel()
	{
		_informationPanelObject.SetActive(false);
	}

	private void DismissConfirmationPanel()
	{
		_confirmationPanelObject.SetActive(false);
	}

	private void DismissUpgradesPanel()
	{
		_upgradesPanelObject.SetActive(false);
	}

	#region UpgradeFuncs
	private void TryUpgradeHealth()
	{
		DataLoader.PersistentData persistentData = DataLoader.SavedPersistentData;
		DataLoader.UpgradesData upgradesData = DataLoader.LoadedUpgradesData;
		if (persistentData.UpgradeLevelHealth == upgradesData.NumHealthUpgradeLevels)
		{
			// NOTE: Do nothing cause already max!
		}
		else
		{
			int cost = upgradesData.HealthUpgradeCosts[persistentData.UpgradeLevelHealth];
			if (persistentData.NumGems < cost) InformNotEnoughGemsForUpgrade();
			else
			{
				PromptConfirmation(
					"BUY UPGRADE",
					"Spend " + ChessgeonUtils.FormatGemString(cost) + " GEMs to upgrade max health?",
					() =>
					{
						if (DataLoader.SpendGems(cost))
						{
							DataLoader.UpgradeHealth();
							SaveUpgrade();
						}
						else
						{
							Debug.LogError("NOT ENOUGH GEMS TO SPEND!");
						}
					});
			}
		}
	}

	private void TryUpgradeCoinDrop()
	{
		// TODO: Implement this.
	}

	private void TryUpgradeShopPrice()
	{
		// TODO: Implement this.
	}

	private void TryUpgradeCardTier()
	{
		// TODO: Implement this.
	}

	private void InformNotEnoughGemsForUpgrade()
	{
		PopupInformation("OOPS...", "Not enough GEMs to purchase upgrade!");

		// TODO: REMOVE THIS DEBUG
		DataLoader.AwardGems(Random.Range(50, 150));
		UpdateGemText();
		DataLoader.SavePersistentData();
	}

	private void SaveUpgrade()
	{
		DataLoader.SavePersistentData();
		UpdateGemText();
		UpdateUpgradesPanelText();
	}
	#endregion
}
