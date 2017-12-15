using System.Collections.Generic;

namespace DaburuTools
{
	public class ActionParallel : Action
	{
		public override void SetUnscaledDeltaTime(bool _bIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);

			// Set the same for children actions.
			for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
				node.Value.SetUnscaledDeltaTime(_bIsUnscaledDeltaTime);
		}

		private LinkedList<Action> _actionLinkedList;
		private LinkedList<Action> _storageLinkedList;  // Used for resetting.

		public ActionParallel()
		{
			_actionLinkedList = new LinkedList<Action>();
		}
		public ActionParallel(params Action[] _Actions)
		{
			_actionLinkedList = new LinkedList<Action>();
			for (int i = 0; i < _Actions.Length; i++)
			{
				if (_Actions[i] == null) continue;
				Add(_Actions[i]);
			}
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_actionLinkedList.Count > 0)
			{
				for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
					node.Value.RunAction();
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

			for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
				node.Value.MakeResettable(_bIsResettable);

			if (_bIsResettable)
				_storageLinkedList = new LinkedList<Action>();
			else
				_storageLinkedList = null;
		}
		public override void Reset()
		{
			for (LinkedListNode<Action> node = _storageLinkedList.First; node != null; node = node.Next)
			{
				node.Value.Reset();
				_actionLinkedList.AddFirst(node.Value);
			}

			_storageLinkedList.Clear();
			_isRunning = false;
		}
		public override void StopAction(bool _bSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Use an array because cannot remove node from linkedlist while traversing.
			Action[] actionList = new Action[_actionLinkedList.Count];
			int numActions = 0;

			for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
			{
				// Ensure they are all running so that the StopAction can work properly.
				if (node.Value._isRunning == false)
					node.Value.RunAction();

				// Add to array to be used later.
				actionList[numActions] = node.Value;
				numActions++;
			}

			for (int i = 0; i < actionList.Length; i++)
			{
				actionList[i].StopAction(_bSnapToDesired);
			}

			OnActionEnd();
			_parent.Remove(this);
		}



		public override bool Add(Action _Action)
		{
			_Action._parent = this;
			_actionLinkedList.AddFirst(_Action);
			return true;
		}
		public bool Add(params Action[] _Actions)
		{
			for (int i = 0; i < _Actions.Length; i++)
			{
				_Actions[i]._parent = this;
				_actionLinkedList.AddFirst(_Actions[i]);
			}

			return true;
		}
		public override bool Remove(Action _Action)
		{
			if (GetListHead() == null) { return false; }

			if (_isResettable)
			{
				_storageLinkedList.AddFirst(_actionLinkedList.Find(_Action).Value);
			}
			return _actionLinkedList.Remove(_Action);
		}
		public override LinkedListNode<Action> GetListHead() { return _actionLinkedList.First; }
		public override bool IsComposite() { return true; }
	}
}
