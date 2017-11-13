using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
	public enum eTileState { Empty, Stairs, Blocked, Enemy, Morphy };

	private eTileState[,] _tileStates;
	private Vector2Int _size;
	private Vector2Int _stairsPos;

	public eTileState[,] TileStates { get { return _tileStates; } }
	public Vector2Int Size { get { return _size; } }
	public Vector2Int StairsPos { get { return _stairsPos; } }

	public Floor(int inMinX, int inMaxX, int inMinY, int inMaxY, DungeonTile.eZone inZone)
	{
		// Here we + 1 the max becasue Random.Range(int) is inclusive exclusive.
		_size = new Vector2Int(Random.Range(inMinX, inMaxX + 1), Random.Range(inMinY, inMaxY + 1));

		_tileStates = new eTileState[Size.x, Size.y];
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				_tileStates[x, y] = eTileState.Empty;
			}
		}

		// Here it's -1 and not -2 becasue the max for Random.Range(int) is exclusive.
		_stairsPos = new Vector2Int(Random.Range(1, Size.x - 1), Random.Range(1, Size.y - 1));
		_tileStates[StairsPos.x, StairsPos.y] = eTileState.Stairs;

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	public bool IsTileEmpty(int inX, int inY)
	{
		return (TileStates[inX, inY] == eTileState.Empty);
	}

	public bool IsTileEmpty(Vector2Int inPos)
	{
		return IsTileEmpty(inPos.x, inPos.y);
	}

	public static bool IsTileWhite(int inX, int inY)
	{
		if (inX % 2 == 0) return (inY % 2 == 0) ? false : true;
		else return (inY % 2 == 0) ? true : false;
	}
}
