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
	[SerializeField] private Image _damageFrame = null;
	[SerializeField] private Image _darkOverlay = null;
	[SerializeField] private RectTransform _phaseBannerTop = null;
	[SerializeField] private RectTransform _phaseBannerBtm = null;
	[SerializeField] private Text _phaseBannerTextTop = null;
	[SerializeField] private Text _phaseBannerTextBtm = null;
	[SerializeField] private Text _comboText = null;
	[SerializeField] private Text _scoreText = null;
	[SerializeField] private Text _floorText = null;

	[Header("Next Floor Canvas")]
	[SerializeField] private Canvas _nextFloorCanvas = null;
	[SerializeField] private CanvasGroup _nextFloorCanvasGrp = null;
	[SerializeField] private Text _nextFloorText = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

			Debug.Assert(_damageFrame != null, "_damageFrame is not assigned.");
			Debug.Assert(_darkOverlay != null, "_darkOverlay is not assigned.");
			Debug.Assert(_phaseBannerTop != null, "_phaseBannerTop is not assigned.");
			Debug.Assert(_phaseBannerBtm != null, "_phaseBannerBtm is not assigned.");
			Debug.Assert(_phaseBannerTextTop != null, "_phaseBannerTextTop is not assigned.");
			Debug.Assert(_phaseBannerTextBtm != null, "_phaseBannerTextBtm is not assigned.");
			Debug.Assert(_comboText != null, "_comboText is not assigned.");
			Debug.Assert(_scoreText != null, "_scoreText is not assigned.");
			Debug.Assert(_floorText != null, "_floorText is not assigned.");

			Debug.Assert(_nextFloorCanvas != null, "_nextFloorCanvas is not assigned.");
			Debug.Assert(_nextFloorCanvasGrp != null, "_nextFloorCanvasGrp is not assigned.");
			Debug.Assert(_nextFloorText != null, "_nextFloorText is not assigned.");
			Debug.Assert(!_nextFloorCanvasGrp.interactable, "_nextFloorCanvasGrp has no interactable elements. Should not be interactable");

			SetDarkOverlayVisible(false);
			SetDamageFrameVisible(false);
			HideNextFloorPanel(null, true);

			_dungeon.OnFloorGenerated += () => { UpdateFloorText(_dungeon.FloorNum); };
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
		_instance._nextFloorCanvasGrp.alpha = 0.0f;
		_instance._nextFloorCanvasGrp.blocksRaycasts = true;
		_instance._nextFloorCanvas.enabled = true;
		CanvasGroupAlphaToAction fadeInCanvas = new CanvasGroupAlphaToAction(_instance._nextFloorCanvasGrp, 1.0f, FADE_IN_DURATION);

		if (inOnComplete != null) fadeInCanvas.OnActionFinish += () => { inOnComplete(); };
		ActionHandler.RunAction(fadeInCanvas);
	}
	public static void HideNextFloorPanel(DTJob.OnCompleteCallback inOnComplete = null, bool inImmediate = false)
	{
		if (inImmediate)
		{
			_instance._nextFloorCanvasGrp.alpha = 0.0f;
			_instance._nextFloorCanvasGrp.blocksRaycasts = false;
			_instance._nextFloorCanvas.enabled = false;

			if (inOnComplete != null) inOnComplete();
		}
		else
		{
			const float FADE_OUT_DURATION = 0.75f;
			
			_instance._nextFloorCanvasGrp.alpha = 1.0f;
			CanvasGroupAlphaToAction fadeOutCanvas = new CanvasGroupAlphaToAction(_instance._nextFloorCanvasGrp, 0.0f, FADE_OUT_DURATION);
			fadeOutCanvas.OnActionFinish += () =>
			{
				_instance._nextFloorCanvasGrp.alpha = 0.0f;
				_instance._nextFloorCanvasGrp.blocksRaycasts = false;
				_instance._nextFloorCanvas.enabled = false;
			};

			if (inOnComplete != null) fadeOutCanvas.OnActionFinish += () => { inOnComplete(); };
			ActionHandler.RunAction(fadeOutCanvas);
		}
	}

	#region HUD Update Functions
	private void UpdateComboText(int inCombo)
	{
		Debug.Assert(inCombo > 0, "inCombo is out of range: " + inCombo);
		_comboText.text = ChessgeonUtils.FormatComboString(inCombo);
	}

	private void UpdateScoreText(int inScore)
	{
		Debug.Assert(inScore > -1, "inScore is out of range: " + inScore);
		_scoreText.text = ChessgeonUtils.FormatScoreString(inScore);
	}

	private void UpdateFloorText(int inFloorNum)
	{
		Debug.Assert(inFloorNum > 0, "inFloor is out of range: " + inFloorNum);
		_floorText.text = ChessgeonUtils.FormatFloorString(inFloorNum);
	}
	#endregion
}
