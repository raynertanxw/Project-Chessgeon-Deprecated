using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveByAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredTotalDelta;
		float mfActionDuration;

		Vector3 mvecAccumulatedDelta;
		float mfElapsedDuration;

		public MoveByAction(Transform inTransform, Graph inGraph, Vector3 inDesiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredDelta(inDesiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public MoveByAction(Transform inTransform, Vector3 inDesiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredDelta(inDesiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredDelta(Vector3 inNewDesiredDelta)
		{
			mvecDesiredTotalDelta = inNewDesiredDelta;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecAccumulatedDelta = Vector3.zero;
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

			_transform.position -= mvecAccumulatedDelta;    // Reverse the previous frame's rotation.

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mvecAccumulatedDelta = Vector3.LerpUnclamped(Vector3.zero, mvecDesiredTotalDelta, t);

			_transform.position += mvecAccumulatedDelta;    // Apply the new delta rotation.

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				Vector3 imperfection = mvecDesiredTotalDelta - mvecAccumulatedDelta;
				_transform.position += imperfection;    // Force to exact delta displacement.

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
				Vector3 imperfection = mvecDesiredTotalDelta - mvecAccumulatedDelta;
				_transform.position += imperfection;    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
