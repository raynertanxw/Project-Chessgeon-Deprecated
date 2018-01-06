using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyKing : GridStratergy
{
	public GridStratergyKing(int inSizeX, int inSizeY, Floor inFloor)
	{
		base._sizeX = inSizeX;
		base._sizeY = inSizeY;
		base._floor = inFloor;
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

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 1;
			upLeft.x += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(upLeft)) possibleMoves.Add(upLeft);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upLeft)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 1;
			upRight.x += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(upRight)) possibleMoves.Add(upRight);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upRight)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -1;
			downLeft.x += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(downLeft)) possibleMoves.Add(downLeft);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downLeft)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -1;
			downRight.x += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(downRight)) possibleMoves.Add(downRight);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downRight)) possibleMoves.Add(downRight);
		}

		return possibleMoves.ToArray();
	}
}
