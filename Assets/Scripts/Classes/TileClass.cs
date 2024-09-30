using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileClass
{
    // Enum for resource types available in the game
    public enum ResourceType { Wood, Brick, Sheep, Ore, Wheat, Desert}

    // Public properties
    public ResourceType resourceType { get; set; } // The type of resource this tile produces
    public int numberToken { get;  set; } // The number token associated with this tile for resource distribution
    public bool isBlocked { get; private set; } // blocked tile is a tile effected by the tsunami (can not preduce resolurces)
    public bool underFog;
    public Vector3Int TilePostion { get; private set; }
    public Vector3 TileWorldPostion { get; private set; }
    public List<CornersClass> AdjacentCorners { get; set; } = new List<CornersClass>();
    public List<SidesClass> AdjacentSides { get; set; } = new List<SidesClass>();
    public List<TileClass> AdjacentTiles { get; set; } = new List<TileClass>();

    public GameObject MyNumberPrefab;


    // Constructor to initialize the tile with specific properties
    public TileClass(ResourceType resourceType, int numberToken, Vector3Int tileposition, Vector3 worldpostion, bool isBlocked = false, GameObject numberTokenPrefab = null, bool underFog = true)
    {
        this.resourceType = resourceType;
        this.numberToken = numberToken;
        this.isBlocked = isBlocked;
        TilePostion = tileposition;
        TileWorldPostion = worldpostion;
        MyNumberPrefab = numberTokenPrefab;
        this.underFog = underFog;
    }

    // Methods to place or remove the robber on this tile
    public void PlaceBlockOnTile()
    {
        isBlocked = true;
    }

    public void RemoveBlockOnTile()
    {
        isBlocked = false;
    }

    public void ChangeTileNumber(int newNumber) 
    {
        this.numberToken = newNumber;
        Dictionary<Vector3Int, TileClass> tempdic = new Dictionary<Vector3Int, TileClass>();
        tempdic.Add(TilePostion, this);

        BoardManager.instance.mapGenerator.UpdateTileNumberVisual(tempdic);
    }
}
