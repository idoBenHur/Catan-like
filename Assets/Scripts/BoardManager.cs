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
    public GameObject RoadPrefab;
    public GameObject TownPrefab;

    public List<GameObject> CornersIndicatorsPrefabList = new List<GameObject>();
    public List<GameObject> SidesIndicatorsPrefabList = new List<GameObject>();


    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> CornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> SidesDic = new Dictionary<Vector3, SidesClass>();


    // the holy resouce dic, (wood, brick, sheep, ore, wheat)
    Dictionary<int, List<int>> ResourcePerRollDic = new Dictionary<int, List<int>>
{
    {2, new List<int> {0, 0, 0, 0, 0}},
    {3, new List<int> {0, 0, 0, 0, 0}},
    {4, new List<int> {0, 0, 0, 0, 0}},
    {5, new List<int> {0, 0, 0, 0, 0}},
    {6, new List<int> {0, 0, 0, 0, 0}},
    {8, new List<int> {0, 0, 0, 0, 0}},
    {9, new List<int> {0, 0, 0, 0, 0}},
    {10, new List<int> {0, 0, 0, 0, 0}},
    {11, new List<int> {0, 0, 0, 0, 0}},
    {12, new List<int> {0, 0, 0, 0, 0}}
};

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

    private void GetResourcesForDiceRoll(int diceResult)
    {
        if (ResourcePerRollDic.TryGetValue(diceResult, out List<int> ResourcesListFromDic))
        {
            // Assuming ResourceType enum values are ordered as Wood, Brick, Sheep, Ore, Wheat
            for (int i = 0; i < ResourcesListFromDic.Count; i++)
            {
                if (ResourcesListFromDic[i] > 0)
                {
                    player.AddResource((ResourceType)i, ResourcesListFromDic[i]);
                }
            }
        }
    }


    private void DistributeResources(int DiceResult)
    {

        if (DiceResult == 7)
        {
            Debug.Log("7");
        }

        else
        {
            foreach (var tile in TilesDictionary.Values)
            {
                if (tile.numberToken == DiceResult && tile.hasRobber == false)
                {
                    foreach(var Corner in tile.AdjacentCorners)
                    {
                        if(Corner.HasSettlement == true)
                        {

                            player.AddResource(tile.resourceType, 1);

                        }
                    }
                    

                }
            }
        }

    }


    private void FirstTurnPlacement() 
    {
        

        Debug.Log("amount of Placed Peices in the first turn " + FirstTurnPlacedPeices);
        



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

                ChangeResourcePerRollDic(ResourcePerRollDic, corner);

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

                    ChangeResourcePerRollDic(ResourcePerRollDic, corner);

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








            
           // ChangeResourcePerRollDic(ResourcePerRollDic, corner);



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








    void UpdateNeighborsCanBeBuiltOn()
    {
        foreach (var CornerClass in CornersDic.Values)
        {
            if( CornerClass.HasSettlement == true)
            {
               foreach(var NeighborCornerKey in CornerClass.AdjacentCorners)
                {
                    CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
                }
                
                

            }

        }
            
    }




    private void ChangeResourcePerRollDic(Dictionary<int, List<int>> dic, CornersClass Corner )
    {
        foreach (var hex in Corner.AdjacentTiles)
        {
           int HexNumber = hex.numberToken;
           var ResourceType = hex.resourceType;

            //  (wood, brick, sheep, ore, wheat)
            switch (ResourceType)
            {
                case TileClass.ResourceType.Wood:
                    ResourcePerRollDic[HexNumber][0] += 1;
                    break;
                case TileClass.ResourceType.Brick:
                    ResourcePerRollDic[HexNumber][1] += 1;
                    break;
                case TileClass.ResourceType.Sheep:
                    ResourcePerRollDic[HexNumber][2] += 1;
                    break;
                case TileClass.ResourceType.Ore:
                    ResourcePerRollDic[HexNumber][3] += 1;
                    break;
                case TileClass.ResourceType.Wheat:
                    ResourcePerRollDic[HexNumber][4] += 1;
                    break;
                default:
                    break; 
            }
        }

        //foreach (KeyValuePair<int, List<int>> kvp in ResourcePerRollDic)
        //{
        //    Debug.Log($"Key: {kvp.Key}, Values: {string.Join(", ", kvp.Value)}");
        //}

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
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {

                var resourceType = ResourcesOnTheMapList[ResourceIndex];

                if (resourceType != TileClass.ResourceType.Desert)
                {
                    //assaigning number token 
                    int RandomNumberTokenIndex = Random.Range(0, availableNumbers.Count);
                    int numberToken = availableNumbers[RandomNumberTokenIndex];
                    availableNumbers.RemoveAt(RandomNumberTokenIndex);

                    var tile = new TileClass(resourceType, numberToken);
                    TilesDictionary.Add(position, tile);

                    //spawn a prefab of the number token
                    Vector3 worldPosition = tilemap.CellToWorld(position);
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
                    var tile = new TileClass(resourceType, 0);
                    TilesDictionary.Add(position, tile);
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

    public void UpdateCornerData()
    {

        //finds AdjacentTiles of a corner and creatae cornner dic 

        foreach (var tilePair in TilesDictionary)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;
            Vector3 worldPosition = tilemap.CellToWorld(position);


            var CornerPositions = GetCornerPositionsForTile(worldPosition);



            foreach (var cornerPos in CornerPositions)
            {
                if (!CornersDic.ContainsKey(cornerPos))
                {
                    CornersDic[cornerPos] = new CornersClass(cornerPos);
                }

                CornersDic[cornerPos].AdjacentTiles.Add(tile);
            }

        }

        //creating adjacent Corners list


        float Side = tilemap.cellSize.x; //  side length of your hex
        float tilemapScale = tilemap.transform.localScale.x;
        float threshold = (Side * Mathf.Sqrt(3) / 2) * tilemapScale;

        foreach (var currentCorner in CornersDic.Values)
        {
            foreach (var possibleAdjacentCorner in CornersDic.Values)
            {
                // Avoid comparing a corner with itself
                if (currentCorner == possibleAdjacentCorner) continue;

                // Find shared tiles between the two corners
                var sharedTiles = currentCorner.AdjacentTiles.Intersect(possibleAdjacentCorner.AdjacentTiles).ToList();
                bool TwoOrMoreAdjacent = sharedTiles.Count >= 2;

                // Special case for corners on the board's edge
                if (TwoOrMoreAdjacent == false && (currentCorner.AdjacentTiles.Count == 1 || possibleAdjacentCorner.AdjacentTiles.Count == 1))
                {

                    
                    TwoOrMoreAdjacent = Vector3.Distance(currentCorner.Position, possibleAdjacentCorner.Position) < threshold;
                }


                // If they share two or more tiles, they are adjacent
                if (TwoOrMoreAdjacent == true)
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

    }


    private void UpdateHexSidesData()
    {


        //Creates the sideDic finds AdjacentTiles of a sides

        foreach (var tilePair in TilesDictionary)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;
            Vector3 worldPosition = tilemap.CellToWorld(position);

            var sidePositions = GetSidesPositionsForTile(worldPosition);

            foreach (var hexSide in sidePositions)
            {
                if (!SidesDic.ContainsKey(hexSide.Position))
                {
                    SidesDic[hexSide.Position] = new SidesClass(hexSide.Position, hexSide.RotationZ);

                }

                SidesDic[hexSide.Position].AdjacentTiles.Add(tile);
            } 
        }


        //Compare all roads to each others, if the are close they are Adjacent 

        foreach (var currentSide in SidesDic.Values)
        {
            foreach (var possibleAdjacentSide in SidesDic.Values)
            {
                // Avoid comparing a corner with itself
                if (currentSide == possibleAdjacentSide) continue;
         
                float size = 1f;
                bool AdjacentSide = Vector3.Distance(currentSide.Position, possibleAdjacentSide.Position) < size;
                    

                if(AdjacentSide == true)
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
                
                else
                {
                    continue;
                }




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


