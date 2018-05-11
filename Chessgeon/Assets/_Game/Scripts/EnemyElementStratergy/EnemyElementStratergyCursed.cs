using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElementStratergyCursed : EnemyElementStratergy
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
		return 999;
	}

	public override int GetNumCardsReward()
	{
		return 6;
	}
}
