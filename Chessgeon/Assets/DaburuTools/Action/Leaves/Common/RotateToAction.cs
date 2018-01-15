using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class RotateToAction : Action
	{
		Transform _transform;
		AnimationCurve _animCurve;
		Vector3 _vecDesiredRotation;
		float _actionDuration;

		Vector3 _vecInitialRotation;
		float _elapsedDuration;

		public RotateToAction(Transform inTransform, Vector3 inDesiredRotation, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetDesiredRotation(inDesiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RotateToAction(Transform inTransform, Vector3 inDesiredRotation, float inActionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetDesiredRotation(inDesiredRotation);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredRotation(Vector3 inNewDesiredRotation)
		{
			_vecDesiredRotation = inNewDesiredRotation;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_vecInitialRotation = _transform.eulerAngles;
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

			float t;
			if (_animCurve == null) t = Mathf.Clamp01(_elapsedDuration / _actionDuration);
			else t = _animCurve.Evaluate(_elapsedDuration / _actionDuration);
			_transform.eulerAngles = Vector3.LerpUnclamped(_vecInitialRotation, _vecDesiredRotation, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.eulerAngles = _vecDesiredRotation;   // Force it to be the exact rotation that it wants.
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
				_transform.eulerAngles = _vecDesiredRotation;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
