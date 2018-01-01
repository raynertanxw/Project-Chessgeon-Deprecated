using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridType { Pawn, Rook, Bishop, Knight, King, Count }

public class GridManager
{
	private Vector2Int _size;
	private GridStratergy mGridAlgorithms;
	public GridStratergy GridAlgorithms { get { return mGridAlgorithms; } }
	private GridType mGridType;
	public GridType gridType { get { return mGridType; } }

	public GridManager(Vector2Int inSize, GridType _type)
    {
		_size = inSize;
		mGridType = _type;

  //      nodes = new Node[_size.x, _size.y];

		//switch (_type)
		//{
		//case GridType.Rook:
		//	mGridAlgorithms = new GridStratergyRook(_size.x, _size.y, nodes);
		//	break;
		//case GridType.Bishop:
		//	mGridAlgorithms = new GridStratergyBishop(_size.x, _size.y, nodes);
		//	break;
		//case GridType.Knight:
		//	mGridAlgorithms = new GridStratergyKnight(_size.x, _size.y, nodes);
		//	break;
		//case GridType.King:
		//	mGridAlgorithms = new GridStratergyKing(_size.x, _size.y, nodes);
		//	break;
		//case GridType.Pawn:
		//	mGridAlgorithms = new GridStratergyPawn(_size.x, _size.y, nodes);
		//	break;
		//}

  //      for (int y = 0; y < _size.y; y++)
  //      {
  //          for (int x = 0; x < _size.x; x++)
  //          {
		//		nodes[x, y] = new Node(_dungeon.DungeonBlocks[x, y]);
		//	}
  //      }

		//for (int y = 0; y < _size.y; y++)
		//{
		//	for (int x = 0; x < _size.x; x++)
		//	{
		//		mGridAlgorithms.GetNSetNodeNeighbours(nodes[x, y]);
		//	}
		//}
    }
}
