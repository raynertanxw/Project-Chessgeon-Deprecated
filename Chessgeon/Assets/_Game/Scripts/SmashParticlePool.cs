using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class SmashParticlePool : GenericObjectPool
{
	private Dictionary<GameObject, SmashParticle> _poolSmashParticle = new Dictionary<GameObject, SmashParticle>();

	protected override bool isObjectActive(int inObjectIndex)
	{
		return _poolSmashParticle[_pool[inObjectIndex]].IsAlive;
	}

	public override void EnableObject(GameObject inObjectInstance, bool inEnable)
	{
		if (inEnable)
		{
			_poolSmashParticle[inObjectInstance].PlaySmashEffect();
		}
		else
		{
			_poolSmashParticle[inObjectInstance].StopSmashEffect();
		}
	}

	protected override GameObject AddInstanceToPool()
	{
		GameObject curObject = base.AddInstanceToPool();
		_poolSmashParticle.Add(curObject, curObject.GetComponent<SmashParticle>());

		return curObject;
	}
}
