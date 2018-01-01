using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyKing : GridStratergy
{
	public GridStratergyKing(int _sizeX, int _sizeY, Node[,] _nodes)
	{
		base._sizeX = _sizeX;
		base._sizeY = _sizeY;
		nodes = _nodes;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours[(int)eMoveType.King] = new LinkedList<Node>();

		// Up
		AssignNeighbour(_node.PosX, _node.PosY + 1, _node, eMoveType.King);
		// Down
		AssignNeighbour(_node.PosX, _node.PosY - 1, _node, eMoveType.King);
		// Left
		AssignNeighbour(_node.PosX - 1, _node.PosY, _node, eMoveType.King);
		// Right
		AssignNeighbour(_node.PosX + 1, _node.PosY, _node, eMoveType.King);

		// Top-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY + 1, _node, eMoveType.King);
		// Top-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY + 1, _node, eMoveType.King);
		// Btm-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY - 1, _node, eMoveType.King);
		// Btm-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY - 1, _node, eMoveType.King);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		if (_curNode.neighbours[(int)eMoveType.King].Contains(_neighbourNode))
			return 1;
		else
			return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
				+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
