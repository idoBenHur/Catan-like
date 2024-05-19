using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public GameObject LinePrefab;

    private LinePrefabScript activeLine;
    public RectTransform uiParent;
    public LayerMask drawableLayer;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Debug.Log("Mouse Down at Screen Position: " + mousePos);
            if (IsMouseInDrawableArea(mousePos))
            {
                GameObject newLine = Instantiate(LinePrefab, uiParent);
                activeLine = newLine.GetComponent<LinePrefabScript>();
                activeLine.drawableLayer = drawableLayer; // Assign the drawable layer to the new line instance


                Vector2 localMousePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, mousePos, null, out localMousePos);
                activeLine.UpdateLine(localMousePos);

               
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            activeLine = null;
        }

        if (activeLine != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, mousePos, null, out localMousePos);
            activeLine.UpdateLine(localMousePos);
        }
    }


    private bool IsMouseInDrawableArea(Vector2 point)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(point);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint, drawableLayer);
        return hit != null;
    }

}
