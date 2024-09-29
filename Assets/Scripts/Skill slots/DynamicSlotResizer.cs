using UnityEngine;
using UnityEngine.UI;

public class DynamicSlotResizer : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform slotRectTransform;
    public int ChildrenPerRow = 4; // How many children you want per row
    public float padding = 10f; // Additional space around the children
    public Vector2 defaultSlotSize; // Default size when the number of children is below the threshold
    public int ChildrenThreshold = 4; // Number of children before the slot starts to resize

    void Update()
    {
        AdjustSlotSize();
    }

    public void AdjustSlotSize()
    {
        int childCount = gridLayoutGroup.transform.childCount;

        // If the number of children is less than or equal to the threshold, use the default size
        if (childCount <= ChildrenThreshold)
        {
            slotRectTransform.sizeDelta = defaultSlotSize;
            return;
        }

        // calculates how many rows are required
        int rows = Mathf.CeilToInt((float)childCount / ChildrenPerRow);

        // Calculate the new height of the slot based on rows, dice size, spacing, and padding
        float newHeight = rows * gridLayoutGroup.cellSize.y + (rows - 1) * gridLayoutGroup.spacing.y + padding;

        // Calculate the width based on dicePerRow, dice size, spacing, and padding
        float newWidth = ChildrenPerRow * gridLayoutGroup.cellSize.x + (ChildrenPerRow - 1) * gridLayoutGroup.spacing.x + padding;

        // Set the new size of the slot container with extra padding
        slotRectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
