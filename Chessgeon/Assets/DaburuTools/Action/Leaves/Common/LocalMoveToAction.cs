using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class LocalMoveToAction : Action
	{
		Transform _transform;
		AnimationCurve _animCurve;
		Vector3 _vecDesiredLocalPos;
		float _actionDuration;

		Vector3 _vecInitialLocalPos;
		float _elapsedDuration;

		public LocalMoveToAction(Transform inTransform, Vector3 inDesiredLocalPosition, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetDesiredLocalPosition(inDesiredLocalPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public LocalMoveToAction(Transform inTransform, Vector3 inDesiredLocalPosition, float inActionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetDesiredLocalPosition(inDesiredLocalPosition);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredLocalPosition(Vector3 inNewDesiredLocalPosition)
		{
			_vecDesiredLocalPos = inNewDesiredLocalPosition;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_vecInitialLocalPos = _transform.localPosition;
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
			_transform.localPosition = Vector3.LerpUnclamped(_vecInitialLocalPos, _vecDesiredLocalPos, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.localPosition = _vecDesiredLocalPos; // Force it to be the exact local position that it wants.
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
				_transform.localPosition = _vecDesiredLocalPos; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
