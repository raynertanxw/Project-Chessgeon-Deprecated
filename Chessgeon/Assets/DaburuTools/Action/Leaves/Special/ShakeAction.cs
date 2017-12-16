﻿using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class ShakeAction : Action
	{
		Transform _transform;
		int _numShakes;
		float _shakePeriod;
		float _shakeIntensity;
		Graph _attenuationGraph;

		Vector3 _vecDeltaPos;
		float _elapsedDuration;
		int _currentCycle;

		public ShakeAction(Transform inTransform, int inNumShakes, float inShakeIntensity)
		{
			_transform = inTransform;

			SetNumShakes(inNumShakes);
			SetShakeIntensity(inShakeIntensity);
			SetAttenuationGraph(Graph.One);

			SetShakePeriod(0.05f);
		}
		public ShakeAction(Transform inTransform, int inNumShakes, float inShakeIntensity, Graph inAttenuationGraph)
		{
			_transform = inTransform;

			SetNumShakes(inNumShakes);
			SetShakeIntensity(inShakeIntensity);
			SetAttenuationGraph(inAttenuationGraph);

			SetShakePeriod(0.05f);
		}

		public void SetNumShakes(int inNewNumShakes)
		{
			_numShakes = inNewNumShakes;
		}
		public void SetShakeIntensity(float inNewShakeIntensity)
		{
			_shakeIntensity = inNewShakeIntensity;
		}
		public void SetAttenuationGraph(Graph inNewAttenuationGraph)
		{
			_attenuationGraph = inNewAttenuationGraph;
		}
		public void SetShakeFrequency(float inNewShakeFrequency)
		{
			SetShakePeriod(1.0f / inNewShakeFrequency);
		}
		public void SetShakeByDuration(float inNewShakeDuration, int inNewNumShakes)
		{
			SetNumShakes(inNewNumShakes);
			SetShakePeriod(inNewShakeDuration / inNewNumShakes);
		}
		private void SetShakePeriod(float inNewShakePeriod)
		{
			_shakePeriod = inNewShakePeriod;
		}
		private void SetupAction()
		{
			_elapsedDuration = 0f;
			_currentCycle = 0;
			_vecDeltaPos = Vector3.zero;
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

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);
			float mfCycleElapsed = _elapsedDuration - _shakePeriod * _currentCycle;
			if (mfCycleElapsed > _shakePeriod)
			{
				_currentCycle = (int)(_elapsedDuration / _shakePeriod);

				// Remove self after action is finished.
				if (_currentCycle >= _numShakes)
				{
					// Force it back to original position.
					_transform.position -= _vecDeltaPos;

					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Set back to original position.
					_transform.position -= _vecDeltaPos;
					// Set new shake pos.
					float t = _attenuationGraph.Read(_elapsedDuration / (_shakePeriod * _numShakes));
					_vecDeltaPos = Random.insideUnitSphere * _shakeIntensity * t;
					_transform.position += _vecDeltaPos;
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
			_currentCycle = _numShakes;

			if (inSnapToDesired)
			{
				_transform.position -= _vecDeltaPos;    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
