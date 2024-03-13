using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass
{
    // Enum for resource types available in the game
    public enum ResourceType { Wood, Brick, Wheat, Ore, Sheep, Desert }

    // Public properties
    public ResourceType resourceType { get; set; } // The type of resource this tile produces
    public int numberToken { get; private set; } // The number token associated with this tile for resource distribution
    public bool hasRobber { get; private set; } // Indicates if the robber is on this tile

    // Constructor to initialize the tile with specific properties
    public TileClass(ResourceType resourceType, int numberToken, bool hasRobber = false)
    {
        this.resourceType = resourceType;
        this.numberToken = numberToken;
        this.hasRobber = hasRobber;
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
