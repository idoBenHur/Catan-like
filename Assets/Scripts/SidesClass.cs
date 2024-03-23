using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SidesClass
{
    public bool CanBeBuiltOn { get; set; } = false;
    public bool HasRoad { get; set; } = false;
    public List<SidesClass> AdjacentSides { get; set; } = new List<SidesClass>(); // the list of neigbords roads/side
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>(); // tiles next to the road/side
    public List<CornersClass> AdjacentCorners { get; set; } = new List<CornersClass>();

    public float RotationZ { get; private set; } // Rotation in float should ne transform to Quaternion

    public Vector3 Position { get; private set; }

    public SidesClass(Vector3 position, float rotationZ)
    {
        Position = position;
        RotationZ = rotationZ;
    }
}
    


