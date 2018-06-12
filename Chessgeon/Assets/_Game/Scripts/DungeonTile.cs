using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
	public enum eType { Basic, Wall, Stairs };

	[SerializeField] private Mesh _meshTileBasic = null;
	[SerializeField] private Mesh _meshTileWall = null;
	[SerializeField] private Mesh _meshTileStairs = null;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private TileManager _tileManager = null;

	private bool _isInitialised = false;
	private bool _isVisible = true;
	private int _indexX = -1;
	private int _indexY = -1;
	private eType _type = eType.Basic;
	public eType Type { get { return _type; } }

	private void Awake()
	{
		Debug.Assert(_meshTileBasic != null, "_meshTileBasic is not assigned.");
		Debug.Assert(_meshTileWall != null, "_meshTileWall is not assigned.");

		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");
	}

	public void Initialise(TileManager inTileManager, int inIndexX, int inIndexY)
	{
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
                _tileManager.OriginX + TileManager.TileHalfWidth + (_indexX * TileManager.TileWidth),
				0.0f,
                _tileManager.OriginY + TileManager.TileHalfWidth + (_indexY * TileManager.TileWidth));
		}
	}

	public void SetType(eType inType)
	{
		_type = inType;

		switch (inType)
		{
			case eType.Basic:
			{
				_meshFilter.mesh = _meshTileBasic;
				if (Floor.IsTileWhite(_indexX, _indexY)) _meshRenderer.material.SetColor("_Color", Color.white);
				else _meshRenderer.material.SetColor("_Color", Color.black);
				break;
			}
			case eType.Wall:
			{
				_meshFilter.mesh = _meshTileWall;
				if (Floor.IsTileWhite(_indexX, _indexY)) _meshRenderer.material.SetColor("_Color", new Color(0.6f, 0.6f, 0.6f));
				else _meshRenderer.material.SetColor("_Color", new Color(0.4f, 0.4f, 0.4f));
				break;
			}
			case eType.Stairs:
			{
				_meshFilter.mesh = _meshTileStairs;
				_meshRenderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inType.ToString() + "has not been handled.");
				break;
			}
		}
	}

	public void SetVisible(bool inIsVisible)
	{
		if (_isVisible == inIsVisible) return;

		_isVisible = inIsVisible;
		_meshRenderer.enabled = _isVisible;
	}
}
