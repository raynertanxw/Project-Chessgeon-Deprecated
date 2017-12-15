using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class DelayAction : Action
	{
		float mfActionDuration;

		float mfElapsedDuration;

		public DelayAction()
		{
			SetAction(0f);

			SetupAction();
		}
		public DelayAction(float inActionDuration)
		{
			SetAction(inActionDuration);

			SetupAction();
		}

		public void SetAction(float inActionDuration)
		{
			mfActionDuration = inActionDuration;
		}
		private void SetupAction()
		{
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

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
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

			// No need for snap to desired, true same effect as false.
			// Only delays time, will simply run the next Action either ways.

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
