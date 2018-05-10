using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UpgradeData
{
    private int _numHealthUpgradeLevels;
    private int _numCoinDropUpgradeLevels;
    private int _numShopPriceUpgradeLevels;
    private int _numCardTierUpgradeLevels;

    private int[] _healthUpgradeCosts;
    private int[] _coinDropUpgradeCosts;
    private int[] _shopPriceUpgradeCosts;
    private int[] _cardTierUpgradeCosts;

    public int NumHealthUpgradeLevels { get { return _numHealthUpgradeLevels; } }
    public int NumCoinDropUpgradeLevels { get { return _numCoinDropUpgradeLevels; } }
    public int NumShopPriceUpgradeLevels { get { return _numShopPriceUpgradeLevels; } }
    public int NumCardTierUpgradeLevels { get { return _numCardTierUpgradeLevels; } }

    public int[] HealthUpgradeCosts { get { return _healthUpgradeCosts; } }
    public int[] CoinDropUpgradeCosts { get { return _coinDropUpgradeCosts; } }
    public int[] ShopPriceUpgradeCosts { get { return _shopPriceUpgradeCosts; } }
    public int[] CardTierUpgradeCosts { get { return _cardTierUpgradeCosts; } }

    public UpgradeData(
        int inNumHealthUpgradeLevels,
        int inNumCoinDropUpgradeLevels,
        int inNumShopPriceUpgradeLevels,
        int inNumCardTierUpgradeLevels,
        int[] inHealthUpgradeCosts,
        int[] inCoinDropUpgradeCosts,
        int[] inShopPriceUpgradeCosts,
        int[] inCardTierUpgradeCosts)
    {
        _numHealthUpgradeLevels = inNumHealthUpgradeLevels;
        _numCoinDropUpgradeLevels = inNumCoinDropUpgradeLevels;
        _numShopPriceUpgradeLevels = inNumShopPriceUpgradeLevels;
        _numCardTierUpgradeLevels = inNumCardTierUpgradeLevels;

        _healthUpgradeCosts = inHealthUpgradeCosts;
        _coinDropUpgradeCosts = inCoinDropUpgradeCosts;
        _shopPriceUpgradeCosts = inShopPriceUpgradeCosts;
        _cardTierUpgradeCosts = inCardTierUpgradeCosts;
    }
}
