using UnityEngine;
using DG.Tweening;

public class CardHover : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isHovered = false;
    private Tooltip tooltip;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private string cardName; // Store card name directly
    private HandManager handManager;

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    public void SetCardData(CardHandler cardData) 
    {
        cardName = cardData.cardName; // Store the name directly
    }

    public void SetHandManager(HandManager hManager)
    {
        handManager = hManager;
    }

    private void Start()
    {
        tooltip = Object.FindFirstObjectByType<Tooltip>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    private void OnMouseEnter()
    {
        if (isHovered) return;

        isHovered = true;
        transform.DOMoveY(originalPosition.y + 0.2f, 0.2f); // Lift card slightly

        //if (spriteRenderer != null)
        //{
            //spriteRenderer.sortingOrder = 100; // Bring card to front
        //}

        if (tooltip != null && !string.IsNullOrEmpty(cardName))
        {
            tooltip.ShowTooltip(cardName, transform); // Use stored cardName
        }
    }

    private void OnMouseExit()
    {
        isHovered = false;
        transform.DOMoveY(originalPosition.y, 0.2f); // Move back down

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder; // Restore original order
        }

        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }

private void OnMouseDown()
{
    Debug.Log("Mouse clicked on: " + gameObject.name);

    if (handManager == null)
    {
        Debug.LogError("HandManager is NULL! Make sure it's assigned when creating the card.");
        return;
    }

    Debug.Log("HandManager found! Attempting to discard.");
    handManager.DiscardCard(gameObject);
}
}
