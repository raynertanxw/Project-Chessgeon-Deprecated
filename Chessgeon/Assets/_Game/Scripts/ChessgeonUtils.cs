using System.Collections;
using System.Collections.Generic;
using DaburuTools;

public static class ChessgeonUtils
{
	public static string FormatComboString(int inCombo)
	{
		return "x" + inCombo.ToString();
	}

	public static string FormatScoreString(int inScore)
	{
		return Utils.FormatIntWithThousandSeparator(inScore);
	}

	public static string FormatFloorString(int inFloorNum)
	{
		return "F" + inFloorNum.ToString("00");
	}

	public static string FormatGemString(int inNumGems)
	{
		return Utils.FormatIntWithThousandSeparator(inNumGems);
	}
}
