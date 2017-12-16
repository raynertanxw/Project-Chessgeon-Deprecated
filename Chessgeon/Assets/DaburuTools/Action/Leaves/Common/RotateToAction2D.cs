using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateToAction2D : Action
	{
		Transform _transform;
		Graph _graph;
		float mfDesiredZEulerAngle;
		float _actionDuration;

		float mfInitialZEulerAngle;
		float _elapsedDuration;

		public RotateToAction2D(Transform inTransform, Graph inGraph, float inDesiredZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredZEulerAngle(inDesiredZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateToAction2D(Transform inTransform, float inDesiredZEulerAngle, float inActionDuration)
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
			mfDesiredZEulerAngle = inNewDesiredZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfInitialZEulerAngle = _transform.eulerAngles.z;
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
			_transform.eulerAngles = new Vector3(
				_transform.eulerAngles.x,
				_transform.eulerAngles.y,
				Mathf.LerpUnclamped(mfInitialZEulerAngle, mfDesiredZEulerAngle, t)
			);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.eulerAngles = new Vector3(
					_transform.eulerAngles.x,
					_transform.eulerAngles.y,
					mfDesiredZEulerAngle
				);  // Force it to be the exact rotation that it wants.
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
				_transform.eulerAngles = new Vector3(
					_transform.eulerAngles.x,
					_transform.eulerAngles.y,
					mfDesiredZEulerAngle
				);  // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
