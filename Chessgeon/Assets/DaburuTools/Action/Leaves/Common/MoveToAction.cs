using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveToAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 mvecDesiredPos;
		float _actionDuration;

		Vector3 mvecInitialPos;
		float _elapsedDuration;

		public MoveToAction(Transform inTransform, Graph inGraph, Vector3 inDesiredPosition, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredPosition(inDesiredPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public MoveToAction(Transform inTransform, Vector3 inDesiredPosition, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredPosition(inDesiredPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredPosition(Vector3 inNewDesiredPosition)
		{
			mvecDesiredPos = inNewDesiredPosition;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialPos = _transform.position;
			_elapsedDuration = 0f;
		}
		protected override void OnActionBegin()
		{
			base.OnActionBegin();

			SetupAction();
		}



		public override void RunAction()
		{
			base.RunAction();

			if (_transform == null)
			{
				// Debug.LogWarning("DaburuTools.Action: _transform Deleted prematurely");
				_parent.Remove(this);
				return;
			}

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			_transform.position = Vector3.LerpUnclamped(mvecInitialPos, mvecDesiredPos, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.position = mvecDesiredPos;   // Force it to be the exact position that it wants.
				OnActionEnd();
				_parent.Remove(this);
			}
		}
		public override void MakeResettable(bool inIsResettable)
		{
			base.MakeResettable(inIsResettable);
		}
		public override void Reset()
		{
			SetupAction();
		}
		public override void StopAction(bool inSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			_elapsedDuration += _actionDuration;

			if (inSnapToDesired)
			{
				_transform.position = mvecDesiredPos;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
