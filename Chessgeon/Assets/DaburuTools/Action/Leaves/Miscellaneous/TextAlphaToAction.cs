using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class TextAlphaToAction : Action
	{
		Text _text;
		float _desiredAlpha;
		float _actionDuration;
		AnimationCurve _animCurve;

		float _originalAlpha;
		float _elapsedDuration;

		public TextAlphaToAction(Text inText, float inDesiredAlpha, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_text = inText;
			SetAnimCurve(inAnimCurve);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public TextAlphaToAction(Text inText, float inDesiredAlpha, float inActionDuration)
		{
			_text = inText;
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
			_originalAlpha = _text.color.a;
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
			Color newCol = _text.color;
			newCol.a = Mathf.Lerp(_originalAlpha, _desiredAlpha, t);
			_text.color = newCol;

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				// Snap to desired alpha.
				Color finalCol = _text.color;
				finalCol.a = _desiredAlpha;
				_text.color = finalCol;

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
				Color finalCol = _text.color;
				finalCol.a = _desiredAlpha;
				_text.color = finalCol;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
