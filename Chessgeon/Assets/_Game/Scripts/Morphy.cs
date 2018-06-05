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

	[SerializeField] private MorphParticle _morphParticle = null;
	[SerializeField] private MeshRenderer _shieldMeshRen = null;

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

		Debug.Assert(_morphParticle != null, "_morphParticle is not assigned.");
		Debug.Assert(_shieldMeshRen != null, "_shieldMeshRen is not assigned.");

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
			//ActionHandler.RunAction(new ActionParallel(
			//	new ActionRepeatForever(new RotateByAction(_shieldMeshRen.transform, new Vector3(0.0f, 360.0f, 0.0f), 7.5f)),
			//	new ActionRepeatForever(new PulseAction(
			//		_shieldMeshRen.transform,
			//		1,
			//		10.0f,
			//		Vector3.one * 0.95f,
			//		Vector3.one * 1.05f,
			//		Utils.CurveSmoothStep)))
			//);
			ActionHandler.RunAction(new ActionRepeatForever(new RotateByAction(_shieldMeshRen.transform, new Vector3(0.0f, 360.0f, 0.0f), 7.5f)));
			ToggleShieldVisibility(false, false, null);
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

	public void PlayMorphAnimation()
	{
		_morphParticle.PlayMorphEffect();
	}

	public void Hide()
	{
		_isAlive = false;
		_meshRenderer.enabled = false;
		ToggleShieldVisibility(false, false, null);
	}

	public void ToggleShieldVisibility(bool inIsVisible, bool inIsAimated = true, DTJob.OnCompleteCallback inOnComplete = null)
	{
		if (inIsAimated)
		{
			Transform shieldTransform = _shieldMeshRen.transform;
			if (!_shieldMeshRen.enabled && inIsVisible)
			{
				_shieldMeshRen.enabled = inIsVisible;
				shieldTransform.localScale = Vector3.zero;
				ScaleToAction scaleUp = new ScaleToAction(shieldTransform, Vector3.one, 0.5f, Utils.CurveBobber);
				if (inOnComplete != null) scaleUp.OnActionFinish += () => { inOnComplete(); };
				ActionHandler.RunAction(scaleUp);
			}
			else if (_shieldMeshRen.enabled & !inIsVisible)
			{
				shieldTransform.localScale = Vector3.one;
				ScaleToAction scaleUp = new ScaleToAction(shieldTransform, Vector3.one * 3.0f, 0.2f, Utils.CurveInverseExponential);
				ScaleToAction scaleDown = new ScaleToAction(shieldTransform, Vector3.zero, 0.1f, Utils.CurveExponential);
				ActionSequence loseShieldSeq = new ActionSequence(scaleUp, scaleDown);
				if (inOnComplete != null) loseShieldSeq.OnActionFinish += () =>
				{
					_shieldMeshRen.enabled = inIsVisible;
					inOnComplete();
				};
				ActionHandler.RunAction(loseShieldSeq);
			}
			else if (inIsVisible)
			{
				PulseAction pulse = new PulseAction(
                    _shieldMeshRen.transform,
                    1,
                    0.2f,
                    Vector3.one,
                    Vector3.one * 1.1f,
					Utils.CurveInverseSmoothStep);
				if (inOnComplete != null) pulse.OnActionFinish += () => { inOnComplete(); };
				ActionHandler.RunAction(pulse);
			}
			else
			{
				if (inOnComplete != null) inOnComplete();
                _shieldMeshRen.enabled = inIsVisible;
			}
		}
		else
		{
			if (inOnComplete != null) inOnComplete();
			_shieldMeshRen.enabled = inIsVisible;
        }
	}

	public void SpawnAt(Vector2Int inSpawnPos)
	{
		_isAlive = true;
		_pos = inSpawnPos;
		transform.position = _morphyController.Dungeon.TileManager.GetTileTransformPosition(Pos);
		_meshRenderer.enabled = true;

		TransformBackToMorphy();
	}

	public void MoveTo(Vector2Int inTargetPos, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		_pos = inTargetPos;
		Vector3 targetTransformPos = _morphyController.Dungeon.TileManager.GetTileTransformPosition(Pos);
		float moveDuration = 0.6f;
		MoveToAction moveToTarget = new MoveToAction(this.transform, targetTransformPos, moveDuration, Utils.CurveSmoothStep);
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
			transform.position + new Vector3(0.0f, 2.5f, 0.0f),
			moveUpDuration,
			Utils.CurveInverseExponential);
		moveUp.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveUpDuration); };
		MoveToAction slamDown = new MoveToAction(
			this.transform,
			enemyTransformPos,
			0.1f,
			Utils.CurveExponential);
		slamDown.OnActionFinish += () => { DungeonCamera.CameraShake(15, 0.5f, 0.2f); };
		ActionSequence attackSeq = new ActionSequence(moveUp, new DelayAction(0.15f), slamDown);
		attackSeq.OnActionFinish += inTargetEnemy.Kill;
		if (inOnCompleteAction != null) attackSeq.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(attackSeq);
	}

	public void SmashAttack(Enemy[] inEnemies, Vector2Int[] inAllTilesInRange, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		float moveUpDuration = 0.4f;
		Vector3 originPos = transform.position;
		MoveToAction moveUp = new MoveToAction(
			this.transform,
			transform.position + new Vector3(0.0f, 2.5f, 0.0f),
			moveUpDuration,
			Utils.CurveInverseExponential);
		moveUp.OnActionStart += () => { DungeonCamera.FocusCameraToTile(Pos, moveUpDuration); };
		MoveToAction slamDown = new MoveToAction(
			this.transform,
			originPos,
			0.1f,
			Utils.CurveExponential);
		slamDown.OnActionFinish += () =>
		{
			DungeonCamera.CameraShake(35, 1.0f, 0.5f);

			for (int iPos = 0; iPos < inAllTilesInRange.Length; iPos++)
			{
				Vector3 spawnPos = _morphyController.Dungeon.TileManager.GetTileTransformPosition(inAllTilesInRange[iPos]);
				_morphyController.Dungeon.SmashParticlePool.SpawnInstanceAt(spawnPos);
			}

			Enemy curEnemy;
			for (int iEnemy = 0; iEnemy < inEnemies.Length; iEnemy++)
			{
				curEnemy = inEnemies[iEnemy];
				_morphyController.Dungeon.CurrentFloor.SmashEnemyAt(curEnemy.Pos);
                curEnemy.Kill();
			}
		};
		ActionSequence attackSeq = new ActionSequence(moveUp, new DelayAction(0.15f), slamDown, new DelayAction(1.0f));
		if (inOnCompleteAction != null) attackSeq.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(attackSeq);
	}
}
