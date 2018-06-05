using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphParticle : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem = null;

	private const float ALIVE_DURATION = 0.5f;
	private float _aliveTime = 0.0f;
	public bool IsAlive { get; private set; }

	private void Awake()
	{
		Debug.Assert(_particleSystem != null, "_particleSystem is not assigned.");
		_aliveTime = 0.0f;
		IsAlive = false;
	}

	private void Update()
	{
		if (_particleSystem.isPlaying)
		{
			_aliveTime += Time.deltaTime;
			if (_aliveTime >= ALIVE_DURATION)
			{
				StopMorphEffect();
			}
		}
	}

	public void PlayMorphEffect()
	{
		_particleSystem.Stop();
		_particleSystem.Clear();
		_aliveTime = 0.0f;
		IsAlive = true;
		_particleSystem.Play();
	}

	public void StopMorphEffect()
	{
		IsAlive = false;
		_particleSystem.Stop();
		_particleSystem.Clear();
	}
}
