﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class CardManager : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private Image _blockingImage = null;
	[SerializeField] private Image _controlBlocker = null;
	[SerializeField] private RectTransform _cardUseThresholdPoint = null;

	[Header("Card Back Sprites")]
	[SerializeField] private Sprite[] _cardBackSprites = null;

	[Header("Card Front Sprites")]
	[SerializeField] private Sprite[] _cardFrontSprites = null;

	private Card[] _cards = null;

	private const int MAX_CARDS = 4;
	private int _numCardsInHand = -1;
	public int NumCardsInHand { get { return _numCardsInHand; } }
	private bool _isFirstDrawOfGame = true;
	public bool IsFirstDrawOfGame { get { return _isFirstDrawOfGame; } }
	private bool _isFirstDrawOfFloor = true;
	public bool IsFirstDrawOfFloor { get { return _isFirstDrawOfFloor; } }
    private bool _shouldSkipDraw = false;
    public bool ShouldSkipDraw { get { return _shouldSkipDraw; } }
	private int _numCardsUsedInTurn = -1;

    // RandomPools
	private RandomExt.RandomPool _cardTierRandomPool = null;
	private RandomExt.RandomPool _cardMoveTypeRandomPool = null;
	private RandomExt.RandomPool _nonMovementCardTypeRandomPool = null;

	// TODO: Statistics
	private int _statTotalCardsDrawn = -1;
	public int StatTotalCardsDrawn { get { return _statTotalCardsDrawn; } }

	public float CardUseYThreshold { get; private set; }

	private void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
		Debug.Assert(_cardBackSprites.Length == 3, "There is a mistmatch in number of card back sprites.");
		Debug.Assert(_cardFrontSprites.Length == (3 * 5) + (3 * 4), "There is a mismatch in number of card front sprites.");
		Debug.Assert(_blockingImage != null, "_blockingImage is not assigned.");
		Debug.Assert(_controlBlocker != null, "_controlBlocker is not assigned.");
		Debug.Assert(_cardUseThresholdPoint != null, "_cardUseThresholdPoint is not assigned.");

		_cards = new Card[MAX_CARDS];
		for (int iCard = 0; iCard < MAX_CARDS; iCard++)
		{
			_cards[iCard] = transform.Find("Cards").Find("Card " + (iCard + 1)).GetComponent<Card>();
			_cards[iCard].SetCardIndex(iCard);
			_cards[iCard].OnCardExecute += TryExecuteAndDiscardCard;
		}

		_isFirstDrawOfGame = true;
		_isFirstDrawOfFloor = true;
		_shouldSkipDraw = false;
		_dungeon.OnEndPlayerTurn += OnPlayerEndTurn;
		_dungeon.OnFloorCleared += () => {
			_isFirstDrawOfFloor = true;
		};

		// Set up RandomPools.
		_cardTierRandomPool = new RandomExt.RandomPool();
		_cardTierRandomPool.AddEntry((int)eCardTier.Normal, 80);
		_cardTierRandomPool.AddEntry((int)eCardTier.Silver, 15);
		_cardTierRandomPool.AddEntry((int)eCardTier.Gold, 5);

		// Chances for move types
		_cardMoveTypeRandomPool = new RandomExt.RandomPool();
		_cardMoveTypeRandomPool.AddEntry((int)eMoveType.Pawn, 30);
		_cardMoveTypeRandomPool.AddEntry((int)eMoveType.Rook, 20);
		_cardMoveTypeRandomPool.AddEntry((int)eMoveType.Bishop, 20);
		_cardMoveTypeRandomPool.AddEntry((int)eMoveType.Knight, 15);
		_cardMoveTypeRandomPool.AddEntry((int)eMoveType.King, 15);

		// Chances for non movement card types
		_nonMovementCardTypeRandomPool = new RandomExt.RandomPool();
		_nonMovementCardTypeRandomPool.AddEntry((int)eCardType.Joker, 30);
		_nonMovementCardTypeRandomPool.AddEntry((int)eCardType.Smash, 25);
		_nonMovementCardTypeRandomPool.AddEntry((int)eCardType.Clone, 25);
		_nonMovementCardTypeRandomPool.AddEntry((int)eCardType.Draw, 20);

		_dungeon.OnFloorCleared += () => { _isCardInUse = false; };
    }

    private void Start()
    {
		CardUseYThreshold = _cardUseThresholdPoint.position.y;
		HideAllCards();
	}

	private void Update()
	{
		bool areAnyCardsAnimating = false;
		for (int iCard = 0; iCard < MAX_CARDS; iCard++)
		{
			if (_cards[iCard].IsAnimating)
			{
				areAnyCardsAnimating = true;
				break;
			}
		}
		bool isControlBlockerEnabled =
			(!_dungeon.IsPlayersTurn ||
			_isCardInUse ||
			_isAnimatingReog ||
			areAnyCardsAnimating ||
			_dungeon.IsPlayerTurnStartAnimPlaying ||
			_dungeon.FloorCleared ||
			_dungeon.CheckClearFloorConditions());

		if (_controlBlocker.raycastTarget != isControlBlockerEnabled) _controlBlocker.raycastTarget = isControlBlockerEnabled;
	}

	private void HideAllCards()
	{
		for (int iCard = 0; iCard < MAX_CARDS; iCard++)
		{
			SetCardActive(iCard, false);
		}
	}

	public void ResetForNewGame()
	{
		_isFirstDrawOfGame = true;
		_isFirstDrawOfFloor = true;
		_shouldSkipDraw = false;
		_numCardsInHand = 0;
		_numCardsUsedInTurn = 0;
		_statTotalCardsDrawn = 0;
		HideAllCards();
	}

	public void ResetFromPrevRunData(RunData inPrevRunData)
	{
		_isFirstDrawOfGame = inPrevRunData.IsFirstDrawOfGame;
		_isFirstDrawOfFloor = inPrevRunData.IsFirstDrawOfFloor;
		_shouldSkipDraw = true;
		_numCardsInHand = 0;
		_numCardsUsedInTurn = 0;
		_statTotalCardsDrawn = 0; // TODO: Save and load this stat.
		HideAllCards();

		ClearAndSetHand(null, inPrevRunData.CardDatas);
	}

	public void HasSkippedDraw()
	{
		Debug.Assert(_shouldSkipDraw, "Calling HasSkippedDraw when _shouldSkipDraw is already false!");
		_shouldSkipDraw = false;
	}

	public void HasDoneFirstDrawOfFloor()
	{
		Debug.Assert(_isFirstDrawOfFloor, "Calling HasDoneFirstDrawOfFloor when _isFirstDrawOfFloor is already false!");
		_isFirstDrawOfFloor = false;
	}

	public CardData[] GenerateCardHandData()
	{
		ReorganiseCards(null, false);
		CardData[] cardDatas = new CardData[_numCardsInHand];
		for (int iCard = 0; iCard < _numCardsInHand; iCard++)
		{
			cardDatas[iCard] = _cards[iCard].CardData;
		}

		return cardDatas;
	}

	public Sprite GetCardBackSprite(eCardTier inCardTier)
	{
		return _cardBackSprites[(int)inCardTier];
	}
	
	public Sprite GetCardFrontSprite(eCardTier inCardTier, eCardType inCardType, eMoveType inCardMoveType)
	{
		int spriteIndex = ((int)inCardType * 3);
		if (inCardType == eCardType.Movement)
		{
			spriteIndex += (int)inCardMoveType * 3 + (int)inCardTier;
		}
		else
		{
			spriteIndex += (int)inCardTier;
		}

		return _cardFrontSprites[spriteIndex];
	}

	private const float CARD_ANIM_INTERNAL = 0.2f;
	public void DrawCard(int inNumCardsDrawn, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true)
	{
		for (int iCard = 0; iCard < inNumCardsDrawn; iCard++)
		{
			CardData cardData = GenerateRandomCardData();
			if ((iCard + 1) == inNumCardsDrawn
				|| (_numCardsInHand + 1) == MAX_CARDS)
			{
				DrawCard(cardData, inOnComplete, inIsAnimated, (1 + iCard) * CARD_ANIM_INTERNAL);
				break;
			}
			else
			{
				DrawCard(cardData, null, inIsAnimated, (1 + iCard) * CARD_ANIM_INTERNAL);
			}
		}
	}
	public void CloneCard(eCardTier inTierToClone, CardData inCloneData, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true)
	{
        DrawCard(inCloneData, null, inIsAnimated, CARD_ANIM_INTERNAL);
		if ((int)inCloneData.cardTier < (int)inTierToClone) inCloneData.cardTier = inTierToClone;
		DrawCard(inCloneData, inOnComplete, inIsAnimated, 2 * CARD_ANIM_INTERNAL);
	}
	public void DrawCard(CardData inCardData, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true, float inCardAnimInterval = CARD_ANIM_INTERNAL)
	{
		bool needReorg = ReorganiseCards(() => { DrawCard(inCardData, inOnComplete, inIsAnimated, inCardAnimInterval); });

		if (!needReorg)
		{
			if (_numCardsInHand < MAX_CARDS)
			{
				int curCardIndex = _numCardsInHand;
				Debug.Assert(!_cards[curCardIndex].IsEnabled, "Card " + curCardIndex + " is already enabled! Should not be drawn.");

				SetCardActive(curCardIndex, true);
				_cards[curCardIndex].SetCard(inCardData);
				_numCardsInHand++;
				_statTotalCardsDrawn++;

				if (inIsAnimated)
				{
					_cards[curCardIndex].AnimateDrawCard(inCardAnimInterval, inOnComplete);
				}
				else
				{
					if (inOnComplete != null) inOnComplete();
				}

				_isFirstDrawOfGame = false;
			}
			else
			{
				// NOTE: If didn't draw card, then just run inOnComplete.
				if (inOnComplete != null) inOnComplete();
			}
		}
	}

	public void ClearAndSetHand(DTJob.OnCompleteCallback inOnComplete = null, params CardData[] inCardDatas)
	{
		int numCardsDrawn = inCardDatas.Length;
		Debug.Assert(numCardsDrawn <= MAX_CARDS, "Trying to set more cards than max cards in hand.");
		int cardLimit = Mathf.Min(_numCardsInHand + numCardsDrawn, MAX_CARDS);

		for (int iCard = 0; iCard < MAX_CARDS; iCard++)
		{
			SetCardActive(iCard, false);
		}
		_numCardsInHand = 0;

		for (int iCard = 0; iCard < cardLimit; iCard++)
		{
			SetCardActive(iCard, true);
			_cards[iCard].SetCard(inCardDatas[iCard]);
			_numCardsInHand++;
		}

		if (inOnComplete != null) inOnComplete();
	}

	private bool _isAnimatingReog = false;
	private bool ReorganiseCards(DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true)
	{
		const float REORG_ANIM_DURATION = 0.6f;
		int firstEmptyIndex = -1;
		int curIndex = 0;
		bool neededToReorg = false;
		float offsetZ = 0.0f;
		while (firstEmptyIndex != _numCardsInHand)
		{
			if (firstEmptyIndex < 0) // NOTE: Finding the first empty.
			{
				if (!_cards[curIndex].IsEnabled)
				{
					firstEmptyIndex = curIndex;
				}
			}
			else // Finding the next card.
			{
				if (_cards[curIndex].IsEnabled)
				{
					neededToReorg = true;

					SwapCards(curIndex, firstEmptyIndex);
					_cards[curIndex].SetEnabled(false);
					if (inIsAnimated)
					{

						offsetZ += 0.5f;
						_cards[firstEmptyIndex].AnimateMoveToOriginFrom(_cards[curIndex].OriginLocalPos, REORG_ANIM_DURATION, offsetZ);
					}
					_cards[firstEmptyIndex].SetEnabled(true);

					curIndex = -1;
					firstEmptyIndex = -1;
				}
			}

			curIndex++;
			if (curIndex >= _cards.Length) break;
		}

		if (neededToReorg)
		{
			if (inIsAnimated)
			{
				_isAnimatingReog = true;

				DelayAction delayedExecution = new DelayAction(REORG_ANIM_DURATION);
				delayedExecution.OnActionFinish += () =>
				{
					_isAnimatingReog = false;
					if (inOnComplete != null) inOnComplete();
				};
				ActionHandler.RunAction(delayedExecution);
			}
			else
			{
				if (inOnComplete != null) inOnComplete();
			}
		}
		// NOTE: DO NOT RUN inOnComplete if there is no need to reorg.

		return neededToReorg;
	}

	private void SetCardActive(int inCardIndex, bool inIsActive)
	{
		_cards[inCardIndex].SetEnabled(inIsActive);
	}

	private CardData GenerateRandomCardData()
	{
		CardData[] activeCards = GetArrOfActiveCards();

		eCardTier cardTier = (eCardTier)_cardTierRandomPool.GetRandomEntry();
		eMoveType moveType;
		eCardType cardType;

		int numPawnCards = 0;
        int numDrawCards = 0;
        int numCloneCards = 0;
		int numCardsThatAreClones = 0;
        for (int iCard = 0; iCard < activeCards.Length; iCard++)
        {
            CardData curCardData = activeCards[iCard];
            if (curCardData.cardType == eCardType.Movement
                && curCardData.cardMoveType == eMoveType.Pawn)
            {
                numPawnCards++;
            }
            else if (curCardData.cardType == eCardType.Draw)
            {
                numDrawCards++;
            }
            else if (curCardData.cardType == eCardType.Clone)
            {
                numCloneCards++;
            }

			if (curCardData.isCloned)
			{
				numCardsThatAreClones++;
			}
        }

		bool isMovementCardType = RandomExt.WeightedRandomBoolean(80, 20);
		if (isMovementCardType) // Movement
		{
			cardType = eCardType.Movement;

			if (numPawnCards > 1) _cardMoveTypeRandomPool.ChangeWeight((int)eMoveType.Pawn, 0);
			else if (numPawnCards == 1) _cardMoveTypeRandomPool.ChangeWeight((int)eMoveType.Pawn, 10);

			moveType = (eMoveType)_cardMoveTypeRandomPool.GetRandomEntry();
			_cardMoveTypeRandomPool.ResetAllWeightsToInitial();
		}
		else // Non movement.
		{
			if (numDrawCards > 0) _nonMovementCardTypeRandomPool.ChangeWeight((int)eCardType.Draw, 0);

			if (numCloneCards > 0
				|| numCardsThatAreClones > 0) _nonMovementCardTypeRandomPool.ChangeWeight((int)eCardType.Clone, 0);

			cardType = (eCardType)_nonMovementCardTypeRandomPool.GetRandomEntry();
			_nonMovementCardTypeRandomPool.ResetAllWeightsToInitial();

			moveType = eMoveType.Pawn;
		}

		return new CardData(cardTier, cardType, false, moveType);
	}

	private CardData[] GetArrOfActiveCards()
	{
		List<CardData> cardDatas = new List<CardData>();
		for (int iCard = 0; iCard < _cards.Length; iCard++)
		{
			Card curCard = _cards[iCard];
			if (curCard.IsEnabled)
			{
				cardDatas.Add(curCard.CardData);
			}
		}

		return cardDatas.ToArray();
	}

	private void SwapCards(int inCardIndexA, int inCardIndexB)
	{
		CardData cardDataA = _cards[inCardIndexA].CardData;
		CardData cardDataB = _cards[inCardIndexB].CardData;
		_cards[inCardIndexA].SetCard(cardDataB);
		_cards[inCardIndexB].SetCard(cardDataA);
	}

	private void OnPlayerEndTurn()
	{
		if (_numCardsUsedInTurn < 1)
		{
			// NOTE: Used to draw card only if did not use any cards.
            // Took it out for now since you're drawing no matter what.
			//DrawCard(1);
		}
		else
		{
			ReorganiseCards();
		}

		_numCardsUsedInTurn = 0;
	}

	private bool _isCardInUse = false;
	public bool IsCardInUse { get { return _isCardInUse; } }
	public void SignalCardUsed()
	{
		Debug.Assert(_isCardInUse, "Trying to signal card used when _isCardInUse is false!");
		_isCardInUse = false;
	}
	private bool _isCloneMode = false;
	public bool IsCloneMode { get { return _isCloneMode; } }
	private eCardTier _tierToClone = eCardTier.Normal;
	private void TryExecuteAndDiscardCard(int inCardIndex)
	{
		Debug.Assert(!_isCardInUse, "Trying to use card when _isCardinUse is already true!");
		Card card = _cards[inCardIndex];
		CardData cardData = card.CardData;
		_isCardInUse = true;

		if (_isCloneMode)
		{
			bool cloneIsCardRejected = false;
			string cloneRejectReasonStr = string.Empty;
			if (cardData.isCloned)
			{
				cloneIsCardRejected = true;
				cloneRejectReasonStr = "Card is already cloned";
			}

			if (cloneIsCardRejected)
			{
				_isCardInUse = false;
				card.ReturnCardAndUnexecute(cloneRejectReasonStr);
			}
			else
			{
				CardData clonesData = cardData;
				clonesData.isCloned = true;

				_numCardsInHand--;
				_cards[inCardIndex].AnimateCardExecuteAndDisable(() =>
				{
					CloneCard(
						_tierToClone,
						clonesData,
						() => {
							_isCardInUse = false;
						},
						true);
					_isCloneMode = false;
					_tierToClone = eCardTier.Normal;
				});
			}
		}
		#region NormalExecutionMode
		else
		{
			Utils.GenericVoidDelegate postExecuteCardAnimActions = null;
			bool isCardRejected = false;
			string rejectReasonStr = string.Empty;
			switch (cardData.cardType)
			{
				case eCardType.Joker:
				{
                    int numMoves = -1;
                    eMoveType moveType = eMoveType.Pawn;
                    switch (cardData.cardTier)
                    {
                        case eCardTier.Normal:
                        {
                            numMoves = 1;
                            moveType = (eMoveType)Random.Range(0, 3);
                            break;
                        }
                        case eCardTier.Silver:
                        {
                            numMoves = 2;
                            moveType = (eMoveType)Random.Range(0, 4);
                            break;
                        }
                        case eCardTier.Gold:
                        {
                            numMoves = 3;
                            moveType = (eMoveType)Random.Range(1, 5);
                            break;
                        }
                        default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
                    }
                    _dungeon.MorphyController.MorphTo(moveType, numMoves);
                    DungeonCamera.FocusCameraToTile(_dungeon.MorphyController.MorphyPos, 0.6f, null);
					break;
				}
				case eCardType.Clone:
				{
					// NOTE: Check if there are even cards to clone.
					bool hasCloneableCards = false;
					for (int iCard = 0; iCard < MAX_CARDS; iCard++)
					{
						if (!_cards[iCard].IsEnabled) continue;
						else if (iCard == card.CardIndex) continue;
						else if (_cards[iCard].CardData.isCloned) continue;
						else
						{
							hasCloneableCards = true;
							break;
						}
					}

					if (hasCloneableCards)
					{
						_tierToClone = cardData.cardTier;

						postExecuteCardAnimActions += () =>
						{
							DungeonPopup.PopSidePopup("Select a card to clone it.");
							_isCloneMode = true;
							_isCardInUse = false;
						};
					}
					else // NOTE: Return the card.
					{
						isCardRejected = true;
						_isCardInUse = false;
						rejectReasonStr = "No Cloneable Cards";
					}
	                break;
				}
				case eCardType.Smash:
				{
					postExecuteCardAnimActions += () =>
					{
						_dungeon.MorphyController.Smash(cardData.cardTier, () =>
						{
							_isCardInUse = false;
						});
					};
					break;
				}
				case eCardType.Draw:
				{
					int numDraws = -1;
					switch (cardData.cardTier)
					{
						case eCardTier.Normal: numDraws = 2; break;
						case eCardTier.Silver: numDraws = 3; break;
						case eCardTier.Gold: numDraws = 4; break;
						default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
					}

                    postExecuteCardAnimActions += () => { DrawCard(numDraws, () => 
						{
							_isCardInUse = false;
						});
					};
					break;
				}
				case eCardType.Movement:
				{
					int numMoves = -1;
					switch (cardData.cardTier)
					{
						case eCardTier.Normal: numMoves = 1; break;
						case eCardTier.Silver: numMoves = 2; break;
						case eCardTier.Gold: numMoves = 3; break;
						default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
					}
					_dungeon.MorphyController.MorphTo(cardData.cardMoveType, numMoves);
					DungeonCamera.FocusCameraToTile(_dungeon.MorphyController.MorphyPos, 0.6f, null);
					break;
				}
				default:
				{
					Debug.LogWarning("case: " + cardData.cardType.ToString() + " has not been handled.");
					break;
				}
			}

			if (isCardRejected)
			{
				_isCardInUse = false;
				card.ReturnCardAndUnexecute(rejectReasonStr);
			}
			else
			{
				_numCardsUsedInTurn++;
				_numCardsInHand--;
				_cards[inCardIndex].AnimateCardExecuteAndDisable(() =>
				{
					if (postExecuteCardAnimActions != null) postExecuteCardAnimActions();
				});
			}
		}
		#endregion
	}
}
