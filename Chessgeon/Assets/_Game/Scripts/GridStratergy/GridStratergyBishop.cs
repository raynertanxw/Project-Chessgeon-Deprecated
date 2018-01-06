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

	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();
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
