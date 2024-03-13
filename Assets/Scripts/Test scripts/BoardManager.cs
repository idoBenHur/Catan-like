using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap; // Assign this in the inspector
    public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile; // Assign these in the inspector
    public GameObject NumberTokenPrefab;

    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    private List<TileClass.ResourceType> ResourcesList = new List<TileClass.ResourceType>();
    private List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };


    void Start()
    {
        GenerateResourceList();
        InitializeTiles();
        UpdateVisualRepresentation();
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



    public void ChangeResourceType(TileClass.ResourceType fromType, TileClass.ResourceType toType)
    {
        foreach (KeyValuePair<Vector3Int, TileClass> tileEntry in TilesDictionary)
        {
            if (tileEntry.Value.resourceType == fromType)
            {
                tileEntry.Value.resourceType = toType;
            }
        }

        // Optionally, update the visual representation if needed
        UpdateVisualRepresentation();
    }


    public void test() 
    {
        ChangeResourceType(TileClass.ResourceType.Wood, TileClass.ResourceType.Brick);
    }
}
