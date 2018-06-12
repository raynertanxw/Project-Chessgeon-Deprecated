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

	private void Awake()
	{
		Debug.Assert(_storyCanvas != null, "_storyCanvas is not assigned.");
		Debug.Assert(_menuCanvas != null, "_menuCanvas it not assigned.");

		Debug.Assert(_nonUIStoryParentObject != null, "_nonUIStoryParentObject.");
	}

	private void Start()
	{
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
	}

	public void DismissStory()
	{
		// TODO: Call start showing menu canvas.
		_storyCanvas.SetVisible(false);
		_menuCanvas.SetVisible(true);

		_storyCanvas.ReleaseRenderTexture();
		_nonUIStoryParentObject.SetActive(false);
	}

	public void ContinueStory()
	{
		// TODO: Do blocking and all if not yet done.
		// NOTE: For now just dismiss.
		DismissStory();
	}
}
