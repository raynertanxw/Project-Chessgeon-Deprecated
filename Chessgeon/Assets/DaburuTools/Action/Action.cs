using System.Collections.Generic;

namespace DaburuTools
{
	public class Action
	{
		// Unscaled Delta Time Settings
		protected bool _isUnscaledDeltaTime = false;
		public virtual void SetUnscaledDeltaTime(bool InIsUnscaledDeltaTime) { _isUnscaledDeltaTime = InIsUnscaledDeltaTime; }
		protected float ActionDeltaTime(bool InIsUnscaledDeltaTime)
		{
			if (InIsUnscaledDeltaTime)
				return UnityEngine.Time.unscaledDeltaTime;
			else
				return UnityEngine.Time.deltaTime;
		}

		public Action _parent = null;
		public bool _isRunning = false;
		public bool _isResettable = false;
		public OnActionBeginDelegate OnActionStart = EmptyFunc;
		public OnActionUpdateDelegate OnActionUpdate = EmptyFunc;
		public OnActionEndDelegate OnActionFinish = EmptyFunc;

		// Optional.
		public delegate void OnActionBeginDelegate();
		public delegate void OnActionUpdateDelegate();
		public delegate void OnActionEndDelegate();
		protected virtual void OnActionBegin() { _isRunning = true; OnActionStart(); }
		protected virtual void OnActionRun() { OnActionUpdate(); }
		protected virtual void OnActionEnd() { _isRunning = false; OnActionFinish(); }
		private static void EmptyFunc() { }

		// All must implement.
		public virtual void RunAction()
		{
			if (!_isRunning)
				OnActionBegin();

			OnActionRun();
		}
		public virtual void MakeResettable(bool _bIsResettable) { _isResettable = _bIsResettable; }
		public virtual void Reset() { }
		public virtual void StopAction(bool _bSnapToDesired = false) { }
		// Do not override ActionRecurisve.
		public void StopActionRecursive(bool _bSnapToDesired = false)
		{
			if (!_isRunning)
				return;

			// Stop itself.
			StopAction(_bSnapToDesired);

			if (_parent != null)
				_parent.StopActionRecursive(_bSnapToDesired);
		}

		// Leaves do not need to override these functions.
		public virtual bool Add(Action _Action) { return false; }
		public virtual bool Remove(Action _Action) { return false; }
		public virtual LinkedListNode<Action> GetListHead() { return null; }
		public virtual bool IsComposite() { return false; }
	}
}
