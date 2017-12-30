﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eMoveType { Pawn, Rook, Bishop, Knight, King }
public class Floor
{
	public enum eTileState { Empty, Stairs, Blocked, Enemy, Morphy };

	private Dungeon _dungeon = null;

	private eTileState[,] _tileStates;
	private Enemy[,] _enemies; // TODO: Next time.
	private Vector2Int _size;
	private Vector2Int _stairsPos;
	private Vector2Int _morphyPos;
	private int _floorNum;

	public Vector2Int Size { get { return _size; } }
	public Vector2Int StairsPos { get { return _stairsPos; } }
	public Vector2Int MorphyPos { get {return _morphyPos; } }
	public int FloorNum { get { return _floorNum; } }

	public Floor(Dungeon inDungeon)
	{
		_dungeon = inDungeon;
	}

	public void GenerateAndSetupNewFloor(int inMinX, int inMaxX, int inMinY, int inMaxY, DungeonTile.eZone inZone, int inFloorNum)
	{
		_floorNum = inFloorNum;

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

		_dungeon.TileManager.SetUpFloorTerrain();

        // Spawn the enemies.
		_enemies = new Enemy[Size.x, Size.y];
        int numEnemiesToSpawn = Size.x * Size.y / 10;
        int numEnemies = 0;

        while (numEnemies < numEnemiesToSpawn)
        {
            Vector2Int newEnemyPos = new Vector2Int(Random.Range(0, Size.x), Random.Range(0, Size.y));
            if (IsTileEmpty(newEnemyPos))
			{
				_enemies[newEnemyPos.x, newEnemyPos.y] = _dungeon.EnemyManager.SpawnEnemyAt(newEnemyPos);
				SetTileState(newEnemyPos, Floor.eTileState.Enemy);

				numEnemies++;
			}
		}

        // Spawn the player.
        while (true)
		{
            Vector2Int morphySpawnPos = new Vector2Int(Random.Range(0, Size.x), Random.Range(0, Size.y));
            if (IsTileEmpty(morphySpawnPos))
            {
                SetTileState(morphySpawnPos, Floor.eTileState.Morphy);
				_morphyPos = morphySpawnPos;
                break;
            }
        }
		_dungeon.MorphyController.SetUpPlayer();

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	private void SetTileState(Vector2Int inPos, eTileState inTileState) { SetTileState(inPos.x, inPos.y, inTileState); }
	private void SetTileState(int inX, int inY, eTileState inTileState)
	{
		_tileStates[inX, inY] = inTileState;
	}

	public Enemy GetEnemyAt(Vector2Int inPos) { return GetEnemyAt(inPos.x, inPos.y); }
	public Enemy GetEnemyAt(int inX, int inY)
	{
		Enemy enemyAtPos = _enemies[inX, inY];
		Debug.Assert(enemyAtPos != null, "There is no enemy at pos: (" + inX + ", " + inY + ")");

		return enemyAtPos;
	}

	public void MoveMorphyTo(Vector2Int inTargetPos)
    {
        Debug.Assert(IsValidMorphyMove(inTargetPos), inTargetPos + " is not a valid Morphy move!");

        if (_dungeon.CurrentFloor.IsTileOfState(inTargetPos, Floor.eTileState.Stairs))
        {
            // TODO: Anything for Floor to do when Morphy reaches stairs?	
        }
        else
        {
            if (IsTileOfState(inTargetPos, Floor.eTileState.Enemy)) // Player killed enemy.
            {
				_enemies[inTargetPos.x, inTargetPos.y] = null;
            }

            SetTileState(inTargetPos, Floor.eTileState.Morphy);
            SetTileState(MorphyPos, Floor.eTileState.Empty);
			_morphyPos = inTargetPos;
        }
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
			if (IsValidPos(inX, inY)) Debug.Log(inX + ", " + inY + " is of state: " + _tileStates[inX, inY].ToString());
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
