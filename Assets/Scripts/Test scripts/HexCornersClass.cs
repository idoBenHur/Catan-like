using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCornersClass
{
    public Vector3 Position { get; set; }
    public bool HasSettlement = false;
    public bool CanBeBuiltOn { get; set; } = true;
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>();
    public List<Vector3> AdjacentCornerPositions { get; set; } = new List<Vector3>();

    // public HashSet<Vector3Int> AdjacentTiles { get; set; } = new HashSet<Vector3Int>();


    public HexCornersClass(Vector3 position, bool Settlement = false)
    {
        Position = position;



    }




}
