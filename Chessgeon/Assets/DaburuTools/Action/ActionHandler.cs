using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace DaburuTools
{
	public class ActionHandler : MonoBehaviour
	{
		private static ActionHandler _instance = null;
		public static ActionHandler Instance { get { return _instance; } }

		private MasterActionParallel _masterActionParallel;

		private void SetUpActionHandler()
		{
			_masterActionParallel = new MasterActionParallel();
		}

		void Awake()
		{
			if (_instance == null)
				_instance = this;
			else
				Destroy(this.gameObject);

			SetUpActionHandler();
		}

		void Update()
		{
			_masterActionParallel.RunAction();
		}

		void OnDestroy()
		{
			_instance = null;
		}

		#region Client Functions
		public static void RunAction(params Action[] InActions)
		{
#if UNITY_EDITOR
			if (_instance == null)
			{
				Debug.LogWarning("DaburuTools.Action: MISSING ACTIONHANDLER. Please check if you have an ActionHandler in the scene.\nOtherwise, add one by going to the Menu bar and selecting DaburuTools > Action > Create ActionHandler");
				return;
			}
#endif

			_instance._masterActionParallel.Add(InActions);
		}
		#endregion

		#region Nested Special ActionParallelClass
		private sealed class MasterActionParallel : Action
		{
			public override void SetUnscaledDeltaTime(bool InIsUnscaledDeltaTime)
			{
				base.SetUnscaledDeltaTime(InIsUnscaledDeltaTime);

				// Set the same for children actions.
				for (int i = 0; i < _actionList.Count; i++)
					_actionList[i].SetUnscaledDeltaTime(InIsUnscaledDeltaTime);
			}

			private List<Action> _actionList;

			public MasterActionParallel()
			{
				_actionList = new List<Action>(128);
			}



			public override void RunAction()
			{
				base.RunAction();

				if (_actionList.Count > 0)
				{
					for (int i = 0; i < _actionList.Count; i++)
						_actionList[i].RunAction();
				}
				else
				{
					OnActionEnd();

					if (_parent != null)
						_parent.Remove(this);
				}
			}
			public override void StopAction(bool InSnapToDesired)
			{
				return;
			}



			public override bool Add(Action InAction)
			{
				InAction._parent = this;
				_actionList.Add(InAction);
				return true;
			}
			public bool Add(params Action[] InActions)
			{
				for (int i = 0; i < InActions.Length; i++)
				{
					InActions[i]._parent = this;
					_actionList.Add(InActions[i]);
				}

				return true;
			}
			public override bool Remove(Action InAction)
			{
				if (_actionList.Count == 0) { return false; }

				return _actionList.Remove(InAction);
			}
		}
		#endregion
	}
}
