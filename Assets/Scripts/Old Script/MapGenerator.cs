using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile darkGreenTile;
    public Tile lightGreenTile;
    public Tile yellowTile;
    public Tile orangeTile;
    public Tile grayTile;
    public Tile darkYellowTile;
    public GameObject myprefab;

    private List<Vector3Int> tilePositions = new List<Vector3Int>();

    void Start()
    {
        // Get all the tile positions
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilePositions.Add(pos);
            }
        }

        // Shuffle the positions
        Shuffle(tilePositions);

        // Color the tiles based on the shuffled positions
        ColorTiles();
    }

    public void ReshuffleBoard()
    {
        // Clear the previous colors
        tilemap.ClearAllTiles();

        // Shuffle the positions again
        Shuffle(tilePositions);

        // Color the tiles again based on the shuffled positions
        ColorTiles();
    }

    void ColorTiles()
    {
        for (int i = 0; i < tilePositions.Count; i++)
        {
            Tile TileToSet = null;
            bool spawnPrefab = true;

            if (i < 4)
            {
                TileToSet = darkGreenTile;
            }
            else if (i < 8)
            {
                TileToSet = lightGreenTile;
            }
            else if (i < 12)
            {
                TileToSet = yellowTile;
            }
            else if (i < 15)
            {
                TileToSet = orangeTile;
            }
            else if (i < 18)
            {
                TileToSet = grayTile;
            }
            else if (i < 19)
            {
                TileToSet= darkYellowTile;
                spawnPrefab = false;
            }

            tilemap.SetTile(tilePositions[i], TileToSet);

            if (spawnPrefab == true)
            {
                Vector3 worldPosition = tilemap.CellToWorld(tilePositions[i]); // Convert tile position from local tilemap space to world space
                Instantiate(myprefab, worldPosition, Quaternion.identity);  // Instantiate the prefab at the tile's location
            }
        }
    }

    // Shuffle the list of positions
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }



}