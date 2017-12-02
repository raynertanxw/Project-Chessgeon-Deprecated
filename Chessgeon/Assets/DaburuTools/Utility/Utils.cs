using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public static class Utils
	{
		public static float GetDesignWidthFromDesignHeight(float inDesignHeight)
		{
			float designWidth = inDesignHeight / Screen.height * Screen.width;
			return designWidth;
		}
	}
}
