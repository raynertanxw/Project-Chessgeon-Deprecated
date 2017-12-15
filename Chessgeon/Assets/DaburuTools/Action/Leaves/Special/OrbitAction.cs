using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class OrbitAction : Action
	{
		Transform _transform;
		Transform mOrbitPointTransform;
		Vector3 mOrbitAxisDir;
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

		public OrbitAction(
			Transform inTransform, Transform mOrbitPointTransform,
			Vector3 _orbitAxisDir,
			int _numCycles, Graph _revolutionGraph,
			float _cycleDuration,
			bool _preventOwnAxisRotation = true)
		{
			_transform = inTransform;
			SetOrbitPointTransform(mOrbitPointTransform);
			SetOrbitAxisDir(_orbitAxisDir);
			SetNumCycles(_numCycles);
			SetRevolutionGraph(_revolutionGraph);
			SetCycleDuration(_cycleDuration);
			mbPreventOwnAxisRotation = _preventOwnAxisRotation;
		}

		public void SetOrbitPointTransform(Transform _newOrbitPointTransform)
		{
			mOrbitPointTransform = _newOrbitPointTransform;
		}
		public void SetOrbitAxisDir(Vector3 _newOrbitAxisDir)
		{
			mOrbitAxisDir = _newOrbitAxisDir;
		}
		public void SetNumCycles(int _newNumCycles)
		{
			mnNumCycles = _newNumCycles;
		}
		public void SetRevolutionGraph(Graph _newRevolutionGraph)
		{
			mRevolutionGraph = _newRevolutionGraph;
		}
		public void SetCycleDuration(float _newCycleDuration)
		{
			mfCycleDuration = _newCycleDuration;
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
