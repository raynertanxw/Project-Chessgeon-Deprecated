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
	[SerializeField] private RectTransform _phaseBannerTop = null;
	[SerializeField] private RectTransform _phaseBannerBtm = null;
	[SerializeField] private Text _phaseBannerTextTop = null;
	[SerializeField] private Text _phaseBannerTextBtm = null;

	[SerializeField] private RectTransform _cardDrawer = null;

	[Header("Animation Graphs")]
	[SerializeField] private AnimationCurve _cardDrawerBobber = null;
	[SerializeField] private AnimationCurve _cardDrawerDipper = null;

	Graph InverseSmoothStep;

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

			Debug.Assert(_cardDrawer != null, "_cardDrawer is not assigned.");

			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			SetDarkOverlayVisible(false);
			EnableCardDrawer(false, false);

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

			LocalRotateByAction2D topRotIn = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, Graph.InverseExponential, -86.0f, 0.6f);
			LocalRotateByAction2D bottomRotIn = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, Graph.InverseExponential, -86.0f, 0.6f);
			ActionParallel rotateIn = new ActionParallel(topRotIn, bottomRotIn);
			rotateIn.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			DelayAction rotInOutDelay = new DelayAction(0.6f);

			LocalRotateByAction2D topRotOut = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, Graph.Exponential, -86.0f, 0.6f);
			LocalRotateByAction2D bottomRotOut = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, Graph.Exponential, -86.0f, 0.6f);
			ActionParallel rotateOut = new ActionParallel(topRotOut, bottomRotOut);
			rotateOut.OnActionStart += () => { /*TODO: Play the shing shing sound.*/ };

			ActionSequence rotInOutSeq = new ActionSequence(rotateIn, rotInOutDelay, rotateOut);
			rotInOutSeq.OnActionFinish += () => {
				_instance.SetDarkOverlayVisible(false);
				_instance._phaseAnimPlaying = false;
			};

			DelayAction stallFrontDelay = new DelayAction(0.5f);
			DelayAction stallEndDelay = new DelayAction(0.5f);

			LocalRotateByAction2D topRotStall = new LocalRotateByAction2D(_instance._phaseBannerTop.transform, _instance.InverseSmoothStep, -8.0f, 0.8f);
			LocalRotateByAction2D bottomRotStall = new LocalRotateByAction2D(_instance._phaseBannerBtm.transform, _instance.InverseSmoothStep, -8.0f, 0.8f);
			ActionParallel rotateStall = new ActionParallel(topRotStall, bottomRotStall);

			ActionSequence rotStallSeq = new ActionSequence(stallFrontDelay, rotateStall, stallEndDelay);


			DelayAction alphaDelay = new DelayAction(0.8f);
			ImageAlphaToAction alphaIn = new ImageAlphaToAction(_instance._darkOverlay, Graph.Linear, 0.5f, 0.5f);
			ImageAlphaToAction alphaOut = new ImageAlphaToAction(_instance._darkOverlay, Graph.Linear, 0.0f, 0.5f);
			ActionSequence alphaFadeSeq = new ActionSequence(alphaIn, alphaDelay, alphaOut);

			ActionHandler.RunAction(rotInOutSeq, rotStallSeq, alphaFadeSeq);
		}
	}

	private bool _cardDrawerAnimPlaying = false;
	public static void EnableCardDrawer(bool inIsEnabled, bool inIsAnimated = true)
	{
		const float ENABLED_X_POS = -200.0f;
		const float DISABLED_X_POS = -1300.0f;

		if (!_instance._cardDrawerAnimPlaying)
		{
			Vector2 newAnchorPos = _instance._cardDrawer.anchoredPosition;
			if (inIsEnabled && _instance._cardDrawer.localPosition.x != ENABLED_X_POS)
			{
				newAnchorPos.x = ENABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					MoveToAnchoredPosAction openDrawer = new MoveToAnchoredPosAction(_instance._cardDrawer, newAnchorPos, 0.6f, _instance._cardDrawerBobber);
					openDrawer.OnActionFinish += () => { _instance._cardDrawerAnimPlaying = false; };
					ActionHandler.RunAction(openDrawer);
				}
				else { _instance._cardDrawer.anchoredPosition = newAnchorPos; }
			}
			else if (!inIsEnabled && _instance._cardDrawer.localPosition.x != DISABLED_X_POS)
			{
				newAnchorPos.x = DISABLED_X_POS;
				if (inIsAnimated)
				{
					_instance._cardDrawerAnimPlaying = true;
					MoveToAnchoredPosAction closeDrawer = new MoveToAnchoredPosAction(_instance._cardDrawer, newAnchorPos, 0.6f, _instance._cardDrawerDipper);
					closeDrawer.OnActionFinish += () => { _instance._cardDrawerAnimPlaying = false; };
					ActionHandler.RunAction(closeDrawer);
				}
				else { _instance._cardDrawer.anchoredPosition = newAnchorPos; }
			}
		}
	}
}
