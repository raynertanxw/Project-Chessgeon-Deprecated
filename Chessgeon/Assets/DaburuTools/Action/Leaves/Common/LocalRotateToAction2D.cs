using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateToAction2D : Action
	{
		Transform _transform;
		Graph mGraph;
		float mfDesiredLocalZEulerAngle;
		float mfActionDuration;

		float mfInitialLocalZEulerAngle;
		float mfElapsedDuration;

		public LocalRotateToAction2D(Transform inTransform, Graph _graph, float _desiredLocalZEulerAngle, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetDesiredLocalZEulerAngle(_desiredLocalZEulerAngle);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public LocalRotateToAction2D(Transform inTransform, float _desiredLocalZEulerAngle, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredLocalZEulerAngle(_desiredLocalZEulerAngle);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph _newGraph)
		{
			mGraph = _newGraph;
		}
		public void SetDesiredLocalZEulerAngle(float _newDesiredLocalZEulerAngle)
		{
			mfDesiredLocalZEulerAngle = _newDesiredLocalZEulerAngle;
		}
		public void SetActionDuration(float _newActionDuration)
		{
			mfActionDuration = _newActionDuration;
		}
		private void SetupAction()
		{
			mfInitialLocalZEulerAngle = _transform.localEulerAngles.z;
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
			_transform.localEulerAngles = new Vector3(
				_transform.localEulerAngles.x,
				_transform.localEulerAngles.y,
				Mathf.LerpUnclamped(mfInitialLocalZEulerAngle, mfDesiredLocalZEulerAngle, t)
			);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					mfDesiredLocalZEulerAngle
				);  // Force it to be the exact rotation that it wants.
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
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					mfDesiredLocalZEulerAngle
				);  // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}