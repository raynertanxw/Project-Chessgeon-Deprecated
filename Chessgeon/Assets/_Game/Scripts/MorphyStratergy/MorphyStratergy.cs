using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MorphyStratergy
{
	public abstract Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Morphy.eType inType, Floor inFloor);
}
