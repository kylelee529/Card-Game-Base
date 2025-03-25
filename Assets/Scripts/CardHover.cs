using UnityEngine;
using DG.Tweening;

public class CardHover : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isHovered = false;
    private bool isSelected = false;
    private HandManager handManager;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;

    private CardHandler cardData;
    private Tooltip tooltip;

    public void SetOriginalPosition(Vector3 position) => originalPosition = position;
    public Vector3 GetOriginalPosition() => originalPosition;
    public void SetHandManager(HandManager hManager) => handManager = hManager;
    public void SetCardData(CardHandler data) => cardData = data;
    public CardHandler GetCardData() => cardData;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalSortingOrder = spriteRenderer.sortingOrder;

        tooltip = FindFirstObjectByType<Tooltip>(); // Find Tooltip in scene
    }

    private void OnMouseEnter()
    {
        if (isHovered || isSelected) return;

        isHovered = true;
        transform.DOMoveY(originalPosition.y + 0.2f, 0.2f);
        spriteRenderer.sortingOrder = 100;

        if (tooltip != null && cardData != null)
        {
            tooltip.ShowTooltip(cardData.cardName, transform); // Pass the card's transform
        }
    }

    private void OnMouseExit()
    {
        if (isSelected) return;

        isHovered = false;
        transform.DOMoveY(originalPosition.y, 0.2f);
        spriteRenderer.sortingOrder = originalSortingOrder;

        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }

    private void OnMouseDown()
    {
        if (handManager == null)
        {
            Debug.LogError("HandManager is NULL! Make sure it's assigned.");
            return;
        }

        isSelected = !isSelected;
        handManager.SelectCard(gameObject);
    }

    public void HideTooltip() // Call this before destroying a card
    {
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
}
