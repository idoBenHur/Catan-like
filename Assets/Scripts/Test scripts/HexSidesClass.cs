using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexSidesClass
{
    public bool CanBeBuiltOn { get; set; } = true;
    public bool HasRoad { get; set; } = false;
    public List<HexSidesClass> AdjacentSides { get; set; } = new List<HexSidesClass>(); // the list of neigbords roads/side
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>(); // tiles next to the road/side
    public float RotationZ { get; private set; } // Rotation in float should ne transform to Quaternion

    public Vector3 Position { get; private set; }

    public HexSidesClass(Vector3 position, float rotationZ)
    {
        Position = position;
        RotationZ = rotationZ;
    }
}
    


