using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	public class ImageAlphaToAction : Action
	{
		Image mImage;
		float mfDesiredAlpha;
		float _actionDuration;
		Graph _graph;

		float mfOriginalAlpha;
		float _elapsedDuration;

		public ImageAlphaToAction(Image inImage, Graph inGraph, float inDesiredAlpha, float inActionDuration)
		{
			mImage = inImage;
			SetGraph(inGraph);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public ImageAlphaToAction(Image inImage, float inDesiredAlpha, float inActionDuration)
		{
			mImage = inImage;
			SetGraph(Graph.Linear);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredAlpha(float inNewDesiredAlpha)
		{
			mfDesiredAlpha = inNewDesiredAlpha;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			mfOriginalAlpha = mImage.color.a;
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

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			Color newCol = mImage.color;
			newCol.a = _graph.Read(Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t));
			mImage.color = newCol;

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				// Snap to desired alpha.
				Color finalCol = mImage.color;
				finalCol.a = mfDesiredAlpha;
				mImage.color = finalCol;

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
				Color finalCol = mImage.color;
				finalCol.a = mfDesiredAlpha;
				mImage.color = finalCol;
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
