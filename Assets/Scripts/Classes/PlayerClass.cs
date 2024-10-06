using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileClass;
using static Unity.VisualScripting.Member;

public class PlayerClass
{
    public Dictionary<ResourceType, int> PlayerResources { get; private set; }
    public Dictionary<ResourceType, int> ResourcesMaxStorage { get; private set; }
    public event Action OnResourcesChanged;
    public event Action OnTrade;
    public event Action OnHarborsGained;
    public int TradeCount; // used for an old boon, will modifiy the boon and will remove this value when i have the power to do it...
    public int? TradeModifier = null; //brute force change for trade ratio (used by boon) 
    public List<TileClass> TilesSurrondedByRoadsList = new List<TileClass>(); // used for an old boon, will modifiy the boon and will remove this value when i have the power to do it...
    public List<HarborClass> OwnedHarbors = new List<HarborClass>();
    public List<CornersClass> SettelmentsList { get; private set; } = new List<CornersClass>();
    public List<SidesClass> RoadsList { get; set; } = new List<SidesClass>();


    public PlayerClass()
    {
        PlayerResources = new Dictionary<ResourceType, int>
        {
            { ResourceType.Wood, 5 },
            { ResourceType.Brick, 5 },
            { ResourceType.Sheep, 5 },
            { ResourceType.Ore, 5 },
            { ResourceType.Wheat, 5 }
        };

        ResourcesMaxStorage = new Dictionary<ResourceType, int> // OLD!
        {
            { ResourceType.Wood, 1 },
            { ResourceType.Brick, 1 },
            { ResourceType.Sheep, 1 },
            { ResourceType.Ore, 1 },
            { ResourceType.Wheat, 1 }
        };



        TradeCount = 0;
        
    }

    public void AddResource(ResourceType type, int amount, Vector3 Source)
    {
        if (PlayerResources.ContainsKey(type))
        {
            PlayerResources[type] += amount;
            OnResourcesChanged?.Invoke();
            AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceCreate);

            for (int i = 0; i < amount; i++)
            {
                BoardManager.instance.uiManager.ResourceAddedAnimation(type, Source);
            }


        }
  

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

        BoardManager.instance.uiManager.UpdateResourceDisplay();
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



    public int CheckResourceAmount(ResourceType resource)
    {
        if (PlayerResources.ContainsKey(resource))
        {
            return PlayerResources[resource];
        }
        return 0; // If the resource doesn't exist, return 0
    }







    public void TradeWithBank(ResourceType offerType, ResourceType requestType)
    {
             
        Dictionary<ResourceType, int> tempDictionary = new Dictionary<ResourceType, int>();

        int MinRequiredAmount = 4;  // Default trading ratio without harbor
        
        foreach (var harbor in OwnedHarbors) 
        {
            if(offerType.ToString() == harbor.TradeResource.ToString())
            {
                MinRequiredAmount = harbor.TradeRatio;
                break;
            }
            else if(harbor.TradeResource == HarborResourceType.Any)
            {
                MinRequiredAmount = harbor.TradeRatio;
            }
            
        }

        if (TradeModifier != null) { MinRequiredAmount = TradeModifier.Value; }

        tempDictionary.Add(offerType, MinRequiredAmount);

        SubtractResources(tempDictionary);
        var sourcePosition = BoardManager.instance.uiManager.TradePannel.transform.position;
        AddResource(requestType, 1, sourcePosition);
        TradeCount++;
        OnTrade?.Invoke();




    }

    

    public void AddSettelment(CornersClass corner)
    {
        SettelmentsList.Add(corner);

        if(corner.Harbor != null)
        {
            OwnedHarbors.Add(corner.Harbor);
            OnHarborsGained?.Invoke();
        }
    }






}

