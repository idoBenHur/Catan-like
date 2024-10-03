using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TEST2 : MonoBehaviour
{
    public Tilemap hexTilemap; // Reference to your tilemap
    public Grid grid; // The grid component that holds the tilemap
    public GameObject cornerPrefab; // The prefab to spawn at each corner

    // You can adjust these offset values in the Inspector for fine-tuning.
    public float topYIncrease = 0.1505F;//0.1f; // Increase Y value for top corners
    public float bottomYDecrease = -10F;//0.1f; // Decrease Y value for bottom corners

    private Vector3[] hexCorners; // Stores the local offsets of the hex corners

    void Start()
    {
        // Get grid cell size and tilemap scale automatically
        Vector3 gridCellSize = grid.cellSize; // Cell size from the grid component
        Vector3 tilemapScale = hexTilemap.transform.localScale; // Scale of the tilemap

        // Define the relative positions of the corners for a point-down hexagon
        hexCorners = new Vector3[6];

        // Adjust based on proportions of a point-down hexagon
        float width = gridCellSize.x * tilemapScale.x;
        float height = gridCellSize.y * tilemapScale.y;

        // Adjust hex corners with custom Y offsets for tall hexagons
        hexCorners[0] = new Vector3(0, height * 0.5f + topYIncrease, 0); // Top
        hexCorners[1] = new Vector3(width * 0.5f, height * 0.25f + topYIncrease, 0); // Top-right
        hexCorners[5] = new Vector3(-width * 0.5f, height * 0.25f + topYIncrease, 0); // Top-left

        hexCorners[2] = new Vector3(width * 0.5f, -height * 0.25f - bottomYDecrease, 0); // Bottom-right
        hexCorners[4] = new Vector3(-width * 0.5f, -height * 0.25f - bottomYDecrease, 0); // Bottom-left
        hexCorners[3] = new Vector3(0, -height * 0.5f - bottomYDecrease, 0); // Bottom

        // Iterate through each tile position in the tilemap
        foreach (Vector3Int position in hexTilemap.cellBounds.allPositionsWithin)
        {
            if (hexTilemap.HasTile(position))
            {
                // Get the world position of the current tile, factoring in the tileAnchor for Y offset
                Vector3 worldPosition = hexTilemap.CellToWorld(position) + hexTilemap.tileAnchor;

                // Spawn prefab at each corner of the hexagon
                foreach (Vector3 cornerOffset in hexCorners)
                {
                    Vector3 cornerPosition = worldPosition + cornerOffset;
                    Instantiate(cornerPrefab, cornerPosition, Quaternion.identity);
                }
            }
        }
    }
}
