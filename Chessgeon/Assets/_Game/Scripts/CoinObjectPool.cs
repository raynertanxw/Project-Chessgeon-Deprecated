using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class CoinObjectPool : GenericObjectPool
{
	private Dictionary<GameObject, MeshRenderer> _poolMeshRen = new Dictionary<GameObject, MeshRenderer>();

	protected override bool isObjectActive(int inObjectIndex)
	{
		return _poolMeshRen[_pool[inObjectIndex]].enabled;
	}

	public override void EnableObject(GameObject inObjectInstance, bool inEnable)
	{
		_poolMeshRen[inObjectInstance].enabled = inEnable;
	}

	protected override GameObject AddInstanceToPool()
	{
		GameObject curObject = base.AddInstanceToPool();
		_poolMeshRen.Add(curObject, curObject.GetComponent<MeshRenderer>());

		return curObject;
	}
}
