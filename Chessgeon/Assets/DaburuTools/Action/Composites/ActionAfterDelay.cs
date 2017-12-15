namespace DaburuTools
{
	public class ActionAfterDelay : Action
	{
		public override void SetUnscaledDeltaTime(bool inIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);

			// Set the same for children actions.
			_delayedAction.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);
		}

		private Action _delayedAction;
		private float _timeDelay;
		private float _timePassed;

		public ActionAfterDelay(Action inAction, float inTimeDelay)
		{
			inAction._parent = this;
			_delayedAction = inAction;
			_timeDelay = inTimeDelay;
			_timePassed = 0f;
		}



		public override void RunAction()
		{
			base.RunAction();

			// Delay the action till delay span has passed.
			if (_timePassed < _timeDelay)
			{
				_timePassed += ActionDeltaTime(_isUnscaledDeltaTime);
				return;
			}

			if (_delayedAction != null)
			{
				_delayedAction.RunAction();
			}
			else
			{
				OnActionEnd();

				if (_parent != null)
					_parent.Remove(this);
			}
		}
		public override void MakeResettable(bool inIsResettable)
		{
			base.MakeResettable(inIsResettable);

			if (_delayedAction != null)
				_delayedAction.MakeResettable(inIsResettable);
		}
		public override void Reset()
		{
			if (!_isResettable)
				return;

			_timePassed = 0f;
			_delayedAction.Reset();
		}
		public override void StopAction(bool inSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			if (_delayedAction != null)
			{
				if (_delayedAction._isRunning == false)
					_delayedAction.RunAction();
				// Need another null check, incase the delayed action is a sequence or parallel.
				// When they are going to run, they might have deleted themself because they are empty.
				if (_delayedAction != null)
					_delayedAction.StopAction(inSnapToDesired);
			}

			OnActionEnd();
			_parent.Remove(this);
		}



		// Doesn't make sense to add. Don't need to override Add.
		public override bool Remove(Action inAction)
		{
			// Simply de-reference to let GC collect.
			if (!_isResettable)
				_delayedAction = null;
			else
				_parent.Remove(this);

			return true;
		}
		// No LinkedList to return. Don't need to override GetListHead.
		public override bool IsComposite() { return true; }
	}
}
