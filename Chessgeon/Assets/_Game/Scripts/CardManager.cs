using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
	[SerializeField] private Texture[] _cardTextures = null;

	private Card[] _cards = null;

	private const int NUM_TOTAL_CARDS = 7;

	private void Awake()
	{
		Debug.Assert(_cardTextures.Length == (3 * 5) + (3 * 5), "There is a mismatch in number of textures and number of cards.");

		_cards = new Card[NUM_TOTAL_CARDS];
		for (int iCard = 0; iCard < NUM_TOTAL_CARDS; iCard++)
		{
			_cards[iCard] = transform.Find("Card " + (iCard + 1)).GetComponent<Card>();
		}
	}

	private void Start()
	{
		SetInitialHand();
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

	public void SetInitialHand()
	{
		const int NUM_STARTING_HAND = 7; // TODO: Balance this figure. for now 7 for debugging purposes.
		for (int iCard = 0; iCard < NUM_TOTAL_CARDS; iCard++)
		{
			if (iCard < NUM_STARTING_HAND)
			{
				SetCardActive(iCard, true);
				_cards[iCard].SetCard(GenerateRandomCardData());
			}
			else
			{
				SetCardActive(iCard, false);
			}
		}
	}

	private void SetCardActive(int inCardIndex, bool inIsActive)
	{
		_cards[inCardIndex].gameObject.SetActive(inIsActive);
	}

	private CardData GenerateRandomCardData()
	{
		// TODO: DEBUG FOR NOW. Re-balance once all is in.
		return new CardData(eCardTier.Normal, eCardType.Movement, (eMoveType)Random.Range(0, 5));
	}
}
