using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class RawImageAlphaToAction : Action
	{
		// 1: Declare your other variables such as Transforms, Graphs, etc.
		RawImage mRawImage;
		Graph mGraph;
		float mfDesiredAlpha;
		float mfActionDuration;

		float mfOriginalAlpha;
		float mfElapsedDuration;

		public RawImageAlphaToAction(RawImage inRawImage, Graph inGraph, float inDesiredAlpha, float inActionDuration)
		{
			mRawImage = inRawImage;
			SetGraph(inGraph);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public RawImageAlphaToAction(RawImage inRawImage, float inDesiredAlpha, float inActionDuration)
		{
			mRawImage = inRawImage;
			SetGraph(Graph.Linear);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredAlpha(float inNewDesiredAlpha)
		{
			mfDesiredAlpha = inNewDesiredAlpha;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfOriginalAlpha = mRawImage.color.a;
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
			Color newCol = mRawImage.color;
			newCol.a = mGraph.Read(Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t));
			mRawImage.color = newCol;

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				// Snap to desired alpha.
				Color finalCol = mRawImage.color;
				finalCol.a = mfDesiredAlpha;
				mRawImage.color = finalCol;

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
				// Snap to desired alpha.
				Color finalCol = mRawImage.color;
				finalCol.a = mfDesiredAlpha;
				mRawImage.color = finalCol;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
