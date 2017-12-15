using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateToAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredRotation;
		float mfActionDuration;

		Vector3 mvecInitialRotation;
		float mfElapsedDuration;

		public RotateToAction(Transform inTransform, Graph inGraph, Vector3 _desiredRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredRotation(_desiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateToAction(Transform inTransform, Vector3 _desiredRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredRotation(_desiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredRotation(Vector3 _newDesiredRotation)
		{
			mvecDesiredRotation = _newDesiredRotation;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialRotation = _transform.eulerAngles;
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
			_transform.eulerAngles = Vector3.LerpUnclamped(mvecInitialRotation, mvecDesiredRotation, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
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
			mfElapsedDuration += mfActionDuration;

			if (inSnapToDesired)
			{
				_transform.eulerAngles = mvecDesiredRotation;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
