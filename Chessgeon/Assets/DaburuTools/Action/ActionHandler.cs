using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace DaburuTools
{
	public class ActionHandler : MonoBehaviour
	{
		private static ActionHandler sInstance = null;
		public static ActionHandler Instance { get { return sInstance; } }

		private MasterActionParallel mMasterActionParallel;

		private void SetUpActionHandler()
		{
			mMasterActionParallel = new MasterActionParallel();
		}

		void Awake()
		{
			if (sInstance == null)
				sInstance = this;
			else
				Destroy(this.gameObject);

			SetUpActionHandler();
		}

		void Update()
		{
			mMasterActionParallel.RunAction();
		}

		void OnDestroy()
		{
			sInstance = null;
		}

		#region Client Functions
		public static void RunAction(params Action[] _Actions)
		{
#if UNITY_EDITOR
			if (sInstance == null)
			{
				Debug.LogWarning("DaburuTools.Action: MISSING ACTIONHANDLER. Please check if you have an ActionHandler in the scene.\nOtherwise, add one by going to the Menu bar and selecting DaburuTools > Action > Create ActionHandler");
				return;
			}
#endif

			sInstance.mMasterActionParallel.Add(_Actions);
		}
		#endregion

		#region Nested Special ActionParallelClass
		private sealed class MasterActionParallel : Action
		{
			public override void SetUnscaledDeltaTime(bool _bIsUnscaledDeltaTime)
			{
				base.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);

				// Set the same for children actions.
				for (int i = 0; i < mActionList.Count; i++)
					mActionList[i].SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);
			}

			private List<Action> mActionList;

			public MasterActionParallel()
			{
				mActionList = new List<Action>(128);
			}



			public override void RunAction()
			{
				base.RunAction();

				if (mActionList.Count > 0)
				{
					for (int i = 0; i < mActionList.Count; i++)
						mActionList[i].RunAction();
				}
				else
				{
					OnActionEnd();

					if (mParent != null)
						mParent.Remove(this);
				}
			}
			public override void StopAction(bool _bSnapToDesired)
			{
				return;
			}



			public override bool Add(Action _Action)
			{
				_Action.mParent = this;
				mActionList.Add(_Action);
				return true;
			}
			public bool Add(params Action[] _Actions)
			{
				for (int i = 0; i < _Actions.Length; i++)
				{
					_Actions[i].mParent = this;
					mActionList.Add(_Actions[i]);
				}

				return true;
			}
			public override bool Remove(Action _Action)
			{
				if (mActionList.Count == 0) { return false; }

				return mActionList.Remove(_Action);
			}
		}
		#endregion
	}
}
