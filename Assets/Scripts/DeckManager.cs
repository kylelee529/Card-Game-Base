using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardHandler[] allCards; // Assign 52 CardHandler assets in Inspector
    private List<CardHandler> deck = new();
    
    private void Start()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    private void InitializeDeck()
    {
        deck = new List<CardHandler>(allCards); // Copy all cards into deck
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardHandler temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public CardHandler DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        CardHandler drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }
}
