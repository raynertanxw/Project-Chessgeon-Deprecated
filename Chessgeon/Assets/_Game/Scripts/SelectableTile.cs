using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileSelectedEvent : UnityEvent<Vector2Int>
{
}

public class SelectableTile : MonoBehaviour
{
	private MeshRenderer _meshRenderer = null;
	private BoxCollider _collider = null;

	private Vector2Int _tilePos;
	public TileSelectedEvent OnTileSelected;

	private void Awake()
	{
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();
		_collider = gameObject.GetComponent<BoxCollider>();

		OnTileSelected = new TileSelectedEvent();
	}

	private void OnMouseDown()
	{
		OnTileSelected.Invoke(_tilePos);
		OnTileSelected.RemoveAllListeners();
	}

	public void SetAt(Vector2Int inTilePos, Vector3 inTileTransformPos, params UnityAction<Vector2Int>[] inTileSelectedActions)
	{
		_tilePos = inTilePos;
		transform.position = inTileTransformPos;
		OnTileSelected.RemoveAllListeners();
		for (int iAction = 0; iAction < inTileSelectedActions.Length; iAction++)
		{
			OnTileSelected.AddListener(inTileSelectedActions[iAction]);
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
