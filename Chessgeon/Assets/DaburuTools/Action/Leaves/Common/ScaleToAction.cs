﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class ScaleToAction : Action
	{
		Transform _transform;
		Graph mGraph;
		Vector3 mvecDesiredScale;
		float mfActionDuration;

		Vector3 mvecInitialScale;
		float mfElapsedDuration;

		public ScaleToAction(Transform inTransform, Graph _graph, Vector3 _desiredScale, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(_graph);
			SetDesiredScale(_desiredScale);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public ScaleToAction(Transform inTransform, Vector3 _desiredScale, float _actionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredScale(_desiredScale);
			SetActionDuration(_actionDuration);

			SetupAction();
		}
		public void SetGraph(Graph _newGraph)
		{
			mGraph = _newGraph;
		}
		public void SetDesiredScale(Vector3 _newDesiredScale)
		{
			mvecDesiredScale = _newDesiredScale;
		}
		public void SetActionDuration(float _newActionDuration)
		{
			mfActionDuration = _newActionDuration;
		}
		private void SetupAction()
		{
			mvecInitialScale = _transform.localScale;
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

			if (_transform == null)
			{
				// Debug.LogWarning("DaburuTools.Action: _transform Deleted prematurely");
				_parent.Remove(this);
				return;
			}

			mfElapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t = mGraph.Read(mfElapsedDuration / mfActionDuration);
			_transform.localScale = Vector3.LerpUnclamped(mvecInitialScale, mvecDesiredScale, t);

			// Remove self after action is finished.
			if (mfElapsedDuration >= mfActionDuration)
			{
				_transform.localScale = mvecDesiredScale;   // Force it to be the exact scale that it wants.
				OnActionEnd();
				_parent.Remove(this);
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
			if (!_isRunning)
				return;

			// Prevent it from Resetting.
			MakeResettable(false);

			// Simulate the action has ended. Does not really matter by how much.
			mfElapsedDuration += mfActionDuration;

			if (_bSnapToDesired)
			{
				_transform.localScale = mvecDesiredScale;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
