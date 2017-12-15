using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class AxisRotateByAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecAxis;
		float mfDesiredAngleDelta;
		float mfActionDuration;

		float mfAccumulatedAngleDelta;
		float mfElapsedDuration;

		public AxisRotateByAction(Transform inTransform, Graph _graph, Vector3 _axis, float _desiredAngleDelta, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetAxis(_axis);
			SetDesiredAngleDelta(_desiredAngleDelta);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public AxisRotateByAction(Transform inTransform, Vector3 _axis, float _desiredAngleDelta, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetAxis(_axis);
			SetDesiredAngleDelta(_desiredAngleDelta);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetAxis(Vector3 inNewAxis)
		{
			mvecAxis = inNewAxis;
		}
		public void SetDesiredAngleDelta(float inNewDesiredAngleDelta)
		{
			mfDesiredAngleDelta = inNewDesiredAngleDelta;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfAccumulatedAngleDelta = 0;
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

			_transform.Rotate(mvecAxis, -mfAccumulatedAngleDelta, Space.World); // Reverse the previous frame's rotation.

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mfAccumulatedAngleDelta = Mathf.LerpUnclamped(0.0f, mfDesiredAngleDelta, t);

			_transform.Rotate(mvecAxis, mfAccumulatedAngleDelta, Space.World);  // Apply the new delta rotation.

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				float imperfection = mfDesiredAngleDelta - mfAccumulatedAngleDelta;
				_transform.Rotate(mvecAxis, imperfection, Space.World); // Force to exact delta displacement.

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
				float imperfection = mfDesiredAngleDelta - mfAccumulatedAngleDelta;
				_transform.Rotate(mvecAxis, imperfection, Space.World); // Force to exact delta displacement.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
