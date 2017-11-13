using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyStratergyRook : MorphyStratergy
{
	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Floor inFloor)
	{
		List<Vector2Int> possibleMoves = new List<Vector2Int>();

		{
			Vector2Int up = inPos;
			up.y += 1;
			if (inFloor.IsValidMorphyMove(up)) possibleMoves.Add(up);
		}

		{
			Vector2Int down = inPos;
			down.y += -1;
			if (inFloor.IsValidMorphyMove(down)) possibleMoves.Add(down);
		}

		{
			Vector2Int left = inPos;
			left.x += -1;
			if (inFloor.IsValidMorphyMove(left)) possibleMoves.Add(left);
		}

		{
			Vector2Int right = inPos;
			right.x += 1;
			if (inFloor.IsValidMorphyMove(right)) possibleMoves.Add(right);
		}

		return possibleMoves.ToArray();
	}
}
