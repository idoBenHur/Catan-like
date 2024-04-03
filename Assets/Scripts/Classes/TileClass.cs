using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass
{
    // Enum for resource types available in the game
    public enum ResourceType { Wood, Brick, Sheep, Ore, Wheat, Desert}

    // Public properties
    public ResourceType resourceType { get; set; } // The type of resource this tile produces
    public int numberToken { get; private set; } // The number token associated with this tile for resource distribution
    public bool hasRobber { get; private set; } // Indicates if the robber is on this tile
    public Vector3Int TilePostion { get; private set; }
    public Vector3 TileWorldPostion { get; private set; }
    public List<CornersClass> AdjacentCorners { get; set; } = new List<CornersClass>();
    public List<SidesClass> AdjacentSides { get; set; } = new List<SidesClass>();
    public GameObject MyNumberPrefab;


    // Constructor to initialize the tile with specific properties
    public TileClass(ResourceType resourceType, int numberToken, Vector3Int tileposition, Vector3 worldpostion, bool hasRobber = false, GameObject numberTokenPrefab = null)
    {
        this.resourceType = resourceType;
        this.numberToken = numberToken;
        this.hasRobber = hasRobber;
        TilePostion = tileposition;
        TileWorldPostion = worldpostion;
        MyNumberPrefab = numberTokenPrefab;
    }

    // Methods to place or remove the robber on this tile
    public void PlaceRobber()
    {
        hasRobber = true;
    }

    public void RemoveRobber()
    {
        hasRobber = false;
    }
}
