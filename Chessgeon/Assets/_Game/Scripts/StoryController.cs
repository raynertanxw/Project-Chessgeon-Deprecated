using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
	private const string STORY_INTRO_VIEWED_KEY = "STORY_INTRO_VIEWED";

	[SerializeField] private StoryCanvas _storyCanvas = null;
	[SerializeField] private MenuCanvas _menuCanvas = null;

	[Header("Non-UI Story Objects")]
	[SerializeField] private GameObject _nonUIStoryParentObject = null;

    [Header("Other Managers")]
    [SerializeField] private TileManager _tileManager = null;
	[SerializeField] private Dungeon _dungeon = null;

	private void Awake()
	{
		Debug.Assert(_storyCanvas != null, "_storyCanvas is not assigned.");
		Debug.Assert(_menuCanvas != null, "_menuCanvas it not assigned.");

		Debug.Assert(_nonUIStoryParentObject != null, "_nonUIStoryParentObject.");

		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
        Debug.Assert(_tileManager != null, "_tileManager is not assigned.");
	
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
		_nonUIStoryParentObject.SetActive(true);

		StartCoroutine(StoryCoroutine());
	}

	private void DismissStory()
	{
		// TODO: Call start showing menu canvas.
		_storyCanvas.SetVisible(false);
		_menuCanvas.SetVisible(true);

		_storyCanvas.ReleaseRenderTexture();
		_nonUIStoryParentObject.SetActive(false);
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
