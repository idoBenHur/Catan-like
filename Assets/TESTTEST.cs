using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TESTTEST : MonoBehaviour
{
    public Grid grid;  // Reference to the Grid component
    public Tilemap tilemap;  // Reference to the Tilemap component
    public GameObject prefab;  // The object to instantiate at each corner

    // Use precise vertical offset value
    public float verticalOffset = 0.1500583f;  // More precise value for vertical offset

    // Define the small hexagon's dimensions in Unity units (based on its pixel size and PPU)
    private float hexWidth = 1.0f;  // Exact small hexagon width in Unity units (1 based on 686 PPU)
    private float hexHeight = 0.8600583f;  // Exact small hexagon height in Unity units (590 pixels)

    // Snap to grid increment
    public float snapIncrement = 0.00001F;  // Snap to the nearest 0.001 units

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

    // Function to calculate the 6 corner positions for a hexagon
    public List<Vector3> GetCorrectedCornerPositions(Vector3 hexCenterPosition, Vector3 tilemapScale)
    {
        List<Vector3> corners = new List<Vector3>();

        // Calculate half of the small hexagon's width and height, accounting for the tilemap's scale
        float halfWidth = (hexWidth / 2f) * tilemapScale.x;
        float halfHeight = (hexHeight / 2f) * tilemapScale.y;

        // Top corner (the correct one)
        Vector3 topCorner = SnapVector3(new Vector3(hexCenterPosition.x, hexCenterPosition.y + halfHeight, hexCenterPosition.z), snapIncrement);
        corners.Add(topCorner);  // Add the top corner to the list

        // Now calculate the other corners relative to the top corner

        // Top-right corner
        Vector3 topRightCorner = SnapVector3(new Vector3(hexCenterPosition.x + halfWidth, hexCenterPosition.y + halfHeight / 2f, hexCenterPosition.z), snapIncrement);
        corners.Add(topRightCorner);

        // Bottom-right corner
        Vector3 bottomRightCorner = SnapVector3(new Vector3(hexCenterPosition.x + halfWidth, hexCenterPosition.y - halfHeight / 2f, hexCenterPosition.z), snapIncrement);
        corners.Add(bottomRightCorner);

        // Bottom corner
        Vector3 bottomCorner = SnapVector3(new Vector3(hexCenterPosition.x, hexCenterPosition.y - halfHeight, hexCenterPosition.z), snapIncrement);
        corners.Add(bottomCorner);

        // Bottom-left corner
        Vector3 bottomLeftCorner = SnapVector3(new Vector3(hexCenterPosition.x - halfWidth, hexCenterPosition.y - halfHeight / 2f, hexCenterPosition.z), snapIncrement);
        corners.Add(bottomLeftCorner);

        // Top-left corner
        Vector3 topLeftCorner = SnapVector3(new Vector3(hexCenterPosition.x - halfWidth, hexCenterPosition.y + halfHeight / 2f, hexCenterPosition.z), snapIncrement);
        corners.Add(topLeftCorner);

        return corners;
    }

    // Snap each point to a grid based on the snap increment
    Vector3 SnapVector3(Vector3 vector, float increment)
    {
        return new Vector3(
            Mathf.Round(vector.x / increment) * increment,
            Mathf.Round(vector.y / increment) * increment,
            Mathf.Round(vector.z / increment) * increment  // Z is likely 0, but just in case
        );
    }
}
