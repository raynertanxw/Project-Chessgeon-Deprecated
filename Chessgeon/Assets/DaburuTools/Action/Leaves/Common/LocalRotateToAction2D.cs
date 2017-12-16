using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateToAction2D : Action
	{
		Transform _transform;
		Graph _graph;
		float mfDesiredLocalZEulerAngle;
		float _actionDuration;

		float mfInitialLocalZEulerAngle;
		float _elapsedDuration;

		public LocalRotateToAction2D(Transform inTransform, Graph inGraph, float inDesiredLocalZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredLocalZEulerAngle(inDesiredLocalZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalRotateToAction2D(Transform inTransform, float inDesiredLocalZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredLocalZEulerAngle(inDesiredLocalZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredLocalZEulerAngle(float inNewDesiredLocalZEulerAngle)
		{
			mfDesiredLocalZEulerAngle = inNewDesiredLocalZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfInitialLocalZEulerAngle = _transform.localEulerAngles.z;
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
			_transform.localEulerAngles = new Vector3(
				_transform.localEulerAngles.x,
				_transform.localEulerAngles.y,
				Mathf.LerpUnclamped(mfInitialLocalZEulerAngle, mfDesiredLocalZEulerAngle, t)
			);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					mfDesiredLocalZEulerAngle
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
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					mfDesiredLocalZEulerAngle
				);  // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}