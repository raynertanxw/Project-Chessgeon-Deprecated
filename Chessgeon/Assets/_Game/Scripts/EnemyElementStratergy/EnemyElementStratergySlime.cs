﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElementStratergySlime : EnemyElementStratergy
{
	public override void SpecialKillEvents(Enemy _enemy)
	{
		// TODO: implement this.
	}

	public override void SpecialTakeDamageEvents()
	{
		// TODO: implement this.
	}

	public override int GetDamagePower()
	{
		return 2;
	}

	public override int GetNumCoinsReward()
	{
		return 1; // TODO: Implement this.
	}
}