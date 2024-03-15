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



public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap; // Assign this in the inspector
    public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile; // Assign these in the inspector
    public GameObject NumberTokenPrefab;

    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };
    private Dictionary<Vector3, HexCornersClass> CornersDic = new Dictionary<Vector3, HexCornersClass>();
    public GameObject CornerIndicatorPrefab;
    public GameObject testprefab;
    public List<GameObject> IndicatorsPrefabList = new List<GameObject>();
    public Dictionary<Vector3, HexSidesClass> HexSidesDic = new Dictionary<Vector3, HexSidesClass>();




    // the holy resouce dic, (wood, brick, sheep, ore, wheat)
    Dictionary<int, List<int>> ResourcePerRollDic = new Dictionary<int, List<int>>
{
    {1, new List<int> {0, 0, 0, 0, 0}},
    {2, new List<int> {0, 0, 0, 0, 0}},
    {3, new List<int> {0, 0, 0, 0, 0}},
    {4, new List<int> {0, 0, 0, 0, 0}},
    {5, new List<int> {0, 0, 0, 0, 0}},
    {6, new List<int> {0, 0, 0, 0, 0}},
    {7, new List<int> {0, 0, 0, 0, 0}},
    {8, new List<int> {0, 0, 0, 0, 0}},
    {9, new List<int> {0, 0, 0, 0, 0}},
    {10, new List<int> {0, 0, 0, 0, 0}},
    {11, new List<int> {0, 0, 0, 0, 0}},
    {12, new List<int> {0, 0, 0, 0, 0}}
};

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
        if(instance == null)
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
        IntialMapResourcesShuffle();
        InitializeTiles();
        UpdateCornerData();
        UpdateVisualRepresentation();

        var elementAtIndex = CornersDic.ElementAt(7);
        var value = elementAtIndex.Value;
        var key = elementAtIndex.Key;
        value.HasSettlement = true;

        UpdateHexSidesData();
      //  ShowBuildIndicators();

        // ChangeResourcePerRollDic(ResourcePerRollDic, value);

        //Instantiate(randomprefab, key, Quaternion.identity);
        //UpdateNeighborsCanBeBuiltOn();










    }

    public void ShowBuildIndicators()
    {
    
        foreach (var corner in CornersDic.Values)
        {
            if (corner.CanBeBuiltOn)
            {
                GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                IndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<BuildIndicator>().Setup(corner.Position);
            }
        }
        return;
        
        

    }


    public void BuildSettlementAt(Vector3 cornerPosition)
    {
        if (CornersDic.TryGetValue(cornerPosition, out HexCornersClass corner) && corner.CanBeBuiltOn)
        {
            // Update game state to reflect the new settlement
            corner.CanBeBuiltOn = false;
            corner.HasSettlement = true;

            foreach (var NeighborCornerKey in corner.AdjacentCornerPositions)
            {
                CornersDic[NeighborCornerKey].CanBeBuiltOn = false;
            }

            foreach(var indicator in IndicatorsPrefabList)
            {
                Destroy(indicator.gameObject);
            }
            ShowBuildIndicators();
            ChangeResourcePerRollDic(ResourcePerRollDic, corner);



        }
    }






    void UpdateNeighborsCanBeBuiltOn()
    {
        foreach (var CornerClass in CornersDic.Values)
        {
            if( CornerClass.HasSettlement == true)
            {
               foreach(var NeighborCornerKey in CornerClass.AdjacentCornerPositions)
                {
                    CornersDic[NeighborCornerKey].CanBeBuiltOn = false;
                }
                
                

            }

        }
            
    }




    private void ChangeResourcePerRollDic(Dictionary<int, List<int>> dic, HexCornersClass Corner )
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

        foreach (KeyValuePair<int, List<int>> kvp in ResourcePerRollDic)
        {
            Debug.Log($"Key: {kvp.Key}, Values: {string.Join(", ", kvp.Value)}");
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

        //finds AdjacentTiles of a corner

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
                    CornersDic[cornerPos] = new HexCornersClass(cornerPos);
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
                    if (!currentCorner.AdjacentCornerPositions.Contains(possibleAdjacentCorner.Position))
                    {
                        currentCorner.AdjacentCornerPositions.Add(possibleAdjacentCorner.Position);
                    }

                    if (!possibleAdjacentCorner.AdjacentCornerPositions.Contains(currentCorner.Position))
                    {
                        possibleAdjacentCorner.AdjacentCornerPositions.Add(currentCorner.Position);
                    }
                }
            }
        }

    }


    private void UpdateHexSidesData()
    {
        foreach (var tilePair in TilesDictionary)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;
            Vector3 worldPosition = tilemap.CellToWorld(position);

            var sidePositions = GetSidesPositionsForTile(worldPosition);

            foreach (var hexSide in sidePositions)
            {
                if (!HexSidesDic.ContainsKey(hexSide.Position))
                {
                    HexSidesDic[hexSide.Position] = new HexSidesClass(hexSide.Position, hexSide.RotationZ);

                }

                HexSidesDic[hexSide.Position].AdjacentTiles.Add(tile);
            } 
        }


        //if 2 "roads"/sides have the same adjacent tile, check thier distance. if they close, they are adjacent

        foreach (var currentSide in HexSidesDic.Values)
        {
            foreach (var possibleAdjacentSide in HexSidesDic.Values)
            {
                // Avoid comparing a corner with itself
                if (currentSide == possibleAdjacentSide) continue;

                // Find shared tiles between the two corners
                var sharedTiles = currentSide.AdjacentTiles.Intersect(possibleAdjacentSide.AdjacentTiles).ToList();
                
                float size = 1f;

                if (currentSide.AdjacentTiles.Intersect(possibleAdjacentSide.AdjacentTiles).Count() == 1)
                {
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
                }
                else
                {
                    continue;
                }




            }
        }



    }


    public List<HexSidesClass> GetSidesPositionsForTile(Vector3 HexCenterPosition)
    {
        List<HexSidesClass> sides = new List<HexSidesClass>();
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

            sides.Add(new HexSidesClass(RoundVector3(sideMidPointPos, 2), rotationZ));



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


