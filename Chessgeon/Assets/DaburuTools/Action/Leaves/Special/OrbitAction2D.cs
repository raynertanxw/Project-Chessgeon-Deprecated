using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class OrbitAction2D : Action
	{
		Transform _transform;
		Transform _orbitPointTransform;
		Vector3 _orbitAxisDir;
		bool _isClockwise;
		public bool IsClockwise
		{
			get { return _isClockwise; }
			set
			{
				_isClockwise = value;
				if (_isClockwise)
					_orbitAxisDir = -Vector3.forward;
				else
					_orbitAxisDir = Vector3.forward;
			}
		}
		int _numCycles;
		Graph _revolutionGraph;
		float _cycleDuration;
		bool _preventOwnAxisRotation;
		public bool PreventOwnAxisRotation
		{
			get { return _preventOwnAxisRotation; }
			set { _preventOwnAxisRotation = value; }
		}

		float _elapsedDuration;
		int _currentCycle;

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
			_preventOwnAxisRotation = inPreventOwnAxisRotation;
		}

		public void SetOrbitPointTransform(Transform inNewOrbitPointTransform)
		{
			_orbitPointTransform = inNewOrbitPointTransform;
		}
		public void SetNumCycles(int inNewNumCycles)
		{
			_numCycles = inNewNumCycles;
		}
		public void SetRevolutionGraph(Graph inNewRevolutionGraph)
		{
			_revolutionGraph = inNewRevolutionGraph;
		}
		public void SetCycleDuration(float inNewCycleDuration)
		{
			_cycleDuration = inNewCycleDuration;
		}
		private void SetupAction()
		{
			_elapsedDuration = 0f;
			_currentCycle = 0;
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
			float mfCycleElapsedOld = _elapsedDuration - _cycleDuration * _currentCycle;
			float tOld = _revolutionGraph.Read(mfCycleElapsedOld / _cycleDuration);
			_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, -360.0f * tOld);
			// Offset Rotation so that the orbit action does not affect the object's rotation.
			if (PreventOwnAxisRotation)
				_transform.Rotate(_orbitAxisDir, 360.0f * tOld);

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);
			float mfCycleElapsed = _elapsedDuration - _cycleDuration * _currentCycle;
			if (mfCycleElapsed < _cycleDuration)
			{
				float t = _revolutionGraph.Read(mfCycleElapsed / _cycleDuration);
				_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, 360.0f * t);

				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (_preventOwnAxisRotation)
					_transform.Rotate(_orbitAxisDir, 360.0f * -t);
			}
			else
			{
				_currentCycle++;
				// Remove self after action is finished.
				if (_currentCycle >= _numCycles)
				{
					// Force it to be the end position of the cycle.
					_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, 360.0f);
					// Offset Rotation so that the orbit action does not affect the object's rotation.
					if (_preventOwnAxisRotation)
						_transform.Rotate(_orbitAxisDir, -360.0f);
					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Do the interpolation for the beginning of the next cycle.
					float t = _revolutionGraph.Read((mfCycleElapsed - _cycleDuration) / _cycleDuration);
					_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, 360.0f * t);

					// Offset Rotation so that the orbit action does not affect the object's rotation.
					if (_preventOwnAxisRotation)
						_transform.Rotate(_orbitAxisDir, 360.0f * -t);
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
			_currentCycle = _numCycles;

			if (inSnapToDesired)
			{
				// Undo previous frame's rotation.
				float mfCycleElapsedOld = _elapsedDuration - _cycleDuration * _currentCycle;
				float tOld = _revolutionGraph.Read(mfCycleElapsedOld / _cycleDuration);
				_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, -360.0f * tOld);
				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (PreventOwnAxisRotation)
					_transform.Rotate(_orbitAxisDir, 360.0f * tOld);

				// Force it to be the end position of the cycle.
				_transform.RotateAround(_orbitPointTransform.position, _orbitAxisDir, 360.0f);
				// Offset Rotation so that the orbit action does not affect the object's rotation.
				if (_preventOwnAxisRotation)
					_transform.Rotate(_orbitAxisDir, -360.0f);
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
