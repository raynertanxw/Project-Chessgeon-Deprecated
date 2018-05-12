using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
	public const string FILENAME = "PlayerData";
	public const string NUM_GEMS_KEY = "NUM_GEMS";

    private int _numGems;

    public int NumGems { get { return _numGems; } }

    public PlayerData(int inNumGems)
    {
        _numGems = inNumGems;
    }

    public void AwardGems(int inNumGemsAwarded) { _numGems += inNumGemsAwarded; }
    public bool SpendGems(int inNumGemsToSpend)
    {
        int numGemsAfterSpending = NumGems - inNumGemsToSpend;
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
}
