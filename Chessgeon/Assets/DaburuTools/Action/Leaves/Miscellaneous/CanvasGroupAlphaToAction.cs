using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class CanvasGroupAlphaToAction : Action
	{
		CanvasGroup _canvasGroup;
		float _desiredAlpha;
		float _actionDuration;
		AnimationCurve _animCurve;

		float _originalAlpha;
		float _elapsedDuration;

		public CanvasGroupAlphaToAction(CanvasGroup inCanvasGroup, float inDesiredAlpha, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_canvasGroup = inCanvasGroup;
			SetAnimCurve(inAnimCurve);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public CanvasGroupAlphaToAction(CanvasGroup inCanvasGroup, float inDesiredAlpha, float inActionDuration)
		{
			_canvasGroup = inCanvasGroup;
			SetAnimCurve(null);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredAlpha(float inNewDesiredAlpha)
		{
			_desiredAlpha = inNewDesiredAlpha;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_originalAlpha = _canvasGroup.alpha;
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

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t;
			if (_animCurve == null) t = Mathf.Clamp01(_elapsedDuration / _actionDuration);
			else t = _animCurve.Evaluate(_elapsedDuration / _actionDuration);
			_canvasGroup.alpha = Mathf.Lerp(_originalAlpha, _desiredAlpha, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				// Snap to desired alpha.
				_canvasGroup.alpha = _desiredAlpha;

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
			_elapsedDuration = _actionDuration;

			if (inSnapToDesired)
			{
				// Snap to desired alpha.
				_canvasGroup.alpha = _desiredAlpha;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
