using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class VolumeToAction : Action
	{
		AudioSource _audioSource;
		float _desiredVolume;
		float _actionDuration;
		AnimationCurve _animCurve;

		float _originalVolume;
		float _elapsedDuration;

		public VolumeToAction(AudioSource inAudioSource, float inDesiredVolume, float inActionDuration, AnimationCurve inAnimCurve)
		{
			_audioSource = inAudioSource;
			SetAnimCurve(inAnimCurve);
			SetDesiredVolume(inDesiredVolume);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public VolumeToAction(AudioSource inAudioSource, float inDesiredVolume, float inActionDuration)
		{
			_audioSource = inAudioSource;
			SetAnimCurve(null);
			SetDesiredVolume(inDesiredVolume);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetAnimCurve(AnimationCurve inNewAnimCurve)
		{
			_animCurve = inNewAnimCurve;
		}
		public void SetDesiredVolume(float inNewDesiredVolume)
		{
			_desiredVolume = inNewDesiredVolume;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_originalVolume = _audioSource.volume;
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
			_audioSource.volume = Mathf.Lerp(_originalVolume, _desiredVolume, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				// Snap volume to desired volume.
				_audioSource.volume = _desiredVolume;

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
				// Snap volume to desired volume.
				_audioSource.volume = _desiredVolume;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
