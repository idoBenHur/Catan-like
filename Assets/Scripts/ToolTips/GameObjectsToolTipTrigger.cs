using UnityEngine;
using DG.Tweening; // Ensure you have the DOTween namespace if you're using it.

public class GameObjectsToolTipTrigger : MonoBehaviour
{
    public string header;

    [TextArea(3, 10)]
    public string text;
    private Tween tooltipTween;

    void OnMouseEnter()
    {
        // Show the tooltip after a delay
        tooltipTween = DOVirtual.DelayedCall(0.3f, () => ToolTipSystem.Show(text, header));
    }

    void OnMouseExit()
    {
        // Kill the delayed call if it exists and is active
        if (tooltipTween != null && tooltipTween.IsActive())
        {
            tooltipTween.Kill();
        }

        // Hide the tooltip
        ToolTipSystem.Hide();
    }
}
