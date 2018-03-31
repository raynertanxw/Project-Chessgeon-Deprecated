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
	[SerializeField] private Image _damageFrame = null;
	[SerializeField] private Image _darkOverlay = null;
	[SerializeField] private RectTransform _phaseBannerTop = null;
	[SerializeField] private RectTransform _phaseBannerBtm = null;
	[SerializeField] private Text _phaseBannerTextTop = null;
	[SerializeField] private Text _phaseBannerTextBtm = null;
	[SerializeField] private RectTransform _heartsHolder = null;
	[SerializeField] private Text _scoreText = null;
	[SerializeField] private Text _floorText = null;
	[SerializeField] private Text _coinText = null;
	[SerializeField] private Image _nextFloorPanel = null;
	[SerializeField] private Text _nextFloorText = null;

	[Header("Meshes")]
	[SerializeField] private Mesh _heartFullMesh = null;
	[SerializeField] private Mesh _heartHalfMesh = null;
	[SerializeField] private Mesh _shieldMesh = null;

	private const int NUM_HEARTS = 5;
	private MeshRenderer[] _heartMeshRens = null;
	private MeshFilter[] _heartMeshFilters = null;
	private const int NUM_SHIELD = 5;
	private MeshRenderer[] _shieldMeshRens = null;
	private MeshFilter[] _shieldMeshFilters = null;


	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");
			Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

			Debug.Assert(_damageFrame != null, "_damageFrame is not assigned.");
			Debug.Assert(_darkOverlay != null, "_darkOverlay is not assigned.");
			Debug.Assert(_phaseBannerTop != null, "_phaseBannerTop is not assigned.");
			Debug.Assert(_phaseBannerBtm != null, "_phaseBannerBtm is not assigned.");
			Debug.Assert(_phaseBannerTextTop != null, "_phaseBannerTextTop is not assigned.");
			Debug.Assert(_phaseBannerTextBtm != null, "_phaseBannerTextBtm is not assigned.");
			Debug.Assert(_heartsHolder != null, "_heartsHolder is not assigned.");
			Debug.Assert(_scoreText != null, "_scoreText is not assigned.");
			Debug.Assert(_floorText != null, "_floorText is not assigned.");
			Debug.Assert(_coinText != null, "_coinText is not assigned.");
			Debug.Assert(_nextFloorPanel != null, "_nextFloorPanel is not assigned.");
			Debug.Assert(_nextFloorText != null, "_nextFloorText is not assigned.");

			Debug.Assert(_heartFullMesh != null, "_heartFullMesh is not assigned.");
			Debug.Assert(_heartHalfMesh != null, "_heartHalfMesh is not assigned.");
			Debug.Assert(_shieldMesh != null, "_shieldMesh is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			_heartMeshRens = new MeshRenderer[NUM_HEARTS];
			_heartMeshFilters = new MeshFilter[NUM_HEARTS];
			for (int iHeart = 0; iHeart < NUM_HEARTS; iHeart++)
			{
				_heartMeshRens[iHeart] = _heartsHolder.GetChild(iHeart).GetComponent<MeshRenderer>();
				_heartMeshFilters[iHeart] = _heartsHolder.GetChild(iHeart).GetComponent<MeshFilter>();
			}

			_shieldMeshRens = new MeshRenderer[NUM_SHIELD];
			_shieldMeshFilters = new MeshFilter[NUM_SHIELD];
			for (int iShield = 0; iShield < NUM_SHIELD; iShield++)
			{
				_shieldMeshRens[iShield] = _heartMeshRens[iShield].transform.GetChild(0).GetComponent<MeshRenderer>();
				_shieldMeshFilters[iShield] = _heartMeshRens[iShield].transform.GetChild(0).GetComponent<MeshFilter>();
			}

			SetDarkOverlayVisible(false);
			SetDamageFrameVisible(false);
			SetHealtUI(0);
			SetShieldUI(0);
			HideNextFloorPanel(null, true);

			_dungeon.OnFloorGenerated += () => { UpdateFloorText(_dungeon.FloorNum); };
		}
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	private void Start()
	{
		ActionParallel rotateShieldsForever = new ActionParallel();
		for (int iShield = 0; iShield < NUM_SHIELD; iShield++)
		{
			rotateShieldsForever.Add(new ActionRepeatForever(new RotateByAction(
				_shieldMeshRens[iShield].transform,
				new Vector3(0.0f, 360.0f, 0.0f),
				7.5f)));
		}
		ActionHandler.RunAction(rotateShieldsForever);
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

	private void SetDamageFrameVisible(bool inIsVisible)
	{
		_damageFrame.enabled = inIsVisible;
	}

	private void SetDamageFrameAlpha(float inAlpha)
	{
		if (_damageFrame.color.a != inAlpha)
		{
			Color newCol = _damageFrame.color;
			newCol.a = inAlpha;
			_damageFrame.color = newCol;
		}
	}

	private bool _damageFrameAnimPlaying = false;
	public static void PlayDamageFrameAnimation(float inDuration = 0.5f, DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (_instance._damageFrameAnimPlaying)
		{
			Debug.LogWarning("Trying to call PlayDamageFrameAnimation when its previous call has not finished animating.");
		}
		else
		{
			_instance._damageFrameAnimPlaying = true;

			_instance.SetDamageFrameAlpha(0.6f);
			_instance.SetDamageFrameVisible(true);

			ImageAlphaToAction fadeAway = new ImageAlphaToAction(_instance._damageFrame, 0.0f, inDuration);
			fadeAway.OnActionFinish += () =>
			{
				_instance._damageFrameAnimPlaying = false;
				_instance.SetDamageFrameVisible(false);
			};
			if (inOnComplete != null) fadeAway.OnActionFinish += () => { inOnComplete(); };

			ActionHandler.RunAction(fadeAway);
		}
	}

	private bool _phaseAnimPlaying = false;
	public static void PlayPhaseAnimation(bool inIsPlayersTurn, DTJob.OnCompleteCallback inOnComplete = null)
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

			LocalRotateByAction2D topRotIn = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -86.0f, 0.6f, Utils.CurveInverseExponential);
			LocalRotateByAction2D bottomRotIn = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -86.0f, 0.6f, Utils.CurveInverseExponential);
			ActionParallel rotateIn = new ActionParallel(topRotIn, bottomRotIn);
			rotateIn.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			DelayAction rotInOutDelay = new DelayAction(0.6f);

			LocalRotateByAction2D topRotOut = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -86.0f, 0.6f, Utils.CurveExponential);
			LocalRotateByAction2D bottomRotOut = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -86.0f, 0.6f, Utils.CurveExponential);
			ActionParallel rotateOut = new ActionParallel(topRotOut, bottomRotOut);
			rotateOut.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			ActionSequence rotInOutSeq = new ActionSequence(rotateIn, rotInOutDelay, rotateOut);
			rotInOutSeq.OnActionFinish += () => {
				_instance.SetDarkOverlayVisible(false);
				_instance._phaseAnimPlaying = false;
			};

			DelayAction stallFrontDelay = new DelayAction(0.5f);
			DelayAction stallEndDelay = new DelayAction(0.5f);

			LocalRotateByAction2D topRotStall = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, -8.0f, 0.8f, Utils.CurveInverseSmoothStep);
			LocalRotateByAction2D bottomRotStall = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, -8.0f, 0.8f, Utils.CurveInverseSmoothStep);
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

	public static void ShowNextFloorPanel(int inFloorNum, DTJob.OnCompleteCallback inOnComplete = null)
	{
		const float FADE_IN_DURATION = 1.5f;
		_instance._nextFloorText.text = ChessgeonUtils.FormatFloorString(inFloorNum);
		ImageAlphaToAction fadeInPanel = new ImageAlphaToAction(_instance._nextFloorPanel, 1.0f, FADE_IN_DURATION);
		TextAlphaToAction fadeInText = new TextAlphaToAction(_instance._nextFloorText, 1.0f, FADE_IN_DURATION);

		ActionParallel fadeInNextFloorPanel = new ActionParallel(fadeInPanel, fadeInText);
		if (inOnComplete != null) fadeInNextFloorPanel.OnActionFinish += () => { inOnComplete(); };
		ActionHandler.RunAction(fadeInNextFloorPanel);
	}
	public static void HideNextFloorPanel(DTJob.OnCompleteCallback inOnComplete = null, bool inImmediate = false)
	{
		if (inImmediate)
		{
			Color col = _instance._nextFloorPanel.color;
			col.a = 0.0f;
			_instance._nextFloorPanel.color = col;
			col = _instance._nextFloorText.color;
			col.a = 0.0f;
			_instance._nextFloorText.color = col;
			if (inOnComplete != null) inOnComplete();
		}
		else
		{
			const float FADE_OUT_DURATION = 0.75f;
			ImageAlphaToAction fadeOutPanel = new ImageAlphaToAction(_instance._nextFloorPanel, 0.0f, FADE_OUT_DURATION);
			TextAlphaToAction fadeOutText = new TextAlphaToAction(_instance._nextFloorText, 0.0f, FADE_OUT_DURATION);

			ActionParallel fadeOutNextFloorPanel = new ActionParallel(fadeOutPanel, fadeOutText);
			if (inOnComplete != null) fadeOutNextFloorPanel.OnActionFinish += () => { inOnComplete(); };
			ActionHandler.RunAction(fadeOutNextFloorPanel);
		}
	}

	#region HUD Update Functions
	public static void SetHealtUI(int inHealth)
	{
		Debug.Assert(inHealth <= NUM_HEARTS * 2, inHealth + " is beyond the max health displayble.");
		int numHeartsToDisplay = inHealth / 2;
		int halfHeartIndex = (inHealth % 2 == 1) ? numHeartsToDisplay : -1;
		for (int iHeart = 0; iHeart < NUM_HEARTS; iHeart++)
		{
			MeshFilter curHeartFilter = _instance._heartMeshFilters[iHeart];
			if (iHeart < numHeartsToDisplay)
			{
				curHeartFilter.mesh = _instance._heartFullMesh;
			}
			else if (iHeart == numHeartsToDisplay &&
				iHeart == halfHeartIndex)
			{
				curHeartFilter.mesh = _instance._heartHalfMesh;
			}
			else
			{
				curHeartFilter.mesh = null;
			}
		}
	}

	public static void SetShieldUI(int inShield)
	{
		Debug.Assert(inShield <= NUM_SHIELD, inShield + " is beyond the max shield displayable.");
		for (int iShield = 0; iShield < NUM_SHIELD; iShield++)
		{
			MeshFilter curShieldFilter = _instance._shieldMeshFilters[iShield];
			if (iShield < inShield)
			{
				curShieldFilter.mesh = _instance._shieldMesh;
			}
			else
			{
				curShieldFilter.mesh = null;
			}
		}
	}

	private void UpdateFloorText(int inFloorNum)
	{
		Debug.Assert(inFloorNum > 0, "inFloor is out of range: " + inFloorNum);
		_floorText.text = ChessgeonUtils.FormatFloorString(inFloorNum);
	}

	public static void UpdateCoinText(int inNumCoins)
	{
		Debug.Assert(inNumCoins > -1, "inNumCoins is out of range: " + inNumCoins);
		_instance._coinText.text = ChessgeonUtils.FormatCoinString(inNumCoins);
	}
	#endregion
}
