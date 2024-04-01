using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileClass;

public class PlayerClass
{
    public Dictionary<ResourceType, int> PlayerResources { get; private set; }
    public event Action OnResourcesChanged;  // Event to notify when resources change
    public int VictoryPoints;
    public List<CornersClass> SettelmentsList { get; set; } = new List<CornersClass>();


    public PlayerClass()
    {
        PlayerResources = new Dictionary<ResourceType, int>
        {
            { ResourceType.Wood, 0 },
            { ResourceType.Brick, 0 },
            { ResourceType.Sheep, 0 },
            { ResourceType.Ore, 0 },
            { ResourceType.Wheat, 0 }
        };

        VictoryPoints = 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (PlayerResources.ContainsKey(type))
        {
            PlayerResources[type] += amount;
        }
        else
        {
            PlayerResources[type] = amount;
        }

        OnResourcesChanged?.Invoke();
    }

    public void SubtractResources(Dictionary<ResourceType, int> cost)
    {
        if (CanAffordToBuild(cost))
        {
            foreach (var item in cost)
            {
                PlayerResources[item.Key] -= item.Value; // Assume Resources[item.Key] is guaranteed to exist and have enough quantity
            }
        }

        OnResourcesChanged?.Invoke();
    }



    // the function gets a building cost, and checks if player has the right resources 
    public bool CanAffordToBuild(Dictionary<ResourceType, int> cost)
    {
        foreach (var item in cost)
        {
            if (!PlayerResources.ContainsKey(item.Key) || PlayerResources[item.Key] < item.Value)
            {
                return false; // Not enough of one of the required resources
            }
        }
        return true; // Enough resources to afford the cost
    }


 


    public void TradeWithBank(ResourceType offerType, ResourceType requestType)
    {
             
        Dictionary<ResourceType, int> tempDictionary = new Dictionary<ResourceType, int>();
        tempDictionary.Add(offerType, 4);

        SubtractResources(tempDictionary);
        AddResource(requestType, 1);

        
    }


    public void AddVictoryPoints(int VictoryPointsAmount)
    {
       VictoryPoints = VictoryPoints + VictoryPointsAmount;
    }






}

