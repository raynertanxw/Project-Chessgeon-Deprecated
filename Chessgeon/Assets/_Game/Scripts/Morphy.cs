using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class Morphy : MonoBehaviour
{
	[SerializeField] private Mesh _meshMorphy = null;
	[SerializeField] private Mesh _meshPiecePawn = null;
	[SerializeField] private Mesh _meshPieceRook = null;
	[SerializeField] private Mesh _meshPieceBishop = null;
	[SerializeField] private Mesh _meshPieceKnight = null;
	[SerializeField] private Mesh _meshPieceKing = null;

	private bool _isInitialised = false;
	private MorphyController _morphyController = null;
	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;

	private bool _isAlive = false;
	public bool IsAlive { get { return _isAlive; } }
	private eMoveType _currentType;
	public eMoveType CurrentType { get { return _currentType; } }
	private bool _isInMorphyForm = true;
	public bool IsInMorphyForm { get { return _isInMorphyForm; } }
	private Vector2Int _pos;
	public Vector2Int Pos { get { return _pos; } }


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

		Debug.Assert(_isInitialised == false, "_isInitialised is true. Did you try to call Awake() twice, or after Initialise()?");

		TransformBackToMorphy();
	}

	public void Initialise(MorphyController inMorphyController)
	{
		if (_isInitialised)
		{
			Debug.LogWarning("Trying to intialise Enemy when it is already initialised");
		}
		else
		{
			_morphyController = inMorphyController;
			// TODO: Next time all the set up for particle systems and such? If any and all, needing to turn them off, etc.
		}
	}

	public void SetType(eMoveType inType)
	{
		_meshRenderer.material.SetColor("_Color", Color.green);
		_currentType = inType;
		_isInMorphyForm = false;

		switch(inType)
		{
			case eMoveType.Pawn:
			{
				_meshFilter.mesh = _meshPiecePawn;
				break;
			}
			case eMoveType.Rook:
			{
				_meshFilter.mesh = _meshPieceRook;
				break;
			}
			case eMoveType.Bishop:
			{
				_meshFilter.mesh = _meshPieceBishop;
				break;
			}
			case eMoveType.Knight:
			{
				_meshFilter.mesh = _meshPieceKnight;
				break;
			}
			case eMoveType.King:
			{
				_meshFilter.mesh = _meshPieceKing;
				break;
			}
			default:
			{
				Debug.LogWarning("case: " + inType.ToString() + " has not been handled.");
				break;
			}
		}
	}

	public void TransformBackToMorphy()
	{
		_isInMorphyForm = true;
		_meshRenderer.material.SetColor("_Color", Color.green);
		_meshFilter.mesh = _meshMorphy;
	}

	public void Hide()
	{
		_isAlive = false;
		_meshRenderer.enabled = false;
	}

	public void SpawnAt(Vector2Int inSpawnPos)
	{
		_isAlive = true;
		_pos = inSpawnPos;
		transform.position = _morphyController.Dungeon.TileManager.GetTileTransformPosition(Pos);
		_meshRenderer.enabled = true;

		TransformBackToMorphy();
		// TODO: Set all the health, stats, etc here?
	}

	public void MoveTo(Vector2Int inTargetPos, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		_pos = inTargetPos;
		Vector3 targetTransformPos = _morphyController.Dungeon.TileManager.GetTileTransformPosition(Pos);
		float moveDuration = 0.6f;
		MoveToAction moveToTarget = new MoveToAction(this.transform, Graph.SmoothStep, targetTransformPos, moveDuration);
		ActionAfterDelay moveAfterDelay = new ActionAfterDelay(moveToTarget, 0.1f);
		moveAfterDelay.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveDuration); };
		if (inOnCompleteAction != null) moveAfterDelay.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(moveAfterDelay);
	}

	public void MoveAndAttack(Vector2Int inTargetPos, Enemy inTargetEnemy, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		_pos = inTargetPos;
		Vector3 enemyTransformPos = _morphyController.Dungeon.TileManager.GetTileTransformPosition(Pos);
		float moveUpDuration = 0.4f;
		MoveToAction moveUp = new MoveToAction(
			this.transform,
			Graph.InverseExponential,
			transform.position + new Vector3(0.0f, 2.5f, 0.0f),
			moveUpDuration);
		moveUp.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveUpDuration); };
		MoveToAction slamDown = new MoveToAction(
			this.transform,
			Graph.Exponential,
			enemyTransformPos,
			0.1f);
		slamDown.OnActionFinish += () => { DungeonCamera.CameraShake(15, 0.5f, 0.2f); };
		ActionSequence attackSeq = new ActionSequence(moveUp, new DelayAction(0.15f), slamDown);
		attackSeq.OnActionFinish += inTargetEnemy.Kill;
		if (inOnCompleteAction != null) attackSeq.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(attackSeq);
	}
}
