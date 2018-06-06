using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : MonoBehaviour
{
	[SerializeField] private Button _settingsBtn = null;

	[Header("Settings Canvas")]
	[SerializeField] private Canvas _settingCanvas = null;
	[SerializeField] private Button _closeBtn = null;
	[SerializeField] private Button _resetTutorialBtn = null;

	private void Awake()
	{
		Debug.Assert(_settingsBtn != null, "_settingBtn is not assigned.");

		Debug.Assert(_settingCanvas != null, "_settingCanvas is not assigned.");
		Debug.Assert(_closeBtn != null, "_closeBtn is not assigned.");
		Debug.Assert(_resetTutorialBtn != null, "_resetTutorialBtn is not assigned.");

		_settingsBtn.onClick.AddListener(() => { SetSettingCanvasVisible(true); });

		_closeBtn.onClick.AddListener(() => { SetSettingCanvasVisible(false); });
		_resetTutorialBtn.onClick.AddListener(ResetTutorial);

		SetSettingCanvasVisible(false);
	}

	private void SetSettingCanvasVisible(bool inIsVisible)
	{
		_settingCanvas.enabled = inIsVisible;
	}

	private void ResetTutorial()
	{
		// TODO: Implement this.
		Debug.Log("Reset Tutorial");
	}
}
