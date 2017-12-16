using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateByAction2D : Action
	{
		Transform _transform;
		Graph _graph;
		float _desiredTotalZEulerAngle;
		float _actionDuration;

		float _accumulatedZEulerAngle;
		float _elapsedDuration;

		public RotateByAction2D(Transform inTransform, Graph inGraph, float inDesiredZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredZEulerAngle(inDesiredZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateByAction2D(Transform inTransform, float inDesiredZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredZEulerAngle(inDesiredZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredZEulerAngle(float inNewDesiredZEulerAngle)
		{
			_desiredTotalZEulerAngle = inNewDesiredZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_accumulatedZEulerAngle = 0f;
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

			Vector3 previousDeltaRot = new Vector3(
				0.0f,
				0.0f,
				_accumulatedZEulerAngle);
			_transform.Rotate(-previousDeltaRot);   // Reverse the previous frame's rotation.

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			_accumulatedZEulerAngle = Mathf.LerpUnclamped(0.0f, _desiredTotalZEulerAngle, t);

			Vector3 newDeltaRot = new Vector3(
				0.0f,
				0.0f,
				_accumulatedZEulerAngle);
			_transform.Rotate(newDeltaRot); // Apply the new delta rotation.

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				Vector3 imperfection = Vector3.forward * (_desiredTotalZEulerAngle - _accumulatedZEulerAngle);
				_transform.Rotate(imperfection);    // Force to exact delta displacement.

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
				Vector3 imperfection = Vector3.forward * (_desiredTotalZEulerAngle - _accumulatedZEulerAngle);
				_transform.Rotate(imperfection);    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
