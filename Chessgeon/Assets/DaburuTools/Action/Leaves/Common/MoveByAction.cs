using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveByAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 _vecDesiredTotalDelta;
		float _actionDuration;

		Vector3 _vecAccumulatedDelta;
		float _elapsedDuration;

		public MoveByAction(Transform inTransform, Graph inGraph, Vector3 inDesiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredDelta(inDesiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public MoveByAction(Transform inTransform, Vector3 inDesiredDelta, float inActionDuration)
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
			_vecDesiredTotalDelta = inNewDesiredDelta;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_vecAccumulatedDelta = Vector3.zero;
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

			_transform.position -= _vecAccumulatedDelta;    // Reverse the previous frame's rotation.

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			_vecAccumulatedDelta = Vector3.LerpUnclamped(Vector3.zero, _vecDesiredTotalDelta, t);

			_transform.position += _vecAccumulatedDelta;    // Apply the new delta rotation.

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				Vector3 imperfection = _vecDesiredTotalDelta - _vecAccumulatedDelta;
				_transform.position += imperfection;    // Force to exact delta displacement.

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
				Vector3 imperfection = _vecDesiredTotalDelta - _vecAccumulatedDelta;
				_transform.position += imperfection;    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
