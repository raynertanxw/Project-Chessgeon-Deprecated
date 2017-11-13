using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabDungeonTile = null;
	[SerializeField] private Dungeon _dungeon = null;

	private const float TILE_WIDTH = 1.0f;
	private const float TILE_HALF_WIDTH = TILE_WIDTH / 2.0f;
	private const float ORIGIN_X = 0.0f;
	private const float ORIGIN_Y = 0.0f;
	
	public float TileWidth { get { return TILE_WIDTH; } }
	public float TileHalfWidth { get { return TILE_HALF_WIDTH; } }
	public float OriginX { get { return ORIGIN_X; } }
	public float OriginY { get { return ORIGIN_Y; } }

	private DungeonTile[,] _dungeonTiles = null;

	private void Awake()
	{
		Debug.Assert(_prefabDungeonTile != null, "_prefabDungeonTile is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_dungeonTiles = new DungeonTile[_dungeon.MaxX, _dungeon.MaxY];
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

	public void GenerateFloorTerrain(Floor inFloor)
	{
		Debug.Log("Generating Floor Terrain of size: (" + inFloor.Size.x + ", " + inFloor.Size.y + ")");

		// Hide ALL tiles.
		HideAllTiles();

		// Set all others as basic tiles.
		for (int y = 0; y < (inFloor.Size.y); y++)
		{
			for (int x = 0; x < (inFloor.Size.x); x++)
			{
				_dungeonTiles[x, y].SetTileType(DungeonTile.eType.Basic);
				_dungeonTiles[x, y].SetVisible(true);
			}
		}

		// Set the stairs tile.
		_dungeonTiles[inFloor.StairsPos.x, inFloor.StairsPos.y].SetTileType(DungeonTile.eType.Stairs);

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	public Vector3 GetTileTransformPosition(int inPosX, int inPosY)
	{
		return _dungeonTiles[inPosX, inPosY].transform.position;
	}
}
