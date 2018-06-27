using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

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
		// TODO: Call start showing menu canvas animation.
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
		_storyCanvas.HideTextPanel();
		yield return new WaitForSeconds(0.5f);

		_storyCanvas.ShowTextPanel("Say \"Hi Morphy!\"");
		yield return new WaitForSeconds(0.5f);
        _storyCanvas.SetContinueText("Hi Morphy!");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(1.0f);
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
		_storyCanvas.SetContinueVisible(false);
		_storyCanvas.HideTextPanel();


		yield return new WaitForSeconds(0.5f);
		_shouldContinueStory = false;
		_storyObjects.FocusCameraTo(1.5f, 1.5f, 2.0f, () => { _shouldContinueStory = true; });
		while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyObjects.SpawnInQueen(4.5f, 6.5f);
		yield return new WaitForSeconds(0.5f);
		_storyCanvas.ShowTextPanel("This is Morphy's Queen");
        _storyCanvas.SetContinueText("Continue");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);

		_storyCanvas.ShowTextPanel("Morphy loves his Queen");
        _storyCanvas.SetContinueText("Continue");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);

		_storyCanvas.ShowTextPanel("He uses his\npowers to\nprotect her.");
        _storyCanvas.SetContinueText("Continue");
		_storyCanvas.SetContinueVisible(true);
        while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		_storyCanvas.SetContinueVisible(false);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(0.5f);
		_storyObjects.MorphMorphy(eMoveType.Pawn);
		yield return new WaitForSeconds(0.5f);
		_storyObjects.MorphMorphy(eMoveType.Rook);
		yield return new WaitForSeconds(0.5f);
		_storyObjects.MorphMorphy(eMoveType.Bishop);
		yield return new WaitForSeconds(0.5f);
		_storyObjects.MorphMorphy(eMoveType.Knight);
		yield return new WaitForSeconds(0.5f);
		_storyObjects.MorphMorphy(eMoveType.King);

		yield return new WaitForSeconds(1.5f);
		_storyObjects.TransformMorphyBack();
		yield return new WaitForSeconds(1.0f);

		_storyCanvas.ShowTextPanel("One day...");
		yield return new WaitForSeconds(1.0f);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(1.0f);
		_storyObjects.SpawnInEvilPurpleOrb(3.5f, 1.5f);
        yield return new WaitForSeconds(2.0f);
		_shouldContinueStory = false;
        MoveToAction moveEvilPurpleOrbToQueen = new MoveToAction(
            _storyObjects.EvilPurpleOrbTransform,
            _storyObjects.QueenTransform.position,
            0.75f,
            Utils.CurveSmoothStep);
		moveEvilPurpleOrbToQueen.OnActionFinish += ContinueStory;
		_storyCanvas.ShowTextPanel("Morphy's Queen got\npossessed!!!");
        ActionHandler.RunAction(moveEvilPurpleOrbToQueen);
        while (!_shouldContinueStory) { yield return null; }

        _storyObjects.CameraShake(50, 1.0f, 0.75f);
        yield return new WaitForSeconds(0.7f);
		_storyCanvas.HideTextPanel();
		_shouldContinueStory = false;
		Vector3 morphyPos = _storyObjects.MorphyTransform.position;
		MoveToAction hopUp = new MoveToAction(_storyObjects.MorphyTransform, morphyPos + (Vector3.up * 2.0f), 0.15f, Utils.CurveInverseExponential);
		ShakeAction shakeMorphy = new ShakeAction(_storyObjects.MorphyTransform, 25, 0.1f);
		shakeMorphy.SetShakeByDuration(0.35f, 40);
		MoveToAction hopDown = new MoveToAction(_storyObjects.MorphyTransform, morphyPos, 0.10f, Utils.CurveInverseExponential);
		ActionSequence hopSeq = new ActionSequence(hopUp, shakeMorphy, hopDown);
		hopSeq.OnActionFinish += ContinueStory;
		ActionHandler.RunAction(hopSeq);

		while (!_shouldContinueStory) { yield return null; }
        _storyCanvas.ShowTextPanel("Oh no!");
		yield return new WaitForSeconds(1.0f);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(0.5f);
		_storyCanvas.ShowTextPanel("\">:(\"");

		yield return new WaitForSeconds(1.0f);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(0.5f);
		Vector3 displacement = new Vector3(7.0f, 0.0f, 0.0f);
		MoveByAction moveMorphy = new MoveByAction(_storyObjects.MorphyTransform, displacement, 3.0f, Utils.CurveSmoothStep);
		MoveByAction moveQueen = new MoveByAction(_storyObjects.QueenTransform, displacement, 3.0f, Utils.CurveSmoothStep);
		MoveByAction moveEvilPurpleOrb = new MoveByAction(_storyObjects.EvilPurpleOrbTransform, displacement, 3.0f, Utils.CurveSmoothStep);
		ActionParallel moveAll = new ActionParallel(moveMorphy, moveQueen, moveEvilPurpleOrb);
		_shouldContinueStory = false;
		moveAll.OnActionFinish += ContinueStory;
		ActionHandler.RunAction(moveAll);
		_storyObjects.FocusCameraTo(8.5f, 1.5f, 3.0f);

		yield return new WaitForSeconds(0.5f);
		_storyCanvas.ShowTextPanel("Morphy tries to save his Queen!");
		yield return new WaitForSeconds(1.5f);
		_storyCanvas.HideTextPanel();
		while (!_shouldContinueStory) { yield return null; }

		float alpha = 1.0f;
		const float fadeSpeed = 0.75f;
		while (alpha > 0.0f)
		{
			alpha -= fadeSpeed * Time.deltaTime;
			_storyObjects.SetQueenAlpha(alpha);
			yield return null;
		}
		_storyObjects.StopEvilPurpleOrb();
		_storyCanvas.ShowTextPanel("But he couldn't save her...");
		yield return new WaitForSeconds(2.0f);
		_storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(0.5f);
		_storyCanvas.ShowTextPanel("\":(\"");
        yield return new WaitForSeconds(1.0f);
        _storyCanvas.ShowTextPanel("\":'(\"");
        yield return new WaitForSeconds(1.0f);
        _storyCanvas.ShowTextPanel("\":''''''''''''(\"");
        yield return new WaitForSeconds(1.0f);
        _storyCanvas.HideTextPanel();

		yield return new WaitForSeconds(0.5f);
		_storyObjects.CameraShake(50, 0.5f, 1.5f);

		yield return new WaitForSeconds(1.6f);
		_storyObjects.ChangeTileToStairs(9, 7);

		yield return new WaitForSeconds(0.5f);
        _storyCanvas.ShowTextPanel("Oh? What's this!");
		yield return new WaitForSeconds(1.0f);

		_shouldContinueStory = false;
		MoveToAction moveMorphyToStairs = new MoveToAction(
			_storyObjects.MorphyTransform,
			_storyObjects.MorphyTransform.position + new Vector3(0.0f, 0.0f, 2.0f),
            2.0f,
			Utils.CurveSmoothStep);
		moveMorphyToStairs.OnActionFinish += ContinueStory;
		ActionHandler.RunAction(moveMorphyToStairs);
		while (!_shouldContinueStory) { yield return null; }

		_storyCanvas.ShowTextPanel("And so begins\nMorphy's pursuit");
		yield return new WaitForSeconds(2.5f);
		_storyCanvas.ShowTextPanel("Through the...\nChessgeons!");

		yield return new WaitForSeconds(1.0f);
		_shouldContinueStory = false;
		_storyCanvas.SetContinueText("Let's Begin!");
		_storyCanvas.SetContinueVisible(true);
		while (!_shouldContinueStory) { yield return null; }
		_shouldContinueStory = false;
		DismissStory();
	}
}
