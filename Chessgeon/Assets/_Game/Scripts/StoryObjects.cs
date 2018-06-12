using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void SetActive(bool inIsActive)
	{
		if (inIsActive && !_isSetUp) SetUpStory();
		gameObject.SetActive(inIsActive);
	}
}
