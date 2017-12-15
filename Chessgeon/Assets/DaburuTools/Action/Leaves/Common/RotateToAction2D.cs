using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateToAction2D : Action
	{
		Transform _transform;
		Graph mGraph;
		float mfDesiredZEulerAngle;
		float mfActionDuration;

		float mfInitialZEulerAngle;
		float mfElapsedDuration;

		public RotateToAction2D(Transform inTransform, Graph inGraph, float _desiredZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredZEulerAngle(_desiredZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateToAction2D(Transform inTransform, float _desiredZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredZEulerAngle(_desiredZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredZEulerAngle(float _newDesiredZEulerAngle)
		{
			mfDesiredZEulerAngle = _newDesiredZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfInitialZEulerAngle = _transform.eulerAngles.z;
			mfElapsedDuration = 0f;
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

			mfElapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			_transform.eulerAngles = new Vector3(
				_transform.eulerAngles.x,
				_transform.eulerAngles.y,
				Mathf.LerpUnclamped(mfInitialZEulerAngle, mfDesiredZEulerAngle, t)
			);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
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
			mfElapsedDuration += mfActionDuration;

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
