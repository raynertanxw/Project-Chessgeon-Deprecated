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
		_storyCanvas.SetVisible(false);
		// TODO: Call start showing menu canvas.
		_menuCanvas.SetVisible(true);

		_storyCanvas.ReleaseRenderTexture();
		_storyObjects.SetActive(false);

		PlayerPrefs.SetInt(STORY_INTRO_VIEWED_KEY, 1);
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
		_storyObjects.FocusCameraTo(-4.5f, -13.5f, 0.0f, null, false);
		// TODO: Set camera start position.

		yield return new WaitForSeconds(1.0f);
		_storyCanvas.ShowTextPanel("Hi there!");
		yield return new WaitForSeconds(2.0f);
		_storyCanvas.HideTextPanel();
		_shouldContinueStory = false;
		_storyObjects.FocusCameraTo(0.5f, 0.5f, 2.0f, () => { _shouldContinueStory = true; });
		while (!_shouldContinueStory) { yield return null; }
		
		yield return new WaitForSeconds(0.5f);
		_storyObjects.SpawnInMorphy(2.5f, 4.5f);
		yield return new WaitForSeconds(1.5f);
		_storyCanvas.ShowTextPanel("This...\nis Morphy.");
		yield return new WaitForSeconds(1.5f);

		_storyCanvas.SetContinueText("Continue");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);

		_storyCanvas.ShowTextPanel("Say \"Hi Morphy!\"");
		yield return new WaitForSeconds(0.5f);
        _storyCanvas.SetContinueText("Hi Morphy!");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);

		_storyCanvas.ShowTextPanel("\"...\"");
		yield return new WaitForSeconds(1.5f);
        _storyCanvas.SetContinueText("...");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(1.5f);
		_storyCanvas.ShowTextPanel("Sorry,\n");
		yield return new WaitForSeconds(1.0f);
		_storyCanvas.ShowTextPanel("Sorry,\nHe's a bit shy");
		yield return new WaitForSeconds(1.0f);
        _storyCanvas.SetContinueText("No Worries!");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;


		// TODO: Spawn in queen.

		// "This is Morphy's Queen"

		// "Morphy loves his Queen"

		// "One day..."

		// fly in evil purple orb.

		// "Morphy's Queen got possessed!"

		// purple orb hit Queen and possess.
		// Morphy do a little shock hop.

		// "Oh no!"

		// " \">:(\" "

		// "Morphy tries to save his Queen!"

		// Queen and Morphy move to end of board.
		// Queen simply fades into the distance...

		// "But he couldn't save her...

		// " \" :( \" "
		// " \" :"( \" "
		// " \" :""""""""( \" "
		// Camera shake???

		// Stairs nearby pops into existence. (Make sure got some particle effects).
		
		// "Oh? What's this!"

		// Morphy move to stairs.

		// "And so, begin's Morphy's pursuit"
		// "Through the...\n Chessgeons!"


		_storyCanvas.ShowTextPanel("IN DEVELOPMENT");
		_storyCanvas.SetContinueText("Let's Begin!");
		while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		DismissStory();
	}
}
