﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveToAnchoredPosAction : Action
	{
		RectTransform _transform;
		AnimationCurve _animCurve;
		Vector2 _vecDesiredAnchoredPos;
		float _actionDuration;

		Vector2 _vecInitialAnchoredPos;
		float _elapsedDuration;

		public MoveToAnchoredPosAction(RectTransform inTransform, Vector2 inDesiredAnchoredPos, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetDesiredAnchoredPos(inDesiredAnchoredPos);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public MoveToAnchoredPosAction(RectTransform inTransform, Vector2 inDesiredAnchoredPos, float inActionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetDesiredAnchoredPos(inDesiredAnchoredPos);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredAnchoredPos(Vector2 inNewDesiredAnchoredPos)
		{
			_vecDesiredAnchoredPos = inNewDesiredAnchoredPos;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_vecInitialAnchoredPos = _transform.anchoredPosition;
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
			_transform.anchoredPosition = Vector2.LerpUnclamped(_vecInitialAnchoredPos, _vecDesiredAnchoredPos, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.anchoredPosition = _vecDesiredAnchoredPos; // Force it to be the exact anchored position that it wants.
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
				_transform.anchoredPosition = _vecDesiredAnchoredPos; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
