using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPurpleOrb : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particleSystem = null;

	private Renderer _ren = null;

	private void Awake()
	{
		Debug.Assert(_particleSystem != null, "_particleSystem is not assigned.");
		Debug.Assert(!_particleSystem.main.playOnAwake, "EvilPurpleOrb _particleSystem should not play on Awake");
		Debug.Assert(!_particleSystem.main.prewarm, "EvilPurpleOrb _particleSystem should not prewarm.");
		Debug.Assert(_particleSystem.main.loop, "EvilPurpleOrb _particleSystem should loop.");

		_ren = _particleSystem.GetComponent<Renderer>();
		SetVisible(false);
	}

	public void StartParticleEffect()
	{
		_particleSystem.Play();
	}

	public void StopParticleEffect()
	{
		_particleSystem.Stop();
	}

	// TODO: Do proper handling of this in StoryObject and StoryController.
	public void SetVisible(bool inIsVisible)
	{
		if (inIsVisible
		    && _ren.enabled != inIsVisible)
		{
			// NOTE: Clear before appearing.
			_particleSystem.Clear();
		}
		_ren.enabled = inIsVisible;
	}
}
