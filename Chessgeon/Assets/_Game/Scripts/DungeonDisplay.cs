using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class DungeonDisplay : MonoBehaviour
{
	private static DungeonDisplay _instance = null;

	[SerializeField] private Dungeon _dungeon = null;

	[Header("Canvas UI Elements")]
	[SerializeField] private Image _darkOverlay = null;
	[SerializeField] private Image _phaseBannerTop = null;
	[SerializeField] private Image _phaseBannerBtm = null;

	[Header("Sprite Assets")]
	[SerializeField] private Sprite _phaseBannerPlayerTop = null;
	[SerializeField] private Sprite _phaseBannerPlayerBtm = null;
	[SerializeField] private Sprite _phaseBannerEnemyTop = null;
	[SerializeField] private Sprite _phaseBannerEnemyBtm = null;

	Graph InverseSmoothStep;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

			Debug.Assert(_darkOverlay != null, "_darkOverlay is not assigned.");
			Debug.Assert(_phaseBannerTop != null, "_phaseBannerTop is not assigned.");
			Debug.Assert(_phaseBannerBtm != null, "_phaseBannerBtm is not assigned.");

			Debug.Assert(
				_phaseBannerPlayerTop != null
				&& _phaseBannerPlayerBtm != null
				&& _phaseBannerEnemyTop != null
				&& _phaseBannerEnemyBtm != null,
				"One or more Phase Banner sprites is not assigned.");

			SetDarkOverlayVisible(false);

			InverseSmoothStep = new Graph((float _x) =>
			{
				return Mathf.Lerp(
					1f - ((1f - _x) * (1f - _x)),
					_x * _x,
					_x);
			});
		}
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void SetDarkOverlayVisible(bool inIsVisible)
	{
		_darkOverlay.enabled = inIsVisible;
	}

	private void SetDarkOverlayAlpha(float inAlpha)
	{
		if (_darkOverlay.color.a != inAlpha)
		{
			Color newCol = _darkOverlay.color;
			newCol.a = inAlpha;
			_darkOverlay.color = newCol;
		}
	}

	private bool _phaseAnimPlaying = false;
	public static void PlayPhaseAnimation(bool inIsPlayersTurn)
	{
		if (_instance._phaseAnimPlaying)
		{
			Debug.LogWarning("Trying to call PlayPhaseAnimation when its previous call has not finished animating.");
		}
		else
		{
			_instance._phaseAnimPlaying = true;

			_instance.SetDarkOverlayAlpha(0.0f);
			_instance.SetDarkOverlayVisible(true);

			if (inIsPlayersTurn)
			{
				_instance._phaseBannerTop.sprite = _instance._phaseBannerPlayerTop;
				_instance._phaseBannerBtm.sprite = _instance._phaseBannerPlayerBtm;
			}
			else
			{
				_instance._phaseBannerTop.sprite = _instance._phaseBannerEnemyTop;
				_instance._phaseBannerBtm.sprite = _instance._phaseBannerEnemyBtm;
			}

			_instance._phaseBannerTop.rectTransform.localEulerAngles = Vector3.forward * 90.0f;
			_instance._phaseBannerBtm.rectTransform.localEulerAngles = Vector3.forward * 90.0f;

			RotateByAction2D topRotIn = new RotateByAction2D(_instance._phaseBannerTop.transform, Graph.InverseExponential, -86.0f, 0.6f);
			RotateByAction2D bottomRotIn = new RotateByAction2D(_instance._phaseBannerBtm.transform, Graph.InverseExponential, -86.0f, 0.6f);
			ActionParallel rotateIn = new ActionParallel(topRotIn, bottomRotIn);
			rotateIn.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			DelayAction rotInOutDelay = new DelayAction(0.6f);

			RotateByAction2D topRotOut = new RotateByAction2D(_instance._phaseBannerTop.transform, Graph.Exponential, -86.0f, 0.6f);
			RotateByAction2D bottomRotOut = new RotateByAction2D(_instance._phaseBannerBtm.transform, Graph.Exponential, -86.0f, 0.6f);
			ActionParallel rotateOut = new ActionParallel(topRotOut, bottomRotOut);
			rotateOut.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			ActionSequence rotInOutSeq = new ActionSequence(rotateIn, rotInOutDelay, rotateOut);
			rotInOutSeq.OnActionFinish += () => {
				_instance.SetDarkOverlayVisible(false);
				_instance._phaseAnimPlaying = false;
			};

			DelayAction stallFrontDelay = new DelayAction(0.5f);
			DelayAction stallEndDelay = new DelayAction(0.5f);

			RotateByAction2D topRotStall = new RotateByAction2D(_instance._phaseBannerTop.transform, _instance.InverseSmoothStep, -8.0f, 0.8f);
			RotateByAction2D bottomRotStall = new RotateByAction2D(_instance._phaseBannerBtm.transform, _instance.InverseSmoothStep, -8.0f, 0.8f);
			ActionParallel rotateStall = new ActionParallel(topRotStall, bottomRotStall);

			ActionSequence rotStallSeq = new ActionSequence(stallFrontDelay, rotateStall, stallEndDelay);


			DelayAction alphaDelay = new DelayAction(0.8f);
			ImageAlphaToAction alphaIn = new ImageAlphaToAction(_instance._darkOverlay, Graph.Linear, 0.5f, 0.5f);
			ImageAlphaToAction alphaOut = new ImageAlphaToAction(_instance._darkOverlay, Graph.Linear, 0.0f, 0.5f);
			ActionSequence alphaFadeSeq = new ActionSequence(alphaIn, alphaDelay, alphaOut);

			ActionHandler.RunAction(rotInOutSeq, rotStallSeq, alphaFadeSeq);
		}
	}
}
