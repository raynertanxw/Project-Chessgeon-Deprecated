namespace DaburuTools
{
	public class ActionRepeatForever : Action
	{
		public override void SetUnscaledDeltaTime(bool inIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);

			// Set the same for children actions.
			_repeatedAction.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);
		}

		private Action _repeatedAction;

		public ActionRepeatForever(Action inAction)
		{
			inAction._parent = this;
			_repeatedAction = inAction;

			inAction.MakeResettable(true);
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_repeatedAction != null)
				_repeatedAction.RunAction();
		}
		public override void MakeResettable(bool inIsResettable)
		{
			UnityEngine.Debug.LogWarning("ActionRepeatForever cannot be resetted");
		}
		public override void Reset()
		{
			UnityEngine.Debug.LogWarning("ActionRepeatForever cannot be resetted");
		}
		public override void StopAction(bool inSnapToDesired)
		{
			if (!_isRunning)
				return;

			if (_repeatedAction._isRunning == false)
				_repeatedAction.RunAction();
			_repeatedAction.StopAction(inSnapToDesired);

			OnActionEnd();
			_parent.Remove(this);
		}



		// Doesn't make sense to add. Don't need to override Add.
		public override bool Remove(Action inAction)
		{
			_repeatedAction.Reset();
			return true;
		}
		// No LinkedList to return. Don't need to override GetListHead.
		public override bool IsComposite() { return true; }
	}
}
