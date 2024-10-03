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
    private float hexWidth = 686f / 693f;  // Exact small hexagon width in Unity units (1 based on 686 PPU)
    private float hexHeight = 693f / 693f;  // Exact small hexagon height in Unity units (590 pixels)

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
                Vector3 worldPosition = tilemap.CellToWorld(hexagonPosition);

                // Apply tilemap scale adjustment to the world position (center of the hexagon)
                Vector3 scaledWorldPosition = new Vector3(
                    worldPosition.x * tilemapScale.x,
                    worldPosition.y * tilemapScale.y,
                    worldPosition.z * tilemapScale.z
                );

                // Apply the vertical offset for the smaller hexagon
                scaledWorldPosition.y += verticalOffset;



                // Get the corners for this hexagon
                List<Vector3> corners = GetCorrectedCornerPositions3(worldPosition, tilemapScale);
               // List<Vector3> corners = idotry(worldPosition);

                

                // Instantiate the prefab at each corner
                foreach (Vector3 corner in corners)
                {
                    Instantiate(prefab, corner, Quaternion.identity);
                    Debug.Log("Corner position: " + corner);
                }
            }
        }
    }



    private List<Vector3> idotry(Vector3 center)
    {
        Vector3 tilemapScale = tilemap.transform.localScale;
        float size = Mathf.Max(tilemap.cellSize.x, tilemap.cellSize.y) / 2;
        List<Vector3> corners = new List<Vector3>();

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30; // Offset by -30 degrees for pointy top
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            Vector3 cornerOffset = new Vector3(size * Mathf.Cos(angle_rad), size * Mathf.Sin(angle_rad), 0);
            cornerOffset.x *= tilemapScale.x; // Scale adjustment for x
            cornerOffset.y *= tilemapScale.y; // Scale adjustment for y
            Vector3 cornerWorld = center + cornerOffset;
            Vector3 roundedCornerPos = RoundVector3(cornerWorld, 1);
            corners.Add(roundedCornerPos);

        }


        return corners;
    }

















    List<Vector3> GetCorrectedCornerPositions(Vector3 center, Vector3 tilemapScale)
    {
        List<Vector3> corners = new List<Vector3>();

        // Hexagon dimensions in Unity units
        float hexWidth = 1f; // 686 pixels per unit, so width is 1 unit
        float hexHeight = 590f / 686f; // 590 pixels converted to Unity units

        // Adjust the dimensions by tilemap scale
        float adjustedHexWidth = hexWidth * tilemapScale.x;
        float adjustedHexHeight = hexHeight * tilemapScale.y;

        // Calculate the radius (distance from center to corners)
        float radiusX = adjustedHexWidth / 2f;
        float radiusY = adjustedHexHeight / 2f;

        // Angles for the 6 corners of the hexagon (starting at -30 degrees, 60-degree increments)
        float[] angles = { -Mathf.PI / 6, Mathf.PI / 6, Mathf.PI / 2, 5 * Mathf.PI / 6, 7 * Mathf.PI / 6, 3 * Mathf.PI / 2 };

        // Calculate corner positions using the center point and angles
        foreach (float angle in angles)
        {
            float x = center.x + radiusX * Mathf.Cos(angle);
            float y = center.y + radiusY * Mathf.Sin(angle);
            corners.Add(new Vector3(x, y, center.z));
        }

        return corners;
    }



    public List<Vector3> GetCorrectedCornerPositions3(Vector3 hexCenterPosition, Vector3 tilemapScale)
    {
        // Calculate small hexagon's effective cell size based on its actual width and height
        float sizeX = (hexWidth / 2) * tilemapScale.x;  // Apply tilemap scale to the sizeX
        float sizeY = (hexHeight / 2) * tilemapScale.y; // Apply tilemap scale to the sizeY

        List<Vector3> corners = new List<Vector3>();

        // Bottom three corners will have a specific upwards offset of 0.1500583f
        float bottomCornerOffsetY = 0.1500583f * tilemapScale.y; // Correct offset for the small hexagon's bottom corners

        // Loop through all 6 corners of the hexagon
        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30;
            float angle_rad = Mathf.Deg2Rad * angle_deg;

            // Calculate the offsets for X and Y
            Vector3 cornerOffset = new Vector3(sizeX * Mathf.Cos(angle_rad), sizeY * Mathf.Sin(angle_rad), 0);



            // Apply the bottom corner offset to the lower three corners
            if (i == 3 || i == 4 || i == 5)  // Bottom corners
            {
              //cornerOffset.y += bottomCornerOffsetY;
            }

            // Calculate the world position of each corner by adding the offset to the center position
            Vector3 cornerWorldPosition = hexCenterPosition + cornerOffset;

            // Optionally round the corner positions for more precision
            Vector3 roundedCornerPos = RoundVector3(cornerWorldPosition, 1);
            corners.Add(roundedCornerPos);
        }

        return corners;
    }


    Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10.0f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }


















    // Function to calculate the 6 corner positions for a hexagon
    public List<Vector3> GetCorrectedCornerPositions2(Vector3 hexCenterPosition, Vector3 tilemapScale)
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
