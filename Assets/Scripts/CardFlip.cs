using DG.Tweening;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    private bool flipped = false;
    private bool isFlipping = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isFlipping)
        {
            Flip();
        }
    }

    private void Flip()
    {
        flipped = !flipped;
        isFlipping = true;

        transform.DORotate(new Vector3(0, flipped ? 180f : 0f, 0), 0.25f).SetEase(Ease.OutQuad).OnComplete(() => isFlipping = false);
    }
}
