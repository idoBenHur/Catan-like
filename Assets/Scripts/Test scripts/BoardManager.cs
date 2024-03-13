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
    private List<TileClass.ResourceType> ResourcesList = new List<TileClass.ResourceType>();
    private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };
    private Dictionary<Vector3, HexCornersClass> CornersDic = new Dictionary<Vector3, HexCornersClass>();
    public GameObject randomprefab;



    // the holy resouce dic, (wood, brick, sheep, ore, wheat)
    Dictionary<int, List<int>> ResourceDic = new Dictionary<int, List<int>>
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


    void Start()
    {
        GenerateResourceList();
        InitializeTiles();
        UpdateCornerData();
        UpdateVisualRepresentation();

        var elementAtIndex = CornersDic.ElementAt(1);
        var value = elementAtIndex.Value;
        var key = elementAtIndex.Key;



        ChangeResoucesDic(ResourceDic, value);

        Instantiate(randomprefab, key, Quaternion.identity);

    }


    private void ChangeResoucesDic(Dictionary<int, List<int>> dic, HexCornersClass Corner )
    {
        foreach (var hex in Corner.AdjacentTiles)
        {
           int HexNumber = hex.numberToken;
           var ResourceType = hex.resourceType;


            switch (ResourceType)
            {
                case TileClass.ResourceType.Wood:
                    ResourceDic[HexNumber][0] += 1;
                    break;
                case TileClass.ResourceType.Brick:
                    ResourceDic[HexNumber][1] += 1;
                    break;
                case TileClass.ResourceType.Wheat:
                    ResourceDic[HexNumber][2] += 1;
                    break;
                case TileClass.ResourceType.Ore:
                    ResourceDic[HexNumber][3] += 1;
                    break;
                case TileClass.ResourceType.Sheep:
                    ResourceDic[HexNumber][4] += 1;
                    break;
                default:
                    break; 
            }
        }

        foreach (KeyValuePair<int, List<int>> kvp in ResourceDic)
        {
            Debug.Log($"Key: {kvp.Key}, Values: {string.Join(", ", kvp.Value)}");
        }

    }




    void GenerateResourceList()
    {
        ResourcesList.Clear();

        // Add the fixed amount of each resource type directly

        for (int i = 0; i < 4; i++) { ResourcesList.Add(TileClass.ResourceType.Sheep); }
        for (int i = 0; i < 4; i++) { ResourcesList.Add(TileClass.ResourceType.Wood); }
        for (int i = 0; i < 4; i++) { ResourcesList.Add(TileClass.ResourceType.Wheat); }
        for (int i = 0; i < 3; i++) { ResourcesList.Add(TileClass.ResourceType.Brick); }
        for (int i = 0; i < 3; i++) { ResourcesList.Add(TileClass.ResourceType.Ore); }
        ResourcesList.Add(TileClass.ResourceType.Desert);

        // Shuffle the list to randomize the board layout
        int n = ResourcesList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            var value = ResourcesList[k];
            ResourcesList[k] = ResourcesList[n];
            ResourcesList[n] = value;
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

                var resourceType = ResourcesList[ResourceIndex];

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


        foreach (var tilePair in TilesDictionary)
        {
            Vector3Int position = tilePair.Key;
            TileClass tile = tilePair.Value;

            var CornerPositions = GetCornerPositionsForTile(tile, position);

            foreach (var cornerPos in CornerPositions)
            {
                if (!CornersDic.ContainsKey(cornerPos))
                {
                    CornersDic[cornerPos] = new HexCornersClass(cornerPos);
                }

                CornersDic[cornerPos].AdjacentTiles.Add(tile);
            }

        }
    }

    public List<Vector3> GetCornerPositionsForTile(TileClass tile, Vector3 HexCenterPostion)
    {
        List<Vector3> corners = new List<Vector3>();
        float radius = 1.0f; // Radius of your hex tile, adjust based on your game's scale

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i + 30; // Offset by 30 degrees for pointy-topped hexes
            float angleRad = Mathf.Deg2Rad * angleDeg;
            Vector3 cornerPos = new Vector3(HexCenterPostion.x + radius * Mathf.Cos(angleRad), HexCenterPostion.y + radius * Mathf.Sin(angleRad), HexCenterPostion.z);
            corners.Add(cornerPos);
        }

        return corners;
    }


}
