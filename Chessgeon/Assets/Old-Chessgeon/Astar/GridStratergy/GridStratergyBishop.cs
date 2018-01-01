using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyBishop : GridStratergy
{
	public GridStratergyBishop(int inSizeX, int inSizeY, Floor inFloor)
	{
		base._sizeX = inSizeX;
		base._sizeY = inSizeY;
		base._floor = inFloor;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours[(int)eMoveType.Bishop] = new LinkedList<Node>();

		// Top-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY + 1, _node, eMoveType.Bishop);
		// Top-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY + 1, _node, eMoveType.Bishop);
		// Btm-Right
		AssignNeighbour(_node.PosX + 1, _node.PosY - 1, _node, eMoveType.Bishop);
		// Btm-Left
		AssignNeighbour(_node.PosX - 1, _node.PosY - 1, _node, eMoveType.Bishop);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		if (_curNode.neighbours[(int)eMoveType.Bishop].Contains(_neighbourNode))
			return 1;
		else
			return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
				+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
