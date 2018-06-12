using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class StoryObjects : MonoBehaviour
{
	[SerializeField] private Camera _storyCam = null;
	[SerializeField] private Morphy _storyMorphy = null;

	[Header("Other Managers")]
	[SerializeField] private Dungeon _dungeon = null;

	[Header("Tiles")]
	[SerializeField] private Transform _storyTileParent = null;
	[SerializeField] private GameObject _prefabStoryTile = null;

	private void Awake()
	{
		Debug.Assert(_storyCam != null, "_storyCam is not assigned.");
		Debug.Assert(_storyMorphy != null, "_storyMorphy is not assigned.");

		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		Debug.Assert(_storyTileParent != null, "_storyTileParent is not assigned.");
		Debug.Assert(_prefabStoryTile != null, "_prefabStoryTile is not assigned.");
	}

	private bool _isSetUp = false;
	private void SetUpStory()
	{
		if (!_isSetUp)
		{
			GenerateTiles();
			_storyMorphy.gameObject.SetActive(false);
		}
	}

	private void GenerateTiles()
	{
		for (int x = 0; x < _dungeon.MaxX; x++)
        {
			for (int y = 0; y < _dungeon.MaxY; y++)
            {
				StoryTile newStoryTile = GameObject.Instantiate(_prefabStoryTile).GetComponent<StoryTile>();
				newStoryTile.transform.SetParent(_storyTileParent);
				newStoryTile.SetIndex(x, y);
            }
        }
	}

	public void SpawnInMorphy(float inX, float inY)
	{
		_storyMorphy.gameObject.SetActive(true);
		_storyMorphy.transform.position = new Vector3(
			inX,
			_storyMorphy.transform.position.y,
			inY);
		_storyMorphy.TransformBackToMorphy();
		_storyMorphy.PlayMorphAnimation();
	}

	public void MorphMorphy(eMoveType inMoveType)
	{
		_storyMorphy.SetType(inMoveType);
		_storyMorphy.PlayMorphAnimation();
	}

	public void TransformMorphyBack()
	{
		_storyMorphy.TransformBackToMorphy();
		_storyMorphy.PlayMorphAnimation();
	}

	public void SetActive(bool inIsActive)
	{
		if (inIsActive && !_isSetUp) SetUpStory();
		gameObject.SetActive(inIsActive);
	}

	public void FocusCameraTo(float inX, float inZ, float inDuration, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimted = true)
	{
		Vector3 targetPos = new Vector3(
			inX,
			_storyCam.transform.position.y,
			inZ);

		if (inIsAnimted)
		{
			MoveToAction moveToFocus = new MoveToAction(_storyCam.transform, targetPos, inDuration, Utils.CurveSmoothStep);
			if (inOnComplete != null) moveToFocus.OnActionFinish += () => { inOnComplete(); };
			ActionHandler.RunAction(moveToFocus);
		}
		else
		{
			_storyCam.transform.position = targetPos;
		}
	}

	public void CameraShake(int inNumShakes, float inIntensity, float inDuration)
	{
		ShakeAction camShake = new ShakeAction(_storyCam.transform, inNumShakes, inIntensity, Utils.CurveInverseLinear);
		camShake.SetShakeByDuration(inDuration, inNumShakes);
		ActionHandler.RunAction(camShake);
	}
}
