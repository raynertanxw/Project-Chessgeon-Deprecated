using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabDungeonTile = null;

	private const float TILE_WIDTH = 1.0f;
	private const float TILE_HALF_WIDTH = TILE_WIDTH / 2.0f;
	private const float ORIGIN_X = 0.0f;
	private const float ORIGIN_Y = 0.0f;
	
	public float TileWidth { get { return TILE_WIDTH; } }
	public float TileHalfWidth { get { return TILE_HALF_WIDTH; } }
	public float OriginX { get { return ORIGIN_X; } }
	public float OriginY { get { return ORIGIN_Y; } }

	private bool _isInitialised = false;
	private DungeonTile[,] _dungeonTiles = null;
	private Dungeon _dungeon = null;

	private void Awake()
	{
		Debug.Assert(_prefabDungeonTile != null, "_prefabDungeonTile is not assigned.");

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");
		_isInitialised = false;
	}

	public void Initialise(int inMaxX, int inMaxY, Dungeon inDungeon)
	{
		// If initialised, don't do anything.
		if (_isInitialised)
		{
			Debug.LogWarning("Trying to initialise TileManager when it is already initialised");
		}
		else
		{
			_dungeon = inDungeon;

			_dungeonTiles = new DungeonTile[inMaxX + 2, inMaxY + 2]; // +2 cause of the bounding edges.
			for (int x = 0; x < _dungeonTiles.GetLength(0); x++)
			{
				for (int y = 0; y < _dungeonTiles.GetLength(1); y++)
				{
					DungeonTile newDungeonTile = GameObject.Instantiate(_prefabDungeonTile).GetComponent<DungeonTile>();
					newDungeonTile.transform.SetParent(this.transform);
					newDungeonTile.Initialise(this, x, y);
					newDungeonTile.SetTile(DungeonTile.eType.Basic, DungeonTile.eZone.Classic);

					_dungeonTiles[x, y] = newDungeonTile;
				}
			}

			HideAllTiles();
		}
	}

	private void HideAllTiles()
	{
		for (int x = 0; x < _dungeonTiles.GetLength(0); x++)
		{
			for (int y = 0; y < _dungeonTiles.GetLength(1); y++)
			{
				_dungeonTiles[x, y].SetVisible(false);
			}
		}
	}

	public void GenerateFloorTerrain(int inFloorX, int inFloorY)
	{
		Debug.Log("Generating Floor Terrain of size: (" + inFloorX + ", " + inFloorY + ")");

		// Hide ALL tiles.
		HideAllTiles();

		// Set up the boundary.
		for (int y = 0; y < (inFloorY + 2); y++)
		{
			_dungeonTiles[0, y].SetTileType(DungeonTile.eType.Wall);
			_dungeonTiles[0, y].SetVisible(true);

			_dungeonTiles[inFloorX + 1, y].SetTileType(DungeonTile.eType.Wall);
			_dungeonTiles[inFloorX + 1, y].SetVisible(true);
		}
		for (int x = 1; x < (inFloorX + 1); x++)
		{
			_dungeonTiles[x, 0].SetTileType(DungeonTile.eType.Wall);
			_dungeonTiles[x, 0].SetVisible(true);

			_dungeonTiles[x, inFloorY + 1].SetTileType(DungeonTile.eType.Wall);
			_dungeonTiles[x, inFloorY + 1].SetVisible(true);
		}

		// Set all others as basic tiles.
		for (int y = 1; y < (inFloorY + 1); y++)
		{
			for (int x = 1; x < (inFloorX + 1); x++)
			{
				_dungeonTiles[x, y].SetTileType(DungeonTile.eType.Basic);
				_dungeonTiles[x, y].SetVisible(true);
			}
		}

		// Set the stairs tile.
		GetTileByPos(_dungeon.StairsPosX, _dungeon.StairsPosY).SetTileType(DungeonTile.eType.Stairs);

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	private DungeonTile GetTileByPos(int inPosX, int inPosY)
	{
		return _dungeonTiles[inPosX + 1, inPosY + 1];
	}
}
