using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public static class RandomExt
	{
		public static bool WeightedRandomBoolean(int inTrueWeight, int inFalseWeight)
		{
			Debug.Assert(inTrueWeight > 0 && inFalseWeight > 0, "Either inTrueWeight or inFalseWeight is < 1");
			int weightSums = inTrueWeight + inFalseWeight;

			int randVal = Random.Range(0, weightSums);
			if (randVal < inTrueWeight) return true;
			else return false;
		}

		public class RandomPool
		{
			private struct Entry
			{
				public int Value { get; private set; }
				public int ProbabilityWeight { get; private set; }
				
				public Entry (int inValue, int inProbabilityWeight)
				{
					Value = inValue;
					ProbabilityWeight = inProbabilityWeight;
				}
			}
			
			private List<Entry> _entryList = null;
			private int _sumOfWeights = 0;
            
			public RandomPool()
			{
				_entryList = new List<Entry>();
			}

			public void AddEntry(int inValue, int inProbabilityWeight)
			{
				for (int iEntry = 0; iEntry < _entryList.Count; iEntry++)
				{
					Debug.Assert(_entryList[iEntry].Value != inValue, "There is already an entry in RandomPool of value: " + inValue);
				}

				_entryList.Add(new Entry(inValue, inProbabilityWeight));
				_sumOfWeights += inProbabilityWeight;
			}

			public void ChangeWeight(int inValue, int inNewProbabilityWeight)
			{
				int index = -1;
				for (int iEntry = 0; iEntry < _entryList.Count; iEntry++)
				{
					if (_entryList[iEntry].Value == inValue)
					{
						index = iEntry;
						break;
					}
				}

				Debug.Assert(index > -1, "Could not find entry in RandomPool of value: " + inValue);

				int originalProbabilityWeight = _entryList[index].ProbabilityWeight;
				_sumOfWeights -= originalProbabilityWeight;
				_entryList[index] = new Entry(inValue, inNewProbabilityWeight);
                _sumOfWeights += inNewProbabilityWeight;
			}

			public int GetRandomEntry()
			{
				if (_sumOfWeights < 1)
				{
					Debug.LogError("RandomPool has no entries. Cannot get random entry.");
					return -1;
				}
				else
				{
					// NOTE: Take for e.g. two weights. 5 each. Sum is 10. each should be 50% chance.
					// Range must be 0 (inclusive) to sum which is 10 (exclusive) so the possible
                    // numbers generated is 0,1,2,3,4,5,6,7,8,9 which is a total of 10 numbers.
                    // Now we check < first weight which is 5 so 0,1,2,3,4 all qualify, leaving the
                    // remaining 5,6,7,8,9 to be the last entry's probability.
					int randVal = Random.Range(0, _sumOfWeights); // NOTE: Must check only < to be fair.
					int counterSum = 0;
					for (int iEntry = 0; iEntry < _entryList.Count; iEntry++)
					{
						Entry curEntry = _entryList[iEntry];
						counterSum += curEntry.ProbabilityWeight;
						if (randVal < counterSum)
						{
							return curEntry.Value;
						}
					}

					Debug.LogError("Unable to return an entry. Was the randVal out of range?");
					return -1; 
				}
			}
        }
	}
}
