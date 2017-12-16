using UnityEngine;
using System.Collections;

namespace DaburuTools
{
	public class ShakeAction : Action
	{
		Transform _transform;
		int mnNumShakes;
		float mfShakePeriod;
		float mfShakeIntensity;
		Graph mAttenuationGraph;

		Vector3 mVecDeltaPos;
		float _elapsedDuration;
		int mnCurrentCycle;

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
			mnNumShakes = inNewNumShakes;
		}
		public void SetShakeIntensity(float inNewShakeIntensity)
		{
			mfShakeIntensity = inNewShakeIntensity;
		}
		public void SetAttenuationGraph(Graph inNewAttenuationGraph)
		{
			mAttenuationGraph = inNewAttenuationGraph;
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
			mfShakePeriod = inNewShakePeriod;
		}
		private void SetupAction()
		{
			_elapsedDuration = 0f;
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

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);
			float mfCycleElapsed = _elapsedDuration - mfShakePeriod * mnCurrentCycle;
			if (mfCycleElapsed > mfShakePeriod)
			{
				mnCurrentCycle = (int)(_elapsedDuration / mfShakePeriod);

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
					float t = mAttenuationGraph.Read(_elapsedDuration / (mfShakePeriod * mnNumShakes));
					mVecDeltaPos = Random.insideUnitSphere * mfShakeIntensity * t;
					_transform.position += mVecDeltaPos;
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
			mnCurrentCycle = mnNumShakes;

			if (inSnapToDesired)
			{
				_transform.position -= mVecDeltaPos;    // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
