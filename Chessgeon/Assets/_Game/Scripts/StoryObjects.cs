﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class StoryObjects : MonoBehaviour
{
	[SerializeField] private Camera _storyCam = null;
	[SerializeField] private Morphy _storyMorphy = null;
	[SerializeField] private Queen _storyQueen = null;
	[SerializeField] private EvilPurpleOrb _evilPurpleOrb = null;
	[SerializeField] private SmashParticle _smashParticle = null;

	[Header("Other Managers")]
	[SerializeField] private Dungeon _dungeon = null;

	[Header("Tiles")]
	[SerializeField] private Transform _storyTileParent = null;
	[SerializeField] private GameObject _prefabStoryTile = null;
	[SerializeField] private Mesh _stairMesh = null;

	public Transform MorphyTransform { get { return _storyMorphy.transform; } }
	public Transform QueenTransform { get { return _storyQueen.transform; } }
	public Transform EvilPurpleOrbTransform { get { return _evilPurpleOrb.transform; } }

	private StoryTile[,] _storyTiles = null;

	private void Awake()
	{
		Debug.Assert(_storyCam != null, "_storyCam is not assigned.");
		Debug.Assert(_storyMorphy != null, "_storyMorphy is not assigned.");
		Debug.Assert(_storyQueen != null, "_storyQueen is not assigned.");
		Debug.Assert(_evilPurpleOrb != null, "_evilPurpleOrb is not assigned.");
		Debug.Assert(_smashParticle != null, "_smashParticle is not assigned.");

		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		Debug.Assert(_storyTileParent != null, "_storyTileParent is not assigned.");
		Debug.Assert(_prefabStoryTile != null, "_prefabStoryTile is not assigned.");
		Debug.Assert(_stairMesh != null, "_stairMesh is not assigned.");
	}

	private bool _isSetUp = false;
	private void SetUpStory()
	{
		if (!_isSetUp)
		{
			GenerateTiles();
			_storyMorphy.gameObject.SetActive(false);
			_storyQueen.gameObject.SetActive(false);
		}
	}

	private void GenerateTiles()
	{
		_storyTiles = new StoryTile[_dungeon.MaxX, _dungeon.MaxY];
		for (int x = 0; x < _dungeon.MaxX; x++)
        {
			for (int y = 0; y < _dungeon.MaxY; y++)
            {
				StoryTile newStoryTile = GameObject.Instantiate(_prefabStoryTile).GetComponent<StoryTile>();
				newStoryTile.transform.SetParent(_storyTileParent);
				newStoryTile.SetIndex(x, y);
				_storyTiles[x, y] = newStoryTile;
            }
        }
	}

	public void ChangeTileToStairs(int inX, int inY)
	{
		Debug.Assert(inX >= 0 && inX < _dungeon.MaxX, "inX is out of range.");
		Debug.Assert(inY >= 0 && inY < _dungeon.MaxY, "inY is out of range.");

		StoryTile stairTile = _storyTiles[inX, inY];
		stairTile.ChangeMesh(_stairMesh);
		stairTile.ChangeColor(new Color(0.5f, 0.5f, 0.5f));
		_smashParticle.transform.position = stairTile.transform.position;
		_smashParticle.PlaySmashEffect();
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

	public void SpawnInQueen(float inX, float inY)
	{
		_storyQueen.gameObject.SetActive(true);
		_storyQueen.SetAlpha(1.0f);
		_storyQueen.transform.position = new Vector3(
			inX,
			_storyQueen.transform.position.y,
			inY);
		_storyQueen.PlayMorphAnimation();
	}

	public void SetQueenAlpha(float inAlpha)
	{
		_storyQueen.SetAlpha(inAlpha);
	}

	public void SpawnInEvilPurpleOrb(float inX, float inY)
	{
		_evilPurpleOrb.SetVisible(true);
		_evilPurpleOrb.transform.position = new Vector3(
			inX,
			_evilPurpleOrb.transform.position.y,
			inY
		);
		_evilPurpleOrb.StartParticleEffect();
	}

	public void StopEvilPurpleOrb()
	{
		_evilPurpleOrb.StopParticleEffect();
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
