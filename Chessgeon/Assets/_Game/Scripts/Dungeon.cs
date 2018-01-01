﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DaburuTools;

public class Dungeon : MonoBehaviour
{
	[SerializeField] private TileManager _tileManager = null;
	[SerializeField] private EnemyManager _enemyManager = null;
	[SerializeField] private MorphyController _morphyController = null;
	public TileManager TileManager { get { return _tileManager; } }
	public EnemyManager EnemyManager { get { return _enemyManager; } }
	public MorphyController MorphyController { get { return _morphyController; } }

	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MAX_X = 15;
	private const int DUNGEON_MIN_Y = 5;
	private const int DUNGEON_MAX_Y = 15;
	private const int DUNGEON_MAX_ENEMIES = 50;
	public int MinX { get { return DUNGEON_MIN_X; } }
	public int MaxX { get { return DUNGEON_MAX_X; } }
	public int MinY { get { return DUNGEON_MIN_Y; } }
	public int MaxY { get { return DUNGEON_MAX_Y; } }
	public int MaxNumEnemies { get { return DUNGEON_MAX_ENEMIES; } }

	private bool _morphyHasReachedStairs = false;
	public bool MorphyHasReachedStairs { get { return _morphyHasReachedStairs; } }
	private bool _hasGameStarted = false;
	public bool HasGameStarted { get { return _hasGameStarted; } }
	private bool _isPlayersTurn = false;
	public bool IsPlayersTurn { get { return _isPlayersTurn; } }

	private int _floorNum = -1;
	private Floor _floor = null;
	public Floor CurrentFloor { get { return _floor; } }
	public Vector2Int StairsPos { get { return _floor.StairsPos; } } // TODO: Decide if this is btr or make Floor publically accessible.
	public int FloorNum { get { return _floor.FloorNum; } }

	private DungeonFSM _dungeonFSM = null;

	public UnityEvent OnFloorGenerated;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");
		Debug.Assert(_enemyManager != null, "_enemyManager is not assigned.");
		Debug.Assert(_morphyController != null, "_morphyController is not assigned.");

		_morphyController.OnMorphyReachStairs.AddListener(OnMorphyReachStairs);
		_floor = new Floor(this);
	}

	private void Update()
	{
		if (HasGameStarted)
		{
			_dungeonFSM.Execute();
		}
	}

	private void GenerateFloor()
	{
		// Do resetting.
		_morphyHasReachedStairs = false;

		_floor.GenerateAndSetupNewFloor(DUNGEON_MIN_X, DUNGEON_MAX_X, DUNGEON_MIN_Y, DUNGEON_MAX_Y, DungeonTile.eZone.Classic, _floorNum);

		OnFloorGenerated.Invoke();
	}

	public void StartGame()
	{
		_hasGameStarted = true;
		_isPlayersTurn = false;

		_floorNum = 1;
		GenerateFloor();
		_dungeonFSM = new DungeonFSM(this);
	}

	public void EndGame()
	{
		_hasGameStarted = false;
		_isPlayersTurn = false;

		// TODO: Disable some UI and stuff?
		//		 Present GameOver panel and such
	}

	private void OnMorphyReachStairs()
	{
		_morphyHasReachedStairs = true;
	}

	public void ProgressToNextFloor()
	{
		_floorNum++;
		GenerateFloor();
	}

	#region DungeonFSM
	private class DungeonFSM
	{
		private Dungeon _dungeon = null;
		public Dungeon Dungeon { get { return _dungeon; } }
		private DungeonState _currentState = null;
		private Dictionary<eDungeonState, DungeonState> _dungeonStates = null;

		public DungeonFSM(Dungeon inDungeon)
		{
			_dungeon = inDungeon;

			_dungeonStates = new Dictionary<eDungeonState, DungeonState>();
			_dungeonStates.Add(eDungeonState.StartFloor, new DungeonStateStartFloor(this));
			_dungeonStates.Add(eDungeonState.EndFloor, new DungeonStateEndFloor(this));
			_dungeonStates.Add(eDungeonState.PlayerPhase, new DungeonStatePlayerPhase(this));
			_dungeonStates.Add(eDungeonState.EnemyPhase, new DungeonStateEnemyPhase(this));
			_dungeonStates.Add(eDungeonState.GameOver, new DungeonStateGameOver(this));

			ChangeState(eDungeonState.StartFloor);
		}

		public void Execute()
		{
			_currentState.ExecuteState();
		}

		private void ChangeState(eDungeonState inNewState)
		{
			Debug.Log("Changing State from: " + _currentState + " to " + inNewState.ToString());
			if (_currentState != null) _currentState.ExitState();
			_currentState = _dungeonStates[inNewState];
			_currentState.OnEnterState();
		}

		#region DungeonStates
		private enum eDungeonState { StartFloor, EndFloor, PlayerPhase, EnemyPhase, GameOver }
		private abstract class DungeonState
		{
			protected DungeonFSM _dungeonFSM = null;

			public abstract void OnEnterState();
			public abstract void ExitState();
			public abstract void ExecuteState();
		}

		private class DungeonStateStartFloor : DungeonState
		{
			public DungeonStateStartFloor(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
				// TODO: MoveTo the camera to the player's location.
				//		 Then move to the stairs, and then move back to player?
				DTJob camFocusStairs = new DTJob((OnComplete) =>
				{
					DungeonCamera.FocusCameraToTile(_dungeonFSM.Dungeon.StairsPos, 2.0f, OnComplete);
				});
				DTJob camFocusPlayer = new DTJob((OnComplete) =>
				{
					DungeonCamera.FocusCameraToTile(_dungeonFSM.Dungeon.MorphyController.MorphyPos, 1.0f, OnComplete);
				}, camFocusStairs);
				DTJobList focusStairsAndPlayer = new DTJobList(
					() => { _finishedAnims = true; },
					camFocusPlayer);
				focusStairsAndPlayer.ExecuteAllJobs();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
				_finishedAnims = false;
			}

			bool _finishedAnims = false;
			public override void ExecuteState()
			{
				if (_finishedAnims) _dungeonFSM.ChangeState(eDungeonState.PlayerPhase);
			}
		}

		private class DungeonStateEndFloor : DungeonState
		{
			public DungeonStateEndFloor(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
				// TODO: Display stats and all that.
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
			}

			public override void ExecuteState()
			{
				// TODO: Wait for the player to click the okay button, then set black fade.
				//		 and hand over to dungeon to generate the next floor.
				//		 once generated, then lift the black fade and then switch to start floor.
				_dungeonFSM.Dungeon.ProgressToNextFloor();
				_dungeonFSM.ChangeState(eDungeonState.StartFloor);
			}
		}
		private class DungeonStatePlayerPhase : DungeonState
		{
			public DungeonStatePlayerPhase(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
				DungeonCardDrawer.OnPlayerEndTurn.AddListener(() =>
					{
						_dungeonFSM.ChangeState(eDungeonState.EnemyPhase);
					});
			}

			public override void OnEnterState()
			{
				// TODO: Do the animation for indicating start of player phase.
				//		 And draw cards for the player.
				_dungeonFSM._dungeon._isPlayersTurn = true;
				DTJob playPhaseAnimJob = new DTJob((OnJobComplete) => {
					DungeonDisplay.PlayPhaseAnimation(_dungeonFSM._dungeon.IsPlayersTurn, OnJobComplete); });
				DTJob enableCardDrawerJob = new DTJob((OnJobComplete) => {
					DungeonCardDrawer.EnableCardDrawer(true, true, OnJobComplete); },
					playPhaseAnimJob);

				DTJobList startPlayerPhase = new DTJobList(null, enableCardDrawerJob);
				startPlayerPhase.ExecuteAllJobs();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
				_dungeonFSM._dungeon._isPlayersTurn = false;
			}

			public override void ExecuteState()
			{
				if (_dungeonFSM.Dungeon.MorphyHasReachedStairs) _dungeonFSM.ChangeState(eDungeonState.EndFloor);
			}
		}
		private class DungeonStateEnemyPhase : DungeonState
		{
			public DungeonStateEnemyPhase(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
				// TODO: Do the animation for indicating start of enemy phase.
				DungeonDisplay.PlayPhaseAnimation(_dungeonFSM._dungeon.IsPlayersTurn);
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
				_delayTimer = 0.0f;
			}

			float _delayTimer = 0.0f;
			public override void ExecuteState()
			{
				// TODO: Iterate through all enemies and process each of their turns.
				//		 Prob just call on EnemyManager.processNextEnemy or smt like that.
				//		 Prefably have the waiting be done on this FSM side and not EnemyManager.
				_delayTimer += Time.deltaTime;
				if (_delayTimer >= 2.5f) _dungeonFSM.ChangeState(eDungeonState.PlayerPhase);
				
				// TODO: If the player is killed, immediately transition to the GameOverState.
			}
		}
		private class DungeonStateGameOver : DungeonState
		{
			public DungeonStateGameOver(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
				_dungeonFSM.Dungeon.EndGame();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
			}

			public override void ExecuteState()
			{
				if (_dungeonFSM.Dungeon.HasGameStarted) _dungeonFSM.ChangeState(eDungeonState.StartFloor);
			}
		}
		#endregion
	}
	#endregion

	#region AStarDebug
	private LinkedList<Node> _debugPath = null;
	private eMoveType _debugMoveType = eMoveType.Knight;
	private void OnDrawGizmos()
	{
		if (!_hasGameStarted)
			return;

		Gizmos.color = Color.grey;
		for (int x = 0; x < CurrentFloor.Size.x; x++)
		{
			for (int y = 0; y < CurrentFloor.Size.y; y++)
			{
				switch(CurrentFloor.Nodes[x, y].State)
				{
					case Floor.eTileState.Empty: Gizmos.color = Color.gray; break;
					case Floor.eTileState.Stairs: Gizmos.color = Color.cyan; break;
					case Floor.eTileState.Blocked: Gizmos.color = Color.red; break;
					case Floor.eTileState.Enemy: Gizmos.color = new Color (1.0f, 0.0f, 1.0f); break;
					case Floor.eTileState.Morphy: Gizmos.color = Color.green; break;
					default: Gizmos.color = Color.magenta; break;
				}

				Gizmos.DrawCube(TileManager.GetTileTransformPosition(new Vector2Int(x, y)) + Vector3.up * 3.0f, Vector3.one * 0.5f);
			}
		}

		bool needRecalc = false;
		if (_debugPath == null) needRecalc = true;
		if (needRecalc)
		{
			Enemy firstEnemy = null;
			for (int x = 0; x < CurrentFloor.Size.x; x++)
			{
				for (int y = 0; y < CurrentFloor.Size.y; y++)
				{
					if (CurrentFloor.IsTileOfState(x, y, Floor.eTileState.Enemy))
					{
						firstEnemy = CurrentFloor.GetEnemyAt(x, y);
						break;
					}
				}
				if (firstEnemy != null) break;
			}
			_debugPath = AStarManager.FindPath(CurrentFloor.Nodes[firstEnemy.Pos.x, firstEnemy.Pos.y],
				CurrentFloor.Nodes[CurrentFloor.MorphyPos.x, CurrentFloor.MorphyPos.y],
				CurrentFloor,
				_debugMoveType);
		}

		if (_debugPath != null)
		{
			for (LinkedListNode<Node> j = _debugPath.First; j.Next != null; j = j.Next)
			{
				Node node = (Node)j.Value;
				Node next = (Node)j.Next.Value;
				Vector3 upVector = Vector3.up * 6.0f;
				Debug.DrawLine(TileManager.GetTileTransformPosition(node.PosX, node.PosY) + upVector,
					TileManager.GetTileTransformPosition(next.PosX, next.PosY) + upVector,
					Color.magenta);
			}
		}
	}
	#endregion
}
