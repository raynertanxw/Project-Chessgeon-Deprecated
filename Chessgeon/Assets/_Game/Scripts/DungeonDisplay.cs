﻿using System.Collections;
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
	[SerializeField] private RectTransform _phaseBannerTop = null;
	[SerializeField] private RectTransform _phaseBannerBtm = null;
	[SerializeField] private Text _phaseBannerTextTop = null;
	[SerializeField] private Text _phaseBannerTextBtm = null;

	[Header("Animation Curves")]
	[SerializeField] private AnimationCurve _inverseSmoothStep = null;
	[SerializeField] private AnimationCurve _exponential = null;
	[SerializeField] private AnimationCurve _inverseExponential = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");
			Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

			Debug.Assert(_darkOverlay != null, "_darkOverlay is not assigned.");
			Debug.Assert(_phaseBannerTop != null, "_phaseBannerTop is not assigned.");
			Debug.Assert(_phaseBannerBtm != null, "_phaseBannerBtm is not assigned.");
			Debug.Assert(_phaseBannerTextTop != null, "_phaseBannerTextTop is not assigned.");
			Debug.Assert(_phaseBannerTextBtm != null, "_phaseBannerTextBtm is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			SetDarkOverlayVisible(false);
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
	public static void PlayPhaseAnimation(bool inIsPlayersTurn, OnJobComplete inOnComplete = null)
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
				const string txtPlayersTurn = "Player's Turn";
				_instance._phaseBannerTextTop.text = txtPlayersTurn;
				_instance._phaseBannerTextBtm.text = txtPlayersTurn;
			}
			else
			{
				const string txtEnemysTurn = "Enemy's Turn";
				_instance._phaseBannerTextTop.text = txtEnemysTurn;
				_instance._phaseBannerTextBtm.text = txtEnemysTurn;
			}

			_instance._phaseBannerTop.localEulerAngles = Vector3.forward * -90.0f;
			_instance._phaseBannerBtm.localEulerAngles = Vector3.forward * 90.0f;

			LocalRotateByAction2D topRotIn = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -86.0f, 0.6f, _instance._inverseExponential);
			LocalRotateByAction2D bottomRotIn = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -86.0f, 0.6f, _instance._inverseExponential);
			ActionParallel rotateIn = new ActionParallel(topRotIn, bottomRotIn);
			rotateIn.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			DelayAction rotInOutDelay = new DelayAction(0.6f);

			LocalRotateByAction2D topRotOut = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -86.0f, 0.6f, _instance._exponential);
			LocalRotateByAction2D bottomRotOut = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -86.0f, 0.6f, _instance._exponential);
			ActionParallel rotateOut = new ActionParallel(topRotOut, bottomRotOut);
			rotateOut.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			ActionSequence rotInOutSeq = new ActionSequence(rotateIn, rotInOutDelay, rotateOut);
			rotInOutSeq.OnActionFinish += () => {
				_instance.SetDarkOverlayVisible(false);
				_instance._phaseAnimPlaying = false;
			};

			DelayAction stallFrontDelay = new DelayAction(0.5f);
			DelayAction stallEndDelay = new DelayAction(0.5f);

			LocalRotateByAction2D topRotStall = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -8.0f, 0.8f, _instance._inverseSmoothStep);
			LocalRotateByAction2D bottomRotStall = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -8.0f, 0.8f, _instance._inverseSmoothStep);
			ActionParallel rotateStall = new ActionParallel(topRotStall, bottomRotStall);

			ActionSequence rotStallSeq = new ActionSequence(stallFrontDelay, rotateStall, stallEndDelay);


			DelayAction alphaDelay = new DelayAction(0.8f);
			ImageAlphaToAction alphaIn = new ImageAlphaToAction(_instance._darkOverlay, 0.5f, 0.5f);
			ImageAlphaToAction alphaOut = new ImageAlphaToAction(_instance._darkOverlay, 0.0f, 0.5f);
			ActionSequence alphaFadeSeq = new ActionSequence(alphaIn, alphaDelay, alphaOut);

			ActionParallel phaseAnim = new ActionParallel(rotInOutSeq, rotStallSeq, alphaFadeSeq);
			if (inOnComplete != null) phaseAnim.OnActionFinish += () => { inOnComplete(); };

			ActionHandler.RunAction(phaseAnim);
		}
	}
}
