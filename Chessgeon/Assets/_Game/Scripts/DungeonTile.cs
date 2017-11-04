using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
	public enum eType { Basic, Wall };
	public enum eZone { Classic };

	[SerializeField] private Mesh _meshTileBasic = null;
	[SerializeField] private Mesh _meshTileWall = null;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private TileManager _tileManager = null;

	private bool _isInitialised = false;
	private bool _isVisible = true;
	private int _indexX = -1;
	private int _indexY = -1;
	private eType _type = eType.Basic;
	private eZone _zone = eZone.Classic;

	private void Awake()
	{
		Debug.Assert(_meshTileBasic != null, "_meshTileBasic is not assigned.");
		Debug.Assert(_meshTileWall != null, "_meshTileWall is not assigned.");

		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");
		_isInitialised = false;
	}

	public void Initialise(TileManager inTileManager, int inIndexX, int inIndexY)
	{
		// If initialised, don't do anything.
		if (_isInitialised)
		{
			Debug.LogWarning("Trying to initialise DungeonTile when it is already initialised");
		}
		else
		{
			_tileManager = inTileManager;
			_indexX = inIndexX;
			_indexY = inIndexY;

			transform.rotation = Quaternion.identity;
			transform.position = new Vector3( // Note: Y "pos" is used for Z cause it is flat down.
				_tileManager.OriginX + _tileManager.TileHalfWidth + (_indexX * _tileManager.TileWidth),
				0.0f,
				_tileManager.OriginY + _tileManager.TileHalfWidth + (_indexY * _tileManager.TileWidth));
		}
	}

	public void SetTile(eType inType, eZone inZone)
	{
		SetTileZone(inZone);
		SetTileType(inType);
	}

	// TODO: Remember to change the material based on the zone!
	public void SetTileType(eType inType)
	{
		switch (inType)
		{
			case eType.Basic:
			{
				_meshFilter.mesh = _meshTileBasic;
				if (IsWhiteTile) _meshRenderer.material.SetColor("_Color", Color.white);
				else _meshRenderer.material.SetColor("_Color", Color.black);
				break;
			}
			case eType.Wall:
			{
				_meshFilter.mesh = _meshTileWall;
				if (IsWhiteTile) _meshRenderer.material.SetColor("_Color", new Color(0.6f, 0.6f, 0.6f));
				else _meshRenderer.material.SetColor("_Color", new Color(0.4f, 0.4f, 0.4f));
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inType.ToString() + "has not been handled.");
				break;
			}
		}
	}

	public void SetTileZone(eZone inZone)
	{
		switch (inZone)
		{
			case eZone.Classic:
			{
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inZone.ToString() + "has not been handled.");
				break;
			}
		}
	}

	private bool IsWhiteTile
	{
		get
		{
			Debug.Assert(_indexX != -1 && _indexY != -1, "_indexX or _indexY has not been intialised!");
			if (_indexX % 2 == 0) return (_indexY % 2 == 0) ? true : false;
			else return (_indexY % 2 == 0) ? false : true;
		}
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		_meshRenderer.enabled = _isVisible;
	}
}
