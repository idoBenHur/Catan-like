using UnityEngine;
using UnityEngine.UI;

public class DynamicSlotResizer : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform slotRectTransform;
    public int dicePerRow = 4; // How many dice you want per row
    public float padding = 10f; // Additional space around the dice
    public Vector2 defaultSlotSize = new Vector2(100f, 100f); // Default size when the number of dice is below the threshold
    public int diceThreshold = 4; // Number of dice before the slot starts to resize

    void Update()
    {
        //AdjustSlotSize();
    }

    public void AdjustSlotSize()
    {
        int childCount = gridLayoutGroup.transform.childCount;

        // If the number of dice is less than or equal to the threshold, use the default size
        if (childCount <= diceThreshold)
        {
            slotRectTransform.sizeDelta = defaultSlotSize;
            return;
        }

        // If the number of dice exceeds the threshold, calculate the new size
        int rows = Mathf.CeilToInt((float)childCount / dicePerRow);

        // Calculate the new height of the slot based on rows, dice size, spacing, and padding
        float newHeight = rows * gridLayoutGroup.cellSize.y + (rows - 1) * gridLayoutGroup.spacing.y + padding;

        // Calculate the width based on dicePerRow, dice size, spacing, and padding
        float newWidth = dicePerRow * gridLayoutGroup.cellSize.x + (dicePerRow - 1) * gridLayoutGroup.spacing.x + padding;

        // Set the new size of the slot container with extra padding
        slotRectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
