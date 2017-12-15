using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class VolumeToAction : Action
	{
		AudioSource mAudioSource;
		float mfDesiredVolume;
		float mfActionDuration;
		Graph mGraph;

		float mfOriginalVolume;
		float mfElapsedDuration;

		public VolumeToAction(AudioSource _audioSource, Graph _graph, float _desiredVolume, float _actionDuration)
		{
			mAudioSource = _audioSource;
			SetGraph(_graph);
			SetDesiredVolume(_desiredVolume);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public VolumeToAction(AudioSource _audioSource, float _desiredVolume, float _actionDuration)
		{
			mAudioSource = _audioSource;
			SetGraph(Graph.Linear);
			SetDesiredVolume(_desiredVolume);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredVolume(float _newDesiredVolume)
		{
			mfDesiredVolume = _newDesiredVolume;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfOriginalVolume = mAudioSource.volume;
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

			mfElapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mAudioSource.volume = Mathf.Lerp(mfOriginalVolume, mfDesiredVolume, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				// Snap volume to desired volume.
				mAudioSource.volume = mfDesiredVolume;

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
			mfElapsedDuration = mfActionDuration;

			if (inSnapToDesired)
			{
				// Snap volume to desired volume.
				mAudioSource.volume = mfDesiredVolume;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
