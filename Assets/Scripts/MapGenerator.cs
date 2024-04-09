using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile; // Assign these in the inspector
    public GameObject NumberTokenPrefab;
    public GameObject RoadPrefab;
    public GameObject TownPrefab;
    public GameObject CityPrefab;


    public Dictionary<Vector3Int, TileClass> InitialTilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> InitialCornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> InitialSidesDic = new Dictionary<Vector3, SidesClass>();


    private List<TileClass.ResourceType> ResourcesOnTheMapList = new List<TileClass.ResourceType>
    {
        TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep,
        TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood,
        TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat,
        TileClass.ResourceType.Brick, TileClass.ResourceType.Brick, TileClass.ResourceType.Brick,
        TileClass.ResourceType.Ore, TileClass.ResourceType.Ore, TileClass.ResourceType.Ore,
        TileClass.ResourceType.Desert
    };

    private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };



    // Main function to initiate the map generation process (called from board manager)
    public void InitialBuildMap()
    {
        InitialMapResourcesShuffle();
        InitializeTiles();
        UpdateTileTypeVisual(InitialTilesDictionary);
        UpdateTileNumberVisual(InitialTilesDictionary);

        CreateDicsAndAdjacentTiles();
        CreateNeighborsLists();
    }

    public void LoadMapVisuals(Dictionary<Vector3Int, TileClass> TilesDic)
    {
        UpdateTileTypeVisual(TilesDic);
        UpdateTileNumberVisual(TilesDic);
        UpdateTownsAndRoadsVisual();
    }


    // Shuffle the resources and number tokens to ensure random distribution on the map
    void InitialMapResourcesShuffle()
    {
        int n = ResourcesOnTheMapList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            var value = ResourcesOnTheMapList[k];
            ResourcesOnTheMapList[k] = ResourcesOnTheMapList[n];
            ResourcesOnTheMapList[n] = value;
        }
    }


    // Initialize the tiles on the map, assigning resources and number tokens

    void InitializeTiles()
    {

        int ResourceIndex = 0;

        foreach (var TilePosition in tilemap.cellBounds.allPositionsWithin) 
        {
            if (tilemap.HasTile(TilePosition))
            {
                Vector3 worldPosition = tilemap.CellToWorld(TilePosition);
                var resourceType = ResourcesOnTheMapList[ResourceIndex];  

                if (resourceType != TileClass.ResourceType.Desert)
                {
                    int RandomNumberTokenIndex = Random.Range(0, availableNumbers.Count);
                    int numberToken = availableNumbers[RandomNumberTokenIndex];
                    availableNumbers.RemoveAt(RandomNumberTokenIndex);



                    //GameObject prefabInstance = Instantiate(NumberTokenPrefab, worldPosition, Quaternion.identity);
                    //TextMeshPro textMesh = prefabInstance.transform.GetChild(0).GetComponent<TextMeshPro>();
                    //textMesh.text = numberToken.ToString();

                    //if (numberToken == 6 || numberToken == 8)
                    //{
                    //    textMesh.color = Color.red;
                    //    textMesh.fontStyle = FontStyles.Bold;
                    //}


                    
                    var tile = new TileClass(resourceType, numberToken, TilePosition, worldPosition,false); 
                    InitialTilesDictionary.Add(TilePosition, tile);




                }

                else
                {
                    var tile = new TileClass(resourceType, 0, TilePosition, worldPosition);
                    InitialTilesDictionary.Add(TilePosition, tile);
                }


                ResourceIndex++;

            }


        }
      
    }

    // Update the visual representation of the map in the tilemap component

    public void UpdateTileTypeVisual(Dictionary<Vector3Int, TileClass> TilesDic)
    {
        foreach (var tilePair in TilesDic)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;


            switch (tile.resourceType)
            {
                case TileClass.ResourceType.Wood:
                    tilemap.SetTile(position, woodTile);
                    break;
                case TileClass.ResourceType.Brick:
                    tilemap.SetTile(position, brickTile);
                    break;
                case TileClass.ResourceType.Wheat:
                    tilemap.SetTile(position, wheatTile);
                    break;
                case TileClass.ResourceType.Ore:
                    tilemap.SetTile(position, oreTile);
                    break;
                case TileClass.ResourceType.Sheep:
                    tilemap.SetTile(position, sheepTile);
                    break;
                case TileClass.ResourceType.Desert:
                    tilemap.SetTile(position, desertTile);
                    break;
                default:
                    break; 
            }
        }
    }



    public void UpdateTileNumberVisual(Dictionary<Vector3Int, TileClass> TilesDic)
    {
        

        foreach (var Tile in TilesDic)
        {


            if (Tile.Value.resourceType != TileClass.ResourceType.Desert)
            {

                GameObject prefabInstance = Instantiate(NumberTokenPrefab, Tile.Value.TileWorldPostion, Quaternion.identity);
                TextMeshPro textMesh = prefabInstance.transform.GetChild(0).GetComponent<TextMeshPro>();
                int TileNumber = Tile.Value.numberToken;
                textMesh.text = TileNumber.ToString();

                if (TileNumber == 6 || TileNumber == 8)
                {
                    textMesh.color = Color.red;
                    textMesh.fontStyle = FontStyles.Bold;
                }

                Tile.Value.MyNumberPrefab = prefabInstance;


            }
            
        }

    }

    public void UpdateTownsAndRoadsVisual()
    {
        var player = BoardManager.instance.player;
        foreach (var town in player.SettelmentsList)
        {
            if(town.HasCityUpgade == true)
            {
                Instantiate(CityPrefab, town.Position, Quaternion.identity);
            }
            else
            {
                Instantiate(TownPrefab, town.Position, Quaternion.identity);

            }
        }

        foreach(var road in player.RoadsList)
        {
            Quaternion SideRotation = Quaternion.Euler(0, 0, road.RotationZ);
            Instantiate(RoadPrefab, road.Position, SideRotation);

        }
    }


    // Create dictionaries for corners and sides, and determine their adjacent tiles
    private void CreateDicsAndAdjacentTiles()
    {
        foreach (var tilePair in InitialTilesDictionary)
        {
            Vector3Int TilePosition = tilePair.Key;
            TileClass TileValue = tilePair.Value;
            Vector3 TileWorldPosition = tilemap.CellToWorld(TilePosition);


            var CornerPositions = GetCornerPositionsForTile(TileWorldPosition);
            var sidePositions = GetSidesPositionsForTile(TileWorldPosition);



            foreach (var CornerPos in CornerPositions)
            {
                if (!InitialCornersDic.ContainsKey(CornerPos))
                {
                    InitialCornersDic[CornerPos] = new CornersClass(CornerPos);
                }

                InitialCornersDic[CornerPos].AdjacentTiles.Add(TileValue);
                TileValue.AdjacentCorners.Add(InitialCornersDic[CornerPos]);
            }


            foreach (var SidePos in sidePositions)
            {
                if (!InitialSidesDic.ContainsKey(SidePos.Position))
                {
                    InitialSidesDic[SidePos.Position] = new SidesClass(SidePos.Position, SidePos.RotationZ);

                }

                InitialSidesDic[SidePos.Position].AdjacentTiles.Add(TileValue);
                TileValue.AdjacentSides.Add(SidePos);

            }



        }


    }

    // Calculate and store the positions and rotations for each side of the hex tiles
    public List<SidesClass> GetSidesPositionsForTile(Vector3 HexCenterPosition)
    {
        List<SidesClass> sides = new List<SidesClass>();
        float tilemapScale = tilemap.transform.localScale.x;
        float outerRadius = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);
        float apothem = outerRadius * Mathf.Cos(Mathf.PI / 6); 

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 sideMidPointPos = new Vector3(
                HexCenterPosition.x + apothem * Mathf.Cos(angle_rad),
                HexCenterPosition.y + apothem * Mathf.Sin(angle_rad),
                HexCenterPosition.z
            );
            float rotationZ = angle_deg + 90f; 

            sides.Add(new SidesClass(RoundVector3(sideMidPointPos, 2), rotationZ));



        }

        return sides;
    }


    // Calculate and store the positions for each corner of the hex tiles
    public List<Vector3> GetCornerPositionsForTile(Vector3 HexCenterPostion)
    {
        List<Vector3> corners = new List<Vector3>();
        float tilemapScale = tilemap.transform.localScale.x;
        float size = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i + 30; 
            //float angleRad = Mathf.Deg2Rad * angleDeg;
            float angleRad = Mathf.PI / 180 * angleDeg;
            Vector3 cornerPos = new Vector3(HexCenterPostion.x + size * Mathf.Cos(angleRad), HexCenterPostion.y + size * Mathf.Sin(angleRad), HexCenterPostion.z);
            Vector3 roundedCornerPos = RoundVector3(cornerPos, 1);
            corners.Add(roundedCornerPos);



        }


        return corners;
    }



    // Helper function to round vector components to a specified number of decimal places
    Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10.0f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }



    // Determine adjacency relationships between corners and sides, and between themselves
    private void CreateNeighborsLists()
    {

        float adjacencyThreshold = 1f;
        foreach (var cornerEntry in InitialCornersDic)
        {
            CornersClass corner = cornerEntry.Value;

            foreach (var sideEntry in InitialSidesDic)
            {
                SidesClass side = sideEntry.Value;

                if (Vector3.Distance(side.Position, corner.Position) <= adjacencyThreshold)
                {
                    corner.AdjacentSides.Add(side);
                    side.AdjacentCorners.Add(corner);

                }
            }
        }


        //corner

        foreach (var currentCorner in InitialCornersDic.Values)
        {
            foreach (var possibleAdjacentCorner in InitialCornersDic.Values)
            {
                if (currentCorner == possibleAdjacentCorner) continue;

                var sharedSide = currentCorner.AdjacentSides.Intersect(possibleAdjacentCorner.AdjacentSides).FirstOrDefault();
                bool areAdjacent = sharedSide != null;

                if (areAdjacent)
                {
                    if (!currentCorner.AdjacentCorners.Contains(possibleAdjacentCorner))
                    {
                        currentCorner.AdjacentCorners.Add(possibleAdjacentCorner);
                    }

                    if (!possibleAdjacentCorner.AdjacentCorners.Contains(currentCorner))
                    {
                        possibleAdjacentCorner.AdjacentCorners.Add(currentCorner);
                    }
                }
            }
        }


        //corner

        //side
        foreach (var currentSide in InitialSidesDic.Values)
        {
            foreach (var possibleAdjacentSide in InitialSidesDic.Values)
            {
                if (currentSide == possibleAdjacentSide) continue;

                var sharedCorner = currentSide.AdjacentCorners.Intersect(possibleAdjacentSide.AdjacentCorners).FirstOrDefault();
                bool areAdjacent = sharedCorner != null;

                if (areAdjacent)
                {
                    if (!currentSide.AdjacentSides.Contains(possibleAdjacentSide))
                    {
                        currentSide.AdjacentSides.Add(possibleAdjacentSide);
                    }

                    if (!possibleAdjacentSide.AdjacentSides.Contains(currentSide))
                    {
                        possibleAdjacentSide.AdjacentSides.Add(currentSide);
                    }
                }
            }
        }

        //side

    }
}
