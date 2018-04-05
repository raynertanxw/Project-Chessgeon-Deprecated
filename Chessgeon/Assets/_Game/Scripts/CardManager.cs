using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DaburuTools;

public class CardManager : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;
	[SerializeField] private Image _controlBlocker = null;

	[Header("Card Texture")]
	[SerializeField] private Texture[] _cardTextures = null;

	private Card[] _cards = null;

	private const int MAX_CARDS = 7;
	private int _numCardsInHand = -1;
	private bool _isFirstDraw = true;
	public bool IsFirstDraw { get { return _isFirstDraw; } }
	private bool _skipNextDraw = false;
	public bool SkipNextDraw { get { return _skipNextDraw; } }

	// TODO: Statistics
	private int _statTotalCardsDrawn = -1;
	public int StatTotalCardsDrawn { get { return _statTotalCardsDrawn; } }

	private void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
		Debug.Assert(_cardTextures.Length == (3 * 5) + (3 * 5), "There is a mismatch in number of textures and number of cards.");
		Debug.Assert(_controlBlocker != null, "_controlBlocker is not assigned.");

		_cards = new Card[MAX_CARDS];
		for (int iCard = 0; iCard < MAX_CARDS; iCard++)
		{
			_cards[iCard] = transform.Find("Card " + (iCard + 1)).GetComponent<Card>();
			_cards[iCard].SetCardIndex(iCard);
			_cards[iCard].OnCardExecute += TryExecuteAndDiscardCard;
		}

		_isFirstDraw = true;
		_numCardsInHand = 0;
		_statTotalCardsDrawn = 0;
		ToggleControlBlocker(true);
	}

	private void Start()
	{
		HideAllCards();
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
		_isFirstDraw = true;
		_skipNextDraw = false;
		_numCardsInHand = 0;
		_statTotalCardsDrawn = 0;
		HideAllCards();
	}

	public void ResetFromCardHandData(DataLoader.CardHandData inCardHandData)
	{
		_isFirstDraw = false;
		_skipNextDraw = true;
		_numCardsInHand = 0;
		_statTotalCardsDrawn = 0; // TODO: Save and load this stat.
		HideAllCards();

		DrawCard(null, false, inCardHandData.CardDatas);
	}

	public void NextDrawSkipped()
	{
		_skipNextDraw = false;
	}

	public DataLoader.CardHandData GenerateCardHandData()
	{
		ReorganiseCards(null, false);
		CardData[] cardDatas = new CardData[_numCardsInHand];
		for (int iCard = 0; iCard < _numCardsInHand; iCard++)
		{
			cardDatas[iCard] = _cards[iCard].CardData;
		}

		return new DataLoader.CardHandData(cardDatas);
	}

	public Texture GetCardTexture(eCardTier inCardTier, eCardType inCardType, eMoveType inCardMoveType)
	{
		int texIndex = ((int)inCardType * 3);
		if (inCardType == eCardType.Movement)
		{
			texIndex += (int)inCardTier * 5 + (int)inCardMoveType;
		}
		else
		{
			texIndex += (int)inCardTier;
		}

		return _cardTextures[texIndex];
	}

	public void ToggleControlBlocker(bool inBlocked)
	{
		_controlBlocker.raycastTarget = inBlocked;
	}

	public void DrawCard(int inNumCardsDrawn, DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true)
	{
		CardData[] cardDatas = new CardData[inNumCardsDrawn];
		for (int iCard = 0; iCard < inNumCardsDrawn; iCard++)
		{
			cardDatas[iCard] = GenerateRandomCardData();
		}
		DrawCard(inOnComplete, inIsAnimated, cardDatas);
	}
	public void DrawCard(DTJob.OnCompleteCallback inOnComplete = null, bool inIsAnimated = true, params CardData[] inCardDatas)
	{
		bool needReorg = ReorganiseCards(() => { DrawCard(inOnComplete, inIsAnimated, inCardDatas); });

		if (!needReorg)
		{
			int numCardsDrawn = inCardDatas.Length;
			int cardLimit = Mathf.Min(_numCardsInHand + numCardsDrawn, MAX_CARDS);
			int cardsDrawn = 0;

			if (inIsAnimated)
			{
				const float CARD_ANIM_INTERVAL = 0.5f;
				for (int iCard = _numCardsInHand; iCard < cardLimit; iCard++)
				{
					Debug.Assert(!_cards[iCard].IsEnabled, "Card " + iCard + " is already enabled! Should not be drawn.");
					SetCardActive(iCard, true);
					_cards[iCard].SetCard(inCardDatas[cardsDrawn]);
					if ((iCard + 1) == cardLimit)
					{
						_cards[iCard].AnimateDrawCard(cardsDrawn * CARD_ANIM_INTERVAL, inOnComplete);
					}
					else
					{
						_cards[iCard].AnimateDrawCard(cardsDrawn * CARD_ANIM_INTERVAL);
					}
					cardsDrawn++;
					_numCardsInHand++;
					_statTotalCardsDrawn++;
				}
			}
			else
			{
				for (int iCard = _numCardsInHand; iCard < cardLimit; iCard++)
				{
					Debug.Assert(!_cards[iCard].IsEnabled, "Card " + iCard + " is already enabled! Should not be drawn.");
					SetCardActive(iCard, true);
					_cards[iCard].SetCard(inCardDatas[cardsDrawn]);
					if ((iCard + 1) == cardLimit)
					{
						if (inOnComplete != null) inOnComplete();
					}
					cardsDrawn++;
					_numCardsInHand++;
					_statTotalCardsDrawn++;
				}
			}

			// NOTE: If didn't draw card, then just run inOnComplete.
			if (cardsDrawn == 0 && inOnComplete != null) inOnComplete();
		}

		_isFirstDraw = false;
	}

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
				DelayAction delayedExecution = new DelayAction(REORG_ANIM_DURATION);
				delayedExecution.OnActionFinish += () => { inOnComplete(); };
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
		// Chances for card tier.
		// Normal: 80%
		// Silver: 15%
		// Gold  : 5%
		eCardTier cardTier = eCardTier.Normal;
		float cardTierRandValue = Random.value;
		if (cardTierRandValue < 0.05f) cardTier = eCardTier.Gold;
		else if (cardTierRandValue < 0.2f) cardTier = eCardTier.Silver;
		else cardTier = eCardTier.Normal;

		// Chances for movement.
		// Movement: 70%
		// Non Movement: 30%
		eMoveType moveType = eMoveType.Pawn;
		eCardType cardType = eCardType.Movement;
		if (Random.value <= 0.7f) // Movement
		{
			cardType = eCardType.Movement;
			// Chances for move types
			// Pawn: 30%
			// Rook: 20%
			// Bishop: 20%
			// Knight: 15%
			// King: 15%
			float moveTypeRandValue = Random.value;
			if (moveTypeRandValue <= 0.3f) moveType = eMoveType.Pawn;
			else if (moveTypeRandValue <= 0.5f) moveType = eMoveType.Rook;
			else if (moveTypeRandValue <= 0.7f) moveType = eMoveType.Bishop;
			else if (moveTypeRandValue <= 0.85f) moveType = eMoveType.Knight;
			else moveType = eMoveType.King;
		}
		else
		{
			// Non-movement
			// Non movement percentages
			// Joker: 20%
			// Shield: 20%
			// Smash: 10%
			// Clone: 20%
			// Draw: 30%
			float cardTypeRandValue = Random.value;
			if (cardTypeRandValue <= 0.2f) cardType = eCardType.Joker;
			else if (cardTypeRandValue <= 0.4f) cardType = eCardType.Shield;
			else if (cardTypeRandValue <= 0.5f) cardType = eCardType.Smash;
			else if (cardTypeRandValue <= 0.7f) cardType = eCardType.Clone;
			else cardType = eCardType.Draw;
		}

		return new CardData(cardTier, cardType, false, moveType);
	}

	private void SwapCards(int inCardIndexA, int inCardIndexB)
	{
		CardData cardDataA = _cards[inCardIndexA].CardData;
		CardData cardDataB = _cards[inCardIndexB].CardData;
		_cards[inCardIndexA].SetCard(cardDataB);
		_cards[inCardIndexB].SetCard(cardDataA);
	}

	private bool _isCloneMode = false;
	private int _numToClone = -1;
	private void TryExecuteAndDiscardCard(int inCardIndex)
	{
		Card card = _cards[inCardIndex];
		CardData cardData = card.CardData;

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
				card.ReturnCardAndUnexecute(cloneRejectReasonStr);
			}
			else
			{
				CardData newClonedData = cardData;
				newClonedData.isCloned = true;

				CardData[] cloneCardDatas = new CardData[_numToClone];
				for (int iClone = 0; iClone < _numToClone; iClone++)
				{
					cloneCardDatas[iClone] = newClonedData;
				}

				_isCloneMode = false;
				_numToClone = -1;
				ToggleControlBlocker(true);

				_numCardsInHand--;
				_cards[inCardIndex].AnimateCardExecuteAndDisable(() =>
				{
					DungeonCardDrawer.EnableEndTurnBtn();
					DrawCard(() =>
					{
						ToggleControlBlocker(false);
					},
					true,
					cloneCardDatas);
				});
			}
		}
		#region NormalExecutionMode
		else
		{
			bool closeDrawerAfterExecute = true;
			Utils.GenericVoidDelegate postExecuteCardAnimActions = null;
			DTJob.OnCompleteCallback postCloseDrawerAnimActions = null;
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
					DungeonCamera.FocusCameraToTile(_dungeon.MorphyController.MorphyPos, 0.6f);
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
						closeDrawerAfterExecute = false;
						int numClones = -1;
						switch (cardData.cardTier)
						{
							case eCardTier.Normal: numClones = 1; break;
							case eCardTier.Silver: numClones = 2; break;
							case eCardTier.Gold: numClones = 3; break;
							default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
						}

						DungeonCardDrawer.DisableEndTurnBtn("Select a card to clone");
						postExecuteCardAnimActions += () =>
						{
							_isCloneMode = true;
							_numToClone = numClones + 1; // NOTE: 1 is to replace the copy itself.
						};
					}
					else // NOTE: Return the card.
					{
						isCardRejected = true;
						rejectReasonStr = "No Cloneable Cards";
					}
	                break;
				}
				case eCardType.Smash:
				{
					postCloseDrawerAnimActions += () => { _dungeon.MorphyController.Smash(cardData.cardTier); };
					break;
				}
				case eCardType.Draw:
				{
					closeDrawerAfterExecute = false;
					int numDraws = -1;
					switch (cardData.cardTier)
					{
						case eCardTier.Normal: numDraws = 2; break;
						case eCardTier.Silver: numDraws = 3; break;
						case eCardTier.Gold: numDraws = 5; break;
						default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
					}

					postExecuteCardAnimActions += () => { DrawCard(numDraws, () => { ToggleControlBlocker(false); }); };
					ToggleControlBlocker(true);
					break;
				}
				case eCardType.Shield:
				{
					closeDrawerAfterExecute = false;
					int numShield = -1;
					switch (cardData.cardTier)
					{
						case eCardTier.Normal: numShield = 1; break;
						case eCardTier.Silver: numShield = 3; break;
						case eCardTier.Gold: numShield = 5; break;
						default: Debug.LogError("case: " + cardData.cardTier.ToString() + " has not been handled."); break;
					}

					postExecuteCardAnimActions += () => { _dungeon.MorphyController.AwardShield(numShield); };
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
					DungeonCamera.FocusCameraToTile(_dungeon.MorphyController.MorphyPos, 0.6f);
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
				card.ReturnCardAndUnexecute(rejectReasonStr);
			}
			else
			{
				_numCardsInHand--;
				_cards[inCardIndex].AnimateCardExecuteAndDisable(() =>
				{
					if (closeDrawerAfterExecute)
					{
						DungeonCardDrawer.EnableCardDrawer(false, true, true, postCloseDrawerAnimActions);
					}

					if (postExecuteCardAnimActions != null) postExecuteCardAnimActions();
				});
			}
		}
		#endregion
	}
}
