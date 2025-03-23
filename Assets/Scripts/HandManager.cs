using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    private List<GameObject> handCards = new();

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
            hoverScript.SetCardData(cardData); // Pass card data
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

            // Set layer order based on index (ensures leftmost is under, rightmost is over)
            SpriteRenderer sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = i;
            }
        }
    }

    public void DiscardCard(GameObject card)
{
    Debug.Log("Attempting to discard: " + card.name);

    if (handCards.Count == 0)
    {
        Debug.LogWarning("Hand is empty! Nothing to discard.");
        return;
    }

    if (!handCards.Contains(card))
    {
        Debug.LogWarning("Card not found in hand list: " + card.name);
        Debug.Log("Current hand contents:");
        foreach (var c in handCards)
        {
            Debug.Log("- " + c.name);
        }
        return;
    }

    handCards.Remove(card);

    // Stop DOTween animations on the card before destroying it
    card.transform.DOKill();  

    // Hide tooltip before deletion
    Tooltip tooltip = Object.FindFirstObjectByType<Tooltip>();
    if (tooltip != null)
    {
        tooltip.HideTooltip();
    }

    Destroy(card);
    Debug.Log("Card discarded: " + card.name);

    // Rearrange the remaining cards
}


}
