using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    private int _numGems;
    private int _upgradeLevelHealth;
    private int _upgradeLevelCoinDrop;
    private int _upgradeLevelShopPrice;
    private int _upgradeLevelCardTier;

    public int NumGems { get { return _numGems; } }
    public int UpgradeLevelHealth { get { return _upgradeLevelHealth; } }
    public int UpgradeLevelCoinDrop { get { return _upgradeLevelCoinDrop; } }
    public int UpgradeLevelShopPrice { get { return _upgradeLevelShopPrice; } }
    public int UpgradeLevelCardTier { get { return _upgradeLevelCardTier; } }

    public PlayerData(
        int inNumGems,
        int inUpgradeLevelHealth,
        int inUpgradeLevelCoinDrop,
        int inUpgradeLevelShopPrice,
        int inUpgradeLevelCardTier)
    {
        _numGems = inNumGems;
        _upgradeLevelHealth = inUpgradeLevelHealth;
        _upgradeLevelCoinDrop = inUpgradeLevelCoinDrop;
        _upgradeLevelShopPrice = inUpgradeLevelShopPrice;
        _upgradeLevelCardTier = inUpgradeLevelCardTier;
    }

    public void AwardGems(int inNumGemsAwarded) { _numGems += inNumGemsAwarded; }
    public bool SpendGems(int inNumGemsToSpend)
    {
        int numGemsAfterSpending = DataLoader.SavedPlayerData.NumGems - inNumGemsToSpend;
        if (numGemsAfterSpending < 0)
        {
            return false;
        }
        else
        {
            _numGems = numGemsAfterSpending;
            return true;
        }
    }
    public void UpgradeHealth() { _upgradeLevelHealth++; }
    public void UpgradeCoinDrop() { _upgradeLevelCoinDrop++; }
    public void UpgradeShopPrice() { _upgradeLevelShopPrice++; }
    public void UpgradeCardTier() { _upgradeLevelCardTier++; }
}
