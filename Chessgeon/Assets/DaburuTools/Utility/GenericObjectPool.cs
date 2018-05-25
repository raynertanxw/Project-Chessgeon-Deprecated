using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaburuTools
{
	public class GenericObjectPool : MonoBehaviour
	{
		[SerializeField] protected int _poolSize = 10;
		[SerializeField] protected bool _isFlexiblePoolSize = true;
		[SerializeField] protected GameObject _poolObjectPrefab = null;

		protected List<GameObject> _pool = null;

		protected void Awake()
		{
			Debug.Assert(_poolSize > -1, "Pool Size is < 0.");
			Debug.Assert(_poolObjectPrefab != null, "_poolObjectPrefab is not assigned. Pool named: " + gameObject.name);
			Debug.Assert(_pool == null, "_pool is already initialised!");

			_pool = new List<GameObject>(_poolSize);
			for (int iObject = 0; iObject < _poolSize; iObject++)
			{
				AddInstanceToPool();
				EnableObject(iObject, false);
			}
		}

		// NOTE: Override if using a different way to detect i.e. other component's visual state.
		virtual protected bool isObjectActive(int inObjectIndex)
		{
			return _pool[inObjectIndex].activeInHierarchy;
		}

		// NOTE: Override if want to use a different way to enbale the object. i.e. toggling MeshRenderer.
		protected void EnableObject(int inObjectIndex, bool inEnable) { EnableObject(_pool[inObjectIndex], inEnable); }
		virtual public void EnableObject(GameObject inObjectInstance, bool inEnable)
		{
			inObjectInstance.SetActive(inEnable);
		}

		virtual protected GameObject AddInstanceToPool()
		{
			GameObject curObject = GameObject.Instantiate(_poolObjectPrefab);
			curObject.transform.SetParent(this.transform);
			_pool.Add(curObject);

			return curObject;
		}

		public GameObject GetInstance()
		{
			GameObject availableInstance = null;
			for (int iObject = 0; iObject < _poolSize; iObject++)
			{
				if (isObjectActive(iObject))
				{
					continue;
				}
				else
				{
					availableInstance = _pool[iObject];
					break;
				}
			}

			if (availableInstance == null && _isFlexiblePoolSize)
			{
				AddInstanceToPool();
				_poolSize++;
				EnableObject(_poolSize - 1, false);
				return _pool[_poolSize - 1];
			}

			return availableInstance;
		}

		public GameObject SpawnInstanceAt(Vector3 inSpawnPos) { return SpawnInstanceAt(inSpawnPos, true); }
		public GameObject SpawnInstanceAt(Vector3 inSpawnPos, bool inEnable)
		{
			GameObject instance = GetInstance();
			if (instance == null) return instance;
			else
			{
				instance.transform.position = inSpawnPos;
				EnableObject(instance, inEnable);
				return instance;
			}
		}
	}
}
