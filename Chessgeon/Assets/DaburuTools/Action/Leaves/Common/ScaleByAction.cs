using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class ScaleByAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredScaleDelta;
		float mfActionDuration;

		Vector3 mvecAccumulatedScale;
		float mfElapsedDuration;

		public ScaleByAction(Transform inTransform, Graph inGraph, Vector3 _desiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredDelta(_desiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public ScaleByAction(Transform inTransform, Vector3 _desiredDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredDelta(_desiredDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredDelta(Vector3 _newDesiredDelta)
		{
			mvecDesiredScaleDelta = _newDesiredDelta - Vector3.one;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecAccumulatedScale = Vector3.one;
			mfElapsedDuration = 0f;
		}
		private Vector3 CalcInverseAccumulatedScale()
		{
			Vector3 inverseAccumulatedScale = _transform.localScale;
			inverseAccumulatedScale.x /= mvecAccumulatedScale.x;
			inverseAccumulatedScale.y /= mvecAccumulatedScale.y;
			inverseAccumulatedScale.z /= mvecAccumulatedScale.z;

			return inverseAccumulatedScale;
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
			Vector3 delta = Vector3.LerpUnclamped(Vector3.zero, mvecDesiredScaleDelta, t) + Vector3.one - mvecAccumulatedScale;

			_transform.localScale = Vector3.Scale(CalcInverseAccumulatedScale(), mvecAccumulatedScale + delta);
			mvecAccumulatedScale += delta;


			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				Vector3 finalScaleVec = CalcInverseAccumulatedScale();
				finalScaleVec = Vector3.Scale(finalScaleVec, mvecDesiredScaleDelta + Vector3.one);
				_transform.localScale = finalScaleVec;  // Force it to be the exact scale that it wants.

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
				Vector3 finalScaleVec = CalcInverseAccumulatedScale();
				finalScaleVec = Vector3.Scale(finalScaleVec, mvecDesiredScaleDelta + Vector3.one);
				_transform.localScale = finalScaleVec;  // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
