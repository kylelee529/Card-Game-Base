using UnityEngine;

public class TableauPile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Card card = collision.GetComponent<Card>();
        if (card != null && transform.childCount > 0)
        {
            Card topCard = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
            if (topCard.isFaceUp && card.value == topCard.value - 1)
            {
                card.transform.SetParent(transform);
                card.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, 0);
            }
        }
        else if (card.value == 13) // King can be placed on empty tableau
        {
            card.transform.SetParent(transform);
            card.transform.position = transform.position;
        }
    }
}
