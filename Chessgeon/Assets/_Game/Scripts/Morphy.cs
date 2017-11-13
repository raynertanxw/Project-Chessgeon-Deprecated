using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphy : MonoBehaviour
{
	public enum eType { Pawn, Rook, Bishop, Knight, King, Morphy }

	[SerializeField] private Mesh _meshMorphy = null;
	[SerializeField] private Mesh _meshPiecePawn = null;
	[SerializeField] private Mesh _meshPieceRook = null;
	[SerializeField] private Mesh _meshPieceBishop = null;
	[SerializeField] private Mesh _meshPieceKnight = null;
	[SerializeField] private Mesh _meshPieceKing = null;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private eType _currentType;
	private bool _isAlive = false;
	public bool IsAlive { get { return _isAlive; } }

	private void Awake()
	{
		Debug.Assert(_meshMorphy != null, "_meshMorphy is not assigned.");
		Debug.Assert(_meshPiecePawn != null, "_meshPiecePawn is not assigned.");
		Debug.Assert(_meshPieceRook != null, "_meshPieceRook is not assigned.");
		Debug.Assert(_meshPieceBishop != null, "_meshPieceBishop is not assigned.");
		Debug.Assert(_meshPieceKnight != null, "_meshPieceKnight is not assigned.");
		Debug.Assert(_meshPieceKing != null, "_meshPieceKing is not assigned.");

		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();
	}

	public void SetType(eType inType)
	{
		_meshRenderer.material.SetColor("_Color", Color.green);

		switch(inType)
		{
			case eType.Pawn:
			{
				_meshFilter.mesh = _meshPiecePawn;
				break;
			}
			case eType.Rook:
			{
				_meshFilter.mesh = _meshPieceRook;
				break;
			}
			case eType.Bishop:
			{
				_meshFilter.mesh = _meshPieceBishop;
				break;
			}
			case eType.Knight:
			{
				_meshFilter.mesh = _meshPieceKnight;
				break;
			}
			case eType.King:
			{
				_meshFilter.mesh = _meshPieceKing;
				break;
			}
			case eType.Morphy:
			{
				_meshFilter.mesh = _meshMorphy;
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inType.ToString() + " has not been handled.");
				break;
			}
		}
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

		SetType(eType.Morphy);
		// TODO: Set all the health, stats, etc here?
	}
}
