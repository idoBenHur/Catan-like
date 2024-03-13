using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class IndicatorPlacementRoad : MonoBehaviour
{
    public GameObject prefabToPlace;
    public Tilemap tilemap;
    private bool HasRoadInRadius = false;


    private void Start()
    {
       // FindHexesCenterPoint();
    }


    public void FindHexesCenterPoint()
    {

        // Gets the "smallest" & the "biggest" tile postion int the tilemap gird.  for example min = (-6, -2, 0); max = (7, 4, 0); (x, y, z)
        Vector3Int minTile = tilemap.cellBounds.min;
        Vector3Int maxTile = tilemap.cellBounds.max;


        for (int x = minTile.x; x < maxTile.x; x++)
        {

            for (int y = minTile.y; y < maxTile.y; y++)
            {

                Vector3Int GridPosition = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                Vector3 WorldPostion = tilemap.CellToWorld(GridPosition);



                if (tilemap.HasTile(GridPosition))
                {
                    PlacePrefabsAroundHex(WorldPostion);
                }
            }
        }

    }


    void PlacePrefabsAroundHex(Vector3 HexCenter)
    {

        float tilemapScale = tilemap.transform.localScale.x;
        float outerRadius = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);
        float apothem = outerRadius * Mathf.Cos(Mathf.PI / 6); // Cos(30°) in radians

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 sideMidpoint = new Vector3(HexCenter.x + apothem * Mathf.Cos(angle_rad), HexCenter.y + apothem * Mathf.Sin(angle_rad), HexCenter.z);


            Vector2 sideMidpoint2D = new Vector2(sideMidpoint.x, sideMidpoint.y);
            Collider2D ExsitingColiderInPosition = Physics2D.OverlapPoint(sideMidpoint2D);

            if (ExsitingColiderInPosition != null)
            {
                continue;
            }


            Collider2D[] hitCollidersInRadius = Physics2D.OverlapCircleAll(sideMidpoint2D, 1f);  //check for coliders in a certin radius
            HasRoadInRadius = false;

            foreach (var hitCollider in hitCollidersInRadius)
            {
                if (hitCollider.gameObject.CompareTag("Road"))
                {
                    HasRoadInRadius = true;

                }

            }

            if (HasRoadInRadius == true)
            {
                Quaternion rotation = Quaternion.Euler(0f, 0f, (angle_deg + 90f));
                Instantiate(prefabToPlace, sideMidpoint, rotation);
                Debug.Log("hi");

            }

        }
    }
}