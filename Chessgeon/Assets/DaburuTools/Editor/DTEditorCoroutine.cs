using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DaburuTools
{
	public class DTEditorCoroutine
	{
		public static DTEditorCoroutine start( IEnumerator _routine )
		{
			DTEditorCoroutine coroutine = new DTEditorCoroutine(_routine);
			coroutine.start();
			return coroutine;
		}

		readonly IEnumerator routine;
		DTEditorCoroutine( IEnumerator _routine )
		{
			routine = _routine;
		}

		void start()
		{
			//Debug.Log("start");
			EditorApplication.update += update;
		}

		public void stop()
		{
			//Debug.Log("stop");
			EditorApplication.update -= update;
		}

		void update()
		{
			/* NOTE: no need to try/catch MoveNext,
			 * if an IEnumerator throws its next iteration returns false.
			 * Also, Unity probably catches when calling EditorApplication.update.
			 */

			//Debug.Log("update");
			if (!routine.MoveNext())
			{
				stop();
			}
		}
	}
}