using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class CanvasGroupAlphaToAction : Action
	{
		CanvasGroup mCanvasGroup;
		float mfDesiredAlpha;
		float mfActionDuration;
		Graph mGraph;

		float mfOriginalAlpha;
		float mfElapsedDuration;

		public CanvasGroupAlphaToAction(CanvasGroup _canvasGroup, Graph _graph, float _desiredAlpha, float _actionDuration)
		{
			mCanvasGroup = _canvasGroup;
			SetGraph(_graph);
			SetDesiredAlpha(_desiredAlpha);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public CanvasGroupAlphaToAction(CanvasGroup _canvasGroup, float _desiredAlpha, float _actionDuration)
		{
			mCanvasGroup = _canvasGroup;
			SetGraph(Graph.Linear);
			SetDesiredAlpha(_desiredAlpha);
			SetActionDuration(_actionDuration);

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
			mfOriginalAlpha = mCanvasGroup.alpha;
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
			mCanvasGroup.alpha = mGraph.Read(Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t));

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				// Snap to desired alpha.
				mCanvasGroup.alpha = mfDesiredAlpha;

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
				mCanvasGroup.alpha = mfDesiredAlpha;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
