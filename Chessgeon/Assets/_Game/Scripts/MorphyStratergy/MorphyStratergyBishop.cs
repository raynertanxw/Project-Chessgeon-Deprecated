using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyStratergyBishop : MorphyStratergy
{
	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Morphy.eType inType, Floor inFloor)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int upLeft = inPos;
			upLeft.y += 1;
			upLeft.x += -1;
			if (inFloor.IsValidMorphyMove(upLeft)) possibleMoves.Add(upLeft);
		}

		{
			Vector2Int upRight = inPos;
			upRight.y += 1;
			upRight.x += 1;
			if (inFloor.IsValidMorphyMove(upRight)) possibleMoves.Add(upRight);
		}

		{
			Vector2Int downLeft = inPos;
			downLeft.y += -1;
			downLeft.x += -1;
			if (inFloor.IsValidMorphyMove(downLeft)) possibleMoves.Add(downLeft);
		}

		{
			Vector2Int downRight = inPos;
			downRight.y += -1;
			downRight.x += 1;
			if (inFloor.IsValidMorphyMove(downRight)) possibleMoves.Add(downRight);
		}

		return possibleMoves.ToArray();
	}
}
