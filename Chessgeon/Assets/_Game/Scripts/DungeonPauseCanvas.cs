using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class DungeonPauseCanvas : MonoBehaviour
{
	private static DungeonPauseCanvas _instance = null;

	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private Menu _menu = null;

	[SerializeField] private GameObject _pauseBtnObject = null;
	[SerializeField] private GameObject _pausePanelObject = null;
	[SerializeField] private Button _pauseBtn = null;
	[SerializeField] private Button _resumeBtn = null;
	[SerializeField] private Button _mainMenuBtn = null;
	private RectTransform _pauseBtnRectTransform = null;

	private bool _isEnabled = true;
	private static bool _isPaused = false;
	public static bool IsPaused { get { return _isPaused; } }
	public static Utils.GenericVoidDelegate OnIsPausedChanged = null;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Debug.Assert(gameObject.GetComponent<Canvas>().worldCamera != null, "There is no assigned RenderCamera for DungeonDisplay Canavs.");
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetDesignWidthFromDesignHeight(1920.0f), 1920.0f);

			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			Debug.Assert(_menu != null, "_menu is not assigned.");

			Debug.Assert(_pauseBtnObject != null, "_pauseBtnObject is not assigned.");
			Debug.Assert(_pausePanelObject != null, "_pausePanelObject is not assigned.");
			Debug.Assert(_pauseBtn != null, "_pauseBtn is not assigned.");
			Debug.Assert(_resumeBtn != null, "_resumeBtn is not assigned.");
			Debug.Assert(_mainMenuBtn != null, "_mainMenuBtn is not assigned.");

			_pauseBtnRectTransform = _pauseBtnObject.GetComponent<RectTransform>();

			_pauseBtn.onClick.AddListener(Pause);
			_resumeBtn.onClick.AddListener(DismissPausePanel);
			_mainMenuBtn.onClick.AddListener(BackToMainMenu);

			DismissPausePanel();
			SetEnablePauseBtn(false, null, false);
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

	private void Pause()
	{
		_isPaused = true;
		if (OnIsPausedChanged != null) OnIsPausedChanged();
		_pausePanelObject.SetActive(true);
	}

	private void DismissPausePanel()
	{
		_isPaused = false;
		if (OnIsPausedChanged != null) OnIsPausedChanged();
		_pausePanelObject.SetActive(false);
	}

	private void BackToMainMenu()
	{
		_dungeon.SaveGame();
		DismissPausePanel();
		_menu.ReturnToMainMenu();
		SetEnablePauseBtn(false, null, false);
	}

	public static void SetInteractablePauseBtn(bool inIsInteractable)
	{
		_instance._pauseBtn.interactable = inIsInteractable;
	}

	public static void SetEnablePauseBtn(bool inIsEnabled, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true)
	{
		if (inIsEnabled != _instance._isEnabled)
		{
			_instance._pauseBtn.interactable = false;
			Vector2 newAnchoredPos = inIsEnabled ? new Vector2(-100.0f, -100.0f) : new Vector2(100.0f, -100.0f);
			Utils.GenericVoidDelegate onFinish = () =>
			{
				_instance._pauseBtn.interactable = inIsEnabled;
				if (inOnComplete != null) inOnComplete();
			};
			if (inIsAnimated)
			{
				MoveToAnchoredPosAction movePauseBtn = new MoveToAnchoredPosAction(
					_instance._pauseBtnRectTransform,
					newAnchoredPos,
					0.4f,
					inIsEnabled ? Utils.CurveBobber : Utils.CurveDipper);
				movePauseBtn.OnActionFinish += onFinish;

				ActionHandler.RunAction(movePauseBtn);
			}
			else
			{
				_instance._pauseBtnRectTransform.anchoredPosition = newAnchoredPos;
				onFinish();
			}

			_instance._isEnabled = inIsEnabled;
		}
		else
		{
			Debug.LogWarning("Pause btn _isEnabled is already " + inIsEnabled);
			if (inOnComplete != null) inOnComplete();
		}
	}
}
