using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class MoveToAnchoredPosAction : Action
	{
		RectTransform mTransform;
		Graph mGraph;
		Vector2 mvecDesiredAnchoredPos;
		float mfActionDuration;

		Vector2 mvecInitialAnchoredPos;
		float mfElapsedDuration;

		public MoveToAnchoredPosAction(RectTransform _transform, Graph _graph, Vector2 _desiredAnchoredPos, float _actionDuration)
		{
			mTransform = _transform;
			SetGraph(_graph);
			SetDesiredAnchoredPos(_desiredAnchoredPos);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public MoveToAnchoredPosAction(RectTransform _transform, Vector2 _desiredAnchoredPos, float _actionDuration)
		{
			mTransform = _transform;
			SetGraph(Graph.Linear);
			SetDesiredAnchoredPos(_desiredAnchoredPos);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph _newGraph)
		{
			mGraph = _newGraph;
		}
		public void SetDesiredAnchoredPos(Vector2 _newDesiredAnchoredPos)
		{
			mvecDesiredAnchoredPos = _newDesiredAnchoredPos;
		}
		public void SetActionDuration(float _newActionDuration)
		{
			mfActionDuration = _newActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialAnchoredPos = mTransform.anchoredPosition;
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

			if (mTransform == null)
			{
				// Debug.LogWarning("DaburuTools.Action: mTransform Deleted prematurely");
				mParent.Remove(this);
				return;
			}

			mfElapsedDuration += ActionDeltaTime(mbIsUnscaledDeltaTime);

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			mTransform.anchoredPosition = Vector2.LerpUnclamped(mvecInitialAnchoredPos, mvecDesiredAnchoredPos, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				mTransform.anchoredPosition = mvecDesiredAnchoredPos; // Force it to be the exact anchored position that it wants.
				OnActionEnd();
				mParent.Remove(this);
			}
		}
		public override void MakeResettable(bool _bIsResettable)
		{
			base.MakeResettable(_bIsResettable);
		}
		public override void Reset()
		{
			SetupAction();
		}
		public override void StopAction(bool _bSnapToDesired)
		{
			if (!mbIsRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			mfElapsedDuration += mfActionDuration;

			if (_bSnapToDesired)
			{
				mTransform.anchoredPosition = mvecDesiredAnchoredPos; // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			mParent.Remove(this);
		}
	}
}
