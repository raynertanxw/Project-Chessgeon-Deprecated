using UnityEngine;
using System.Collections;

public abstract class GridStratergy
{
	// TODO: Change this to Vetcor2Int. the size
	protected int _sizeX;
	protected int _sizeY;
	protected Floor _floor;

	public abstract void GetNSetNodeNeighbours(Node _node);
	public abstract int HeuristicEstimatedCost(Node _curNode, Node _goalNode);
	public abstract int NeighbourPathCost(Node _curNode, Node _neighbourNode);
	public enum eMoveEntity { Morphy, Enemy }
	public abstract Vector2Int[] CalcPossibleMoves(Vector2Int inPos, eMoveEntity inMoveEntity);

	protected void AssignNeighbour(int inPosX, int inPosY, Node inNode, eMoveType inMoveType)
	{
		if (inPosX < 0 || inPosY < 0 || inPosX >= _sizeX || inPosY >= _sizeY)
		{
//			Debug.LogWarning("Failed Attempt to Assigne Node: Neighbour out of index.");
			return;
		}

		inNode.neighbours[(int)inMoveType].AddFirst(_floor.Nodes[inPosX, inPosY]);
	}
}
