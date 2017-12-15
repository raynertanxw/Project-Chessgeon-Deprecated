using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateByAction2D : Action
	{
		Transform _transform;
		Graph mGraph;
		float mfDesiredTotalZEulerAngle;
		float mfActionDuration;

		float mfAccumulatedZEulerAngle;
		float mfElapsedDuration;

		public RotateByAction2D(Transform inTransform, Graph _graph, float _desiredZEulerAngle, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetDesiredZEulerAngle(_desiredZEulerAngle);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public RotateByAction2D(Transform inTransform, float _desiredZEulerAngle, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredZEulerAngle(_desiredZEulerAngle);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredZEulerAngle(float _newDesiredZEulerAngle)
		{
			mfDesiredTotalZEulerAngle = _newDesiredZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfAccumulatedZEulerAngle = 0f;
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

			Vector3 previousDeltaRot = new Vector3(
				0.0f,
				0.0f,
				mfAccumulatedZEulerAngle);
			_transform.Rotate(-previousDeltaRot);   // Reverse the previous frame's rotation.

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mfAccumulatedZEulerAngle = Mathf.LerpUnclamped(0.0f, mfDesiredTotalZEulerAngle, t);

			Vector3 newDeltaRot = new Vector3(
				0.0f,
				0.0f,
				mfAccumulatedZEulerAngle);
			_transform.Rotate(newDeltaRot); // Apply the new delta rotation.

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				Vector3 imperfection = Vector3.forward * (mfDesiredTotalZEulerAngle - mfAccumulatedZEulerAngle);
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
			mfElapsedDuration += mfActionDuration;

			if (inSnapToDesired)
			{
				Vector3 imperfection = Vector3.forward * (mfDesiredTotalZEulerAngle - mfAccumulatedZEulerAngle);
				_transform.Rotate(imperfection);    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
