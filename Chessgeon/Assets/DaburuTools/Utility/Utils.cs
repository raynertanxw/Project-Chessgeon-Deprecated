using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public static class Utils
	{
		public delegate void GenericVoidDelegate();

		public static float GetDesignWidthFromDesignHeight(float inDesignHeight)
		{
			float designWidth = inDesignHeight / Screen.height * Screen.width;
			return designWidth;
		}



		#region CommonStringFormatting
		public static string FormatIntWithThousandSeparator(int inInt, bool inWithDecimals = false)
		{
			if (inWithDecimals)
			{
				return string.Format("{0:n}", inInt);
			}
			else
			{
				return string.Format("{0:n0}", inInt);
			}
		}
		#endregion


		#region AnimationCurveDefaults
		public static readonly AnimationCurve CurveExponential = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 115.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveInverseExponential = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 120.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveLinear = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 57.5f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 57.5f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveInverseLinear = new AnimationCurve(
			new Keyframe(0.0f, 1.0f, 0.0f * Mathf.Deg2Rad, -57.5f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 0.0f, -57.5f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveOne = new AnimationCurve(
			new Keyframe(0.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveZero = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveHalf = new AnimationCurve(
			new Keyframe(0.0f, 0.5f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 0.5f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveSmoothStep = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveInverseSmoothStep = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 110.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 110.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveBobber = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(0.8f, 1.2f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));

		public static readonly AnimationCurve CurveDipper = new AnimationCurve(
			new Keyframe(0.0f, 0.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(0.2f, -0.2f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad),
			new Keyframe(1.0f, 1.0f, 0.0f * Mathf.Deg2Rad, 0.0f * Mathf.Deg2Rad));
		#endregion
	}
}
