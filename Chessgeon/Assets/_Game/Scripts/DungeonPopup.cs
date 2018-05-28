using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class DungeonPopup : MonoBehaviour
{
	private static DungeonPopup _instance;

	[Header("Canvas UI Elements")]
	[SerializeField] private Image _popupPanel = null;
	[SerializeField] private CanvasGroup _popupCanvasGrp = null;
	[SerializeField] private Text _popupText = null;

	private float _fadeDelay = 0.0f;
	private const float FADE_SPEED = 1.5f;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<GraphicRaycaster>() == null, "There is a GraphicRaycaster component on Dungeon Display Canvas. Remove it.");
			Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");

			Debug.Assert(_popupPanel != null, "_popupPanel is not assigned.");
			Debug.Assert(_popupCanvasGrp != null, "_popupCanvasGrp is not assigned.");
			Debug.Assert(_popupText != null, "_popupText is not assigned.");

			_popupCanvasGrp.alpha = 0.0f;
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
		if (_fadeDelay < 0.0f)
		{
			_popupCanvasGrp.alpha -= FADE_SPEED * Time.deltaTime;
		}
		else
		{
			_fadeDelay -= Time.deltaTime;
		}
	}

	public static void PopText(string inMsg, float inFadeDelay = 1.0f)
	{
		_instance._popupText.text = inMsg;
		_instance._popupCanvasGrp.alpha = 1.0f;
		_instance._fadeDelay = inFadeDelay;
	}
}
