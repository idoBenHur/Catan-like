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
    public float topYIncrease ;// Increase Y value for top corners
    public float bottomYDecrease; // Decrease Y value for bottom corners

    private Vector3[] hexCorners; // Stores the local offsets of the hex corners
    private Dictionary<Vector3, SidesClass> InitialSidesDic = new Dictionary<Vector3, SidesClass>();
    private Dictionary<Vector3, CornersClass> InitialCornersDic = new Dictionary<Vector3, CornersClass>();



    void Start()
    {
        foreach (Vector3Int position in hexTilemap.cellBounds.allPositionsWithin)
        {
            if (hexTilemap.HasTile(position))
            {
                Vector3 worldPosition = hexTilemap.CellToWorld(position);
                List<Vector3> cornerPositions = GetHexCornerPositions(worldPosition);
                var sidePosition = GetSidesPositionsForTile(worldPosition, cornerPositions);


                foreach (var corner in cornerPositions)
                {
                    if (!InitialCornersDic.ContainsKey(corner))
                    {
                        InitialCornersDic[corner] = new CornersClass(corner);
                        Instantiate(cornerPrefab, corner, Quaternion.identity);


                    }
                }


                //foreach (var side in sidePosition)
                //{
                //    if (!InitialSidesDic.ContainsKey(side.Position))
                //    {
                //        InitialSidesDic[side.Position] = new SidesClass(side.Position, side.RotationZ);
                //        Instantiate(cornerPrefab, side.Position, Quaternion.identity);
                //    }





                //}
            }




        };





            

        //// Get grid cell size and tilemap scale automatically
        //Vector3 gridCellSize = grid.cellSize; // Cell size from the grid component
        //Vector3 tilemapScale = hexTilemap.transform.localScale; // Scale of the tilemap

        //// Define the relative positions of the corners for a point-down hexagon
        //hexCorners = new Vector3[6];

        //// Adjust based on proportions of a point-down hexagon
        //float width = gridCellSize.x * tilemapScale.x;
        //float height = gridCellSize.y * tilemapScale.y;

        //// Adjust hex corners with custom Y offsets for tall hexagons
        //hexCorners[0] = new Vector3(0, height * 0.5f + topYIncrease, 0); // Top
        //hexCorners[1] = new Vector3(width * 0.5f, height * 0.25f + topYIncrease, 0); // Top-right
        //hexCorners[5] = new Vector3(-width * 0.5f, height * 0.25f + topYIncrease, 0); // Top-left

        //hexCorners[2] = new Vector3(width * 0.5f, -height * 0.25f - bottomYDecrease, 0); // Bottom-right
        //hexCorners[4] = new Vector3(-width * 0.5f, -height * 0.25f - bottomYDecrease, 0); // Bottom-left
        //hexCorners[3] = new Vector3(0, -height * 0.5f - bottomYDecrease, 0); // Bottom

        //// Iterate through each tile position in the tilemap
        //foreach (Vector3Int position in hexTilemap.cellBounds.allPositionsWithin)
        //{
        //    if (hexTilemap.HasTile(position))
        //    {
        //        // Get the world position of the current tile, factoring in the tileAnchor for Y offset
        //        Vector3 worldPosition = hexTilemap.CellToWorld(position); //+ hexTilemap.tileAnchor;

        //        // Spawn prefab at each corner of the hexagon
        //        foreach (Vector3 cornerOffset in hexCorners)
        //        {
        //            Vector3 cornerPosition = worldPosition + cornerOffset;
        //            Instantiate(cornerPrefab, cornerPosition, Quaternion.identity);
        //        }
        //    }
        //}
    }


    public List<Vector3> GetHexCornerPositions(Vector3 tileWorldPosition)
    {
        Vector3 gridCellSize = grid.cellSize; // Cell size from the grid component
        Vector3 tilemapScale = hexTilemap.transform.localScale;

        //float topIncrease = 0.150583f;
        //float bottomIncrease = -0.150583f;


        List<Vector3> hexCorners = new List<Vector3>();
        List<Vector3> RoundedHexCorners = new List<Vector3>();


        // Adjust based on proportions of a point-down hexagon
        float width = gridCellSize.x * tilemapScale.x;
        float height = gridCellSize.y * tilemapScale.y;

        // Calculate hex corners with custom Y offsets for tall hexagons relative to the tile world position
        hexCorners.Add(tileWorldPosition + new Vector3(0, height * 0.5f + topYIncrease, 0)); // Top
        hexCorners.Add(tileWorldPosition + new Vector3(width * 0.5f, height * 0.25f + topYIncrease, 0)); // Top-right
        hexCorners.Add(tileWorldPosition + new Vector3(width * 0.5f, -height * 0.25f + topYIncrease, 0)); // Bottom-right
        hexCorners.Add(tileWorldPosition + new Vector3(0, -height * 0.5f + topYIncrease, 0)); // Bottom
        hexCorners.Add(tileWorldPosition + new Vector3(-width * 0.5f, -height * 0.25f + topYIncrease, 0)); // Bottom-left
        hexCorners.Add(tileWorldPosition + new Vector3(-width * 0.5f, height * 0.25f + topYIncrease, 0)); // Top-left


        foreach (var corner in hexCorners)
        {
            RoundedHexCorners.Add(RoundVector3(corner, 3));
        }

        return RoundedHexCorners;
    }


    public List<SidesClass> GetSidesPositionsForTile(Vector3 HexCenterPosition, List<Vector3> cornersPosition)
    {
        
        List<SidesClass> sides = new List<SidesClass>();

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentCorner = cornersPosition[i];
            Vector3 nextCorner = cornersPosition[(i + 1) % 6]; // Wrap around at the last corner

            // Calculate the midpoint between the current corner and the next
            Vector3 sideMidpoint = (currentCorner + nextCorner) / 2;
            Vector3 roundedSidePos = RoundVector3(sideMidpoint, 3);

            // Calculate rotation: Angle in degrees from the horizontal axis
            float rotationZ = Mathf.Atan2(nextCorner.y - currentCorner.y, nextCorner.x - currentCorner.x) * Mathf.Rad2Deg;

            // Create new SideClass object and add it to the list
            sides.Add(new SidesClass(roundedSidePos, rotationZ));
        }

        return sides;


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
}
