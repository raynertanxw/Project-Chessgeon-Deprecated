using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateToAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 mvecDesiredLocalRotation;
		float _actionDuration;

		Vector3 mvecInitialLocalRotation;
		float _elapsedDuration;

		public LocalRotateToAction(Transform inTransform, Graph inGraph, Vector3 inDesiredLocalRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredLocalRotation(inDesiredLocalRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalRotateToAction(Transform inTransform, Vector3 inDesiredLocalRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredLocalRotation(inDesiredLocalRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredLocalRotation(Vector3 inNewDesiredLocalRotation)
		{
			mvecDesiredLocalRotation = inNewDesiredLocalRotation;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialLocalRotation = _transform.localEulerAngles;
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
			_transform.localEulerAngles = Vector3.LerpUnclamped(mvecInitialLocalRotation, mvecDesiredLocalRotation, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.localEulerAngles = mvecDesiredLocalRotation; // Force it to be the exact local rotation that it wants.
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
				_transform.localEulerAngles = mvecDesiredLocalRotation; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
