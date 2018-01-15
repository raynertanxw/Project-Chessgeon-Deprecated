﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalRotateToAction2D : Action
	{
		Transform _transform;
		AnimationCurve _animCurve;
		float _desiredLocalZEulerAngle;
		float _actionDuration;

		float _initialLocalZEulerAngle;
		float _elapsedDuration;

		public LocalRotateToAction2D(Transform inTransform, float inDesiredLocalZEulerAngle, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetDesiredLocalZEulerAngle(inDesiredLocalZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalRotateToAction2D(Transform inTransform, float inDesiredLocalZEulerAngle, float inActionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetDesiredLocalZEulerAngle(inDesiredLocalZEulerAngle);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredLocalZEulerAngle(float inNewDesiredLocalZEulerAngle)
		{
			_desiredLocalZEulerAngle = inNewDesiredLocalZEulerAngle;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_initialLocalZEulerAngle = _transform.localEulerAngles.z;
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
			_transform.localEulerAngles = new Vector3(
				_transform.localEulerAngles.x,
				_transform.localEulerAngles.y,
				Mathf.LerpUnclamped(_initialLocalZEulerAngle, _desiredLocalZEulerAngle, t)
			);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					_desiredLocalZEulerAngle
				);  // Force it to be the exact rotation that it wants.
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
				_transform.localEulerAngles = new Vector3(
					_transform.localEulerAngles.x,
					_transform.localEulerAngles.y,
					_desiredLocalZEulerAngle
				);  // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}