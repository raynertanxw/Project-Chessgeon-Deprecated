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
		Graph _graph;

		float _originalAlpha;
		float _elapsedDuration;

		public TextAlphaToAction(Text inText, Graph inGraph, float inDesiredAlpha, float inActionDuration)
		{
			_text = inText;
			SetGraph(inGraph);
			SetDesiredAlpha(inDesiredAlpha);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public TextAlphaToAction(Text inText, float inDesiredAlpha, float inActionDuration)
		{
			_text = inText;
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

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			Color newCol = _text.color;
			newCol.a = _graph.Read(Mathf.Lerp(_originalAlpha, _desiredAlpha, t));
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
