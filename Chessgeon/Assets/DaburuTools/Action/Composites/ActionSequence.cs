﻿using System.Collections.Generic;

namespace DaburuTools
{
	public class ActionSequence : Action
	{
		public override void SetUnscaledDeltaTime(bool inIsUnscaledDeltaTime)
		{
			base.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);

			// Set the same for children actions.
			for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
				node.Value.SetUnscaledDeltaTime(inIsUnscaledDeltaTime);
		}

		private LinkedList<Action> _actionLinkedList;
		private LinkedList<Action> _storageLinkedList;  // Used for resetting.

		public ActionSequence()
		{
			_actionLinkedList = new LinkedList<Action>();
		}
		public ActionSequence(params Action[] inActions)
		{
			_actionLinkedList = new LinkedList<Action>();
			for (int i = 0; i < inActions.Length; i++)
			{
				if (inActions[i] == null) continue;
				Add(inActions[i]);
			}
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_actionLinkedList.Count > 0)
			{
				_actionLinkedList.First.Value.RunAction();
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

			for (LinkedListNode<Action> node = _actionLinkedList.First; node != null; node = node.Next)
				node.Value.MakeResettable(inIsResettable);

			if (inIsResettable)
				_storageLinkedList = new LinkedList<Action>();
			else
				_storageLinkedList = null;
		}
		public override void Reset()
		{
			if (!_isResettable)
				return;

			for (LinkedListNode<Action> node = _storageLinkedList.First; node != null; node = node.Next)
			{
				node.Value.Reset();
				_actionLinkedList.AddFirst(node.Value);
			}

			_storageLinkedList.Clear();
			_isRunning = false;
		}
		public override void StopAction(bool inSnapToDesired)
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
				actionList[i].StopAction(inSnapToDesired);
			}

			OnActionEnd();
			_parent.Remove(this);
		}



		public override bool Add(Action inAction)
		{
			inAction._parent = this;
			_actionLinkedList.AddLast(inAction);
			return true;
		}
		public bool Add(params Action[] inActions)
		{
			for (int i = 0; i < inActions.Length; i++)
			{
				inActions[i]._parent = this;
				_actionLinkedList.AddLast(inActions[i]);
			}

			return true;
		}
		public override bool Remove(Action inAction)
		{
			if (GetListHead() == null) { return false; }

			if (_isResettable)
			{
				_storageLinkedList.AddFirst(_actionLinkedList.First.Value);
			}
			return _actionLinkedList.Remove(inAction);
		}
		public override LinkedListNode<Action> GetListHead() { return _actionLinkedList.First; }
		public override bool IsComposite() { return true; }
	}
}
