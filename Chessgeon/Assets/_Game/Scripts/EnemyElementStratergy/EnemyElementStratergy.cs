using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyElementStratergy
{
	public abstract void SpecialKillEvents(Enemy _enemy);
	public abstract void SpecialTakeDamageEvents();
	public abstract int GetDamagePower();
	public abstract int GetNumCoinsReward();
}
