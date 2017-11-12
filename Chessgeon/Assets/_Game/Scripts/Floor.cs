using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
	public enum eTileState { Empty, Stairs, Blocked, Enemy };

	private eTileState[,] _tileStates;
	private Vector2Int _size;
	private Vector2Int _stairsPos;

	public eTileState[,] TileStates { get { return _tileStates; } }
	public Vector2Int Size { get { return _size; } }
	public Vector2Int StairsPos { get { return _stairsPos; } }

	public Floor(int inMinX, int inMaxX, int inMinY, int inMaxY, DungeonTile.eZone inZone)
	{
		_size = new Vector2Int(Random.Range(inMinX, inMaxX), Random.Range(inMinY, inMaxY));

		_tileStates = new eTileState[Size.x, Size.y];
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				_tileStates[x, y] = eTileState.Empty;
			}
		}

		_stairsPos = new Vector2Int(Random.Range(1, Size.x - 2), Random.Range(1, Size.y - 2));
		_tileStates[StairsPos.x, StairsPos.y] = eTileState.Stairs;

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	public bool IsTileEmpty(int inX, int inY)
	{
		return (TileStates[inX, inY] == eTileState.Empty);
	}

	public static bool IsTileWhite(int inX, int inY)
	{
		if (inX % 2 == 0) return (inY % 2 == 0) ? false : true;
		else return (inY % 2 == 0) ? true : false;
	}
}
