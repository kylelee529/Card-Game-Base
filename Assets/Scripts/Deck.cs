using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform drawPile;
    public Transform tableau;

    private List<CardData> deck = new List<CardData>();

    private void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        DealCards();
    }

    private void GenerateDeck()
    {
        string[] suits = { "hearts", "diamonds", "clubs", "spades" };
        for (int i = 1; i <= 13; i++)
        {
            foreach (string suit in suits)
            {
                deck.Add(new CardData { value = i, suit = suit });
            }
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }

    private void DealCards()
    {
        int[] tableauLayout = { 1, 2, 3, 4, 5, 6, 7 };
        int index = 0;

        for (int i = 0; i < tableauLayout.Length; i++)
        {
            for (int j = 0; j < tableauLayout[i]; j++)
            {
                GameObject newCard = Instantiate(cardPrefab, tableau);
                Card cardScript = newCard.GetComponent<Card>();
                bool faceUp = (j == tableauLayout[i] - 1);
                cardScript.SetCard(deck[index].value, deck[index].suit, faceUp);
                newCard.transform.position += new Vector3(i * 100, -j * 30, 0);
                index++;
            }
        }

        for (; index < deck.Count; index++)
        {
            GameObject newCard = Instantiate(cardPrefab, drawPile);
            Card cardScript = newCard.GetComponent<Card>();
            cardScript.SetCard(deck[index].value, deck[index].suit, false);
        }
    }
}

public struct CardData
{
    public int value;
    public string suit;
}
