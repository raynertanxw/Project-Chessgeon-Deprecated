using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphyStratergyMorphy : MorphyStratergy
{
	public override Vector2Int[] CalcPossibleMoves(Vector2Int inPos, Morphy.eType inType, Floor inFloor)
	{
		return new Vector2Int[0];
	}
}
