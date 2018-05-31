using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class DungeonPopup : MonoBehaviour
{
	private static DungeonPopup _instance;

	[Header("Middle Popup Panel")]
	[SerializeField] private CanvasGroup _middlePopupCanvasGrp = null;
	[SerializeField] private Text _middlePopupText = null;
	[Space]
	[Header("Side Popup Panel")]
	[SerializeField] private RectTransform _sidePopupRectTransform = null;
	[SerializeField] private CanvasGroup _sidePopupCanvasGrp = null;
	[SerializeField] private Text _sidePopupText = null;

	private float _middlePopupFadeDelay = 0.0f;
	private const float MIDDLE_POPUP_FADE_SPEED = 1.5f;

	private float _sidePopupFadeDelay = 0.0f;
	private const float SIDE_POPUP_FADE_SPEED = 1.0f;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");

			Debug.Assert(_middlePopupCanvasGrp != null, "_middlePopupCanvasGrp is not assigned.");
			Debug.Assert(_middlePopupText != null, "_middlePopupText is not assigned.");

			Debug.Assert(_sidePopupRectTransform != null, "_sidePopupRectTransform is not assigned.");
			Debug.Assert(_sidePopupCanvasGrp != null, "_sidePopupCanvasGrp is not assigned.");
			Debug.Assert(_sidePopupText != null, "_sidePopupText is not assigned.");

			_middlePopupCanvasGrp.alpha = 0.0f;
			_sidePopupCanvasGrp.alpha = 0.0f;
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

	private void Update()
	{
		if (_middlePopupFadeDelay < 0.0f)
		{
			_middlePopupCanvasGrp.alpha -= MIDDLE_POPUP_FADE_SPEED * Time.deltaTime;
		}
		else
		{
			_middlePopupFadeDelay -= Time.deltaTime;
		}

		if (_sidePopupFadeDelay < 0.0f)
		{
			_sidePopupCanvasGrp.alpha -= SIDE_POPUP_FADE_SPEED * Time.deltaTime;
		}
		else
		{
			_sidePopupFadeDelay -= Time.deltaTime;
		}
	}

	public static void PopMiddlePopup(string inMsg, float inFadeDelay = 1.0f)
	{
		_instance._middlePopupText.text = inMsg;
		_instance._middlePopupCanvasGrp.alpha = 1.0f;
		_instance._middlePopupFadeDelay = inFadeDelay;
	}

	public static void PopSidePopup(string inMsg, float inFadeDelay = 3.0f)
	{
		_instance._sidePopupText.text = inMsg;
		_instance._sidePopupCanvasGrp.alpha = 1.0f;
		_instance._sidePopupFadeDelay = inFadeDelay;
	}
}
