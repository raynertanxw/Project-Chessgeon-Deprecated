using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveToAnchoredPosAction : Action
	{
		RectTransform _transform;
		AnimationCurve _animCurve;
		Vector2 mvecDesiredAnchoredPos;
		float mfActionDuration;

		Vector2 mvecInitialAnchoredPos;
		float mfElapsedDuration;

		public MoveToAnchoredPosAction(RectTransform inTransform, Vector2 _desiredAnchoredPos, float _actionDuration, AnimationCurve inAnimCurve)
		{
			_transform = inTransform;
			SetAnimCurve(inAnimCurve);
			SetDesiredAnchoredPos(_desiredAnchoredPos);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public MoveToAnchoredPosAction(RectTransform inTransform, Vector2 _desiredAnchoredPos, float _actionDuration)
		{
			_transform = inTransform;
			SetAnimCurve(null);
			SetDesiredAnchoredPos(_desiredAnchoredPos);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inAnimCurve)
		{
			_animCurve = inAnimCurve;
		}
		public void SetDesiredAnchoredPos(Vector2 _newDesiredAnchoredPos)
		{
			mvecDesiredAnchoredPos = _newDesiredAnchoredPos;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialAnchoredPos = _transform.anchoredPosition;
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

			float t;
			if (_animCurve == null) t = Mathf.Clamp01(mfElapsedDuration / mfActionDuration);
			else t = _animCurve.Evaluate(mfElapsedDuration / mfActionDuration);
			_transform.anchoredPosition = Vector2.LerpUnclamped(mvecInitialAnchoredPos, mvecDesiredAnchoredPos, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.anchoredPosition = mvecDesiredAnchoredPos; // Force it to be the exact anchored position that it wants.
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
				_transform.anchoredPosition = mvecDesiredAnchoredPos; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
