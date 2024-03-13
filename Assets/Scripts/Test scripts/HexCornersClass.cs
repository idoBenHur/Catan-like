using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCornersClass
{
    public Vector3 Position { get; set; }
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>();
   // public HashSet<Vector3Int> AdjacentTiles { get; set; } = new HashSet<Vector3Int>();


    public HexCornersClass(Vector3 position)
    {
        Position = position;
    }




}
