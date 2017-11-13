using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyStratergyKnight : MorphyStratergy
{
	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Morphy.eType inType, Floor inFloor)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 2;
			upLeft.x += -1;
			if (inFloor.IsValidMorphyMove(upLeft)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 2;
			upRight.x += 1;
			if (inFloor.IsValidMorphyMove(upRight)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int rightUp = inPos;
			rightUp.y += 1;
			rightUp.x += 2;
			if (inFloor.IsValidMorphyMove(rightUp)) possibleMoves.Add(rightUp);
		}

		{
			Vector2Int rightDown = inPos;
			rightDown.y += -1;
			rightDown.x += 2;
			if (inFloor.IsValidMorphyMove(rightDown)) possibleMoves.Add(rightDown);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -2;
			downRight.x += 1;
			if (inFloor.IsValidMorphyMove(downRight)) possibleMoves.Add(downRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -2;
			downLeft.x += -1;
			if (inFloor.IsValidMorphyMove(downLeft)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int leftDown = inPos;
			leftDown.y += -1;
			leftDown.x += -2;
			if (inFloor.IsValidMorphyMove(leftDown)) possibleMoves.Add(leftDown);
		}

		{
			Vector2Int leftUp = inPos;
			leftUp.y += 1;
			leftUp.x += -2;
			if (inFloor.IsValidMorphyMove(leftUp)) possibleMoves.Add(leftUp);
		}

		return possibleMoves.ToArray();
	}
}