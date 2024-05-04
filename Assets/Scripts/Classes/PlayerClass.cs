using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileClass;

public class PlayerClass
{
    public Dictionary<ResourceType, int> PlayerResources { get; private set; }
    //public event Action OnResourcesChanged;  // Event to notify when resources change
    public event Action <int> OnVictoryPointsChanged;
    public event Action OnTrade;
    public int VictoryPoints;
    public int TradeCount;
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
            //OnResourcesChanged?.Invoke();

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
       // OnResourcesChanged?.Invoke();
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

        int requiredAmount = 4;  // Default trading ratio without harbor


        foreach (var harbor in OwnedHarbors) 
        {
            if(offerType.ToString() == harbor.TradeResource.ToString())
            {
                requiredAmount = harbor.TradeRatio;
                break;
            }
            else if(harbor.TradeResource == HarborResourceType.Any)
            {
                requiredAmount = harbor.TradeRatio;
            }
            
        }

        tempDictionary.Add(offerType, requiredAmount);

        SubtractResources(tempDictionary);
        var sourcePosition = BoardManager.instance.uiManager.TradePannel.transform.position;
        AddResource(requestType, 1, sourcePosition);
        TradeCount++;
        OnTrade?.Invoke();




    }


    public void AddVictoryPoints(int VictoryPointsAmount)
    {
       VictoryPoints = VictoryPoints + VictoryPointsAmount;
        OnVictoryPointsChanged?.Invoke(VictoryPoints);
       
    }

    public void AddSettelment(CornersClass corner)
    {
        SettelmentsList.Add(corner);

        if(corner.Harbor != null)
        {
            OwnedHarbors.Add(corner.Harbor);
            Debug.Log("get port");
        }
    }






}

