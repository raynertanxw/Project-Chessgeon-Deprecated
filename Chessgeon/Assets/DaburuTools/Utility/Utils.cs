using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
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
		
		public static float GetDesignHeightFromDesignWidth(float inDesignWidth)
		{
			float designHeight = inDesignWidth / Screen.width * Screen.height;
			return designHeight;
		}



        #region ConvertingTypes
        public static int[] Vector2IntToIntArray(Vector2Int inVector2Int)
        {
            int[] resultingIntArr = new int[2];
            resultingIntArr[0] = inVector2Int.x;
            resultingIntArr[1] = inVector2Int.y;

            return resultingIntArr;
        }

        public static Vector2Int IntArrayToSingleVector2Int(int[] inIntArr)
        {
            Debug.Assert(inIntArr.Length == 2, "inIntArr is of wrong dimensions!");
            return new Vector2Int(inIntArr[0], inIntArr[1]);
        }
        #endregion

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

        #region DateTimeConversions
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        #endregion

        #region Encryption
        public static byte[] StringToByte(string inStrToConvert)
        {
            System.Text.UTF8Encoding utf8Encoding = new System.Text.UTF8Encoding();
            byte[] bytes = utf8Encoding.GetBytes(inStrToConvert);

            return bytes;
        }

        public static string BytesToString(byte[] inBytesToConvert)
        {
            string hashString = "";
            for (int i = 0; i < inBytesToConvert.Length; i++)
            {
                hashString += System.Convert.ToString(inBytesToConvert[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static string SHA1HashString(string inStrToEncrypt)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] hashBytes = sha1.ComputeHash(StringToByte(inStrToEncrypt));
            return BytesToString(hashBytes);
        }

        public static string MD5HashString(string inStrToEncrypt)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(StringToByte(inStrToEncrypt));
            return BytesToString(hashBytes);
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
