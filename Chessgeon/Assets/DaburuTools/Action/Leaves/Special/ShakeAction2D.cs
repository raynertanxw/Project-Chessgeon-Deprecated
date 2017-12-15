using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class ShakeAction2D : Action
	{
		Transform _transform;
		int mnNumShakes;
		float mfShakePeriod;
		float mfShakeIntensity;
		Graph mAttenuationGraph;

		Vector3 mVecDeltaPos;
		float mfElapsedDuration;
		int mnCurrentCycle;

		public ShakeAction2D(Transform inTransform, int _numShakes, float _shakeIntensity)
		{
			_transform = inTransform;

			SetNumShakes(_numShakes);
			SetShakeIntensity(_shakeIntensity);
			SetAttenuationGraph(Graph.One);

			SetShakePeriod(0.05f);
		}
		public ShakeAction2D(Transform inTransform, int _numShakes, float _shakeIntensity, Graph _attenuationGraph)
		{
			_transform = inTransform;

			SetNumShakes(_numShakes);
			SetShakeIntensity(_shakeIntensity);
			SetAttenuationGraph(_attenuationGraph);

			SetShakePeriod(0.05f);
		}

		public void SetNumShakes(int _newNumShakes)
		{
			mnNumShakes = _newNumShakes;
		}
		public void SetShakeIntensity(float _newShakeIntensity)
		{
			mfShakeIntensity = _newShakeIntensity;
		}
		public void SetAttenuationGraph(Graph _newAttenuationGraph)
		{
			mAttenuationGraph = _newAttenuationGraph;
		}
		public void SetShakeFrequency(float _newShakeFrequency)
		{
			SetShakePeriod(1.0f / _newShakeFrequency);
		}
		public void SetShakeByDuration(float _newShakeDuration, int _newNumShakes)
		{
			SetNumShakes(_newNumShakes);
			SetShakePeriod(_newShakeDuration / _newNumShakes);
		}
		private void SetShakePeriod(float _newShakePeriod)
		{
			mfShakePeriod = _newShakePeriod;
		}
		private void SetupAction()
		{
			mfElapsedDuration = 0f;
			mnCurrentCycle = 0;
			mVecDeltaPos = Vector3.zero;
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
			float mfCycleElapsed = mfElapsedDuration - mfShakePeriod * mnCurrentCycle;
			if (mfCycleElapsed > mfShakePeriod)
			{
				mnCurrentCycle = (int)(mfElapsedDuration / mfShakePeriod);

				// Remove self after action is finished.
				if (mnCurrentCycle >= mnNumShakes)
				{
					// Force it back to original position.
					_transform.position -= mVecDeltaPos;

					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Set back to original position.
					_transform.position -= mVecDeltaPos;
					// Set new shake pos.
					float t = mAttenuationGraph.Read(mfElapsedDuration / (mfShakePeriod * mnNumShakes));
					mVecDeltaPos = Random.insideUnitCircle * mfShakeIntensity * t;
					_transform.position += mVecDeltaPos;
				}
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
			mnCurrentCycle = mnNumShakes;

			if (_bSnapToDesired)
			{
				_transform.position -= mVecDeltaPos;    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
