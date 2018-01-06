using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridStratergyPawn : GridStratergy
{
	public GridStratergyPawn(int inSizeX, int inSizeY, Floor inFloor)
	{
		base._sizeX = inSizeX;
		base._sizeY = inSizeY;
		base._floor = inFloor;
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

	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int up = inPos;
			up.y += 1;
			if (IsValidPawnNonCaptureMove(up, inMoveEntity)) possibleMoves.Add(up);
		}

		{
			Vector2Int down = inPos;
			down.y += -1;
			if (IsValidPawnNonCaptureMove(down, inMoveEntity)) possibleMoves.Add(down);
		}

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 1;
			upLeft.x += -1;
			if (IsValidPawnCapture(upLeft, inMoveEntity)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 1;
			upRight.x += 1;
			if (IsValidPawnCapture(upRight, inMoveEntity)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -1;
			downLeft.x += -1;
			if (IsValidPawnCapture(downLeft, inMoveEntity)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -1;
			downRight.x += 1;
			if (IsValidPawnCapture(downRight, inMoveEntity)) possibleMoves.Add(downRight);
		}

		return possibleMoves.ToArray();
	}



	private bool IsValidPawnNonCaptureMove(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		if (_floor.IsValidPos(inPos))
		{
			if (inMoveEntity == eMoveEntity.Morphy)
			{
				return _floor.IsTileOfState(inPos,
				Floor.eTileState.Empty,
				Floor.eTileState.Stairs); // TODO: What about hidden tiles? Future consideration.
			}
			else if (inMoveEntity == eMoveEntity.Enemy)
			{
				return _floor.IsTileOfState(inPos,
					Floor.eTileState.Empty); // TODO: What about hidden tiles? Future consideration.
			}
			else
			{
				Debug.LogError("Did not handle eMoveEntity type: " + inMoveEntity.ToString());
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private bool IsValidPawnCapture(Vector2Int inPos, eMoveEntity inMoveEntity)
	{
		if (_floor.IsValidPos(inPos))
		{
			if (inMoveEntity == eMoveEntity.Morphy)
			{
				return _floor.IsTileOfState(inPos, Floor.eTileState.Enemy);
			}
			else if (inMoveEntity == eMoveEntity.Enemy)
			{
				return _floor.IsTileOfState(inPos, Floor.eTileState.Morphy);
			}
			else
			{
				Debug.LogError("Did not handle eMoveEntity type: " + inMoveEntity.ToString());
				return false;
			}
		}
		else
		{
			return false;
		}
	}
}
