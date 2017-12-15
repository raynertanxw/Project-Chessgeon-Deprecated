using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class SpriteRendererAlphaToAction : Action
	{
		SpriteRenderer mSpriteRenderer;
		float mfDesiredAlpha;
		float mfActionDuration;
		Graph mGraph;

		float mfOriginalAlpha;
		float mfElapsedDuration;

		public SpriteRendererAlphaToAction(SpriteRenderer _spriteRenderer, Graph _graph, float _desiredAlpha, float _actionDuration)
		{
			mSpriteRenderer = _spriteRenderer;
			SetGraph(_graph);
			SetDesiredAlpha(_desiredAlpha);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public SpriteRendererAlphaToAction(SpriteRenderer _spriteRenderer, float _desiredAlpha, float _actionDuration)
		{
			mSpriteRenderer = _spriteRenderer;
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
			mfOriginalAlpha = mSpriteRenderer.color.a;
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
			Color newCol = mSpriteRenderer.color;
			newCol.a = mGraph.Read(Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t));
			mSpriteRenderer.color = newCol;

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				// Snap to desired alpha.
				Color finalCol = mSpriteRenderer.color;
				finalCol.a = mfDesiredAlpha;
				mSpriteRenderer.color = finalCol;

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
				Color finalCol = mSpriteRenderer.color;
				finalCol.a = mfDesiredAlpha;
				mSpriteRenderer.color = finalCol;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
