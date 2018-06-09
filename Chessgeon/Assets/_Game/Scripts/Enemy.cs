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
	private bool _isAliveOnlyForGetter = false;
	public bool IsAlive { get { return _isAliveOnlyForGetter; } }
	private eMoveType _type = eMoveType.Pawn;
	public eMoveType Type { get { return _type; } }
	private Vector2Int _pos;
	public Vector2Int Pos { get { return _pos; } }

	private void Awake()
	{
		Debug.Assert(_meshPiecePawn != null, "_meshPiecePawn is not assigned.");
		Debug.Assert(_meshPieceRook != null, "_meshPieceRook is not assigned.");
		Debug.Assert(_meshPieceBishop != null, "_meshPieceBishop is not assigned.");
		Debug.Assert(_meshPieceKnight != null, "_meshPieceKnight is not assigned.");
		Debug.Assert(_meshPieceKing != null, "_meshPieceKing is not assigned.");

		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshRenderer = gameObject.GetComponent<MeshRenderer>();
		_isAliveOnlyForGetter = false;

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

	private void SetIsAlive(bool inIsAlive)
	{
		if (inIsAlive != _isAliveOnlyForGetter)
		{
			_isAliveOnlyForGetter = inIsAlive;
			_enemyManager.UpdateAliveCount(inIsAlive);
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
		SetIsAlive(false);
		// TODO: Points and stuff.
		_meshRenderer.enabled = false;

		_enemyManager.Dungeon.CardManager.DrawCard(1, () => { _enemyManager.Dungeon.TryClearFloor(); });
	}

	public void Remove()
	{
		SetIsAlive(false);
		_meshRenderer.enabled = false;
	}

	public void SpawnAt(Vector2Int inSpawnPos)
	{
		SetIsAlive(true);
		_pos = inSpawnPos;
		transform.position = _enemyManager.Dungeon.TileManager.GetTileTransformPosition(Pos);
		_meshRenderer.enabled = true;

		// TODO: Reset the health and all that stuff.
	}

	public void ProcessTurn(Floor inCurrentFloor, Utils.GenericVoidDelegate inOnTurnExecuted)
	{
		Node morphyNode = inCurrentFloor.Nodes[inCurrentFloor.MorphyPos.x, inCurrentFloor.MorphyPos.y];
		LinkedList<Node> pathToMorphy = AStarManager.FindPath(
			inCurrentFloor.Nodes[Pos.x, Pos.y],
			morphyNode,
			inCurrentFloor,
			Type);

		if (pathToMorphy == null) // No path to morphy.
		{
			Vector2Int[] possibleMoves = inCurrentFloor.GridStratergyForMoveType(Type).CalcPossibleMoves(Pos, GridStratergy.eMoveEntity.Enemy);
			if (possibleMoves.Length == 0)
			{
				// Do nothing.
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
					AttackMorphy(targetPos, inOnTurnExecuted);
				}
				else
				{
					MoveTo(targetPos, inCurrentFloor, inOnTurnExecuted);
				}
			}
		}
		else
		{
			if (Type == eMoveType.Rook)
			{
				LinkedListNode<Node> nextLLNode = pathToMorphy.First.Next;
				Node nextNode = (Node)nextLLNode.Value;
				Vector2Int nextPos = new Vector2Int(nextNode.PosX, nextNode.PosY);
				Vector2Int rookDiff = nextPos - Pos;

				Vector2Int oldPos = nextPos;
				while (true)
				{
					if (nextPos == inCurrentFloor.MorphyPos)
					{
						// TODO: Can pass here the move to pos before attacking. Just pass in the old Pos.
						AttackMorphy(nextPos, inOnTurnExecuted);
						break;
					}
					else
					{
						nextLLNode = nextLLNode.Next;
						nextNode = (Node)nextLLNode.Value;
						nextPos = new Vector2Int(nextNode.PosX, nextNode.PosY);

						if ((nextPos - oldPos) != rookDiff) // NOTE: Change in direction.
						{
							MoveTo(oldPos, inCurrentFloor, inOnTurnExecuted);
							break;
						}
						else
						{
							// NOTE: Continue until change in direction.
							oldPos = nextPos;
						}
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
					}
					else
					{
						AttackMorphy(nextPos, inOnTurnExecuted);
					}
				}
				else
				{
					MoveTo(nextPos, inCurrentFloor, inOnTurnExecuted);
				}
			}
		}
	}

    private void MoveTo(Vector2Int inTargetPos, Floor inCurrentFloor, Utils.GenericVoidDelegate inOnCompleteAction = null)
	{
		Vector3 targetTransformPos = _enemyManager.Dungeon.TileManager.GetTileTransformPosition(inTargetPos);
		float moveDuration = 0.6f;
		MoveToAction moveToTarget = new MoveToAction(this.transform, targetTransformPos, moveDuration, Utils.CurveSmoothStep);
		ActionAfterDelay moveAfterDelay = new ActionAfterDelay(moveToTarget, 0.1f);
		moveAfterDelay.OnActionStart += () => { DungeonCamera.FocusCameraToTile(inTargetPos, moveDuration); };
		if (inOnCompleteAction != null) moveAfterDelay.OnActionFinish += inOnCompleteAction;
		moveAfterDelay.OnActionFinish += () =>
		{
			inCurrentFloor.MoveEnemy(Pos, inTargetPos);
			_pos = inTargetPos;
		};
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
