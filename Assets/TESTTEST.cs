using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TESTTEST : MonoBehaviour
{
    public Grid grid;  // Reference to the Grid component
    public Tilemap tilemap;  // Reference to the Tilemap component
    public GameObject prefab;  // The object to instantiate at each corner

    // Define the vertical offset between the large and small hexagons
    public float verticalOffset = 0.15f; // 0.15014577259f;  // Adjust based on your design

    // Define the small hexagon's dimensions in Unity units (based on its pixel size and PPU)
    private float hexWidth = 1.0f;  // Small hexagon width in Unity units
    private float hexHeight = 0.86f;//0.86005830903f;//  // Small hexagon height in Unity units

    void Start()
    {
        // Get the scale of the Tilemap
        Vector3 tilemapScale = tilemap.transform.localScale;

        // Loop through all the positions (cells) in the tilemap
        foreach (Vector3Int hexagonPosition in tilemap.cellBounds.allPositionsWithin)
        {
            // Check if there is actually a tile at this position
            if (tilemap.HasTile(hexagonPosition))
            {
                // Get the world position of the large hexagon's center point
                Vector3 worldPosition = grid.CellToWorld(hexagonPosition);

                // Apply tilemap scale adjustment to the world position (center of the hexagon)
                Vector3 scaledWorldPosition = new Vector3(
                    worldPosition.x * tilemapScale.x,
                    worldPosition.y * tilemapScale.y,
                    worldPosition.z * tilemapScale.z
                );

                // Apply the vertical offset for the smaller hexagon
                scaledWorldPosition.y += verticalOffset;

                // Get the corners for this hexagon
                List<Vector3> corners = GetCorrectedCornerPositions(scaledWorldPosition, tilemapScale);

                // Instantiate the prefab at each corner
                foreach (Vector3 corner in corners)
                {
                    Instantiate(prefab, corner, Quaternion.identity);
                    Debug.Log("Corner position: " + corner);
                }
            }
        }
    }

    // Function to calculate the 6 corner positions for a hexagon, focusing on the top corner first
    public List<Vector3> GetCorrectedCornerPositions(Vector3 hexCenterPosition, Vector3 tilemapScale)
    {
        List<Vector3> corners = new List<Vector3>();

        // Calculate half of the small hexagon's width and height, accounting for the tilemap's scale
        float halfWidth = (hexWidth / 2f) * tilemapScale.x;
        float halfHeight = (hexHeight / 2f) * tilemapScale.y;

        // Top corner (the correct one)
        Vector3 topCorner = RoundVector3(new Vector3(hexCenterPosition.x, hexCenterPosition.y + halfHeight, hexCenterPosition.z), 1);
        corners.Add(topCorner);  // Add the top corner to the list

        // Now calculate the other corners relative to the top corner

        // Top-right corner
        Vector3 topRightCorner = RoundVector3(new Vector3(hexCenterPosition.x + halfWidth, hexCenterPosition.y + halfHeight / 2f, hexCenterPosition.z), 1);
        corners.Add(topRightCorner);

        // Bottom-right corner
        Vector3 bottomRightCorner = RoundVector3(new Vector3(hexCenterPosition.x + halfWidth, hexCenterPosition.y - halfHeight / 2f, hexCenterPosition.z), 1);
        corners.Add(bottomRightCorner);

        // Bottom corner
        Vector3 bottomCorner = RoundVector3(new Vector3(hexCenterPosition.x, hexCenterPosition.y - halfHeight, hexCenterPosition.z), 1);
        corners.Add(bottomCorner);

        // Bottom-left corner
        Vector3 bottomLeftCorner = RoundVector3(new Vector3(hexCenterPosition.x - halfWidth, hexCenterPosition.y - halfHeight / 2f, hexCenterPosition.z), 1);
        corners.Add(bottomLeftCorner);

        // Top-left corner
        Vector3 topLeftCorner = RoundVector3(new Vector3(hexCenterPosition.x - halfWidth, hexCenterPosition.y + halfHeight / 2f, hexCenterPosition.z), 1);
        corners.Add(topLeftCorner);

        return corners;
    }

    // Helper function to round Vector3 values
    Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10.0f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
}
