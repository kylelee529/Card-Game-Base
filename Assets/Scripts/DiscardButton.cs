using UnityEngine;
using UnityEngine.UI;

public class DiscardButton : MonoBehaviour
{
    public HandManager handManager;
    public Button discardButton;
    public Sprite normalSprite;  // Default sprite
    public Sprite pressedSprite; // Sprite when pressed
    private Image buttonImage;

    private void Start()
    {
        buttonImage = discardButton.GetComponent<Image>();
        discardButton.onClick.AddListener(OnDiscardButtonPressed);
    }

    private void OnDiscardButtonPressed()
    {
        if (buttonImage != null && pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite; // Change to pressed sprite
        }

        handManager.DiscardSelectedCards();

        // Reset back to normal sprite after a short delay
        Invoke(nameof(ResetButtonSprite), 0.1f);
    }

    private void ResetButtonSprite()
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite; // Revert back to normal sprite
        }
    }
}
