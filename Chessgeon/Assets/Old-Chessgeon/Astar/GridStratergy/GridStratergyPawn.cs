using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyPawn : GridStratergy
{
	public GridStratergyPawn(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		base._sizeX = _sizeX;
		base._sizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours[(int)eMoveType.Pawn] = new LinkedList<Node>();

		// Up
		AssignNeighbour(_node.PosX, _node.PosY + 1, _node, eMoveType.Pawn);
		// Down
		AssignNeighbour(_node.PosX, _node.PosY - 1, _node, eMoveType.Pawn);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		return Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
