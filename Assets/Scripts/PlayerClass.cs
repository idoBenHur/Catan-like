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
        if (CanAfford(cost))
        {
            foreach (var item in cost)
            {
                PlayerResources[item.Key] -= item.Value; // Assume Resources[item.Key] is guaranteed to exist and have enough quantity
            }
        }

        OnResourcesChanged?.Invoke();
    }




    public bool CanAfford(Dictionary<ResourceType, int> cost)
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


    // Check if the player has at least 4 resources of the offer resources type
    public bool CanTradeWithBank(ResourceType offerType)
    {
        return PlayerResources.ContainsKey(offerType) && PlayerResources[offerType] >= 4;

    }


    public void TradeWithBank(ResourceType offerType, ResourceType requestType)
    {
        if (CanTradeWithBank(offerType))
        {
            
            Dictionary<ResourceType, int> tempDictionary = new Dictionary<ResourceType, int>();
            tempDictionary.Add(offerType, 4);

            SubtractResources(tempDictionary);
            AddResource(requestType, 1);

        }
    }


    public void UpdayeVictoryPoints(int VictoryPointsAmount)
    {
       VictoryPoints = VictoryPoints + VictoryPointsAmount;
    }
}

