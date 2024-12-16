using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileClass;

public class Winning_condition3 : MonoBehaviour
{
    private List<TileClass> AllTiles; // List of all tiles on the map
    public int TotalResourcesToWin = 20; // Total resources player must collect
    public List<ResourceType> TheWinningResourceTypesList; // The 3 resource types to include

    private Dictionary<int, float> DICEprobabilities; // Probabilities for dice rolls
    private Dictionary<ResourceType, int> FinalWinningGoals; // Goals for each resource

    private void Start()
    {

    }


    public void setup(Dictionary<Vector3Int, TileClass> dic)
    {
        AllTiles = dic.Values.ToList();//BoardManager.instance.mapGenerator.InitialTilesDictionary.Values.ToList();
        InitializeDiceProbabilities();
        DetermineWinningGoals();
        PrintWinningConditions();
    }

    //Initialize reguular dice roll probabilities (2-12)
    private void InitializeDiceProbabilities()
    {
        DICEprobabilities = new Dictionary<int, float>
        {
            { 2, 1f / 36f },
            { 3, 2f / 36f },
            { 4, 3f / 36f },
            { 5, 4f / 36f },
            { 6, 5f / 36f },
            { 7, 6f / 36f }, // Adjust if 7 triggers a different mechanic
            { 8, 5f / 36f },
            { 9, 4f / 36f },
            { 10, 3f / 36f },
            { 11, 2f / 36f },
            { 12, 1f / 36f }
        };
    }

    // Calculates and assigns the resource collection goals needed to win the game.
    // based on the selected resource types and their probabilities on the map.
    private void DetermineWinningGoals()
    {
        // Holds the probability of each resource type base on all tiles on the map.
        Dictionary<ResourceType, float> AllResourcesProb = new Dictionary<ResourceType, float>();

        foreach (var tile in AllTiles)
        {
            if (tile.resourceType == ResourceType.Desert) continue;

            if (AllResourcesProb.ContainsKey(tile.resourceType) == false)
            {
                AllResourcesProb[tile.resourceType] = 0f;
            }
                

            AllResourcesProb[tile.resourceType] += DICEprobabilities[tile.numberToken];
        }




        // Filter for selected resource types
        var GoalResourcesProb = new Dictionary<ResourceType, float>();
        foreach (var resource in TheWinningResourceTypesList)
        {
            if (AllResourcesProb.ContainsKey(resource))
            {
                GoalResourcesProb[resource] = AllResourcesProb[resource];
            }
        }

        // Calculate total weight of selected resources
        float totalWeight = GoalResourcesProb.Values.Sum();

        // Distribute total resources proportionally
        FinalWinningGoals = new Dictionary<ResourceType, int>();
        foreach (var resource in GoalResourcesProb)
        {
            FinalWinningGoals[resource.Key] = Mathf.RoundToInt((resource.Value / totalWeight) * TotalResourcesToWin);
        }
    }

    private void PrintWinningConditions()
    {
        Debug.Log("Winning Conditions:");
        Debug.Log(FinalWinningGoals.Count); 
        foreach (var goal in FinalWinningGoals)
        {
            Debug.Log($"- Collect {goal.Value} of {goal.Key}");
        }
    }
}
