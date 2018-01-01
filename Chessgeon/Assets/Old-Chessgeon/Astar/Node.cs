using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public float nodePathCost;
    public float totalCost;	// To get Heuristic, totalCost - nodePathCost.
	private Floor.eTileState _tileState;
	public Floor.eTileState State { get { return _tileState; } }
	private Vector2Int _size;
	public Vector2Int Size { get { return _size; } }
	public int PosX { get { return _size.x; } }
	public int PosY { get { return _size.y; } }
	public Node parent;
	public LinkedList<Node>[] neighbours;

	public Node(int inPosX, int inPosY, Floor.eTileState inTileState)
    {
		_size = new Vector2Int(inPosX, inPosY);
		_tileState = inTileState;
		neighbours = new LinkedList<Node>[5];
		Reset();
    }

	public void UpdateState(Floor.eTileState inNewTileState)
	{
		_tileState = inNewTileState;
	}

    public int CompareTo(Node _otherNode)
    {
		if (this.totalCost < _otherNode.totalCost)
            return -1;
		if (this.totalCost > _otherNode.totalCost)
            return 1;

        return 0;
    }

	public void Reset()
	{
		this.nodePathCost = 1.0f;
		this.totalCost = 0.0f;
		this.parent = null;
	}
}
