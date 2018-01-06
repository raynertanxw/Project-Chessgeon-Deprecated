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

	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 2;
			upLeft.x += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(upLeft)) possibleMoves.Add(upLeft);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upLeft)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 2;
			upRight.x += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(upRight)) possibleMoves.Add(upRight);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upRight)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int rightUp = inPos;
			rightUp.y += 1;
			rightUp.x += 2;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(rightUp)) possibleMoves.Add(rightUp);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(rightUp)) possibleMoves.Add(rightUp);
		}

		{
			Vector2Int rightDown = inPos;
			rightDown.y += -1;
			rightDown.x += 2;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(rightDown)) possibleMoves.Add(rightDown);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(rightDown)) possibleMoves.Add(rightDown);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -2;
			downRight.x += 1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(downRight)) possibleMoves.Add(downRight);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downRight)) possibleMoves.Add(downRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -2;
			downLeft.x += -1;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(downLeft)) possibleMoves.Add(downLeft);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downLeft)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int leftDown = inPos;
			leftDown.y += -1;
			leftDown.x += -2;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(leftDown)) possibleMoves.Add(leftDown);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(leftDown)) possibleMoves.Add(leftDown);
		}

		{
			Vector2Int leftUp = inPos;
			leftUp.y += 1;
			leftUp.x += -2;
			if (inMoveEntity == eMoveEntity.Morphy && _floor.IsValidMorphyMove(leftUp)) possibleMoves.Add(leftUp);
			else if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(leftUp)) possibleMoves.Add(leftUp);
		}

		return possibleMoves.ToArray();
	}
}
