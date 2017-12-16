using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class OrbitAction2D : Action
	{
		Transform _transform;
		Transform mOrbitPointTransform;
		Vector3 mOrbitAxisDir;
		bool mbIsClockwise;
		public bool IsClockwise
		{
			get { return mbIsClockwise; }
			set
			{
				mbIsClockwise = value;
				if (mbIsClockwise)
					mOrbitAxisDir = -Vector3.forward;
				else
					mOrbitAxisDir = Vector3.forward;
			}
		}
		int mnNumCycles;
		Graph mRevolutionGraph;
		float mfCycleDuration;
		bool mbPreventOwnAxisRotation;
		public bool PreventOwnAxisRotation
		{
			get { return mbPreventOwnAxisRotation; }
			set { mbPreventOwnAxisRotation = value; }
		}

		float mfElapsedDuration;
		int mnCurrentCycle;

		public OrbitAction2D(
			Transform inTransform, Transform mOrbitPointTransform,
			bool inIsClockwise,
			int inNumCycles, Graph inRevolutionGraph,
			float inCycleDuration,
			bool inPreventOwnAxisRotation = true)
		{
			_transform = inTransform;
			SetOrbitPointTransform(mOrbitPointTransform);
			IsClockwise = inIsClockwise;
			SetNumCycles(inNumCycles);
			SetRevolutionGraph(inRevolutionGraph);
			SetCycleDuration(inCycleDuration);
			mbPreventOwnAxisRotation = inPreventOwnAxisRotation;
		}

		public void SetOrbitPointTransform(Transform inNewOrbitPointTransform)
		{
			mOrbitPointTransform = inNewOrbitPointTransform;
		}
		public void SetNumCycles(int inNewNumCycles)
		{
			mnNumCycles = inNewNumCycles;
		}
		public void SetRevolutionGraph(Graph inNewRevolutionGraph)
		{
			mRevolutionGraph = inNewRevolutionGraph;
		}
		public void SetCycleDuration(float inNewCycleDuration)
		{
			mfCycleDuration = inNewCycleDuration;
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

			// Undo previous frame's rotation.
			float mfCycleElapsedOld = mfElapsedDuration - mfCycleDuration * mnCurrentCycle;
			float tOld = mRevolutionGraph.Read(mfCycleElapsedOld / mfCycleDuration);
			_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, -360.0f * tOld);
			// Offset Rotation so that the orbit action does not affect the object's rotation.
			if (PreventOwnAxisRotation)
				_transform.Rotate(mOrbitAxisDir, 360.0f * tOld);

			mfElapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);
			float mfCycleElapsed = mfElapsedDuration - mfCycleDuration * mnCurrentCycle;
			if (mfCycleElapsed < mfCycleDuration)
			{
				float t = mRevolutionGraph.Read(mfCycleElapsed / mfCycleDuration);
				_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, 360.0f * t);

				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (mbPreventOwnAxisRotation)
					_transform.Rotate(mOrbitAxisDir, 360.0f * -t);
			}
			else
			{
				mnCurrentCycle++;
				// Remove self after action is finished.
				if (mnCurrentCycle >= mnNumCycles)
				{
					// Force it to be the end position of the cycle.
					_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, 360.0f);
					// Offset Rotation so that the orbit action does not affect the object's rotation.
					if (mbPreventOwnAxisRotation)
						_transform.Rotate(mOrbitAxisDir, -360.0f);
					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Do the interpolation for the beginning of the next cycle.
					float t = mRevolutionGraph.Read((mfCycleElapsed - mfCycleDuration) / mfCycleDuration);
					_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, 360.0f * t);

					// Offset Rotation so that the orbit action does not affect the object's rotation.
					if (mbPreventOwnAxisRotation)
						_transform.Rotate(mOrbitAxisDir, 360.0f * -t);
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
				// Undo previous frame's rotation.
				float mfCycleElapsedOld = mfElapsedDuration - mfCycleDuration * mnCurrentCycle;
				float tOld = mRevolutionGraph.Read(mfCycleElapsedOld / mfCycleDuration);
				_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, -360.0f * tOld);
				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (PreventOwnAxisRotation)
					_transform.Rotate(mOrbitAxisDir, 360.0f * tOld);

				// Force it to be the end position of the cycle.
				_transform.RotateAround(mOrbitPointTransform.position, mOrbitAxisDir, 360.0f);
				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (mbPreventOwnAxisRotation)
					_transform.Rotate(mOrbitAxisDir, -360.0f);
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
