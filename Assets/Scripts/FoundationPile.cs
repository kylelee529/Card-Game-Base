using UnityEngine;

public class FoundationPile : MonoBehaviour
{
    public string suit;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Card card = collision.GetComponent<Card>();
        if (card != null && card.suit == suit && card.value == transform.childCount + 1)
        {
            card.transform.SetParent(transform);
            card.transform.position = transform.position;
        }
    }
}
