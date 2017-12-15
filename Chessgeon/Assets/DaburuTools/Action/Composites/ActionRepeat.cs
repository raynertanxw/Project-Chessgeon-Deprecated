namespace DaburuTools
{
	public class ActionRepeat : Action
	{
		public override void SetUnscaledDeltaTime(bool _bIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);

			// Set the same for children actions.
			_repeatedAction.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);
		}

		private Action _repeatedAction;
		private int _numRepeats;
		private int _currentRepeats;
		private bool _readyToReset;

		public ActionRepeat(Action _Action, int _numRepeats)
		{
			_Action._parent = this;
			_repeatedAction = _Action;
			this._numRepeats = _numRepeats;
			_currentRepeats = 0;

			_Action.MakeResettable(true);
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_repeatedAction != null)
			{
				if (_isResettable && _readyToReset)
				{
					OnActionEnd();

					_parent.Remove(this);
				}
				else
				{
					_repeatedAction.RunAction();
				}
			}
			else
			{
				OnActionEnd();

				if (_parent != null)
					_parent.Remove(this);
			}
		}
		public override void MakeResettable(bool _bIsResettable)
		{
			base.MakeResettable(_bIsResettable);

			_readyToReset = false;
		}
		public override void Reset()
		{
			_currentRepeats = 0;
			_readyToReset = false;
			_repeatedAction.Reset();
		}
		public override void StopAction(bool _bSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			_currentRepeats = _numRepeats;

			if (_repeatedAction._isRunning == false)
				_repeatedAction.RunAction();
			_repeatedAction.StopAction(_bSnapToDesired);

			OnActionEnd();
			_parent.Remove(this);
		}



		// Doesn't make sense to add. Don't need to override Add.
		public override bool Remove(Action _Action)
		{
			_currentRepeats++;
			if (_currentRepeats < _numRepeats)
			{
				_repeatedAction.Reset();
				return true;
			}

			// Simply de-reference to let GC collect.
			if (!_isResettable)
				_repeatedAction = null;
			else
				_readyToReset = true;

			return true;
		}
		// No LinkedList to return. Don't need to override GetListHead.
		public override bool IsComposite() { return true; }
	}
}
