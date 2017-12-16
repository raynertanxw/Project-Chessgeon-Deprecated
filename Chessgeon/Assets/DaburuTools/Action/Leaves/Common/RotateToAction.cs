using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateToAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 mvecDesiredRotation;
		float _actionDuration;

		Vector3 mvecInitialRotation;
		float _elapsedDuration;

		public RotateToAction(Transform inTransform, Graph inGraph, Vector3 inDesiredRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredRotation(inDesiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateToAction(Transform inTransform, Vector3 inDesiredRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredRotation(inDesiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredRotation(Vector3 inNewDesiredRotation)
		{
			mvecDesiredRotation = inNewDesiredRotation;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialRotation = _transform.eulerAngles;
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
			_transform.eulerAngles = Vector3.LerpUnclamped(mvecInitialRotation, mvecDesiredRotation, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.eulerAngles = mvecDesiredRotation;   // Force it to be the exact rotation that it wants.
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
				_transform.eulerAngles = mvecDesiredRotation;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
