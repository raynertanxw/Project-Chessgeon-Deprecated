using System.Collections;
using System.Collections.Generic;
using DaburuTools;

public static class ChessgeonUtils
{
	public static string FormatFloorString(int inFloorNum)
	{
		return "F" + inFloorNum.ToString("00");
	}

	public static string FormatGemString(int inNumGems)
	{
		return Utils.FormatIntWithThousandSeparator(inNumGems);
	}
}
