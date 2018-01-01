using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyKnight : GridStratergy
{
	public GridStratergyKnight(int inSizeX, int inSizeY, Floor inFloor)
	{
		base._sizeX = inSizeX;
		base._sizeY = inSizeY;
		base._floor = inFloor;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours[(int)eMoveType.Knight] = new LinkedList<Node>();

		/*	0 2 0 3 0
		 * 	1 0 0 0 4
		 * 	0 0 X 0 0
		 * 	8 0 0 0 5
		 * 	0 7 0 6 0
		 * 
		 * Where X is (_node.PosX, _node.PosY)
		 */

		// 1
		AssignNeighbour(_node.PosX - 2, _node.PosY + 1, _node, eMoveType.Knight);
		// 2
		AssignNeighbour(_node.PosX - 1, _node.PosY + 2, _node, eMoveType.Knight);
		// 3
		AssignNeighbour(_node.PosX + 1, _node.PosY + 2, _node, eMoveType.Knight);
		// 4
		AssignNeighbour(_node.PosX + 2, _node.PosY + 1, _node, eMoveType.Knight);
		// 5
		AssignNeighbour(_node.PosX + 2, _node.PosY - 1, _node, eMoveType.Knight);
		// 6
		AssignNeighbour(_node.PosX + 1, _node.PosY - 2, _node, eMoveType.Knight);
		// 7
		AssignNeighbour(_node.PosX - 1, _node.PosY - 2, _node, eMoveType.Knight);
		// 8
		AssignNeighbour(_node.PosX - 2, _node.PosY - 1, _node, eMoveType.Knight);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		// Better to have actual square values, same as manhatten, otherwise, it biases in moving more,
		// if we were to give it a fixed neighbour path cost of 2 or 1 per move. 
		return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}
}
