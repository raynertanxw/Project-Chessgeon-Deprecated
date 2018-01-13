using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableTile : MonoBehaviour
{
	private MeshRenderer _meshRenderer = null;
	private BoxCollider _collider = null;
	private TileManager _tileManager = null;

	private bool _isInitialised = false;
	private Vector2Int _tilePos;
	public delegate void OnTileSelectedDelegate(Vector2Int inSelectedTile);
	public OnTileSelectedDelegate OnTileSelected;

	private void Awake()
	{
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();
		_collider = gameObject.GetComponent<BoxCollider>();

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");
	}

	public void Initialise(TileManager inTileManager)
	{
		if (_isInitialised)
		{
			Debug.LogWarning("Trying to intialise Enemy when it is already initialised");
		}
		else
		{
			_tileManager = inTileManager;
			// TODO: Next time all the set up for particle systems and such? If any and all, needing to turn them off, etc.
		}
	}

	private void OnMouseDown()
	{
		OnTileSelected(_tilePos);
		OnTileSelected = null;
		_tileManager.HideAllSelectableTiles();
	}

	public void SetAt(Vector2Int inTilePos, params OnTileSelectedDelegate[] inTileSelectedActions)
	{
		_tilePos = inTilePos;
		transform.position = _tileManager.GetTileTransformPosition(_tilePos);
		OnTileSelected = null;
		for (int iAction = 0; iAction < inTileSelectedActions.Length; iAction++)
		{
			OnTileSelected += inTileSelectedActions[iAction];
		}

		SetVisible(true);
	}

	public void Hide()
	{
		SetVisible(false);
	}

	private void SetVisible(bool inIsVisible)
	{
		_meshRenderer.enabled = inIsVisible;
		_collider.enabled = inIsVisible;
	}
}
