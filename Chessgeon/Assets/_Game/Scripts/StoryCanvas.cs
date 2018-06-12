using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryCanvas : MonoBehaviour
{
	[SerializeField] private Canvas _storyMainCanvas = null;
	[SerializeField] private RawImage _storyFeedRawImage = null;
	[SerializeField] private Camera _storyCam = null;
	[SerializeField] private Button _continueBtn = null;
	[SerializeField] private Text _continueText = null;
	[SerializeField] private Canvas _continueCanvas = null;
	[SerializeField] private StoryController _storyCtrl = null;

	[Header("TextPanel")]
	[SerializeField] private CanvasGroup _textPanelCanvasGrp = null;
	[SerializeField] private Text _textPanelText = null;

	private RenderTexture _storyRenTex = null;

	private void Awake()
	{
		Debug.Assert(_storyMainCanvas != null, "_storyMainCanvas is not assigned.");
		Debug.Assert(_storyFeedRawImage != null, "_storyFeedRawImage is not assigned.");
		Debug.Assert(_storyCam != null, "_storyRenTex is not assigned.");
		Debug.Assert(_storyCtrl != null, "_storyCtrl is not assigned.");
		Debug.Assert(_continueBtn != null, "_continueBtn is not assigned.");
		Debug.Assert(_continueText != null, "_continueText is not assigned.");
		Debug.Assert(_continueCanvas != null, "_continueCanvas is not assigned.");
		Debug.Assert(_storyCtrl != null, "_storyCtrl is not assigned.");

		Debug.Assert(_textPanelCanvasGrp != null, "_textPanelCanvasGrp is not assigned.");
		Debug.Assert(_textPanelText != null, "_textPanelText is not assigned.");

		_continueBtn.onClick.AddListener(_storyCtrl.ContinueStory);
	}

	public void SetupStoryCamFeed()
	{
		Vector2Int rawImageSize = new Vector2Int(
			Mathf.RoundToInt(_storyFeedRawImage.rectTransform.rect.size.x),
			Mathf.RoundToInt(_storyFeedRawImage.rectTransform.rect.size.y));
		Debug.Log("Story Tex Size: " + rawImageSize);

		if (_storyRenTex == null) _storyRenTex = new RenderTexture(rawImageSize.x, rawImageSize.y, 16, RenderTextureFormat.ARGB32);
		if (!_storyRenTex.IsCreated()) _storyRenTex.Create();

		_storyFeedRawImage.texture = _storyRenTex;
		_storyCam.targetTexture = _storyRenTex;
	}

	public void ReleaseRenderTexture()
	{
		if (_storyRenTex != null && _storyRenTex.IsCreated()) _storyRenTex.Release();
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_storyMainCanvas.enabled != inIsVisible) _storyMainCanvas.enabled = inIsVisible;
	}

	public void SetContinueText(string inText)
	{
		_continueText.text = inText;
	}

	public void SetContinueVisible(bool inIsVisible)
	{
		if (_continueCanvas.enabled != inIsVisible) _continueCanvas.enabled = inIsVisible;
	}

	public void ShowTextPanel(string inText)
	{
		_textPanelText.text = inText;
		_textPanelCanvasGrp.alpha = 1.0f;
	}

	public void HideTextPanel()
	{
		_textPanelCanvasGrp.alpha = 0.0f;
	}
}
