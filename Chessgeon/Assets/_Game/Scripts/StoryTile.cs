using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTile : MonoBehaviour
{
	[SerializeField] private MeshRenderer _meshRen = null;
	[SerializeField] private MeshFilter _meshFilter = null;

	private Vector2Int _pos = new Vector2Int(-1, -1);

	private void Awake()
	{
		Debug.Assert(_meshRen != null, "_meshRen is not assigned.");
		Debug.Assert(_meshFilter != null, "_meshFilter is not assigned.");
	}

	public void SetIndex(int inPosX, int inPosY)
	{
		_pos.x = inPosX;
		_pos.y = inPosY;

		transform.rotation = Quaternion.identity;
        transform.position = new Vector3( // Note: Y "pos" is used for Z cause it is flat down.
		                                 TileManager.TileHalfWidth + (inPosX * TileManager.TileWidth),
                                         0.0f,
		                                 TileManager.TileHalfWidth + (inPosY * TileManager.TileWidth));

		if (Floor.IsTileWhite(inPosX, inPosY)) _meshRen.material.SetColor("_Color", Color.white);
		else _meshRen.material.SetColor("_Color", Color.black);
	}

	public void ChangeMesh(Mesh inMesh)
	{
		_meshFilter.mesh = inMesh;
	}

	public void ChangeColor(Color inColor)
	{
		_meshRen.material.SetColor("_Color", inColor);
	}
}
