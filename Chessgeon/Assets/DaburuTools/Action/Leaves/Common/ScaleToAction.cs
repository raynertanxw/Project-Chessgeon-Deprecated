using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class ScaleToAction : Action
	{
		Transform _transform;
		Graph _graph;
		Vector3 _vecDesiredScale;
		float _actionDuration;

		Vector3 _vecInitialScale;
		float _elapsedDuration;

		public ScaleToAction(Transform inTransform, Graph inGraph, Vector3 inDesiredScale, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(inGraph);
			SetDesiredScale(inDesiredScale);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public ScaleToAction(Transform inTransform, Vector3 inDesiredScale, float inActionDuration)
		{
			_transform = inTransform;
			SetGraph(Graph.Linear);
			SetDesiredScale(inDesiredScale);
			SetActionDuration(inActionDuration);

			SetupAction();
		}
		public void SetGraph(Graph inNewGraph)
		{
			_graph = inNewGraph;
		}
		public void SetDesiredScale(Vector3 inNewDesiredScale)
		{
			_vecDesiredScale = inNewDesiredScale;
		}
		public void SetActionDuration(float inNewActionDuration)
		{
			_actionDuration = inNewActionDuration;
		}
		private void SetupAction()
		{
			_vecInitialScale = _transform.localScale;
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

			if (_transform == null)
			{
				// Debug.LogWarning("DaburuTools.Action: _transform Deleted prematurely");
				_parent.Remove(this);
				return;
			}

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);

			float t = _graph.Read(_elapsedDuration / _actionDuration);
			_transform.localScale = Vector3.LerpUnclamped(_vecInitialScale, _vecDesiredScale, t);

			// Remove self after action is finished.
			if (_elapsedDuration >= _actionDuration)
			{
				_transform.localScale = _vecDesiredScale;   // Force it to be the exact scale that it wants.
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
			_elapsedDuration += _actionDuration;

			if (inSnapToDesired)
			{
				_transform.localScale = _vecDesiredScale;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
