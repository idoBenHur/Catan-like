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
    public event Action OnResourcesChanged;
    public event Action <int> OnVictoryPointsChanged;
    public event Action OnTrade;
    public event Action OnHarborsGained;
    public int VictoryPoints;
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
            { ResourceType.Wood, 0 },
            { ResourceType.Brick, 0 },
            { ResourceType.Sheep, 0 },
            { ResourceType.Ore, 0 },
            { ResourceType.Wheat, 0 }
        };

        VictoryPoints = 0;
        TradeCount = 0;
        
    }

    public void AddResource(ResourceType type, int amount, Vector3 Source)
    {
        if (PlayerResources.ContainsKey(type))
        {
            PlayerResources[type] += amount;
            OnResourcesChanged?.Invoke();

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


    public void AddVictoryPoints(int VictoryPointsAmount, Vector3 SourcePosition)
    {
       VictoryPoints = VictoryPoints + VictoryPointsAmount;
        OnVictoryPointsChanged?.Invoke(VictoryPoints);

        for (int i = 0; i < VictoryPointsAmount; i++)
        {
            BoardManager.instance.uiManager.VictoryPointsAddedAnimation(SourcePosition);
        }


    }

    public void AddSettelment(CornersClass corner)
    {
        SettelmentsList.Add(corner);

        if(corner.Harbor != null)
        {
            OwnedHarbors.Add(corner.Harbor);
            OnHarborsGained?.Invoke();
            Debug.Log("get port");
        }
    }






}

