using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyRook : GridStratergy
{
	public GridStratergyRook(int inSizeX, int inSizeY, Floor inFloor)
	{
		base._sizeX = inSizeX;
		base._sizeY = inSizeY;
		base._floor = inFloor;
	}

	public override void GetNSetNodeNeighbours (Node _node)
	{
		_node.neighbours[(int)eMoveType.Rook] = new LinkedList<Node>();

		// Up
		AssignNeighbour(_node.PosX, _node.PosY + 1, _node, eMoveType.Rook);
		// Down
		AssignNeighbour(_node.PosX, _node.PosY - 1, _node, eMoveType.Rook);
		// Left
		AssignNeighbour(_node.PosX - 1, _node.PosY, _node, eMoveType.Rook);
		// Right
		AssignNeighbour(_node.PosX + 1, _node.PosY, _node, eMoveType.Rook);
	}

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode)
	{
		return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
	}

	public override int NeighbourPathCost(Node _curNode, Node _neighbourNode)
	{
		return Mathf.Abs(_curNode.PosX - _neighbourNode.PosX)
			+ Mathf.Abs(_curNode.PosY - _neighbourNode.PosY);
	}

	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int up = inPos;
			up.y += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(up)) possibleMoves.Add(up);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(up)) possibleMoves.Add(up);
		}

		{
			Vector2Int down = inPos;
			down.y += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(down)) possibleMoves.Add(down);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(down)) possibleMoves.Add(down);
		}

		{
			Vector2Int left = inPos;
			left.x += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(left)) possibleMoves.Add(left);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(left)) possibleMoves.Add(left);
		}

		{
			Vector2Int right = inPos;
			right.x += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(right)) possibleMoves.Add(right);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(right)) possibleMoves.Add(right);
		}

		return possibleMoves.ToArray();
	}
}
