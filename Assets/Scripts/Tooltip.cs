using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using System.Numerics;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltipBox;
    public Canvas tooltipCanvas;
    public UnityEngine.Vector2 padding = new UnityEngine.Vector2(1f, 1f);

    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string text, Transform cardTransform)
{
    if (tooltipBox == null || tooltipText == null) return;

    // Format suits with colors and bold text
    string formattedText = text.Replace("Spades", "<b><color=#000000>Spades</color></b>")   // Black
                               .Replace("Clubs", "<b><color=#000000>Clubs</color></b>")     // Black
                               .Replace("Hearts", "<b><color=#FF0000>Hearts</color></b>")   // Red
                               .Replace("Diamonds", "<b><color=#FF0000>Diamonds</color></b>"); // Red

    tooltipText.text = formattedText;
    tooltipBox.gameObject.SetActive(true);

    // Force update layout so we get the correct text size
    tooltipText.ForceMeshUpdate();
    Vector2 textSize = tooltipText.textBounds.size;

    // Set the panel size with small padding
    float paddingX = 15f; // Small horizontal padding
    float paddingY = 8f;  // Small vertical padding
    tooltipBox.sizeDelta = new Vector2(textSize.x + paddingX, textSize.y + paddingY);

    // Position tooltip above the card
    Vector3 worldPosition = cardTransform.position + Vector3.up * 2.8f; // Adjust height as needed
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
    tooltipBox.position = screenPoint;
}



    public void HideTooltip()
    {
        if (tooltipBox != null)
        {
            tooltipBox.gameObject.SetActive(false); // Hide the tooltip
        }
    }
}
