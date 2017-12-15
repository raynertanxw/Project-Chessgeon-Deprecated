namespace DaburuTools
{
	public class ActionRepeatForever : Action
	{
		public override void SetUnscaledDeltaTime(bool _bIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);

			// Set the same for children actions.
			_repeatedAction.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);
		}

		private Action _repeatedAction;

		public ActionRepeatForever(Action _Action)
		{
			_Action._parent = this;
			_repeatedAction = _Action;

			_Action.MakeResettable(true);
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_repeatedAction != null)
				_repeatedAction.RunAction();
		}
		public override void MakeResettable(bool _bIsResettable)
		{
			UnityEngine.Debug.LogWarning("ActionRepeatForever cannot be resetted");
		}
		public override void Reset()
		{
			UnityEngine.Debug.LogWarning("ActionRepeatForever cannot be resetted");
		}
		public override void StopAction(bool _bSnapToDesired)
		{
			if (!_isRunning)
				return;

			if (_repeatedAction._isRunning == false)
				_repeatedAction.RunAction();
			_repeatedAction.StopAction(_bSnapToDesired);

			OnActionEnd();
			_parent.Remove(this);
		}



		// Doesn't make sense to add. Don't need to override Add.
		public override bool Remove(Action _Action)
		{
			_repeatedAction.Reset();
			return true;
		}
		// No LinkedList to return. Don't need to override GetListHead.
		public override bool IsComposite() { return true; }
	}
}
