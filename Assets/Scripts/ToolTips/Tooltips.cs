using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Loading;
using static System.Net.Mime.MediaTypeNames;

//[ExecuteInEditMode()]
public class Tooltips : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI textField;
    public LayoutElement layoutElement;
    public int charachterWarpLimit;
    public RectTransform rectTransform;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string text, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        textField.text = text;

        int headerLength = headerField.text.Length;
        int textLength = textField.text.Length;


        layoutElement.enabled = (headerLength > charachterWarpLimit || textLength > charachterWarpLimit) ? true : false;
        //layoutElement.enabled = Mathf.Max(headerField.preferredWidth, textField.preferredWidth) >= layoutElement.preferredWidth;
        //layoutElement.enabled = (headerLength > charachterWarpLimit || textLength > charachterWarpLimit);
    }


    private void Update()
    {

        Vector2 position = Input.mousePosition;

        // Calculate the pivot based on the mouse position
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;


        pivotX = (pivotX > 0.5f) ? 1f : 0f; // Set pivotX to 1 if the mouse is on the right half, else 0 (left half)
        pivotY = (pivotY > 0.5f) ? 1f : 0f;  // Set pivotY to 1 if the mouse is on the top half, else 0 (bottom half)




        rectTransform.pivot = new Vector2(pivotX, pivotY);

        // Calculate the offset to prevent the tooltip from overlapping the cursor
        float offsetX = (pivotX == 1f) ? -10f : 10f; // If the pivotX is 1 (right side), move the tooltip to the left of the cursor by 10 units (negative offset). ellse, the oter way around

        float offsetY = (pivotY == 1f) ? -10f : 10f; // the same but on the Y

        position += new Vector2(offsetX, offsetY);

        // Ensure the tooltip stays within screen bounds
        position.x = Mathf.Clamp(position.x, rectTransform.rect.width * rectTransform.pivot.x, Screen.width - rectTransform.rect.width * (1 - rectTransform.pivot.x));
        position.y = Mathf.Clamp(position.y, rectTransform.rect.height * rectTransform.pivot.y, Screen.height - rectTransform.rect.height * (1 - rectTransform.pivot.y));

        transform.position = position;




    }


}
