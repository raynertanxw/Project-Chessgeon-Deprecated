using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyStratergyPawn : MorphyStratergy
{
	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Floor inFloor)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int up = inPos;
			up.y += 1;
			if (IsValidPawnNonCaptureMove(up, inFloor)) possibleMoves.Add(up);
		}

		{
			Vector2Int down = inPos;
			down.y += -1;
			if (IsValidPawnNonCaptureMove(down, inFloor)) possibleMoves.Add(down);
		}

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 1;
			upLeft.x += -1;
			if (IsValidPawnCapture(upLeft, inFloor)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 1;
			upRight.x += 1;
			if (IsValidPawnCapture(upRight, inFloor)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -1;
			downLeft.x += -1;
			if (IsValidPawnCapture(downLeft, inFloor)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -1;
			downRight.x += 1;
			if (IsValidPawnCapture(downRight, inFloor)) possibleMoves.Add(downRight);
		}

		return possibleMoves.ToArray();
	}



	private bool IsValidPawnNonCaptureMove(Vector2Int inPos, Floor inFloor)
	{
		if (inFloor.IsValidPos(inPos))
		{
			return inFloor.IsTileOfState(inPos,
				Floor.eTileState.Empty,
				Floor.eTileState.Stairs); // TODO: What about hidden tiles? Future consideration.
		}
		else
		{
			return false;
		}
	}

	private bool IsValidPawnCapture(Vector2Int inPos, Floor inFloor)
	{
		if (inFloor.IsValidPos(inPos))
		{
			return inFloor.IsTileOfState(inPos, Floor.eTileState.Enemy);
		}
		else
		{
			return false;
		}
	}
}
