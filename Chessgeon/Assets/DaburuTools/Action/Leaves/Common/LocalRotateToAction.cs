using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateToAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredLocalRotation;
		float mfActionDuration;

		Vector3 mvecInitialLocalRotation;
		float mfElapsedDuration;

		public LocalRotateToAction(Transform inTransform, Graph _graph, Vector3 _desiredLocalRotation, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetDesiredLocalRotation(_desiredLocalRotation);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public LocalRotateToAction(Transform inTransform, Vector3 _desiredLocalRotation, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredLocalRotation(_desiredLocalRotation);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph _newGraph)
		{
			mGraph = _newGraph;
		}
		public void SetDesiredLocalRotation(Vector3 _newDesiredLocalRotation)
		{
			mvecDesiredLocalRotation = _newDesiredLocalRotation;
		}
		public void SetActionDuration(float _newActionDuration)
		{
			mfActionDuration = _newActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialLocalRotation = _transform.localEulerAngles;
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
			_transform.localEulerAngles = Vector3.LerpUnclamped(mvecInitialLocalRotation, mvecDesiredLocalRotation, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.localEulerAngles = mvecDesiredLocalRotation; // Force it to be the exact local rotation that it wants.
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
				_transform.localEulerAngles = mvecDesiredLocalRotation; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
