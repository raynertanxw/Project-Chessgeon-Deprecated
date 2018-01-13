using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabDungeonTile = null;
	[SerializeField] private GameObject _prefabSelectableTile = null;
	[SerializeField] private Dungeon _dungeon = null;
	public Dungeon Dungeon { get { return _dungeon; } }

	private const float TILE_WIDTH = 1.0f;
	private const float TILE_HALF_WIDTH = TILE_WIDTH / 2.0f;
	private const float ORIGIN_X = 0.0f;
	private const float ORIGIN_Y = 0.0f;
	
	public float TileWidth { get { return TILE_WIDTH; } }
	public float TileHalfWidth { get { return TILE_HALF_WIDTH; } }
	public float OriginX { get { return ORIGIN_X; } }
	public float OriginY { get { return ORIGIN_Y; } }

	private DungeonTile[,] _dungeonTiles = null;
	private SelectableTile[] _selectableTiles = null;

	private void Awake()
	{
		Debug.Assert(_prefabDungeonTile != null, "_prefabDungeonTile is not assigned.");
		Debug.Assert(_prefabSelectableTile != null, "_prefabSelectableTile is not assigned.");
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

		_selectableTiles = new SelectableTile[8];
		for (int iSelectable = 0; iSelectable < _selectableTiles.Length; iSelectable++)
		{
			SelectableTile newSelectableTile = GameObject.Instantiate(_prefabSelectableTile).GetComponent<SelectableTile>();
			newSelectableTile.transform.SetParent(this.transform);
			newSelectableTile.Initialise(this);

			_selectableTiles[iSelectable] = newSelectableTile;
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

		HideAllSelectableTiles();
	}

	public void HideAllSelectableTiles()
	{
		for (int iSelectable = 0; iSelectable < _selectableTiles.Length; iSelectable++)
		{
			_selectableTiles[iSelectable].Hide();
		}
	}

	public void SetUpFloorTerrain()
	{
		Debug.Log("Setting up Floor Terrain of size: (" + _dungeon.CurrentFloor.Size.x + ", " + _dungeon.CurrentFloor.Size.y + ")");

		// Hide ALL tiles.
		HideAllTiles();

		// Set all others as basic tiles.
		for (int y = 0; y < (_dungeon.CurrentFloor.Size.y); y++)
		{
			for (int x = 0; x < (_dungeon.CurrentFloor.Size.x); x++)
			{
				_dungeonTiles[x, y].SetTileType(DungeonTile.eType.Basic);
				_dungeonTiles[x, y].SetVisible(true);
			}
		}

		// Set the stairs tile.
		_dungeonTiles[_dungeon.CurrentFloor.StairsPos.x, _dungeon.CurrentFloor.StairsPos.y].SetTileType(DungeonTile.eType.Stairs);

		// TODO: Obstalces (if any)

		// TODO: Special tiles (if any)
	}

	public Vector3 GetTileTransformPosition(Vector2Int inPos) { return GetTileTransformPosition(inPos.x, inPos.y); }
	public Vector3 GetTileTransformPosition(int inPosX, int inPosY)
	{
		return _dungeonTiles[inPosX, inPosY].transform.position;
	}

	public void ShowPossibleMoves(Vector2Int[] inPossibleMoves, SelectableTile.OnTileSelectedDelegate inTileSelectedAction)
	{
		HideAllSelectableTiles();

		for (int iMove = 0; iMove < inPossibleMoves.Length; iMove++)
		{
			_selectableTiles[iMove].SetAt(inPossibleMoves[iMove], inTileSelectedAction);
		}
	}
}
