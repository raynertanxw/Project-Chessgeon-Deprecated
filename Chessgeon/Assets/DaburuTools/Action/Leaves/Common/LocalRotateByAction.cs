using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateByAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 mvecDesiredTotalDelta;
		float _actionDuration;

		Vector3 mvecAccumulatedDelta;
		float _elapsedDuration;

		public LocalRotateByAction(Transform inTransform, Graph inGraph, Vector3 inDesiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredDelta(inDesiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalRotateByAction(Transform inTransform, Vector3 inDesiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredDelta(inDesiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredDelta(Vector3 inNewDesiredDelta)
		{
			mvecDesiredTotalDelta = inNewDesiredDelta;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecAccumulatedDelta = Vector3.zero;
			_elapsedDuration = 0f;
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

			_transform.Rotate(-mvecAccumulatedDelta, Space.Self);   // Reverse the previous frame's rotation.

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			mvecAccumulatedDelta = Vector3.LerpUnclamped(Vector3.zero, mvecDesiredTotalDelta, t);

			_transform.Rotate(mvecAccumulatedDelta, Space.Self);    // Apply the new delta rotation.

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				Vector3 imperfection = mvecDesiredTotalDelta - mvecAccumulatedDelta;
				_transform.Rotate(imperfection, Space.Self);    // Force to exact delta displacement.

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
				Vector3 imperfection = mvecDesiredTotalDelta - mvecAccumulatedDelta;
				_transform.Rotate(imperfection, Space.Self);    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
