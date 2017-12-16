using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class PulseAction : Action
	{
		Transform _transform;
		Vector3 mvecMinScale;
		Vector3 mvecMaxScale;
		int mnNumCycles;
		Graph mExpandGraph, mShrinkGraph;
		float mfExpandDuration, mfShrinkDuration, mfCycleDuration;

		float mfElapsedDuration;
		int mnCurrentCycle;

		public PulseAction(
			Transform inTransform, int inNumCycles,
			Graph inExpandGraph, Graph inShrinkGraph,
			float inExpandDuration, float inShrinkDuration,
			Vector3 inMinScale, Vector3 inMaxScale)
		{
			_transform = inTransform;
			SetNumCycles(inNumCycles);
			SetExpandShrinkGraphs(inExpandGraph, inShrinkGraph);
			SetExpandShrinkDuration(inExpandDuration, inShrinkDuration);
			SetMinMaxScale(inMinScale, inMaxScale);
		}
		public PulseAction(Transform inTransform, int inNumCycles, Graph inExpandShrinkGraph, float inCycleDuration,
			Vector3 inMinScale, Vector3 inMaxScale)
		{
			_transform = inTransform;
			SetNumCycles(inNumCycles);
			SetExpandShrinkGraphs(inExpandShrinkGraph, inExpandShrinkGraph);
			SetExpandShrinkDuration(inCycleDuration / 2.0f, inCycleDuration / 2.0f);
			SetMinMaxScale(inMinScale, inMaxScale);
		}

		public void SetNumCycles(int inNewNumCycles)
		{
			mnNumCycles = inNewNumCycles;
		}
		public void SetExpandShrinkGraphs(Graph inNewExpandGraph, Graph inNewShrinkGraph)
		{
			mExpandGraph = inNewExpandGraph;
			mShrinkGraph = inNewShrinkGraph;
		}
		public void SetExpandShrinkDuration(float inNewExpandDuration, float inNewShrinkDuration)
		{
			mfExpandDuration = inNewExpandDuration;
			mfShrinkDuration = inNewShrinkDuration;
			mfCycleDuration = mfExpandDuration + mfShrinkDuration;
		}
		public void SetMinMaxScale(Vector3 inNewMinScale, Vector3 inNewMaxScale)
		{
			mvecMinScale = inNewMinScale;
			mvecMaxScale = inNewMaxScale;
		}
		private void SetupAction()
		{
			mfElapsedDuration = 0f;
			mnCurrentCycle = 0;
		}
		protected override void OnActionBegin()
		{
			base.OnActionBegin();

			SetupAction();
		}


		// Currently only expands then shrinks. Ending with shrink.
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
			float mfCycleElapsed = mfElapsedDuration - mfCycleDuration * mnCurrentCycle;
			if (mfCycleElapsed < mfExpandDuration) // Expand
			{
				float t = mExpandGraph.Read(mfCycleElapsed / mfExpandDuration);
				_transform.localScale = Vector3.LerpUnclamped(mvecMinScale, mvecMaxScale, t);
			}
			else if (mfCycleElapsed < mfCycleDuration) // Shrink
			{
				float t = mShrinkGraph.Read((mfCycleElapsed - mfExpandDuration) / mfShrinkDuration);
				_transform.localScale = Vector3.LerpUnclamped(mvecMaxScale, mvecMinScale, t);
			}
			else
			{
				mnCurrentCycle++;
				// Remove self after action is finished.
				if (mnCurrentCycle >= mnNumCycles)
				{
					_transform.localScale = mvecMinScale;   // Force it to be the exact scale that it wants.
					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Do the interpolation for the beginning of the next cycle.
					float t = mExpandGraph.Read((mfCycleElapsed - mfCycleDuration) / mfExpandDuration);
					_transform.localScale = Vector3.LerpUnclamped(mvecMinScale, mvecMaxScale, t);
				}
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
			mnCurrentCycle = mnNumCycles;

			if (inSnapToDesired)
			{
				_transform.localScale = mvecMinScale;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
