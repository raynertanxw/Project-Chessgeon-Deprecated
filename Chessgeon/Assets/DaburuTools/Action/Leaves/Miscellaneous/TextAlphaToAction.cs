using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class TextAlphaToAction : Action
	{
		Text mText;
		float mfDesiredAlpha;
		float mfActionDuration;
		Graph mGraph;

		float mfOriginalAlpha;
		float mfElapsedDuration;

		public TextAlphaToAction(Text _text, Graph inGraph, float _desiredAlpha, float inActionDuration)
		{
			mText = _text;
			SetGraph(inGraph);
			SetDesiredAlpha(_desiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public TextAlphaToAction(Text _text, float _desiredAlpha, float inActionDuration)
		{
			mText = _text;
			SetGraph(Graph.Linear);
			SetDesiredAlpha(_desiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			mGraph = inNewGraph;
		}
		public void SetDesiredAlpha(float _newDesiredAlpha)
		{
			mfDesiredAlpha = _newDesiredAlpha;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			mfActionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfOriginalAlpha = mText.color.a;
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
			Color newCol = mText.color;
			newCol.a = mGraph.Read(Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t));
			mText.color = newCol;

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				// Snap to desired alpha.
				Color finalCol = mText.color;
				finalCol.a = mfDesiredAlpha;
				mText.color = finalCol;

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
				Color finalCol = mText.color;
				finalCol.a = mfDesiredAlpha;
				mText.color = finalCol;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
