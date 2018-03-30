﻿using System.Collections;
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
			_cards[iCard].OnCardExecute += ExecuteAndDiscardCard;
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
		_numCardsInHand = 0;
		_statTotalCardsDrawn = 0;
		HideAllCards();
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

	public void DrawCard(int inNumCardsDrawn, DTJob.OnCompleteCallback inOnComplete = null)
	{
		bool needReorg = ReorganiseCards(() => { DrawCard(inNumCardsDrawn, inOnComplete); });

		if (!needReorg)
		{
			int cardLimit = Mathf.Min(_numCardsInHand + inNumCardsDrawn, MAX_CARDS);
			int cardsDrawn = 0;
			const float CARD_ANIM_INTERVAL = 0.5f;
			for (int iCard = _numCardsInHand; iCard < cardLimit; iCard++)
			{
				Debug.Assert(!_cards[iCard].IsEnabled, "Card " + iCard + " is already enabled! Should not be drawn.");
				SetCardActive(iCard, true);
				_cards[iCard].SetCard(GenerateRandomCardData());
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

		_isFirstDraw = false;
	}

	private bool ReorganiseCards(DTJob.OnCompleteCallback inOnComplete = null)
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
					offsetZ += 0.5f;

					SwapCards(curIndex, firstEmptyIndex);
					_cards[curIndex].SetEnabled(false);
					_cards[firstEmptyIndex].AnimateMoveToOriginFrom(_cards[curIndex].OriginLocalPos, REORG_ANIM_DURATION, offsetZ);
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
			DelayAction delayedExecution = new DelayAction(REORG_ANIM_DURATION);
			delayedExecution.OnActionFinish += () => { inOnComplete(); };
			ActionHandler.RunAction(delayedExecution);
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

		// TODO: DEBUG FOR NOW. Re-balance once all is in.
		return new CardData(cardTier, eCardType.Smash, (eMoveType)Random.Range(0, 5));
	}

	private void SwapCards(int inCardIndexA, int inCardIndexB)
	{
		CardData cardDataA = _cards[inCardIndexA].CardData;
		CardData cardDataB = _cards[inCardIndexB].CardData;
		_cards[inCardIndexA].SetCard(cardDataB);
		_cards[inCardIndexB].SetCard(cardDataA);
	}

	private void ExecuteAndDiscardCard(int inCardIndex)
	{
		Card card = _cards[inCardIndex];
		CardData cardData = card.CardData;
		bool closeDrawerAfterExecute = true;
		Utils.GenericVoidDelegate postExecuteCardAnimActions = null;
		DTJob.OnCompleteCallback postCloseDrawerAnimActions = null;

		switch (cardData.cardType)
		{
			case eCardType.Joker:
			{
				Debug.LogWarning("case: " + cardData.cardType.ToString() + " has not been handled.");
				break;
			}
			case eCardType.Duplicate:
			{
				closeDrawerAfterExecute = false;
				Debug.LogWarning("case: " + cardData.cardType.ToString() + " has not been handled.");
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

				postExecuteCardAnimActions += () => { DrawCard(numDraws, ()=> { ToggleControlBlocker(false); }); };
				ToggleControlBlocker(true);
				break;
			}
			case eCardType.Shield:
			{
				Debug.LogWarning("case: " + cardData.cardType.ToString() + " has not been handled.");
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
