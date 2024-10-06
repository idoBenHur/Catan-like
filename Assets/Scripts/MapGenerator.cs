using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileClass;





[System.Serializable]
public class InitialSettlementData
{
    public Vector3 position;
    public bool isCity;
}





public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Grid grid;
    public Tilemap BaseTilemap;
    [SerializeField] private Tilemap FogTileMap;
    [SerializeField] private Tilemap ObjectsTileMap;
    public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile, fogTile, goldTileObject, rumTileObject; // Assign these in the inspector
    public GameObject NumberTokenPrefab;
    public GameObject RoadPrefab;
    public GameObject TownPrefab;
    public GameObject CityPrefab;
    public GameObject HarborPrefab;
    [SerializeField] bool withHarbors;
    [SerializeField] bool RandomHarborsPosition;
    [SerializeField] bool RandomHarborsTypes;



    private List<(CornersClass, CornersClass)> HarborCornersPairs;
    public Dictionary<Vector3Int, TileClass> InitialTilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> InitialCornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> InitialSidesDic = new Dictionary<Vector3, SidesClass>();
    public List<InitialSettlementData> initialSettlements = new List<InitialSettlementData>();
    public List<Vector3> initialRoads = new List<Vector3>();

    //private List<TileClass.ResourceType> ResourcesOnTheMapList = new List<TileClass.ResourceType>
    //{
    //    TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep, TileClass.ResourceType.Sheep,
    //    TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood, TileClass.ResourceType.Wood,
    //    TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat, TileClass.ResourceType.Wheat,
    //    TileClass.ResourceType.Brick, TileClass.ResourceType.Brick, TileClass.ResourceType.Brick,
    //    TileClass.ResourceType.Ore, TileClass.ResourceType.Ore, TileClass.ResourceType.Ore,
    //    TileClass.ResourceType.Desert
    //};

    public List<TileClass.ResourceType> ResourcesOnTheMapList = new List<TileClass.ResourceType>();

    //private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };
    public List<int> availableNumbers = new List<int> ();


    // 693px (the whole hex hieght ) - 590px (the surface area hex hieght) = 103 px. 103 % 693 (pixel per unit of the image) = 0.1486291
    // 0.1486291 = the "units" amount needed to be adjusted to recive the "surface area hex" corners'
    private float HexagonCenterOffset = 0.1486291f;




    // Main function to initiate the map generation process (called from board manager)
    public void InitialBuildMap()
    {
        InitialMapResourcesShuffle();
        InitializeTiles();
        UpdateTileTypeVisual(InitialTilesDictionary);
        UpdateTileNumberVisual(InitialTilesDictionary);

        CreateDicsAndAdjacentTiles();
        CreateNeighborsLists();
        PlaceResourceObjects();

        if (withHarbors == true) 
        {
            SetupHarbors();
            UpdateHarborsVisuals();
        }

        

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
        List<int> TempNumbersList = new List<int> (availableNumbers);

        int ResourceIndex = 0;

        foreach (var TilePosition in BaseTilemap.cellBounds.allPositionsWithin) 
        {
            if (BaseTilemap.HasTile(TilePosition))
            {
                Vector3 centerPosition = BaseTilemap.CellToWorld(TilePosition);
                Vector3 worldPosition = new Vector3(centerPosition.x, centerPosition.y + HexagonCenterOffset, centerPosition.z);           //BaseTilemap.CellToWorld(TilePosition);
                var resourceType = ResourcesOnTheMapList[ResourceIndex];  

                if (resourceType != TileClass.ResourceType.Desert)
                {
                    int RandomNumberTokenIndex = Random.Range(0, TempNumbersList.Count);
                    int numberToken = TempNumbersList[RandomNumberTokenIndex];
                    TempNumbersList.RemoveAt(RandomNumberTokenIndex);



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
                    BaseTilemap.SetTile(position, woodTile);
                    break;
                case TileClass.ResourceType.Brick:
                    BaseTilemap.SetTile(position, brickTile);
                    break;
                case TileClass.ResourceType.Wheat:
                    BaseTilemap.SetTile(position, wheatTile);
                    break;
                case TileClass.ResourceType.Ore:
                    BaseTilemap.SetTile(position, oreTile);
                    break;
                case TileClass.ResourceType.Sheep:
                    BaseTilemap.SetTile(position, sheepTile);
                    break;
                case TileClass.ResourceType.Desert:
                    BaseTilemap.SetTile(position, desertTile);
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
            if (Tile.Value.MyNumberPrefab != null) { Destroy(Tile.Value.MyNumberPrefab); } // if tile already have a number prefab, destroy it

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
            Vector3 TileWorldPosition = BaseTilemap.CellToWorld(TilePosition);



            var CornerPositions = CornersFinderNEW(TileWorldPosition);     // GetCornerPositionsForTile(TileWorldPosition);
            var sidePositions = SidesFinderNEW(TileWorldPosition, CornerPositions);                                        //GetSidesPositionsForTile(TileWorldPosition);

            


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
                TileValue.AdjacentSides.Add(InitialSidesDic[SidePos.Position]);

            }



        }

    }

    // Calculate and store the positions and rotations for each side of the hex tiles
    public List<SidesClass> GetSidesPositionsForTile(Vector3 HexCenterPosition)
    {
        List<Vector3> corners = GetCornerPositionsForTile(HexCenterPosition);
        List<SidesClass> sides = new List<SidesClass>();

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentCorner = corners[i];
            Vector3 nextCorner = corners[(i + 1) % 6]; // Wrap around at the last corner

            // Calculate the midpoint between the current corner and the next
            Vector3 sideMidpoint = (currentCorner + nextCorner) / 2;
            Vector3 roundedSidePos = RoundVector3(sideMidpoint, 2);

            // Calculate rotation: Angle in degrees from the horizontal axis
            float rotationZ = Mathf.Atan2(nextCorner.y - currentCorner.y, nextCorner.x - currentCorner.x) * Mathf.Rad2Deg;

            // Create new SideClass object and add it to the list
            sides.Add(new SidesClass(roundedSidePos, rotationZ));
        }

        return sides;



        






        //List<SidesClass> sides = new List<SidesClass>();
        //float tilemapScale = tilemap.transform.localScale.x;
        //float outerRadius = (tilemap.cellSize.x * tilemapScale) / Mathf.Sqrt(3);
        //float apothem = outerRadius * Mathf.Cos(Mathf.PI / 6); 

        //for (int i = 0; i < 6; i++)
        //{
        //    float angle_deg = 60 * i;
        //    float angle_rad = Mathf.PI / 180 * angle_deg;
        //    Vector3 sideMidPointPos = new Vector3(
        //        HexCenterPosition.x + apothem * Mathf.Cos(angle_rad),
        //        HexCenterPosition.y + apothem * Mathf.Sin(angle_rad),
        //        HexCenterPosition.z
        //    );
        //    float rotationZ = angle_deg + 90f; 

        //   sides.Add(new SidesClass(RoundVector3(sideMidPointPos, 2), rotationZ));



        //}

        //return sides;
    }


    // Calculate and store the positions for each corner of the hex tiles
    public List<Vector3> GetCornerPositionsForTile(Vector3 HexCenterPostion)
    {
        Vector3 tilemapScale = BaseTilemap.transform.localScale; // Get the scale of the tilemap     
        float size = Mathf.Max(BaseTilemap.cellSize.x, BaseTilemap.cellSize.y) / 2; // Half the cell size
        List<Vector3> corners = new List<Vector3>();


        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30; // Offset by -30 degrees for pointy top
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            Vector3 cornerOffset = new Vector3(size * Mathf.Cos(angle_rad), size * Mathf.Sin(angle_rad), 0);
            cornerOffset.x *= tilemapScale.x; // Scale adjustment for x
            cornerOffset.y *= tilemapScale.y; // Scale adjustment for y
            Vector3 cornerWorld = HexCenterPostion + cornerOffset;
            Vector3 roundedCornerPos = RoundVector3(cornerWorld, 1);
            corners.Add(roundedCornerPos);
            
        }

        
        return corners;












        //List<Vector3> corners = new List<Vector3>();
        //float ScaleX = tilemap.transform.localScale.x;
        //float ScaleY = tilemap.transform.localScale.y;
        //float sizeX = (tilemap.cellSize.x * ScaleX) / Mathf.Sqrt(3);
        //float sizeY = (tilemap.cellSize.y * ScaleY) / Mathf.Sqrt(3);

        //for (int i = 0; i < 6; i++)
        //{
        //    float angleDeg = 60 * i + 30; 
        //    //float angleRad = Mathf.Deg2Rad * angleDeg;
        //    float angleRad = (Mathf.PI / 180) * angleDeg;
        //    Vector3 cornerPos = new Vector3(HexCenterPostion.x + sizeX * Mathf.Cos(angleRad), HexCenterPostion.y + sizeX * Mathf.Sin(angleRad), HexCenterPostion.z);
        //    Vector3 roundedCornerPos = RoundVector3(cornerPos, 1);
        //    corners.Add(roundedCornerPos);



        //}



        //return corners;
    }






    private List<Vector3> CornersFinderNEW(Vector3 HexCenterPostion)
    {
        Vector3 gridCellSize = grid.cellSize; 
        Vector3 tilemapScale = BaseTilemap.transform.localScale;




        List<Vector3> hexCorners = new List<Vector3>();
        List<Vector3> RoundedHexCorners = new List<Vector3>();

        float width = gridCellSize.x * tilemapScale.x;
        float height = gridCellSize.y * tilemapScale.y;



        // Calculate hex corners with custom Y offsets for tall hexagons relative to the tile world position
        hexCorners.Add(HexCenterPostion + new Vector3(0, height * 0.5f + HexagonCenterOffset, 0)); // Top
        hexCorners.Add(HexCenterPostion + new Vector3(width * 0.5f, height * 0.25f + HexagonCenterOffset, 0)); // Top-right
        hexCorners.Add(HexCenterPostion + new Vector3(width * 0.5f, -height * 0.25f + HexagonCenterOffset, 0)); // Bottom-right
        hexCorners.Add(HexCenterPostion + new Vector3(0, -height * 0.5f + HexagonCenterOffset, 0)); // Bottom
        hexCorners.Add(HexCenterPostion + new Vector3(-width * 0.5f, -height * 0.25f + HexagonCenterOffset, 0)); // Bottom-left
        hexCorners.Add(HexCenterPostion + new Vector3(-width * 0.5f, height * 0.25f + HexagonCenterOffset, 0)); // Top-left


        foreach (var corner in hexCorners)
        {
            RoundedHexCorners.Add(RoundVector3(corner, 3));
        }

        return RoundedHexCorners;
    }


    private List<SidesClass> SidesFinderNEW(Vector3 HexCenterPosition, List<Vector3> cornersPosition)
    {
        List<SidesClass> sides = new List<SidesClass>();

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentCorner = cornersPosition[i];
            Vector3 nextCorner = cornersPosition[(i + 1) % 6]; // Wrap around at the last corner

            // Calculate the midpoint between the current corner and the next
            Vector3 sideMidpoint = (currentCorner + nextCorner) / 2;
            Vector3 roundedSidePos = RoundVector3(sideMidpoint, 3);

            // Calculate rotation: Angle in degrees from the horizontal axis
            float rotationZ = Mathf.Atan2(nextCorner.y - currentCorner.y, nextCorner.x - currentCorner.x) * Mathf.Rad2Deg;

            // Create new SideClass object and add it to the list
            sides.Add(new SidesClass(roundedSidePos, rotationZ));
        }


        return sides;
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

        //tiles
        foreach (var currentTile in InitialTilesDictionary.Values)
        {
            foreach (var possibleAdjacentTile in InitialTilesDictionary.Values)
            {
                if (currentTile == possibleAdjacentTile) continue;
              

                // Check for shared corners to determine adjacency
                if (currentTile.AdjacentCorners.Intersect(possibleAdjacentTile.AdjacentCorners).Any())
                {
                    if (!currentTile.AdjacentTiles.Contains(possibleAdjacentTile))
                    {
                        currentTile.AdjacentTiles.Add(possibleAdjacentTile);
                    }

                    if (!possibleAdjacentTile.AdjacentTiles.Contains(currentTile))
                    {
                        possibleAdjacentTile.AdjacentTiles.Add(currentTile);
                    }
                }
            }
        }
        //tiles


        


    }
    //give corrner class its harbors and sets harbors type
    public void SetupHarbors() 
    {
        HarborCornersPairs = LoadHarborPositions();
        HarborClass lastHarbor = null;

        List<HarborClass> harborsTypes = new List<HarborClass>
        {
            new HarborClass(HarborResourceType.Any, 3),
            new HarborClass(HarborResourceType.Any, 3),
            new HarborClass(HarborResourceType.Any, 3),
            new HarborClass(HarborResourceType.Any, 3),
            new HarborClass(HarborResourceType.Wood, 2),
            new HarborClass(HarborResourceType.Brick, 2),
            new HarborClass(HarborResourceType.Sheep, 2),
            new HarborClass(HarborResourceType.Ore, 2),
            new HarborClass(HarborResourceType.Wheat, 2)
        };


        foreach (var pair in HarborCornersPairs)
        {
            if (harborsTypes.Count == 0)
                break;

            if (lastHarbor == null)
            {
                int index = RandomHarborsTypes ? Random.Range(0, harborsTypes.Count) : 0; // if true random, if false 0
                lastHarbor = harborsTypes[index];
                harborsTypes.RemoveAt(index);
            }

            pair.Item1.Harbor = lastHarbor;
            pair.Item2.Harbor = lastHarbor;
            lastHarbor = null;
        }


        //for (int i = 0; i < HarborCornersPairs.Count; i++)
        //{
        //    if (harborsTypes.Count > 0)
        //    {
        //        if(lastHarbor == null)
        //        {
        //            int randomIndex = Random.Range(0, harborsTypes.Count);
        //            HarborCornersPairs[i].Item1.Harbor = harborsTypes[randomIndex];
        //            lastHarbor = harborsTypes[randomIndex];
                    
        //        }

        //        HarborCornersPairs[i].Item2.Harbor = lastHarbor;
        //        harborsTypes.Remove(lastHarbor);
        //        lastHarbor = null;

        //    }

   
        //}

    }

    private List<(CornersClass, CornersClass)> LoadHarborPositions() 
    {

        //List<CornersClass> SeaCornners = new List<CornersClass> ();
        //List<(CornersClass, CornersClass)> HarborPairs = new List<(CornersClass, CornersClass)>();



        ////find all coast a Adjacent corners
        //foreach (var corner in InitialCornersDic)
        //{
        //    if( corner.Value.AdjacentTiles.Count < 3 ) 
        //    {
        //        SeaCornners.Add(corner.Value);

        //    }
        //}


        //// create 9 harbor pairs

        //for (int i = 0; i < 9; i++)
        //{
        //    var currentCorrner = SeaCornners[i*3]; // default (non-random)

        //    if (RandomHarborsPosition == true)
        //    {
        //        // pick a random corrner that is not in any pair
        //        int ranodmIndex = Random.Range(0, SeaCornners.Count);
        //        currentCorrner = SeaCornners[ranodmIndex];
        //        while (HarborPairs.Any(pair => pair.Item1 == currentCorrner || pair.Item2 == currentCorrner)) //picks a correnr that is not already in the harbor list
        //        {
        //            ranodmIndex = Random.Range(0, SeaCornners.Count);
        //            currentCorrner = SeaCornners[ranodmIndex];
        //        }
        //    }




        //    //find its Adjacent corrner and make a pair
        //    foreach (var PossiblHarborPair in SeaCornners)
        //    {
        //        if (HarborPairs.Any(pair => pair.Item1 == PossiblHarborPair || pair.Item2 == PossiblHarborPair)) { continue; }

        //        if (currentCorrner.AdjacentCorners.Contains(PossiblHarborPair) == true) 
        //        { 
        //            HarborPairs.Add((currentCorrner, PossiblHarborPair));


        //            break;
        //        }
        //        else continue;
        //    }
        //}


        //return HarborPairs;

        List<CornersClass> SeaCorners = new List<CornersClass>();
        List<(CornersClass, CornersClass)> HarborPairs = new List<(CornersClass, CornersClass)>();

        // Find all coast adjacent corners
        foreach (var corner in InitialCornersDic.Values)
        {
            if (corner.AdjacentTiles.Count < 3)
            {
                SeaCorners.Add(corner);
            }
        }

        if (RandomHarborsPosition)
        {
            // Shuffle the list for random harbor positions
            SeaCorners = SeaCorners.OrderBy(x => Random.Range(0, SeaCorners.Count)).ToList();
        }

        // Create 9 harbor pairs
        int pairsCount = 0;
        while (pairsCount < 9 && SeaCorners.Count >= 2) // Ensure there are enough corners left to form pairs
        {
            CornersClass currentCorner = SeaCorners[0]; // Take the first corner

            // Find its adjacent corner and make a pair
            CornersClass possibleHarborPair = currentCorner.AdjacentCorners.FirstOrDefault(ac =>
                SeaCorners.Contains(ac) && !HarborPairs.Any(pair => pair.Item1 == ac || pair.Item2 == ac));

            if (possibleHarborPair != null)
            {
                HarborPairs.Add((currentCorner, possibleHarborPair));
                SeaCorners.Remove(currentCorner);
                SeaCorners.Remove(possibleHarborPair);
                pairsCount++;
            }
            else
            {
                SeaCorners.Remove(currentCorner); // Remove currentCorner if no pair found
            }
        }

        return HarborPairs;



    }



  



    private void UpdateHarborsVisuals()
    {
        foreach (var cornerPair in HarborCornersPairs) 
        {

            // cucalting harbor postion on screen
            var commonTile = cornerPair.Item1.AdjacentTiles.Intersect(cornerPair.Item2.AdjacentTiles).ToList();
            Vector3 commonTilePosition = commonTile[0].TileWorldPostion;
            Vector3 symmetricalForthPoint = cornerPair.Item1.Position + cornerPair.Item2.Position - commonTilePosition;

            Vector3 cornersMidpoint = (cornerPair.Item1.Position + cornerPair.Item2.Position) / 2;
            float offSet = 0.5f;
            Vector3 adjustedForthPoint = Vector3.Lerp(symmetricalForthPoint, cornersMidpoint, offSet);
            Vector3 directionToMidpoint = cornersMidpoint - adjustedForthPoint;

            float angle = Mathf.Atan2(directionToMidpoint.y, directionToMidpoint.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 180);


            // spawning object and effecting colors/numbers

            GameObject harborInstance = Instantiate(HarborPrefab, adjustedForthPoint, Quaternion.identity);
            cornerPair.Item1.Harbor.HarborGameObject = harborInstance;
            cornerPair.Item2.Harbor.HarborGameObject = harborInstance;


            Transform HarborLegs = harborInstance.transform.GetChild(0);
            HarborLegs.rotation = rotation;

            Transform HarborSpriteObject = harborInstance.transform.GetChild(1);
            SpriteRenderer HarborSprite = HarborSpriteObject.GetComponent<SpriteRenderer>();
            TextMeshPro RatioTextComp = harborInstance.GetComponentInChildren<TextMeshPro>();
            GameObjectsToolTipTrigger HarborToolTip = harborInstance.GetComponent<GameObjectsToolTipTrigger>();


            HarborResourceType PortResourceType = cornerPair.Item1.Harbor.TradeResource;
            Color harborColor = Color.white; // default
            string RatioText = "3:1";



            switch (PortResourceType)
            {
                case HarborResourceType.Wood:
                    harborColor = new Color(35f / 255f, 72f / 255f, 18f / 255f);
                    RatioText = "2:1";
                    HarborToolTip.header = "Wood Port <sprite name=wood>";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 2 <sprite name=wood> for 1 of any resource.";
                    break;
                case HarborResourceType.Brick:
                    harborColor = new Color(192f / 255f, 90f / 255f, 15f / 255f);
                    RatioText = "2:1";
                    HarborToolTip.header = "Brick Port <sprite name=brick>";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 2 <sprite name=brick> for 1 of any resource.";
                    break;
                case HarborResourceType.Sheep:
                    harborColor = new Color(110f / 255f, 212f / 255f, 63f / 255f);
                    RatioText = "2:1";
                    HarborToolTip.header = "Sheep Port <sprite name=sheep>";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 2 <sprite name=sheep> for 1 of any resource.";
                    break;
                case HarborResourceType.Ore:
                    harborColor = new Color(160f / 255f, 162f / 255f, 164f / 255f);
                    RatioText = "2:1";
                    HarborToolTip.header = "Ore Port <sprite name=ore>";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 2 <sprite name=ore> for 1 of any resource.";
                    break;
                case HarborResourceType.Wheat:
                    harborColor = new Color(226f / 255f, 218f / 255f, 25f / 255f);
                    RatioText = "2:1";
                    HarborToolTip.header = "Wheat Port <sprite name=wheat>";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 2 <sprite name=wheat> for 1 of any resource.";
                    break;
                case HarborResourceType.Any:
                    harborColor = Color.white;
                    RatioText = "3:1";
                    HarborToolTip.header = "General Port";
                    HarborToolTip.text = "Building a settlement here will allow you to trade 3 of ANY resource for 1 of any other resource \n\n(doesn't affect better trade ratios from other ports).";
                    break;

            }

            HarborSprite.color = harborColor;
            RatioTextComp.text = RatioText; 

        }
    }


    public void PlaceInitalSettelments()
    {
        if(initialSettlements == null) { return; }


        foreach(var settelment in initialSettlements)
        {

            
            BoardManager.instance.BuildSettlementAt(settelment.position, true);
            

            if(settelment.isCity == true)
            {
                BoardManager.instance.CornersDic.TryGetValue(settelment.position, out CornersClass town);
                BoardManager.instance.UpgradeSettelmentToCity(town, true);
            }
            

            


        }
    }

    public void PlaceInitalRoads()
    {
        if (initialRoads == null) { return; }


        foreach (var road in initialRoads)
        {


            BoardManager.instance.BuildRoadAt(road, true);

        }
    }


    public void PlaceAndRemoveFogTiles()
    {
        var dic = BoardManager.instance.TilesDictionary;

        foreach (var tile in dic.Values)
        {
            if(tile.underFog == true)
            {
                FogTileMap.SetTile(tile.TilePostion, fogTile);
            }
            else
            {
                FogTileMap.SetTile(tile.TilePostion, null);
            }
        }


    }


    private void PlaceResourceObjects()
    {
        foreach (var tile in InitialTilesDictionary.Values)
        {
            switch (tile.resourceType)
            {
                case ResourceType.Wood:
                    
                    break;
                case ResourceType.Brick:
                    ObjectsTileMap.SetTile(tile.TilePostion, rumTileObject);
                    break;
                case ResourceType.Sheep:
                    ObjectsTileMap.SetTile(tile.TilePostion, goldTileObject);
                    break;
                case ResourceType.Wheat:
                    break;
                case ResourceType.Ore:
                    break;
                case ResourceType.Desert:
                    break;

            }
        }
    }

}
