using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class AxisRotateByAction : Action
	{
		Transform _transform;
		AnimationCurve _animCurve;
		Vector3 _vecAxis;
		float _desiredAngleDelta;
		float _actionDuration;

		float _accumulatedAngleDelta;
		float _elapsedDuration;

		public AxisRotateByAction(Transform inTransform, Vector3 inAxis, float inDesiredAngleDelta, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetAxis(inAxis);
			SetDesiredAngleDelta(inDesiredAngleDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public AxisRotateByAction(Transform inTransform, Vector3 inAxis, float inDesiredAngleDelta, float inActionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetAxis(inAxis);
			SetDesiredAngleDelta(inDesiredAngleDelta);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetAxis(Vector3 inNewAxis)
		{
			_vecAxis = inNewAxis;
		}
		public void SetDesiredAngleDelta(float inNewDesiredAngleDelta)
		{
			_desiredAngleDelta = inNewDesiredAngleDelta;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_accumulatedAngleDelta = 0;
			_elapsedDuration = 0f;
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

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			_transform.Rotate(_vecAxis, -_accumulatedAngleDelta, Space.World); // Reverse the previous frame's rotation.

			float t;
			if (_animCurve == null) t = Mathf.Clamp01(_elapsedDuration / _actionDuration);
			else t = _animCurve.Evaluate(_elapsedDuration / _actionDuration);
			_accumulatedAngleDelta = Mathf.LerpUnclamped(0.0f, _desiredAngleDelta, t);

			_transform.Rotate(_vecAxis, _accumulatedAngleDelta, Space.World);  // Apply the new delta rotation.

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				float imperfection = _desiredAngleDelta - _accumulatedAngleDelta;
				_transform.Rotate(_vecAxis, imperfection, Space.World); // Force to exact delta displacement.

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
			_elapsedDuration += _actionDuration;

			if (inSnapToDesired)
			{
				float imperfection = _desiredAngleDelta - _accumulatedAngleDelta;
				_transform.Rotate(_vecAxis, imperfection, Space.World); // Force to exact delta displacement.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
