using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveToAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredPos;
		float mfActionDuration;

		Vector3 mvecInitialPos;
		float mfElapsedDuration;

		public MoveToAction(Transform inTransform, Graph _graph, Vector3 _desiredPosition, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetDesiredPosition(_desiredPosition);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public MoveToAction(Transform inTransform, Vector3 _desiredPosition, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredPosition(_desiredPosition);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredPosition(Vector3 _newDesiredPosition)
		{
			mvecDesiredPos = _newDesiredPosition;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialPos = _transform.position;
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
			_transform.position = Vector3.LerpUnclamped(mvecInitialPos, mvecDesiredPos, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.position = mvecDesiredPos;   // Force it to be the exact position that it wants.
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
				_transform.position = mvecDesiredPos;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
