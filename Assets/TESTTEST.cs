using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TESTTEST : MonoBehaviour
{
    public Grid grid;  // Reference to the Grid component
    public Tilemap tilemap;  // Reference to the Tilemap component
    public GameObject prefab;  // The object to instantiate at each corner

    // Define the vertical offset between the large and small hexagons
    public float verticalOffset = 0.15014577259f;    //0.1501f;  // Calculated previously

    // Define the small hexagon's dimensions in Unity units (based on its pixel size and PPU)
    private float hexWidth = 1.0f;  // Hexagon width in Unity units
    private float hexHeight = 0.86005830903f;    //0.86f;  // Hexagon height in Unity units

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

                // Apply tilemap scale adjustment to the world position
                Vector3 scaledWorldPosition = new Vector3(
                    worldPosition.x * tilemapScale.x,
                    worldPosition.y * tilemapScale.y,
                    worldPosition.z * tilemapScale.z
                );

                // Apply the vertical offset for the smaller hexagon
                scaledWorldPosition.y += verticalOffset;

                // Get the corners for this hexagon
                List<Vector3> corners = GetCornerPositionsForTile(scaledWorldPosition);

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
    public List<Vector3> GetCornerPositionsForTile(Vector3 hexCenterPosition)
    {
        Vector3 tilemapScale = tilemap.transform.localScale; // Get the scale of the tilemap

        // Corrected half size based on the actual dimensions of the smaller hexagon
        float halfWidth = (hexWidth / 2) * tilemapScale.x;
        float halfHeight = (hexHeight / 2) * tilemapScale.y;

        List<Vector3> corners = new List<Vector3>();

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30; // Offset by -30 degrees for pointy top
            float angle_rad = Mathf.Deg2Rad * angle_deg;

            // Use the halfWidth for x direction and halfHeight for y direction
            Vector3 cornerOffset = new Vector3(halfWidth * Mathf.Cos(angle_rad), halfHeight * Mathf.Sin(angle_rad), 0);

            // Calculate the world position for the corner
            Vector3 cornerWorld = hexCenterPosition + cornerOffset;

            // Round the position to avoid floating-point errors
            Vector3 roundedCornerPos = RoundVector3(cornerWorld, 1);  // Rounding to 3 decimal places
            corners.Add(roundedCornerPos);
        }

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
