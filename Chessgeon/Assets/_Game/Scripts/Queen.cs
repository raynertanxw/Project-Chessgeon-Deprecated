using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class Queen : MonoBehaviour
{
	[SerializeField] private MorphParticle _queenMorphParticles = null;

	private void Awake()
	{
		Debug.Assert(_queenMorphParticles != null, "_queenMorphParticles is not assigned.");
	}

	public void PlayMorphAnimation()
	{
		_queenMorphParticles.PlayMorphEffect();
	}
}
