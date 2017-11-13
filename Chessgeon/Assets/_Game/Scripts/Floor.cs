using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
	public enum eTileState { Empty, Stairs, Blocked, Enemy, Morphy };

	private eTileState[,] _tileStates;
	private Vector2Int _size;
	private Vector2Int _stairsPos;

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

	public void SetTileState(Vector2Int inPos, eTileState inTileState) { SetTileState(inPos.x, inPos.y, inTileState); }
	public void SetTileState(int inX, int inY, eTileState inTileState)
	{
		_tileStates[inX, inY] = inTileState;
	}

	public bool IsTileEmpty(Vector2Int inPos) { return IsTileEmpty(inPos.x, inPos.y); }
	public bool IsTileEmpty(int inX, int inY)
	{
		return (_tileStates[inX, inY] == eTileState.Empty);
	}

	public bool IsValidEnemyMove(Vector2Int inPos) { return IsValidEnemyMove(inPos.x, inPos.y); }
	public bool IsValidEnemyMove(int inX, int inY)
	{
		if (IsValidPos(inX, inY))
		{
			eTileState tileState = _tileStates[inX, inY];
			// TODO: Can ememy step on hidden tiles???
			return (tileState == eTileState.Empty
				|| tileState == eTileState.Morphy);
		}
		else
		{
			return false;
		}
	}

	public bool IsValidMorphyMove(Vector2Int inPos) { return IsValidMorphyMove(inPos.x, inPos.y); }
	public bool IsValidMorphyMove(int inX, int inY)
	{
		if (IsValidPos(inX, inY))
		{
			eTileState tileState = _tileStates[inX, inY];
			// TODO: Handle hidden tiles case, if we decide that it is another eTileState.
			return (tileState == eTileState.Empty
				|| tileState == eTileState.Enemy
				|| tileState == eTileState.Stairs);
		}
		else
		{
			return false;
		}
	}

	public bool IsValidPos(Vector2Int inPos) { return IsValidPos(inPos.x, inPos.y); }
	public bool IsValidPos(int inX, int inY)
	{
		return ((inX >= 0 && inX < Size.x) && (inY >= 0 && inY < Size.y));
	}

	public bool IsTileOfState(Vector2Int inPos, params eTileState[] inTileStates) { return IsTileOfState(inPos.x, inPos.y, inTileStates); }
	public bool IsTileOfState(int inX, int inY, params eTileState[] inTileStates)
	{
		for (int iState = 0; iState < inTileStates.Length; iState++)
		{
			if (_tileStates[inX, inY] == inTileStates[iState]) return true;
		}

		return false;
	}

	public static bool IsTileWhite(int inX, int inY)
	{
		if (inX % 2 == 0) return (inY % 2 == 0) ? false : true;
		else return (inY % 2 == 0) ? true : false;
	}
}
