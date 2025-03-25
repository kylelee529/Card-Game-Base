using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using TMPro;
using System.Linq;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI pokerHandText;
    [SerializeField] private TextMeshProUGUI scoreText; // UI for score
    [SerializeField] private int score = 0;

    private List<GameObject> handCards = new();
    private List<GameObject> selectedCards = new();

    private void Start()
    {
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DrawCard();
    }

    private void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;

        CardHandler cardData = deckManager.DrawCard();
        if (cardData == null) return;

        GameObject cardObject = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity);
        SpriteRenderer sr = cardObject.GetComponent<SpriteRenderer>();

        if (sr != null) sr.sprite = cardData.cardSprite;

        CardHover hoverScript = cardObject.GetComponent<CardHover>();
        if (hoverScript != null)
        {
            hoverScript.SetCardData(cardData);
            hoverScript.SetOriginalPosition(spawnPoint.position);
            hoverScript.SetHandManager(this);
        }

        handCards.Add(cardObject);
        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            GameObject card = handCards[i];
            float p = Mathf.Clamp(firstCardPosition + i * cardSpacing, 0f, 1f);
            Vector3 splinePosition = spline.EvaluatePosition(p);

            card.transform.DOMove(splinePosition, 0.25f);
            card.GetComponent<CardHover>()?.SetOriginalPosition(splinePosition);

            SpriteRenderer sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = i;
            }
        }
    }

    public void SelectCard(GameObject card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.transform.DOMoveY(card.GetComponent<CardHover>().GetOriginalPosition().y, 0.2f);
        }
        else
        {
            selectedCards.Add(card);
            card.transform.DOMoveY(card.transform.position.y + 0.3f, 0.2f);
        }

        CheckPokerHand();
    }

    public void DiscardSelectedCards()
    {
        if (selectedCards.Count == 0) return;

        foreach (GameObject card in selectedCards)
        {
            handCards.Remove(card);
            card.transform.DOKill();
            Destroy(card);
        }

        selectedCards.Clear();
        pokerHandText.text = ""; // Reset hand display
        UpdateCardPositions();
    }

    public void CheckPokerHand()
    {
        if (selectedCards.Count < 2)
        {
            pokerHandText.text = ""; // No valid hand
            return;
        }

        // Create a list of CardHandler objects from selected cards
        List<CardHandler> cardDataList = new();
        foreach (GameObject cardObj in selectedCards)
        {
            CardHover hoverScript = cardObj.GetComponent<CardHover>();
            if (hoverScript != null)
            {
                cardDataList.Add(hoverScript.GetCardData());
            }
        }

        // Calculate the score using the new system
        var (handType, score) = PokerHandEvaluator.EvaluateHand(cardDataList);
        pokerHandText.text = "Score: " + score.ToString(); // Convert int to string
    }

    public void PlaySelectedHand()
{
    if (selectedCards.Count < 2) return;

    // Create a list of CardHandler objects from selected cards
    List<CardHandler> cardDataList = new();
    foreach (GameObject cardObj in selectedCards)
    {
        cardDataList.Add(cardObj.GetComponent<CardHover>().GetCardData());
    }

    // Use EvaluateHand to get the hand type (no score yet)
    var (handType, _) = PokerHandEvaluator.EvaluateHand(cardDataList);

    // Fetch base score and multiplier from PokerHandEvaluator
    var (baseChip, baseMultiplier) = PokerHandEvaluator.GetHandScoring(handType);

    // Calculate the Total Face Value of the Cards Played
    int totalFaceValue = cardDataList.Sum(card => card.rank); // Assuming rank is the face value

    // Final Score Calculation (Balatro scoring logic)
    // First add baseChip and totalFaceValue, then multiply by baseMultiplier
    int finalScore = (baseChip + totalFaceValue) * baseMultiplier;

    // Update score: Add the final score to the current score
    score += finalScore;

    // Remove played cards from the hand
    foreach (GameObject card in selectedCards)
    {
        handCards.Remove(card);
        card.transform.DOKill();
        Destroy(card);
    }
    selectedCards.Clear();

    // Display the hand type ONLY in the text (no score here)
    pokerHandText.text = handType;

    // Display the final points in the score text
    UpdateScoreUI();
    UpdateCardPositions();
}




// New method to calculate the multiplier for the hand type
    private int GetBaseMultiplierForHand(string handType)
{
    var scoring = PokerHandEvaluator.GetHandScoring(handType);
    return scoring.multiplier;
}

private int GetBaseChipForHand(string handType)
{
    var scoring = PokerHandEvaluator.GetHandScoring(handType);
    return scoring.baseChip;
}

    private string GetPokerHandType(List<CardHandler> cardDataList)
    {
        var ranks = cardDataList.Select(c => c.rank).OrderBy(r => r).ToList();
        var suits = cardDataList.Select(c => c.suit).ToList();
        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = PokerHandEvaluator.IsStraight(ranks);

        if (isStraight && isFlush) return "Straight Flush";
        else if (PokerHandEvaluator.HasMatchingRanks(ranks, 4)) return "Four of a Kind";
        else if (PokerHandEvaluator.HasFullHouse(ranks)) return "Full House";
        else if (isFlush) return "Flush";
        else if (isStraight) return "Straight";
        else if (PokerHandEvaluator.HasMatchingRanks(ranks, 3)) return "Three of a Kind";
        else if (PokerHandEvaluator.HasTwoPairs(ranks)) return "Two Pair";
        else if (PokerHandEvaluator.HasMatchingRanks(ranks, 2)) return "One Pair";
        
        return "High Card"; // Default case
    }

    private int GetPointsForHand(string handType)
    {
        return handType switch
        {
            "Straight Flush" => 50,
            "Four of a Kind" => 40,
            "Full House" => 30,
            "Flush" => 25,
            "Straight" => 20,
            "Three of a Kind" => 15,
            "Two Pair" => 10,
            "One Pair" => 5,
            _ => 0
        };
    }
    private void UpdateScoreUI()
    {
        scoreText.text = "Chips: " + score;
    }
}
