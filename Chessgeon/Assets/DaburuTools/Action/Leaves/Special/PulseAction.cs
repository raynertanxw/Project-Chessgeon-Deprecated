﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	public class PulseAction : Action
	{
		Transform _transform;
		Vector3 _vecMinScale;
		Vector3 _vecMaxScale;
		int _numCycles;
		AnimationCurve _expandAnimCurve, _shrinkAnimCurve;
		float _expandDuration, _shrinkDuration, _cycleDuration;

		float _elapsedDuration;
		int _currentCycle;

		public PulseAction(
			Transform inTransform, int inNumCycles,
			float inExpandDuration, float inShrinkDuration,
			Vector3 inMinScale, Vector3 inMaxScale,
			AnimationCurve inExpandAnimCurve, AnimationCurve inShrinkAnimCurve)
		{
			_transform = inTransform;
			SetNumCycles(inNumCycles);
			SetExpandShrinkDuration(inExpandDuration, inShrinkDuration);
			SetMinMaxScale(inMinScale, inMaxScale);
			SetExpandShrinkAnimCurves(inExpandAnimCurve, inShrinkAnimCurve);
		}
		public PulseAction(Transform inTransform, int inNumCycles, float inCycleDuration,
			Vector3 inMinScale, Vector3 inMaxScale, AnimationCurve inExpandAnimCurve)
		{
			_transform = inTransform;
			SetNumCycles(inNumCycles);
			SetExpandShrinkAnimCurves(inExpandAnimCurve, inExpandAnimCurve);
			SetExpandShrinkDuration(inCycleDuration / 2.0f, inCycleDuration / 2.0f);
			SetMinMaxScale(inMinScale, inMaxScale);
		}

		public void SetNumCycles(int inNewNumCycles)
		{
			_numCycles = inNewNumCycles;
		}
		public void SetExpandShrinkAnimCurves(AnimationCurve inNewExpandAnimCurve, AnimationCurve inNewShrinkAnimCurve)
		{
			_expandAnimCurve = inNewExpandAnimCurve;
			_shrinkAnimCurve = inNewShrinkAnimCurve;
		}
		public void SetExpandShrinkDuration(float inNewExpandDuration, float inNewShrinkDuration)
		{
			_expandDuration = inNewExpandDuration;
			_shrinkDuration = inNewShrinkDuration;
			_cycleDuration = _expandDuration + _shrinkDuration;
		}
		public void SetMinMaxScale(Vector3 inNewMinScale, Vector3 inNewMaxScale)
		{
			_vecMinScale = inNewMinScale;
			_vecMaxScale = inNewMaxScale;
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

			_elapsedDuration += ActionDeltaTime(_isUnscaledDeltaTime);
			float mfCycleElapsed = _elapsedDuration - _cycleDuration * _currentCycle;
			if (mfCycleElapsed < _expandDuration) // Expand
			{
				float t;
				if (_expandAnimCurve == null) t = Mathf.Clamp01(mfCycleElapsed / _expandDuration);
				else t = _expandAnimCurve.Evaluate(mfCycleElapsed / _expandDuration);
				_transform.localScale = Vector3.LerpUnclamped(_vecMinScale, _vecMaxScale, t);
			}
			else if (mfCycleElapsed < _cycleDuration) // Shrink
			{
				float t;
				if (_shrinkAnimCurve == null) t = Mathf.Clamp01((mfCycleElapsed - _expandDuration) / _shrinkDuration);
				else t = _shrinkAnimCurve.Evaluate((mfCycleElapsed - _expandDuration) / _shrinkDuration);
				_transform.localScale = Vector3.LerpUnclamped(_vecMaxScale, _vecMinScale, t);
			}
			else
			{
				_currentCycle++;
				// Remove self after action is finished.
				if (_currentCycle >= _numCycles)
				{
					_transform.localScale = _vecMinScale;   // Force it to be the exact scale that it wants.
					OnActionEnd();
					_parent.Remove(this);
				}
				else
				{
					// Do the interpolation for the beginning of the next cycle.
					float t = _expandAnimCurve.Evaluate((mfCycleElapsed - _cycleDuration) / _expandDuration);
					_transform.localScale = Vector3.LerpUnclamped(_vecMinScale, _vecMaxScale, t);
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
				_transform.localScale = _vecMinScale;   // Force it to be the exact position that it wants.
			}

			OnActionEnd();
			_parent.Remove(this);
		}
	}
}
