using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using System.Linq;
//using Unity.Mathematics;
using Unity.VisualScripting;
using static UnityEditor.Progress;
using static UnityEngine.UI.GridLayoutGroup;
using static TileClass;

public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap; // Assign this in the inspector
    public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile; // Assign these in the inspector
    public UiManager uiManager;

    private bool FirstTurnIsActive = true;
    private int FirstTurnPlacedPeices = 0;
    private PlayerClass player;
    public GameObject NumberTokenPrefab;
    public GameObject CornerIndicatorPrefab;
    public GameObject SideIndicatorPrefab;
    public GameObject testprefab;
    public RobberPrefab robberPrefab;
    public GameObject RoadPrefab;
    public GameObject TownPrefab;

    public List<GameObject> CornersIndicatorsPrefabList = new List<GameObject>();
    public List<GameObject> SidesIndicatorsPrefabList = new List<GameObject>();


    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> CornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> SidesDic = new Dictionary<Vector3, SidesClass>();




    private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };

    private List<TileClass.ResourceType> ResourcesOnTheMapList = new List<TileClass.ResourceType>
    {
        // Initialize the list with 4 of each common resource and 3 of the rarer resources, plus 1 desert
        TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep,
        TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood,
        TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat,
        TileClass.ResourceType.Brick, TileClass.ResourceType.Brick, TileClass.ResourceType.Brick,
        TileClass.ResourceType.Ore, TileClass.ResourceType.Ore, TileClass.ResourceType.Ore,
        TileClass.ResourceType.Desert
    };

    public static BoardManager instance;


    private void Awake()
    {
        player = new PlayerClass();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }



    void Start()
    {
        uiManager.SetPlayerInUIManager(player);

        IntialMapResourcesShuffle();
        InitializeTiles();
        UpdateVisualRepresentation();

        // UpdateCornerData();
        //UpdateHexSidesData();

        CreateDicsAndAdjacentTiles();
        CreateNeighborsLists();

        FirstTurnPlacement();





        var elementAtIndex = SidesDic.ElementAt(39);
        var value = elementAtIndex.Value;
        var key = elementAtIndex.Key;
 










    }


    public void DiceRoll()
    {
        int die1 = UnityEngine.Random.Range(1, 7); // Generates a number between 1 and 6
        int die2 = UnityEngine.Random.Range(1, 7); // Same here
        int total = die1 + die2;

        // GetResourcesForDiceRoll(total);
        Debug.Log(total);
        DistributeResources(total);

        uiManager.UpdateDiceRollDisplay(total);
    }




    private void DistributeResources(int DiceResult)
    {

        if (DiceResult == 7)
        {
            ChooseRobberTile();
        }

        else
        {
            foreach (var tile in TilesDictionary.Values)
            {
                if (tile.numberToken == DiceResult)
                {
                    foreach(var Corner in tile.AdjacentCorners)
                    {
                        if(Corner.HasSettlement == true && tile.hasRobber == false)
                        {

                            player.AddResource(tile.resourceType, 1);

                        }
                        else if(Corner.HasSettlement == true && tile.hasRobber == true)
                        {
                            Debug.Log("tile has a robber");
                        }
                    }
                    

                }
            
            }
        }

    }

    private void ChooseRobberTile()
    {
        List<TileClass> OptionlTileForRobber = new List<TileClass>();

        foreach(var corner in player.SettelmentsList)
        {
            foreach( var tile in corner.AdjacentTiles)
            {
                if (!OptionlTileForRobber.Contains(tile) && tile.hasRobber ==false)
                {
                    OptionlTileForRobber.Add(tile);
                }
            }
        }


        if(OptionlTileForRobber.Count > 0)
        {
            int randomindex = Random.Range(0, OptionlTileForRobber.Count);
            OptionlTileForRobber[randomindex].PlaceRobber();


            RobberPrefab robber = Instantiate(robberPrefab, OptionlTileForRobber[randomindex].TileWorldPostion + (Vector3.right * 0.5f), Quaternion.identity);
            robber.currentTile = OptionlTileForRobber[randomindex];

            Debug.Log("robber placed on " + OptionlTileForRobber[randomindex].resourceType + " " + OptionlTileForRobber[randomindex].numberToken);
        }



    }





    public void RemoveRobber(RobberPrefab robber, ResourceType resourceType)
    {
        Dictionary<ResourceType, int> amountToReduce = new Dictionary<ResourceType, int> // create temp dic with relvent costs to use to exsiting subtruct resources function 
        {
            { resourceType, 4 }
        };

        player.SubtractResources(amountToReduce);
        robber.currentTile.RemoveRobber(); // Update the tile status
        Destroy(robber.gameObject); // Remove the robber prefab from the scene
    }





    private void FirstTurnPlacement() 
    {
        

       // Debug.Log("amount of Placed Peices in the first turn " + FirstTurnPlacedPeices);
        



        // Build a town
        if (FirstTurnPlacedPeices == 0 || FirstTurnPlacedPeices == 2)
        {
            FirstTurnPlacedPeices++;
            foreach (var corner in CornersDic.Values)
            {

                if (corner.CanBeBuiltOn == true)
                {

                    GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                    CornersIndicatorsPrefabList.Add(indicator);
                    indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner.Position);
                }
                
            }
            return;
        }
       

        // Build a road
        if(FirstTurnPlacedPeices == 1 || FirstTurnPlacedPeices == 3)
        {
            FirstTurnPlacedPeices++;

            foreach (var corner in CornersDic.Values)
            {
                bool HasRoadAlready = false;

                if (corner.HasSettlement == true) 
                {
                    foreach (var SideNearTown in corner.AdjacentSides) // checkes if the town already have roads (meaning it was the player first town)
                    {
                        if(SideNearTown.HasRoad == true) 
                        {
                            HasRoadAlready = true;
                        }
                    }

                    if(HasRoadAlready == false) // make sure that on the second time its displys only the second town's roads
                    {
                        foreach (var SideNearTown in corner.AdjacentSides)
                        {
                            SideNearTown.CanBeBuiltOn = true;
                            Quaternion rotation = Quaternion.Euler(0, 0, SideNearTown.RotationZ);
                            GameObject indicator = Instantiate(SideIndicatorPrefab, SideNearTown.Position, rotation);
                            SidesIndicatorsPrefabList.Add(indicator);
                            indicator.GetComponent<RoadBuildIndicatorPrefab>().Setup(SideNearTown.Position);
                        }
                    }

           


                }
                                                 
            }   


            return;
        }




        if (FirstTurnPlacedPeices == 4)
        {
            FirstTurnPlacedPeices++;
            FirstTurnIsActive = false;
            return;
        }


    }

    public void ShowBuildIndicatorsTowns()
    {

    
        foreach (var corner in CornersDic.Values)
        {
            bool ConnectedToRoad = false;
            bool NearATown = false;

            if(corner.HasSettlement == true)
            {
                continue;
            }


            // checkes if adjusted Corners have a settelment
            foreach (var adjustedCorner in corner.AdjacentCorners)
            {
                if(adjustedCorner.HasSettlement == true)
                {
                    NearATown = true;
                }
            }

            // checkes if adjusted sides have a settelment
            foreach (var adjustedSide in corner.AdjacentSides)
            {
                if(adjustedSide.HasRoad == true)
                {
                    ConnectedToRoad = true;
                }

            }

          
            if (ConnectedToRoad == true && NearATown == false)
            {
                corner.CanBeBuiltOn = true;
            }
            else
            {
                corner.CanBeBuiltOn = false;
            }


            if (corner.CanBeBuiltOn == true)
            {
                
                GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                CornersIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner.Position);
            }
        }
        
        //return;
        
        

    }


    public void BuildSettlementAt(Vector3 cornerPosition)
    {
        if (CornersDic.TryGetValue(cornerPosition, out CornersClass corner) && corner.CanBeBuiltOn)
        {

            if ( FirstTurnIsActive == true)  // if its the first turn ignore resources count,and return to the first turn flow at the end

            {
                corner.CanBeBuiltOn = false;
                corner.HasSettlement = true;

                Instantiate(TownPrefab, corner.Position, Quaternion.identity);

               
                player.SettelmentsList.Add(corner);
                player.UpdayeVictoryPoints(1);
                uiManager.UpdateVictoryPointsDisplay();


                foreach (var NeighborCornerKey in corner.AdjacentCorners)
                {
                    CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
                }



                foreach (var indicator in CornersIndicatorsPrefabList)
                {
                    Destroy(indicator.gameObject);
                }

                FirstTurnPlacement();

            }


            else 
            {
                if(player.CanAfford(PricesClass.TownCost) == true)  // if its not the first turn check for resources amount
                {

                    player.SubtractResources(PricesClass.TownCost);

                    corner.CanBeBuiltOn = false;
                    corner.HasSettlement = true;

                    Instantiate(TownPrefab, corner.Position, Quaternion.identity);


                    player.SettelmentsList.Add(corner);
                    player.UpdayeVictoryPoints(1);
                    uiManager.UpdateVictoryPointsDisplay();



                    foreach (var NeighborCornerKey in corner.AdjacentCorners)
                    {
                        CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
                    }



                    foreach (var indicator in CornersIndicatorsPrefabList)
                    {
                        Destroy(indicator.gameObject);
                    }


                    ShowBuildIndicatorsTowns();

                }

                else
                {
                    Debug.Log("Not enough resources to build a town.");
                }


            }


        }
    }



    public void ShowBuildIndicatorsRoads()
    {
        foreach (var Side in SidesDic.Values)
        {
            if (Side.CanBeBuiltOn == true && Side.HasRoad == false)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, Side.RotationZ);
                GameObject indicator = Instantiate(SideIndicatorPrefab, Side.Position, rotation);
                SidesIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<RoadBuildIndicatorPrefab>().Setup(Side.Position);
            }
        }
        return;
    }



    public void BuildRoadAt(Vector3 SidePosition)
    {
        if (SidesDic.TryGetValue(SidePosition, out SidesClass Side) && Side.CanBeBuiltOn)
        {

            if (FirstTurnIsActive == true)
            {
                Side.CanBeBuiltOn = false;
                Side.HasRoad = true;

                Quaternion SideRotation = Quaternion.Euler(0, 0, Side.RotationZ);
                Instantiate(RoadPrefab, Side.Position, SideRotation);


                foreach (var NeighborsRoads in Side.AdjacentSides)
                {
                    NeighborsRoads.CanBeBuiltOn = true;
                }

                foreach (var indicator in SidesIndicatorsPrefabList)
                {
                    Destroy(indicator.gameObject);
                }

                FirstTurnPlacement();
            }

            else
            {
                if (player.CanAfford(PricesClass.RoadCost) == true)
                {

                    player.SubtractResources(PricesClass.RoadCost);
                    Side.CanBeBuiltOn = false;
                    Side.HasRoad = true;

                    Quaternion SideRotation2 = Quaternion.Euler(0, 0, Side.RotationZ);
                    Instantiate(RoadPrefab, Side.Position, SideRotation2);


                    foreach (var NeighborsRoads in Side.AdjacentSides)
                    {
                        NeighborsRoads.CanBeBuiltOn = true;
                    }

                    foreach (var indicator in SidesIndicatorsPrefabList)
                    {
                        Destroy(indicator.gameObject);
                    }


                    ShowBuildIndicatorsRoads();

                }

                else
                {
                    Debug.Log("Not enough resources to build a road.");
                }
            }


            

        }
    }


















    void IntialMapResourcesShuffle()
    {
        // Shuffle the list to randomize the board layout
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




    void InitializeTiles()
    {

        int ResourceIndex = 0;

        // Loop through all positions in the Tilemap
        foreach (var TilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(TilePosition))
            {
                Vector3 worldPosition = tilemap.CellToWorld(TilePosition);

                var resourceType = ResourcesOnTheMapList[ResourceIndex];

                if (resourceType != TileClass.ResourceType.Desert)
                {
                    //assaigning number token 
                    int RandomNumberTokenIndex = Random.Range(0, availableNumbers.Count);
                    int numberToken = availableNumbers[RandomNumberTokenIndex];
                    availableNumbers.RemoveAt(RandomNumberTokenIndex);

                    var tile = new TileClass(resourceType, numberToken, TilePosition, worldPosition);
                    TilesDictionary.Add(TilePosition, tile);

                    //spawn a prefab of the number token
                    
                    GameObject prefabInstance = Instantiate(NumberTokenPrefab, worldPosition, Quaternion.identity);
                    TextMeshPro textMesh = prefabInstance.transform.GetChild(0).GetComponent<TextMeshPro>();
                    textMesh.text = numberToken.ToString();

                    if (numberToken == 6 || numberToken == 8)
                    {
                        textMesh.color = Color.red;
                        textMesh.fontStyle = FontStyles.Bold;
                    }

                }

                else
                {
                    var tile = new TileClass(resourceType, 0, TilePosition,worldPosition);
                    TilesDictionary.Add(TilePosition, tile);
                }


                ResourceIndex++;
                
            }

            
        }
    }



    void UpdateVisualRepresentation()
    {
        foreach (var tilePair in TilesDictionary)
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
                    break; // Or handle unknown resource type
            }
        }
    }

   


   



    private void CreateDicsAndAdjacentTiles()
    {
        foreach (var tilePair in TilesDictionary)
        {
            Vector3Int TilePosition = tilePair.Key;
            TileClass TileValue = tilePair.Value;
            Vector3 TileWorldPosition = tilemap.CellToWorld(TilePosition);


            var CornerPositions = GetCornerPositionsForTile(TileWorldPosition);
            var sidePositions = GetSidesPositionsForTile(TileWorldPosition);



            foreach (var CornerPos in CornerPositions)
            {
                if (!CornersDic.ContainsKey(CornerPos))
                {
                    CornersDic[CornerPos] = new CornersClass(CornerPos);
                }

                CornersDic[CornerPos].AdjacentTiles.Add(TileValue);
                TileValue.AdjacentCorners.Add(CornersDic[CornerPos]);
            }


            foreach (var SidePos in sidePositions)
            {
                if (!SidesDic.ContainsKey(SidePos.Position))
                {
                    SidesDic[SidePos.Position] = new SidesClass(SidePos.Position, SidePos.RotationZ);

                }

                SidesDic[SidePos.Position].AdjacentTiles.Add(TileValue);
                TileValue.AdjacentSides.Add(SidePos);

            }



        }


    }


    private void CreateNeighborsLists()
    {

        float adjacencyThreshold = 1f; 
        foreach (var cornerEntry in CornersDic)
        {
            CornersClass corner = cornerEntry.Value;

            foreach (var sideEntry in SidesDic)
            {
                SidesClass side = sideEntry.Value;

                // Check if the side is adjacent to the corner
                if (Vector3.Distance(side.Position, corner.Position) <= adjacencyThreshold)
                {
                    corner.AdjacentSides.Add(side);
                    side.AdjacentCorners.Add(corner);

                }
            }
        }


        //corner

        foreach (var currentCorner in CornersDic.Values)
        {
            foreach (var possibleAdjacentCorner in CornersDic.Values)
            {
                // Avoid comparing a corner with itself
                if (currentCorner == possibleAdjacentCorner) continue;

                // Check if the two corners share an adjacent side
                var sharedSide = currentCorner.AdjacentSides.Intersect(possibleAdjacentCorner.AdjacentSides).FirstOrDefault();
                bool areAdjacent = sharedSide != null;

                // If they share an adjacent side, they are adjacent corners
                if (areAdjacent)
                {
                    // Since corners are adjacent, add their positions to each other's list
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
        foreach (var currentSide in SidesDic.Values)
        {
            foreach (var possibleAdjacentSide in SidesDic.Values)
            {
                // Avoid comparing a side with itself
                if (currentSide == possibleAdjacentSide) continue;

                // Check if the two sides share a corner
                var sharedCorner = currentSide.AdjacentCorners.Intersect(possibleAdjacentSide.AdjacentCorners).FirstOrDefault();
                bool areAdjacent = sharedCorner != null;

                // If they share a corner, they are adjacent sides
                if (areAdjacent)
                {
                    // Since sides are adjacent, add them to each other's list
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




    public List<SidesClass> GetSidesPositionsForTile(Vector3 HexCenterPosition)
    {
        List<SidesClass> sides = new List<SidesClass>();
        float tilemapScale = tilemap.transform.localScale.x;
        float outerRadius = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);
        float apothem = outerRadius * Mathf.Cos(Mathf.PI / 6); // Cos(30°) in radians

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            Vector3 sideMidPointPos = new Vector3(
                HexCenterPosition.x + apothem * Mathf.Cos(angle_rad),
                HexCenterPosition.y + apothem * Mathf.Sin(angle_rad),
                HexCenterPosition.z
            );
            float rotationZ = angle_deg + 90f; // Adjust the rotation to match your needs

            sides.Add(new SidesClass(RoundVector3(sideMidPointPos, 2), rotationZ));



        }

        return sides;
    }



    public List<Vector3> GetCornerPositionsForTile(Vector3 HexCenterPostion)
    {
        List<Vector3> corners = new List<Vector3>();
        float tilemapScale = tilemap.transform.localScale.x;
        float size = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i + 30; // Offset by 30 degrees for pointy-topped hexes
            //float angleRad = Mathf.Deg2Rad * angleDeg;
            float angleRad = Mathf.PI / 180 * angleDeg;
            Vector3 cornerPos = new Vector3(HexCenterPostion.x + size * Mathf.Cos(angleRad), HexCenterPostion.y + size * Mathf.Sin(angleRad), HexCenterPostion.z);
            Vector3 roundedCornerPos = RoundVector3(cornerPos, 1);
            corners.Add(roundedCornerPos);


            
        }


        return corners;
    }




    Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10.0f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
}


