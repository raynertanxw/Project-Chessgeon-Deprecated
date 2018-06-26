using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class Queen : MonoBehaviour
{
	[SerializeField] private MorphParticle _queenMorphParticles = null;
	[SerializeField] private MeshRenderer _meshRen = null;

	private void Awake()
	{
		Debug.Assert(_queenMorphParticles != null, "_queenMorphParticles is not assigned.");
		Debug.Assert(_meshRen != null, "_meshRen is not assigned.");
	}

	public void PlayMorphAnimation()
	{
		_queenMorphParticles.PlayMorphEffect();
	}

	public void SetAlpha(float inAlpha)
	{
		Color color = _meshRen.material.GetColor("_Color");
		color.a = inAlpha;
		_meshRen.material.SetColor("_Color", color);
	}
}
