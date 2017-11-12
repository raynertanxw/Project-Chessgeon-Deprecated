﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public enum eType { Pawn, Rook, Bishop, Knight, King }
	public enum eElement { Basic }

	[SerializeField] private Mesh _meshPiecePawn = null;
	[SerializeField] private Mesh _meshPieceRook = null;
	[SerializeField] private Mesh _meshPieceBishop = null;
	[SerializeField] private Mesh _meshPieceKnight = null;
	[SerializeField] private Mesh _meshPieceKing = null;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private EnemyManager _enemyManager = null;

	private bool _isInitialised = false;
	private bool _isAlive = false;
	public bool IsAlive { get { return _isAlive; } }
	private eType _type = eType.Pawn;
	private eElement _element = eElement.Basic;

	private void Awake()
	{
		Debug.Assert(_meshPiecePawn != null, "_meshPiecePawn is not assigned.");
		Debug.Assert(_meshPieceRook != null, "_meshPieceRook is not assigned.");
		Debug.Assert(_meshPieceBishop != null, "_meshPieceBishop is not assigned.");
		Debug.Assert(_meshPieceKnight != null, "_meshPieceKnight is not assigned.");
		Debug.Assert(_meshPieceKing != null, "_meshPieceKing is not assigned.");

		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");
	}

	public void Initialise(EnemyManager inEnemyManager)
	{
		if (_isInitialised)
		{
			Debug.LogWarning("Trying to intialise Enemy when it is already initialised");
		}
		else
		{
			_enemyManager = inEnemyManager;
			// TODO: Next time all the set up for particle systems and such? If any and all, needing to turn them off, etc.
		}
	}

	public void SetEnemy(eType inType, eElement inElement)
	{
		SetEnemyElement(inElement);
		SetEnemyType(inType);
	}

	public void SetEnemyType(eType inType)
	{
		_type = inType;

		switch (inType)
		{
			case eType.Pawn:
			{
				_meshFilter.mesh = _meshPiecePawn;
				// TODO: Set the movement strategies here.
				break;
			}
			case eType.Rook:
			{
				_meshFilter.mesh = _meshPieceRook;
				// TODO: Set the movement strategies here.
				break;
			}
			case eType.Bishop:
			{
				_meshFilter.mesh = _meshPieceBishop;
				// TODO: Set the movement strategies here.
				break;
			}
			case eType.Knight:
			{
				_meshFilter.mesh = _meshPieceKnight;
				// TODO: Set the movement strategies here.
				break;
			}
			case eType.King:
			{
				_meshFilter.mesh = _meshPieceKing;
				// TODO: Set the movement strategies here.
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inType.ToString() + "has not been handled.");
				break;
			}
		}
	}

	public void SetEnemyElement(eElement inElement)
	{
		_element = inElement;

		// TODO: Implement this!
	}

	public void Hide()
	{
		_isAlive = false;
		_meshRenderer.enabled = false;
	}

	public void SpawnAt(Vector3 inSpawnPos)
	{
		_isAlive = true;
		transform.position = inSpawnPos;
		_meshRenderer.enabled = true;

		// TODO: Reset the health and all that stuff.
	}
}
