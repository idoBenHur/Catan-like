using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornersClass
{
    public Vector3 Position { get; set; }
    public bool HasSettlement = false;
    public bool HasCityUpgade = false;
    public bool CanBeBuiltOn { get; set; } = true;
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>();
    public List<CornersClass> AdjacentCorners { get; set; } = new List<CornersClass>();
    public List<SidesClass> AdjacentSides { get; set; } = new List<SidesClass>();


    // public HashSet<Vector3Int> AdjacentTiles { get; set; } = new HashSet<Vector3Int>();


    public CornersClass(Vector3 position, bool Settlement = false)
    {
        Position = position;



    }




}
