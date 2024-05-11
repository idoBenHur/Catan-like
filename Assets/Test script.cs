using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Testscript : MonoBehaviour
{

    public Tilemap tilemap;
    public GameObject prefab;

    void Start()
    {
        SpawnPrefabsAtCorners();
    }

    void SpawnPrefabsAtCorners()
    {
        Vector3 tilemapScale = tilemap.transform.localScale; // Get the scale of the tilemap
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(position.x, position.y, position.z);
            if (!tilemap.HasTile(localPlace)) continue;

            Vector3 center = tilemap.GetCellCenterWorld(localPlace);
            float size = Mathf.Max(tilemap.cellSize.x, tilemap.cellSize.y) / 2; // Half the cell size

            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60 * i - 30; // Offset by -30 degrees for pointy top
                float angle_rad = Mathf.Deg2Rad * angle_deg;
                Vector3 cornerOffset = new Vector3(size * Mathf.Cos(angle_rad), size * Mathf.Sin(angle_rad), 0);
                cornerOffset.x *= tilemapScale.x; // Scale adjustment for x
                cornerOffset.y *= tilemapScale.y; // Scale adjustment for y
                Vector3 cornerWorld = center + cornerOffset;
                Instantiate(prefab, cornerWorld, Quaternion.identity);
            }
        }
    }

}
