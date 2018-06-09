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

	public override int HeuristicEstimatedCost(Node _curNode, Node _goalNode, Node _startNode)
	{
		int goalDiffX = Mathf.Abs(_curNode.PosX - _goalNode.PosX);
		int goalDiffY = Mathf.Abs(_curNode.PosY - _goalNode.PosY);

		int startDiffX = Mathf.Abs(_curNode.PosX - _startNode.PosX);
		int startDiffY = Mathf.Abs(_curNode.PosY - _startNode.PosY);

		return (Mathf.Abs(goalDiffX - goalDiffY) + Mathf.Abs(startDiffX - startDiffY));

		// NOTE: Old code. Doesn't encourage less turns.
		//return Mathf.Abs(_curNode.PosX - _goalNode.PosX)
		//	+ Mathf.Abs(_curNode.PosY - _goalNode.PosY);
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

		if (inMoveEntity == eMoveEntity.Enemy)
		{
			Vector2Int potentialPos;

			// Top Right
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x += 1;
				potentialPos.y += 1;
				if (_floor.IsValidEnemyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Morphy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Bottom Right
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x += 1;
				potentialPos.y -= 1;
				if (_floor.IsValidEnemyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Morphy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Top Left
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x -= 1;
				potentialPos.y += 1;
				if (_floor.IsValidEnemyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Morphy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Bottom Left
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x -= 1;
				potentialPos.y -= 1;
				if (_floor.IsValidEnemyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Morphy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}


			// NOTE: Old code.
			//{
			//	Vector2Int upLeft = inPos;
			//	upLeft.y += 1;
			//	upLeft.x += -1;
			//	if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upLeft)) possibleMoves.Add(upLeft);
			//}

			//{
			//	Vector2Int upRight = inPos;
			//	upRight.y += 1;
			//	upRight.x += 1;
			//	if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(upRight)) possibleMoves.Add(upRight);
			//}

			//{
			//	Vector2Int downLeft = inPos;
			//	downLeft.y += -1;
			//	downLeft.x += -1;
			//	if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downLeft)) possibleMoves.Add(downLeft);
			//}

			//{
			//	Vector2Int downRight = inPos;
			//	downRight.y += -1;
			//	downRight.x += 1;
			//	if (inMoveEntity == eMoveEntity.Enemy && _floor.IsValidEnemyMove(downRight)) possibleMoves.Add(downRight);
			//}
		}
		else if (inMoveEntity == eMoveEntity.Morphy)
		{
			Vector2Int potentialPos;

			// Top Right
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x += 1;
				potentialPos.y += 1;
				if (_floor.IsValidMorphyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Bottom Right
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x += 1;
				potentialPos.y -= 1;
				if (_floor.IsValidMorphyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Top Left
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x -= 1;
				potentialPos.y += 1;
				if (_floor.IsValidMorphyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}

			// Bottom Left
			potentialPos = inPos;
			while (true)
			{
				potentialPos.x -= 1;
				potentialPos.y -= 1;
				if (_floor.IsValidMorphyMove(potentialPos))
				{
					possibleMoves.Add(potentialPos);
					if (_floor.IsTileOfState(potentialPos, Floor.eTileState.Enemy) ||
						_floor.IsTileOfState(potentialPos, Floor.eTileState.Stairs))
					{
						break;
					}
				}
				else { break; }
			}
		}

		return possibleMoves.ToArray();
	}
}
