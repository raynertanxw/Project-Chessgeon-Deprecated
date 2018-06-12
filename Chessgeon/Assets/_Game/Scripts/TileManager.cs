using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TileManager : MonoBehaviour
{
	[SerializeField] private GameObject _prefabDungeonTile = null;
	[SerializeField] private GameObject _prefabSelectableTile = null;
	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private GameObject _boardScrollerImage = null;
	public Dungeon Dungeon { get { return _dungeon; } }

	private const float TILE_WIDTH = 1.0f;
	private const float TILE_HALF_WIDTH = TILE_WIDTH / 2.0f;
	private const float ORIGIN_X = 0.0f;
	private const float ORIGIN_Y = 0.0f;
	
	public static float TileWidth { get { return TILE_WIDTH; } }
	public static float TileHalfWidth { get { return TILE_HALF_WIDTH; } }
	public float OriginX { get { return ORIGIN_X; } }
	public float OriginY { get { return ORIGIN_Y; } }

	private DungeonTile[,] _dungeonTiles = null;
	private List<SelectableTile> _selectableTiles = null;
    private Dictionary<int, SelectableTile> _selectableTileDict = null;

	private void Awake()
	{
		Debug.Assert(_prefabDungeonTile != null, "_prefabDungeonTile is not assigned.");
		Debug.Assert(_prefabSelectableTile != null, "_prefabSelectableTile is not assigned.");
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
		Debug.Assert(_boardScrollerImage != null, "_boardScrollerImage is not assigned.");

		_dungeonTiles = new DungeonTile[_dungeon.MaxX, _dungeon.MaxY];
		for (int x = 0; x < _dungeonTiles.GetLength(0); x++)
		{
			for (int y = 0; y < _dungeonTiles.GetLength(1); y++)
			{
				DungeonTile newDungeonTile = GameObject.Instantiate(_prefabDungeonTile).GetComponent<DungeonTile>();
				newDungeonTile.transform.SetParent(this.transform);
				newDungeonTile.Initialise(this, x, y);
				newDungeonTile.SetType(DungeonTile.eType.Basic);

				_dungeonTiles[x, y] = newDungeonTile;
			}
		}

		_selectableTiles = new List<SelectableTile>();
        _selectableTileDict = new Dictionary<int, SelectableTile>();
		for (int iSelectable = 0; iSelectable < (Dungeon.MaxX + Dungeon.MaxY); iSelectable++)
		{
			CreateNewSelectableTile();
		}

		HideAllTiles();
	}

	private SelectableTile CreateNewSelectableTile()
	{
		SelectableTile newSelectableTile = GameObject.Instantiate(_prefabSelectableTile).GetComponent<SelectableTile>();
		newSelectableTile.transform.SetParent(this.transform);
		newSelectableTile.Initialise(this);

		_selectableTiles.Add(newSelectableTile);
		_selectableTileDict.Add(newSelectableTile.gameObject.GetInstanceID(), newSelectableTile);

		return newSelectableTile;
	}

	RaycastHit _hitInfo;
    Ray _ray;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			pointerData.position = Input.mousePosition;
			
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);
			
			bool blocked = false;
			if (results.Count > 0)
			{
				for (int iHit = 0; iHit < results.Count; iHit++)
				{
					if (results[iHit].gameObject.layer == Constants.LAYER_UI
						&& results[iHit].gameObject != _boardScrollerImage)
					{
						blocked = true;
						break;
					}
				}
			}

			if (!blocked)
			{
				_hitInfo = new RaycastHit();

				const float MAX_DIST = 1000.0f;
				_ray = DungeonCamera.ActiveCamera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(_ray, out _hitInfo, MAX_DIST, Constants.LAYER_MASK_DUNGEON_INTERACTABLE))
				{
					_selectableTileDict[_hitInfo.collider.gameObject.GetInstanceID()].SelectTile();
				}
			}
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

		HideAllSelectableTiles();
	}

	public void HideAllSelectableTiles()
	{
		for (int iSelectable = 0; iSelectable < _selectableTiles.Count; iSelectable++)
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
                _dungeonTiles[x, y].SetType(DungeonTile.eType.Basic);
                _dungeonTiles[x, y].SetVisible(true);
            }
        }

        // Set the stairs tile.
		_dungeonTiles[_dungeon.CurrentFloor.StairsPos.x, _dungeon.CurrentFloor.StairsPos.y].SetType(DungeonTile.eType.Stairs);

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
			if (iMove < _selectableTiles.Count)
			{
				_selectableTiles[iMove].SetAt(inPossibleMoves[iMove], inTileSelectedAction);
			}
			else
			{
				CreateNewSelectableTile().SetAt(inPossibleMoves[iMove], inTileSelectedAction);
			}
		}
	}
}
