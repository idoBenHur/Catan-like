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

        int headerLength = headerField.text.Length;
        int textLength = textField.text.Length;

        // layoutElement.enabled = (headerLength > charachterWarpLimit || textLength > charachterWarpLimit) ? true : false;
        //layoutElement.enabled = Mathf.Max(headerField.preferredWidth, textField.preferredWidth) >= layoutElement.preferredWidth;
        //layoutElement.enabled = (headerLength > charachterWarpLimit || textLength > charachterWarpLimit);


        //Vector2 postion = Input.mousePosition;

        //float pivotX = postion.x / Screen.width;
        //float pivotY = postion.y / Screen.height;

        //rectTransform.pivot = new Vector2(pivotX, pivotY);
        //transform.position = postion;


        Vector2 offset = new Vector2(10f, 20f);
        Vector2 position = Input.mousePosition;

        // Adjusting the tooltip's position to include an offset to keep the cursor from overlapping the text
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;


        float finalPivotX = 0f;
        float finalPivotY = 0f;

        if (pivotX < 0.5) //If mouse on left of screen move tooltip to right of cursor and vice vera
        {
            finalPivotX = -0.2f;
        }

        else
        {
            finalPivotX = 1.01f;
        }

        if (pivotY < 0.5) //If mouse on lower half of screen move tooltip above cursor and vice versa
        {
            finalPivotY = 0;
        }

        else
        {
            finalPivotY = 1;
        }



        // Adjust the pivot to prevent the tooltip from going outside the screen bounds
       // pivotX = pivotX > 0.5f ? 1f : 0f;
       // pivotY = pivotY > 0.5f ? 1f : 0f;




        rectTransform.pivot = new Vector2(finalPivotX, finalPivotY);

        // Set the position of the tooltip with an offset depending on the pivot
        //float offsetX = (pivotX == 1) ? -offset.x : offset.x;
        // float offsetY = (pivotY == 1) ? -offset.y : offset.y;

        //   transform.position = position + new Vector2(offsetX, offsetY);
        transform.position = position;


    }


}
