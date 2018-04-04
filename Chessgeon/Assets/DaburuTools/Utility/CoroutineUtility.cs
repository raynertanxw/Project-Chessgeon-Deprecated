using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public class CoroutineUtility : MonoBehaviour
	{
		private static MonoBehaviour _instanceForGetterOnly;
		private static MonoBehaviour _instance
		{
			get
			{
				if (_instanceForGetterOnly == null)
				{
					_instanceForGetterOnly = new GameObject("CoroutineUtility").AddComponent<CoroutineUtility>();
					DontDestroyOnLoad(_instanceForGetterOnly);
				}
				return _instanceForGetterOnly;
			}
		}

		public static Coroutine StartCoroutineImmediate(IEnumerator inCoroutine)
		{
			return _instance.StartCoroutine(inCoroutine);
		}

		public static void StopPreviousCoroutine(IEnumerator inCoroutine)
		{
			_instance.StopCoroutine(inCoroutine);
		}

		public static void StopPreviousCoroutine(Coroutine inCoroutine)
		{
			_instance.StopCoroutine(inCoroutine);
		}
	}
}
