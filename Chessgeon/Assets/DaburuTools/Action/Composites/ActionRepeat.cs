namespace DaburuTools
{
	public class ActionRepeat : Action
	{
		public override void SetUnscaledDeltaTime(bool inIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);

			// Set the same for children actions.
			_repeatedAction.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);
		}

		private Action _repeatedAction;
		private int _numRepeats;
		private int _currentRepeats;
		private bool _readyToReset;

		public ActionRepeat(Action inAction, int inNumRepeats)
		{
			inAction._parent = this;
			_repeatedAction = inAction;
			this._numRepeats = inNumRepeats;
			_currentRepeats = 0;

			inAction.MakeResettable(true);
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
		public override void MakeResettable(bool inIsResettable)
		{
			base.MakeResettable(inIsResettable);

			_readyToReset = false;
		}
		public override void Reset()
		{
			_currentRepeats = 0;
			_readyToReset = false;
			_repeatedAction.Reset();
		}
		public override void StopAction(bool inSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			_currentRepeats = _numRepeats;

			if (_repeatedAction._isRunning == false)
				_repeatedAction.RunAction();
			_repeatedAction.StopAction(inSnapToDesired);

			OnActionEnd();
			_parent.Remove(this);
		}



		// Doesn't make sense to add. Don't need to override Add.
		public override bool Remove(Action inAction)
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
