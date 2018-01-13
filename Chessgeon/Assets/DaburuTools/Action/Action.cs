using System.Collections.Generic;

namespace DaburuTools
{
	public class Action
	{
		// Unscaled Delta Time Settings
		protected bool _isUnscaledDeltaTime = false;
		public virtual void SetUnscaledDeltaTime(bool inIsUnscaledDeltaTime) { _isUnscaledDeltaTime = inIsUnscaledDeltaTime; }
		protected float ActionDeltaTime(bool inIsUnscaledDeltaTime)
		{
			if (inIsUnscaledDeltaTime)
				return UnityEngine.Time.unscaledDeltaTime;
			else
				return UnityEngine.Time.deltaTime;
		}

		public Action _parent = null;
		public bool _isRunning = false;
		public bool _isResettable = false;
		public Utils.GenericVoidDelegate OnActionStart = EmptyFunc;
		public Utils.GenericVoidDelegate OnActionUpdate = EmptyFunc;
		public Utils.GenericVoidDelegate OnActionFinish = EmptyFunc;

		// Optional.
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
		public virtual void MakeResettable(bool inIsResettable) { _isResettable = inIsResettable; }
		public virtual void Reset() { }
		public virtual void StopAction(bool inSnapToDesired = false) { }
		// Do not override ActionRecurisve.
		public void StopActionRecursive(bool inSnapToDesired = false)
		{
			if (!_isRunning)
				return;

			// Stop itself.
			StopAction(inSnapToDesired);

			if (_parent != null)
				_parent.StopActionRecursive(inSnapToDesired);
		}

		// Leaves do not need to override these functions.
		public virtual bool Add(Action inAction) { return false; }
		public virtual bool Remove(Action inAction) { return false; }
		public virtual LinkedListNode<Action> GetListHead() { return null; }
		public virtual bool IsComposite() { return false; }
	}
}
