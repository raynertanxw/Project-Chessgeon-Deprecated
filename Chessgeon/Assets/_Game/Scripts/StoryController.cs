using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
	private const string STORY_INTRO_VIEWED_KEY = "STORY_INTRO_VIEWED";

	[SerializeField] private StoryCanvas _storyCanvas = null;
	[SerializeField] private MenuCanvas _menuCanvas = null;
	[SerializeField] private StoryObjects _storyObjects = null;

	private void Awake()
	{
		Debug.Assert(_storyCanvas != null, "_storyCanvas is not assigned.");
		Debug.Assert(_menuCanvas != null, "_menuCanvas it not assigned.");
		Debug.Assert(_storyObjects != null, "_storyObjects is not assigned.");

		if (PlayerPrefs.HasKey(STORY_INTRO_VIEWED_KEY))
        {
            DismissStory();
        }
        else
        {
            PlayStory();
        }
	}

	public void PlayStory()
	{
		_storyCanvas.SetVisible(true);
		_menuCanvas.SetVisible(false);

		_storyCanvas.SetupStoryCamFeed();
		_storyObjects.SetActive(true);

		StartCoroutine(StoryCoroutine());
	}

	private void DismissStory()
	{
		// TODO: Call start showing menu canvas.
		_storyCanvas.SetVisible(false);
		_menuCanvas.SetVisible(true);

		_storyCanvas.ReleaseRenderTexture();
		_storyObjects.SetActive(false);
	}

	public void ContinueStory()
	{
		_shouldContinueStory = true;
	}

	private bool _shouldContinueStory = false;
	private IEnumerator StoryCoroutine()
	{
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);
		_storyCanvas.HideTextPanel();
		// TODO: Set camera start position.

		yield return new WaitForSeconds(1.0f);
		_storyCanvas.ShowTextPanel("This...\nis Morphy.");

		yield return new WaitForSeconds(1.5f);
		_storyCanvas.SetContinueVisible(true);
		_storyCanvas.SetContinueText("Continue");
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;

		_storyCanvas.ShowTextPanel("Say \"Hi Morphy!\"");
        _storyCanvas.SetContinueText("Hi Morphy!");
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;

		_storyCanvas.ShowTextPanel("\"...\"");
        _storyCanvas.SetContinueText("...");
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;

		_storyCanvas.ShowTextPanel("Sorry,\nHe's a bit shy");
        _storyCanvas.SetContinueText("No Worries!");
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;

		_storyCanvas.SetContinueText("Let's Begin!");
		while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		DismissStory();
	}
}
