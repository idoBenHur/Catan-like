using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileClass;

public class Winning_condition3 : MonoBehaviour
{
    private List<TileClass> AllTiles; // List of all tiles on the map
    public int TotalResourcesToWin = 20; // Total resources player must collect
    public List<ResourceType> SelectedResourceTypes; // The 3 resource types to include

    private Dictionary<int, float> diceProbabilities; // Probabilities for dice rolls
    private Dictionary<ResourceType, int> winningGoals; // Goals for each resource

    private void Start()
    {

    }


    public void setup(Dictionary<Vector3Int, TileClass> dic)
    {
        AllTiles = dic.Values.ToList();//BoardManager.instance.mapGenerator.InitialTilesDictionary.Values.ToList();
        InitializeDiceProbabilities();
        SetWinningConditions();
      //  PrintWinningConditions();
    }

    // Step 1: Initialize dice roll probabilities (2-12)
    private void InitializeDiceProbabilities()
    {
        diceProbabilities = new Dictionary<int, float>
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

    // Step 2: Calculate absolute probabilities and set winning conditions
    private void SetWinningConditions()
    {
        // Calculate absolute probabilities
        Dictionary<ResourceType, float> resourceProbabilities = new Dictionary<ResourceType, float>();

        foreach (var tile in AllTiles)
        {
            if (tile.resourceType == ResourceType.Desert) continue;

            if (!resourceProbabilities.ContainsKey(tile.resourceType))
                resourceProbabilities[tile.resourceType] = 0f;

            resourceProbabilities[tile.resourceType] += diceProbabilities[tile.numberToken];
        }

        // Filter for selected resource types
        var selectedProbabilities = new Dictionary<ResourceType, float>();
        foreach (var resource in SelectedResourceTypes)
        {
            if (resourceProbabilities.ContainsKey(resource))
            {
                selectedProbabilities[resource] = resourceProbabilities[resource];
            }
        }

        // Calculate total weight of selected resources
        float totalWeight = selectedProbabilities.Values.Sum();

        // Distribute total resources proportionally
        winningGoals = new Dictionary<ResourceType, int>();
        foreach (var resource in selectedProbabilities)
        {
            winningGoals[resource.Key] = Mathf.RoundToInt((resource.Value / totalWeight) * TotalResourcesToWin);
        }
    }

    private void PrintWinningConditions()
    {
        Debug.Log("Winning Conditions:");
        Debug.Log(winningGoals.Count); 
        foreach (var goal in winningGoals)
        {
            Debug.Log("hi");
            Debug.Log($"- Collect {goal.Value} of {goal.Key}");
        }
    }
}
