using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardHandler : ScriptableObject
{
    public string cardName;
    public Sprite cardSprite;
    public string suit; // Hearts, Diamonds, Clubs, Spades
    public int rank;    // 1 (Ace) to 13 (King)
}