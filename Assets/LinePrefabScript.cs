using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinePrefabScript : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    public LayerMask drawableLayer;
    private List<Vector2> points;

    private void Awake()
    {
        points = new List<Vector2>(); // Initialize the points list
        lineRenderer.useWorldSpace = false; // Use local space coordinates
    }

    public void UpdateLine(Vector2 position)
    {
        if (points.Count == 0)
        {
            //points = new List<Vector2>();

            if (IsPointInDrawableArea(position))
            {
                SetPoints(position);
            }
            return;
        }

        if (Vector2.Distance(points.Last(), position) > .1f && IsPointInDrawableArea(position))
        {
            SetPoints(position);
        }

    }

    void SetPoints(Vector2 point)
    {
        points.Add(point);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count -1, point);

    }

    private bool IsPointInDrawableArea(Vector2 point)
    {
        //Vector2 worldPoint = Camera.main.ScreenToWorldPoint(point);
        Collider2D hit = Physics2D.OverlapPoint(point, drawableLayer);
        return hit != null;
    }
}
