using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class Enemy : MonoBehaviour
{
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
	private eMoveType _type = eMoveType.Pawn;
	public eMoveType Type { get { return _type; } }
	private Vector2Int _pos;
	public Vector2Int Pos { get { return _pos; } }
	public enum eTurnAction { Move, Attack, Nothing }
	public struct EnemyTurnAction
	{
		public Enemy Enemy { get; private set; }
		public eTurnAction TurnAction { get; private set; }
		public Vector2Int TargetPos { get; private set; }
		public Utils.GenericVoidDelegate OnComplete { get; private set; }

		public EnemyTurnAction(Enemy inEnemy, eTurnAction inTurnAction, Vector2Int inTargetPos, Utils.GenericVoidDelegate inOnComplete)
		{
			Enemy = inEnemy;
			TurnAction = inTurnAction;
			TargetPos = inTargetPos;
			OnComplete = inOnComplete;
		}
	}

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

	public void SetType(eMoveType inType)
	{
		_type = inType;

		switch (inType)
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
				Debug.LogWarning("case: " + inType.ToString() + "has not been handled.");
				break;
			}
		}
	}

	public void Kill()
	{
		_isAlive = false;
		// TODO: Points and stuff.
		_meshRenderer.enabled = false;

		_enemyManager.Dungeon.CardManager.DrawCard(1, CheckIfFloorCleared);
	}

	private void CheckIfFloorCleared()
	{
		if (_enemyManager.GetArrayOfAliveEnemies().Length < 1)
		{
			_enemyManager.Dungeon.ClearFloor();
		}
	}

	public void Remove()
	{
		_isAlive = false;
		_meshRenderer.enabled = false;
	}

	public void SpawnAt(Vector2Int inSpawnPos)
	{
		_isAlive = true;
		_pos = inSpawnPos;
		transform.position = _enemyManager.Dungeon.TileManager.GetTileTransformPosition(Pos);
		_meshRenderer.enabled = true;

		// TODO: Reset the health and all that stuff.
	}

	public EnemyTurnAction ProcessTurn(Floor inCurrentFloor, Utils.GenericVoidDelegate inOnTurnExecuted, Utils.GenericVoidDelegate inOnProcessed)
	{
		Node morphyNode = inCurrentFloor.Nodes[inCurrentFloor.MorphyPos.x, inCurrentFloor.MorphyPos.y];
		LinkedList<Node> pathToMorphy = AStarManager.FindPath(
			inCurrentFloor.Nodes[Pos.x, Pos.y],
			morphyNode,
			inCurrentFloor,
			Type);
		EnemyTurnAction turnAction;
		if (pathToMorphy == null) // No path to morphy.
		{
			Vector2Int[] possibleMoves = inCurrentFloor.GridStratergyForMoveType(Type).CalcPossibleMoves(Pos, GridStratergy.eMoveEntity.Enemy);
			if (possibleMoves.Length == 0)
			{
				turnAction = new EnemyTurnAction(this, eTurnAction.Nothing, new Vector2Int(-1, -1), inOnTurnExecuted);
			}
			else
			{
				// NOTE: Calc using heuristic which of the possible moves is the closest.
				int closestMoveIndex = -1;
				int closestHeuristic = int.MaxValue;
				for (int iMove = 0; iMove < possibleMoves.Length; iMove++)
				{
					Vector2Int curMove = possibleMoves[iMove];
					GridStratergy gridStratergy = inCurrentFloor.GridStratergy[(int)Type];
					Node targetPosNode = inCurrentFloor.Nodes[curMove.x, curMove.y];
					int curHeuristic = gridStratergy.HeuristicEstimatedCost(targetPosNode, morphyNode);
					if (curHeuristic < closestHeuristic)
					{
						closestMoveIndex = iMove;
						closestHeuristic = curHeuristic;
					}
				}

				// NOTE: This is for pawn which is not able to path cause of the diagonal movement.
				Vector2Int targetPos = possibleMoves[closestMoveIndex];
				if (targetPos == inCurrentFloor.MorphyPos)
				{
					turnAction = new EnemyTurnAction(this, eTurnAction.Attack, targetPos, inOnTurnExecuted);
				}
				else
				{
					inCurrentFloor.MoveEnemy(Pos, targetPos);
					turnAction = new EnemyTurnAction(this, eTurnAction.Move, targetPos, inOnTurnExecuted);
				}
			}
		}
		else
		{
			Node nextNode = (Node)pathToMorphy.First.Next.Value;
			Vector2Int nextPos = new Vector2Int(nextNode.PosX, nextNode.PosY);
			if (nextPos == inCurrentFloor.MorphyPos)
			{
				if (Type == eMoveType.Pawn)
				{
					// NOTE: Just do nothing cause there isn't anything it can do except move furthur away.
					turnAction = new EnemyTurnAction(this, eTurnAction.Nothing, new Vector2Int(-1, -1), inOnTurnExecuted);
				}
				else
				{
					turnAction = new EnemyTurnAction(this, eTurnAction.Attack, nextPos, inOnTurnExecuted);
				}
			}
			else
			{
				inCurrentFloor.MoveEnemy(Pos, nextPos);
				turnAction = new EnemyTurnAction(this, eTurnAction.Move, nextPos, inOnTurnExecuted);
			}
		}

		if (inOnProcessed != null) inOnProcessed();
		return turnAction;
	}

	public void ExecuteTurnAction(EnemyTurnAction inTurnAction)
	{
		switch (inTurnAction.TurnAction)
		{
			case eTurnAction.Move:
				{
					AnimateMoveTo(inTurnAction.TargetPos, inTurnAction.OnComplete);
					break;
				}
			case eTurnAction.Attack:
				{
					AttackMorphy(inTurnAction.TargetPos, inTurnAction.OnComplete);
					break;
				}
			case eTurnAction.Nothing:
				{
					if (inTurnAction.OnComplete != null) inTurnAction.OnComplete();
					break;
				}
			default:
				{
					Debug.LogError(inTurnAction.TurnAction.ToString() + " has not been implemented in ExecuteTurnAction.");
					break;
				}
		}
	}
    
    private void AnimateMoveTo(Vector2Int inTargetPos, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		_pos = inTargetPos;
		Vector3 targetTransformPos = _enemyManager.Dungeon.TileManager.GetTileTransformPosition(Pos);
		float moveDuration = 0.6f;
		MoveToAction moveToTarget = new MoveToAction(this.transform, targetTransformPos, moveDuration, Utils.CurveSmoothStep);
		ActionAfterDelay moveAfterDelay = new ActionAfterDelay(moveToTarget, 0.1f);
		moveAfterDelay.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveDuration); };
		if (inOnCompleteAction != null) moveAfterDelay.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(moveAfterDelay);
	}

	private void AttackMorphy(Vector2Int inTargetPos, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		Vector3 originPos = this.transform.position;
		Vector3 morphyTransformPos = _enemyManager.Dungeon.TileManager.GetTileTransformPosition(inTargetPos);
		float moveUpDuration = 0.4f;
		MoveToAction moveUp = new MoveToAction(
			this.transform,
			transform.position + new Vector3(0.0f, 3.0f, 0.0f),
			moveUpDuration,
			Utils.CurveInverseExponential);
		moveUp.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveUpDuration); };
		MoveToAction slamDown = new MoveToAction(
			this.transform,
			morphyTransformPos + (Vector3.up * 2.0f),
			0.075f,
			Utils.CurveExponential);
		slamDown.OnActionFinish += () => { _enemyManager.Dungeon.MorphyController.TakeDamage(1); };
		MoveToAction moveBack = new MoveToAction(
			this.transform,
			originPos,
			0.5f,
			Utils.CurveSmoothStep);
		ActionSequence attackSeq = new ActionSequence(moveUp, new DelayAction(0.15f), slamDown, moveBack);
		if (inOnCompleteAction != null) attackSeq.OnActionFinish += inOnCompleteAction;
		ActionHandler.RunAction(attackSeq);
	}
}
