using System.Collections;
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
	[SerializeField] private CardManager _cardManager = null;
	public TileManager TileManager { get { return _tileManager; } }
	public EnemyManager EnemyManager { get { return _enemyManager; } }
	public MorphyController MorphyController { get { return _morphyController; } }
	public CardManager CardManager { get { return _cardManager; } }

	[Header("Object Pools")]
	[SerializeField] private SmashParticlePool _smashParticlePool = null;
	public SmashParticlePool SmashParticlePool { get { return _smashParticlePool; } }

	private const int DUNGEON_MIN_X = 5;
	private const int DUNGEON_MAX_X = 10;
	private const int DUNGEON_MIN_Y = 5;
	private const int DUNGEON_MAX_Y = 10;
	private const int DUNGEON_MAX_ENEMIES = 50;
	public int MinX { get { return DUNGEON_MIN_X; } }
	public int MaxX { get { return DUNGEON_MAX_X; } }
	public int MinY { get { return DUNGEON_MIN_Y; } }
	public int MaxY { get { return DUNGEON_MAX_Y; } }
	public int MaxNumEnemies { get { return DUNGEON_MAX_ENEMIES; } }

	private bool _floorCleared = false;
	public bool FloorCleared { get { return _floorCleared; } }
	private bool _hasGameStarted = false;
	public bool HasGameStarted { get { return _hasGameStarted; } }
	private bool _isPlayersTurn = false;
	public bool IsPlayersTurn { get { return _isPlayersTurn; } }
	private bool _isPlayerTurnStartAnimPlaying = false;
	public bool IsPlayerTurnStartAnimPlaying { get { return _isPlayerTurnStartAnimPlaying; } }

	private int _floorNum = -1;
	private Floor _floor = null;
	public Floor CurrentFloor { get { return _floor; } }
	public Vector2Int StairsPos { get { return _floor.StairsPos; } }
	public int FloorNum { get { return _floor.FloorNum; } }

	private DungeonFSM _dungeonFSM = null;

	public Utils.GenericVoidDelegate OnFloorGenerated;
	public Utils.GenericVoidDelegate OnEndPlayerTurn;
	public Utils.GenericVoidDelegate OnFloorCleared;

	private void Awake()
	{
		Debug.Assert(_tileManager != null, "_tileManager is not assigned.");
		Debug.Assert(_enemyManager != null, "_enemyManager is not assigned.");
		Debug.Assert(_morphyController != null, "_morphyController is not assigned.");
		Debug.Assert(_cardManager != null, "_cardManager is not assigned.");

		Debug.Assert(_smashParticlePool != null, "_smashParticlePool is not assigned.");

		_floor = new Floor(this);
	}

	private void Start()
	{
		_dungeonFSM = new DungeonFSM(this);
	}

	private void Update()
	{
		if (HasGameStarted)
		{
			_dungeonFSM.Execute();
			// DEBUG
			//if (Input.GetKeyDown(KeyCode.Space)) SaveGame();
		}
	}

	private void GenerateNewFloor()
	{
		// Do resetting.
		_floorCleared = false;

		_floor.GenerateAndSetupNewFloor(DUNGEON_MIN_X, DUNGEON_MAX_X, DUNGEON_MIN_Y, DUNGEON_MAX_Y, _floorNum);

		OnFloorGenerated.Invoke();
	}

	public void ResetAndStartGame()
	{
		// TODO: Reset all of the UI?
		DungeonCardDrawer.SetEndTurnBtnForLoading(false);
		_morphyController.ResetForNewGame();
		_enemyManager.ResetForNewGame();
		_cardManager.ResetForNewGame();

		_floorNum = 1;
		_floorCleared = false;

		_hasGameStarted = true;
		_isPlayersTurn = false;

		GenerateNewFloor();
		_dungeonFSM.SetToStartFloorState();
	}

	public void StartGameFromPrevRun(RunData inPrevRunData)
	{
		_enemyManager.ResetForNewGame();

		_morphyController.ResetFromPrevRunData(inPrevRunData);
		_cardManager.ResetFromPrevRunData(inPrevRunData);

		_floorNum = inPrevRunData.FloorNum;
		_floorCleared = false;

		_hasGameStarted = true;
		_isPlayersTurn = false;

		_floor.LoadAndSetupNewFloor(inPrevRunData);
		OnFloorGenerated.Invoke();
		_dungeonFSM.SetToStartFloorState();
	}

	public void EndPlayerTurn()
	{
		if (OnEndPlayerTurn != null) OnEndPlayerTurn();
	}

	public void EndGame()
	{
		_hasGameStarted = false;
		_isPlayersTurn = false;
		_dungeonFSM.SetToIdle();

		// TODO: Disable some UI and stuff?
		//		 Present GameOver panel and such
		GameOverCanvas.SetGameOverValues(99999, FloorNum);
		GameOverCanvas.EnableGameOverPanel(true);

		// TODO: Save scores and stuff delete save data cause there is no more game to continue.
		// Which will also prevent them from cheating.
	}

	public void SaveGame()
	{
		RunData prevRunData = new RunData(this);
		GameData.SavePreviousRunData(prevRunData);
		Debug.Log("Game Saved!");
	}

	public bool CheckClearFloorConditions()
	{
		// Check if Morphy is at floor.
		if (CurrentFloor.IsTileOfState(MorphyController.TargetPos, Floor.eTileState.Stairs))
		{
			return true;
		}

		// Check is all enemies are killed.
		if (EnemyManager.GetArrayOfAliveEnemies().Length < 1)
		{
			return true;
		}

		return false;
	}

	public bool TryClearFloor()
	{
		bool conditionsMet = CheckClearFloorConditions();
		if (conditionsMet)
		{
			_floorCleared = true;
            if (OnFloorCleared != null) OnFloorCleared();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void ProgressToNextFloor(DTJob.OnCompleteCallback inOnComplete = null)
	{
		_floorNum++;
		GenerateNewFloor();
		SaveGame();

		if (inOnComplete != null) inOnComplete();
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
			_dungeonStates.Add(eDungeonState.Idle, new DungeonStateIdle(this));

			ChangeState(eDungeonState.Idle);
		}

		public void Execute()
		{
			_currentState.ExecuteState();
		}

		private void ChangeState(eDungeonState inNewState)
		{
			//Debug.Log("Changing State from: " + _currentState + " to " + inNewState.ToString());
			if (_currentState != null) _currentState.ExitState();
			_currentState = _dungeonStates[inNewState];
			_currentState.OnEnterState();
		}

		public void SetToStartFloorState()
		{
			ChangeState(eDungeonState.StartFloor);
		}

		public void SetToIdle()
		{
			ChangeState(eDungeonState.Idle);
		}

		#region DungeonStates
		private enum eDungeonState { StartFloor, EndFloor, PlayerPhase, EnemyPhase, Idle }
		private abstract class DungeonState
		{
			protected DungeonFSM _dungeonFSM = null;

			public abstract void OnEnterState();
			public abstract void ExitState();
			public abstract void ExecuteState();
		}
		
		private class DungeonStateIdle : DungeonState
		{
			public DungeonStateIdle(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
			}

			public override void ExecuteState()
			{
			}

			public override void ExitState()
			{
			}
		}

		private class DungeonStateStartFloor : DungeonState
		{
			public DungeonStateStartFloor(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
			}

			public override void OnEnterState()
			{
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
				DTJob fadeInNextFloorPanel = new DTJob((OnComplete) =>
				{
					DungeonDisplay.ShowNextFloorPanel((_dungeonFSM.Dungeon.FloorNum + 1), OnComplete);
				});
				DTJob generateNewFloor = new DTJob((OnComplete) =>
				{
					_dungeonFSM.Dungeon.ProgressToNextFloor(OnComplete);
				}, fadeInNextFloorPanel);
				DTJob fadeOutNextFloorPanel = new DTJob((OnComplete) =>
				{
					DelayAction delay = new DelayAction(0.75f);
					delay.OnActionFinish += () => { DungeonDisplay.HideNextFloorPanel(OnComplete); };
					ActionHandler.RunAction(delay);
				}, generateNewFloor);
				DTJobList nextFloorAnimSequence = new DTJobList(() => { _dungeonFSM.ChangeState(eDungeonState.StartFloor); }, fadeOutNextFloorPanel);
				nextFloorAnimSequence.ExecuteAllJobs();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
			}

			public override void ExecuteState()
			{
				// Nth here.
			}
		}
		private class DungeonStatePlayerPhase : DungeonState
		{
			public DungeonStatePlayerPhase(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
				_dungeonFSM.Dungeon.OnEndPlayerTurn += () =>
					{
						_dungeonFSM.ChangeState(eDungeonState.EnemyPhase);
					};
			}

			public override void OnEnterState()
			{
				_dungeonFSM._dungeon._isPlayersTurn = true;
				_dungeonFSM._dungeon._isPlayerTurnStartAnimPlaying = true;
				DungeonCardDrawer.SetEndTurnBtnForPlayerPhase();
				DTJob playPhaseAnimJob = new DTJob((OnJobComplete) => {
					DungeonDisplay.PlayPhaseAnimation(_dungeonFSM._dungeon.IsPlayersTurn, OnJobComplete); });
				DTJob turnDrawJob = new DTJob((OnJobComplete) =>
				{
					if (_dungeonFSM.Dungeon.CardManager.IsFirstDrawOfGame)
					{
						_dungeonFSM.Dungeon.CardManager.HasDoneFirstDrawOfFloor();
                        _dungeonFSM.Dungeon.CardManager.DrawCard(3, OnJobComplete);
					}
					else if (_dungeonFSM.Dungeon.CardManager.IsFirstDrawOfFloor)
					{
						_dungeonFSM.Dungeon.CardManager.HasDoneFirstDrawOfFloor();
                        if (_dungeonFSM.Dungeon.CardManager.ShouldSkipDraw) _dungeonFSM.Dungeon.CardManager.HasSkippedDraw(); // NOTE: If reloaded from prev run.
                        _dungeonFSM.Dungeon.CardManager.DrawCard(1, OnJobComplete);
					}
                    else if (_dungeonFSM.Dungeon.CardManager.ShouldSkipDraw)
                    {
                        _dungeonFSM.Dungeon.CardManager.HasSkippedDraw();
                        if (OnJobComplete != null) OnJobComplete();
                    }
					else
                    {
                        _dungeonFSM.Dungeon.CardManager.DrawCard(1, OnJobComplete);
					}
				}, playPhaseAnimJob);

				DTJob saveDataJob = new DTJob((OnJobComplete) =>
				{
					_dungeonFSM.Dungeon.SaveGame();
					OnJobComplete();
				}, turnDrawJob);

				DTJob enableEndTurnBtnJob = new DTJob((OnJobComplete) =>
				{
					_dungeonFSM._dungeon._isPlayerTurnStartAnimPlaying = false;
					if (OnJobComplete != null) OnJobComplete();
				}, saveDataJob);

				DTJob focusOnPlayerJob = new DTJob((OnJobComplete) =>
				{
					DungeonCamera.FocusCameraToTile(_dungeonFSM.Dungeon.MorphyController.MorphyPos, 1.0f, OnJobComplete);
				}, playPhaseAnimJob);

				// NOTE: The on all jobs complete for this one seems to lag??? So we don't use it.
				DTJobList startPlayerPhase = new DTJobList(null, turnDrawJob, focusOnPlayerJob, saveDataJob, enableEndTurnBtnJob);
				startPlayerPhase.ExecuteAllJobs();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
				_dungeonFSM.Dungeon._isPlayersTurn = false;
				if (_dungeonFSM.Dungeon.FloorCleared) DungeonCardDrawer.SetEndTurnBtnForLoading();
				else DungeonCardDrawer.SetEndTurnBtnForEnemyPhase();
			}

			public override void ExecuteState()
			{
				if (_dungeonFSM.Dungeon.FloorCleared) _dungeonFSM.ChangeState(eDungeonState.EndFloor);
			}
		}
		private class DungeonStateEnemyPhase : DungeonState
		{
			public DungeonStateEnemyPhase(DungeonFSM inDungeonFSM)
			{
				_dungeonFSM = inDungeonFSM;
				_enemyTurnActions = new List<Enemy.EnemyTurnAction>();
			}

			public override void OnEnterState()
			{
				DungeonDisplay.PlayPhaseAnimation(_dungeonFSM._dungeon.IsPlayersTurn,
					() =>
					{
                        _readyToExecuteNextEnemyTurnAction = true;
                    });
				_readyToProcessNextEnemy = true;
				_enemiesAlive = _dungeonFSM._dungeon.EnemyManager.GetArrayOfAliveEnemies();
			}

			public override void ExitState()
			{
				// TODO: Any cleanup needed?
				_enemiesAlive = null;
				_currentEnemyIndex = -1;
				_readyToProcessNextEnemy = false;
				_enemyTurnActions.Clear();
			}

			Enemy[] _enemiesAlive = null;
			int _currentEnemyIndex = -1;
            bool _readyToProcessNextEnemy = false;

            List<Enemy.EnemyTurnAction> _enemyTurnActions = null;
			bool _readyToExecuteNextEnemyTurnAction = false;
			public override void ExecuteState()
			{
				if (_readyToProcessNextEnemy)
				{
					_readyToProcessNextEnemy = false;
					_currentEnemyIndex++;
					if (_currentEnemyIndex == _enemiesAlive.Length)
					{
                        // Finished Processing all the enemies.
					}
					else
					{
						Enemy.EnemyTurnAction calculatedAction = _enemiesAlive[_currentEnemyIndex].ProcessTurn(_dungeonFSM._dungeon.CurrentFloor, OnFinishEnemyTurnAction, OnFinishProcessingEnemy);
						if (calculatedAction.TurnAction != Enemy.eTurnAction.Nothing)
						{
							_enemyTurnActions.Add(calculatedAction);
						}
					}
				}
				else if (_readyToExecuteNextEnemyTurnAction)
				{
					_readyToExecuteNextEnemyTurnAction = false;
					if (_enemyTurnActions.Count > 0)
					{
						bool moveTurnFound = false;
						for (int i = 0; i < _enemyTurnActions.Count; i++)
						{
							Enemy.EnemyTurnAction curTurnAction = _enemyTurnActions[i];
							if (curTurnAction.TurnAction == Enemy.eTurnAction.Move)
							{
								moveTurnFound = true;
								curTurnAction.Enemy.ExecuteTurnAction(curTurnAction);
								_enemyTurnActions.RemoveAt(i);
								break;
							}
						}

						if (!moveTurnFound) // NOTE: Left with attacking moves.
						{
							Enemy.EnemyTurnAction curTurnAction = _enemyTurnActions[0];
							Debug.Assert(curTurnAction.TurnAction == Enemy.eTurnAction.Attack, "No enemy move turn found but first index is also not attack type.");
							curTurnAction.Enemy.ExecuteTurnAction(curTurnAction);
						}
					}
					else
					{
						_dungeonFSM.ChangeState(eDungeonState.PlayerPhase);
					}
				}
			}

			private void OnFinishProcessingEnemy()
			{
			    _readyToProcessNextEnemy = true;
			}

			private void OnFinishEnemyTurnAction()
			{
				// TODO: If the player is killed, immediately transition to the GameOverState.
                if (_dungeonFSM.Dungeon.MorphyController.IsDead)
                {
					_readyToExecuteNextEnemyTurnAction = false;
                    // NOTE: Do nothing. Player is dead so just wait for new game where this whole FSM will be recreated.
                }
                else
                {
					_readyToExecuteNextEnemyTurnAction = true;
                }
			}
		}
		#endregion
	}
	#endregion

#if CHESSGEON_RELEASE
#else
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
			if (firstEnemy != null)
			{
				_debugPath = AStarManager.FindPath(CurrentFloor.Nodes[firstEnemy.Pos.x, firstEnemy.Pos.y],
					CurrentFloor.Nodes[CurrentFloor.MorphyPos.x, CurrentFloor.MorphyPos.y],
					CurrentFloor,
					_debugMoveType);
			}
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
#endif
}
