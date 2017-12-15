using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class AxisLocalRotateByAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecAxis;
		float mfDesiredAngleDelta;
		float mfActionDuration;

		float mfAccumulatedAngleDelta;
		float mfElapsedDuration;

		public AxisLocalRotateByAction(Transform inTransform, Graph _graph, Vector3 _axis, float _desiredAngleDelta, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetAxis(_axis);
			SetDesiredAngleDelta(_desiredAngleDelta);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public AxisLocalRotateByAction(Transform inTransform, Vector3 _axis, float _desiredAngleDelta, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetAxis(_axis);
			SetDesiredAngleDelta(_desiredAngleDelta);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph _newGraph)
		{
			mGraph = _newGraph;
		}
		public void SetAxis(Vector3 _newAxis)
		{
			mvecAxis = _newAxis;
		}
		public void SetDesiredAngleDelta(float _newDesiredAngleDelta)
		{
			mfDesiredAngleDelta = _newDesiredAngleDelta;
		}
		public void SetActionDuration(float _newActionDuration)
		{
			mfActionDuration = _newActionDuration;
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

			_transform.Rotate(mvecAxis, -mfAccumulatedAngleDelta, Space.Self);  // Reverse the previous frame's rotation.

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mfAccumulatedAngleDelta = Mathf.LerpUnclamped(0.0f, mfDesiredAngleDelta, t);

			_transform.Rotate(mvecAxis, mfAccumulatedAngleDelta, Space.Self);   // Apply the new delta rotation.

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				float imperfection = mfDesiredAngleDelta - mfAccumulatedAngleDelta;
				_transform.Rotate(mvecAxis, imperfection, Space.Self);  // Force to exact delta displacement.

				OnActionEnd();
				_parent.Remove(this);
			}
		}
		public override void MakeResettable(bool _bIsResettable)
		{
			base.MakeResettable(_bIsResettable);
		}
		public override void Reset()
		{
			SetupAction();
		}
		public override void StopAction(bool _bSnapToDesired)
		{
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			mfElapsedDuration += mfActionDuration;

			if (_bSnapToDesired)
			{
				float imperfection = mfDesiredAngleDelta - mfAccumulatedAngleDelta;
				_transform.Rotate(mvecAxis, imperfection, Space.Self);  // Force to exact delta displacement.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
