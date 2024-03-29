using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GetCornersData : MonoBehaviour
{
    public Tilemap tilemap;
    Dictionary<Vector3, CornersClass> CornersDic = new Dictionary<Vector3, CornersClass>();
    private BoardManager boardManager;





    void Start()
    {

    }

    // 2 main things:
    // 1. fiils corner dic with (vector3 : hex Corner class) without dupes
    // 2. for each hex corner class update the AdjacentTiles.
    public void UpdateCornerData()
    {

        boardManager = FindAnyObjectByType<BoardManager>(); // get the tiles dic
        var TilesDic = boardManager.TilesDictionary;


        foreach (var tilePair in TilesDic)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;

           var CornerPositions = GetCornerPositionsForTile(tile, position);

            foreach (var cornerPos in CornerPositions) 
            {
                if (!CornersDic.ContainsKey(cornerPos))
                {
                    CornersDic[cornerPos] = new CornersClass(cornerPos);
                }

                CornersDic[cornerPos].AdjacentTiles.Add(tile);
            }

        }
    }



    public List<Vector3> GetCornerPositionsForTile(TileClass tile, Vector3 HexCenterPostion)
    {
        List<Vector3> corners = new List<Vector3>();
        float radius = 1.0f; // Radius of your hex tile, adjust based on your game's scale

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i + 30; // Offset by 30 degrees for pointy-topped hexes
            float angleRad = Mathf.Deg2Rad * angleDeg;
            Vector3 cornerPos = new Vector3(HexCenterPostion.x + radius * Mathf.Cos(angleRad), HexCenterPostion.y + radius * Mathf.Sin(angleRad), HexCenterPostion.z);
            corners.Add(cornerPos);
        }

        return corners;
    }




}
