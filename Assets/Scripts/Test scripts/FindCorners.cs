using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class FindCorners : MonoBehaviour
{
    public Tilemap tilemap;
    Dictionary<Vector3, HexCornersClass> corners = new Dictionary<Vector3, HexCornersClass>();
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>();






    void Start()
    {

    }


    public void FindCenterOfHexes()
    {

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
                    FindCornersOfHex(WorldPostion,GridPosition);
                }
            }
        }
    }




    void FindCornersOfHex(Vector3 HexCenter, Vector3Int TileGridPostion)
    {

        
        float tilemapScale = tilemap.transform.localScale.x;
        float size = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i + 30;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 cornerPos = new Vector3(HexCenter.x + size * Mathf.Cos(angle_rad), HexCenter.y + size * Mathf.Sin(angle_rad), HexCenter.z);

            Vector3 roundedCornerPos = RoundVector3(cornerPos, 2); // rounding the postion. 


            if (!corners.ContainsKey(roundedCornerPos))  //If this cornner postion is not already in the list, continue (and add it to the list)
            {
                corners[roundedCornerPos] = new HexCornersClass(roundedCornerPos);


            }
            corners[roundedCornerPos].AdjacentTiles.Add(TileGridPostion);


        }


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
