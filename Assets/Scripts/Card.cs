using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value;
    public string suit;
    public bool isFaceUp;
    
    private Vector3 originalPosition;
    private Transform originalParent;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        UpdateCardVisual();
    }

    public void SetCard(int value, string suit, bool faceUp)
    {
        this.value = value;
        this.suit = suit;
        isFaceUp = faceUp;
        UpdateCardVisual();
    }

    public void FlipCard(bool faceUp)
    {
        isFaceUp = faceUp;
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        string spritePath = isFaceUp ? $"Cards/{suit}_{value}" : "Cards/card_back";
        image.sprite = Resources.Load<Sprite>(spritePath);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isFaceUp) return;

        originalPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isFaceUp) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
}
