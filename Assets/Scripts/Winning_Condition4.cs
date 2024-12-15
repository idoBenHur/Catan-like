using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Winning_Condition4 : MonoBehaviour
{
    private int AmountOfFlagsToWin = 2;
    private int AmountOfFlagsToSpawn = 4;

    [SerializeField] private GameObject theFlag;
    private List<CornersClass> FlagsFound = new List<CornersClass>();





    void Start()
    {
        
        BoardManager.OnTownBuilt += BuiltOnFlag;
    }


    private void OnDestroy()
    {
        BoardManager.OnTownBuilt -= BuiltOnFlag;
    }



    public void initializeFlags(List<CornersClass> allcorrners)
    {
        // Filter corners where all adjacent tiles are under fog
        var eligibleCorners = allcorrners.Where(corner =>
            corner.AdjacentTiles.All(tile => tile.underFog == true)).ToList();

        if (eligibleCorners.Count < AmountOfFlagsToSpawn)
        {
            Debug.LogWarning("Not enough eligible corners to set the desired number of flags.");
            return;
        }

        // Track tiles that already have flags
        HashSet<TileClass> flaggedTiles = new HashSet<TileClass>();

        
        var randomCorners = eligibleCorners.OrderBy(c => Random.value);

        int flagsPlaced = 0;
        foreach (var corner in randomCorners)
        {
            
            if (corner.AdjacentTiles.Any(tile => flaggedTiles.Contains(tile)))
            {
                continue; // Skip this corner if already in the flaggedTiles hashset
            }

            
            corner.HaveAFlag = true;
            flagsPlaced++;

            // Add all adjacent tiles to the flaggedTiles set
            foreach (var tile in corner.AdjacentTiles)
            {
                flaggedTiles.Add(tile);
            }

            // Stop if we've placed enough flags
            if (flagsPlaced >= AmountOfFlagsToSpawn)
            {
                break;
            }
        }

        if (flagsPlaced < AmountOfFlagsToSpawn)
        {
            Debug.LogWarning($"Only {flagsPlaced} flags were placed due to tile constraints.");
        }
    }



    private void BuiltOnFlag()
    {
        int flagsFound = 0;

        foreach(var settelment in BoardManager.instance.player.SettelmentsList)
        {
            if(settelment.HaveAFlag == true)
            {
                flagsFound++;
            }
        }

        if (flagsFound == AmountOfFlagsToWin) 
        {
            BoardManager.instance.uiManager.EndGame(true);
        }
        
    }

    public void spawnFlagVisual(CornersClass corner)
    {
        if (FlagsFound.Contains(corner) == false)
        {
            Instantiate(theFlag, corner.Position, Quaternion.identity);
            FlagsFound.Add(corner);
        }

        
    }



}
