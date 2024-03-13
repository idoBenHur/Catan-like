using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IndicatorPlacementTown : MonoBehaviour
{
    public GameObject prefabToPlace;
    public Tilemap tilemap;
    private HashSet<Vector3> placedPositions;
    private bool HasRoadInRadius = false;
    private bool HasTownInRadius = false;

    void Start()
    {
        placedPositions = new HashSet<Vector3>();
      //  PlacePrefabsAtCorners();
    }

   
    public void PlacePrefabsAtCorners()
    {
        // Gets the "smallest" & the "biggest" tile postion int the tilemap gird.  for example min = (-6, -2, 0); max = (7, 4, 0); (x, y, z)
        Vector3Int minTile = tilemap.cellBounds.min;
        Vector3Int maxTile = tilemap.cellBounds.max;


        // those 2 loops essentially goes throgh every tile in the tilemap == it goes throgh every tile between minTile and maxTile. (goes over all Y postions when X=1, and then again when X=2 and so on)
        //"GridPosition" is the postion of the current tile in the loop. (in grid format postion)
        //it then convert tile grid postion to world postion. ( the new world postion is the center of the hex !) 
        // then, its checks if there is a tile in this certin grid postion (theortcly you can create map with "holes" in it, but with the same grid boundries)
        // if it has a tile, it calls the next function and pass to it the currnent tile world's postion

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
        placedPositions.Clear();
    }

    void PlacePrefabsAroundHex(Vector3 HexCenter)
    {
        //Takes the length of a tile's X (so we will be able to change the size of the map/hexes without a problem) 
        // and then caulcalate the hex size . (while taking into account the currnet scale)
        //the "for loop" runs 6 time 1 for each corner of the hex.
        // the first part calculate the angale, and then using the angale, size and center point, it finds the corner.
        
        float tilemapScale = tilemap.transform.localScale.x;
        float size = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);
        
        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i + 30;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 cornerPos = new Vector3(HexCenter.x + size * Mathf.Cos(angle_rad), HexCenter.y + size * Mathf.Sin(angle_rad), HexCenter.z);
            Vector3 roundedCornerPos = RoundVector3(cornerPos, 2); // rounding the postion. 



            if (!placedPositions.Contains(roundedCornerPos))  //If this cornner postion is not already in the list, continue (and add it to the list)
            {
                placedPositions.Add(roundedCornerPos);
                
                Vector2 center2D = new Vector2(roundedCornerPos.x, roundedCornerPos.y);
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center2D, 1f);  //check for coliders in a certin radius.


                HasRoadInRadius = false;
                HasTownInRadius = false;

                foreach (var hitCollider in hitColliders) 
                {
                    if (hitCollider.gameObject.CompareTag("Road"))
                    {

                        HasRoadInRadius = true;
                    }

                    if (hitCollider.gameObject.CompareTag("Town"))
                    {

                        HasTownInRadius = true;
                    }

                }

                if ( HasTownInRadius == false && HasRoadInRadius == true)
                {
                    Instantiate(prefabToPlace, roundedCornerPos, Quaternion.identity);
                    //Debug.Log(roundedCornerPos);
                }
                
            }
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