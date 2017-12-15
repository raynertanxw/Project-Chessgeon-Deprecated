using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalMoveToAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredLocalPos;
		float mfActionDuration;

		Vector3 mvecInitialLocalPos;
		float mfElapsedDuration;

		public LocalMoveToAction(Transform inTransform, Graph inGraph, Vector3 inDesiredLocalPosition, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredLocalPosition(inDesiredLocalPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalMoveToAction(Transform inTransform, Vector3 inDesiredLocalPosition, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredLocalPosition(inDesiredLocalPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredLocalPosition(Vector3 inNewDesiredLocalPosition)
		{
			mvecDesiredLocalPos = inNewDesiredLocalPosition;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialLocalPos = _transform.localPosition;
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
			_transform.localPosition = Vector3.LerpUnclamped(mvecInitialLocalPos, mvecDesiredLocalPos, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.localPosition = mvecDesiredLocalPos; // Force it to be the exact local position that it wants.
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
				_transform.localPosition = mvecDesiredLocalPos; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
