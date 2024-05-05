using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.Texture2DShaderProperty;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class BoonCondition
{
    public enum ConditionType
    {
        DiceRollEquals,
        ResourceCountLessThan,
        AfterXAmountOfTrades,
        CityNextToEmptyHexThatProvidedResourcesThisTurn,
        AmountOfHarbors,
        MoreSettelmentsThenResources,
        AmountOfTowns,
        RolledDouble,
        DeseretSurrondedByRoad
    }

    public ConditionType type;

    public int value1;
    private int defaultValue1;

    public int value2;
    private int defaultValue2;

    public BoonCondition()
    {
        

    }

    public void StoreDefaults()
    {
        defaultValue1 = value1;
        defaultValue2 = value2;
    }
    

    public void ResetConditionToDefaults()
    {
        // Reset values to defaults
        value1 = defaultValue1;
        value2 = defaultValue2;
    }


}


[System.Serializable]
public class BoonEffect
{
    public enum EffectType
    {
        AddWood,
        GainVictoryPoints,
        GainRandomResources,
        TransformWoodTilesToStoneTiles,
        GetRandomRoad

    }

    public EffectType type;

    public int value1; 
    private int defaultValue1;

    public void StoreDefaults()
    {
        defaultValue1 = value1;
        
    }
    public void ResetEffectToDefaults()
    {
        // Reset values to defaults
        value1 = defaultValue1;
    }

}


[System.Serializable]
public class BoonTrigger
{
    public enum TriggerType
    {
        OnDiceRoll,
        OnTrade,
        OnHarborGained,
        OnBoonActivate,
        OnRoadBuilt
        
        // Add other trigger types as needed
    }

    public TriggerType type;
}


[CreateAssetMenu(fileName = "New Generic Boon", menuName = "Boons/GenericBoon")]
public class GenericBoon : ScriptableObject
{
    public string boonName;
    [TextArea(3, 10)] public string description;
    public List<BoonTrigger> triggers = new List<BoonTrigger>();
    public List<BoonCondition> conditions;
    public List<BoonEffect> effects = new List<BoonEffect>();
    public Sprite boonImage;
    public Color boonColor = Color.white;  // Default color is white
    



    public void Activate()
    {
        PlayerClass player = BoardManager.instance.player;
        //triggers:

        foreach (var trigger in triggers)
        {
            switch (trigger.type)
            {
                case BoonTrigger.TriggerType.OnDiceRoll: // dice roll trigger
                    BoardManager.OnDiceRolled += GoThroughConditionsList; 
                    break;
                case BoonTrigger.TriggerType.OnTrade:
                    player.OnTrade += GoThroughConditionsList;                    
                    break;
                case BoonTrigger.TriggerType.OnHarborGained:                   
                    player.OnHarborsGained += GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnBoonActivate:
                    GoThroughConditionsList();
                    break;
                case BoonTrigger.TriggerType.OnRoadBuilt:
                    BoardManager.OnRoadBuilt += GoThroughConditionsList;
                    break;

                    // Add other cases as needed
            }
        }
    }

    public void Deactivate()
    {
        foreach (var trigger in triggers)
        {
            switch (trigger.type)
            {
                case BoonTrigger.TriggerType.OnDiceRoll:
                    BoardManager.OnDiceRolled -= GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnTrade:
                    PlayerClass player = BoardManager.instance.player;
                    player.OnTrade -= GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnHarborGained:
                    player = BoardManager.instance.player;
                    player.OnHarborsGained -= GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnRoadBuilt:
                    BoardManager.OnRoadBuilt -= GoThroughConditionsList;
                    break;

                    // Add other cases as needed
            }
        }


    }












    public void GoThroughConditionsList()
    {
        foreach (var condition in conditions)
        {
            if (IsConditionMet(condition) == false)
            {
                Debug.Log("conditions not met");
                return;
            }
            Debug.Log("conditions met");

        }

        foreach (var effect in effects)
        {
            ApplyEffect(effect);
            Debug.Log("apply effect");

        }

    }

    private bool IsConditionMet(BoonCondition condition)
    {

        switch (condition.type)
        {
            case BoonCondition.ConditionType.DiceRollEquals: // dice = X
                return BoardManager.instance.TotalDice == condition.value1;

            case BoonCondition.ConditionType.ResourceCountLessThan: // less the X resources in hand
                int resourceCount = 0;
                foreach(var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount += resource.Value;
                }
                return resourceCount < condition.value1;

            case BoonCondition.ConditionType.AfterXAmountOfTrades: // every X trades
                condition.value2++;
                int currentTrades = condition.value2;
                Debug.Log(currentTrades);
                return (currentTrades % (condition.value1) == 0 && currentTrades != 0);

            case BoonCondition.ConditionType.CityNextToEmptyHexThatProvidedResourcesThisTurn: // settelment next to empty tile
                int diceTotal = BoardManager.instance.TotalDice;
                bool hasEmptyAdjust = false;
                bool ProvidedThisTurn = false;

                foreach (var settelment in BoardManager.instance.player.SettelmentsList)
                {
                    if (settelment.HasCityUpgade == true)
                    {
                         foreach(var AdjustTile in settelment.AdjacentTiles)
                        {
                            if(AdjustTile.hasRobber == true ) { hasEmptyAdjust = true;}
                            if (AdjustTile.numberToken == diceTotal) { ProvidedThisTurn = true;}
                        }
                    } 
                }
                return (hasEmptyAdjust == true && ProvidedThisTurn == true);

            case BoonCondition.ConditionType.AmountOfHarbors:               
                return (BoardManager.instance.player.OwnedHarbors.Count >= condition.value1);

            case BoonCondition.ConditionType.MoreSettelmentsThenResources:
                resourceCount = 0;

                foreach (var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount = resourceCount + resource.Value;
                }    
                return (BoardManager.instance.player.SettelmentsList.Count >= resourceCount);

            case BoonCondition.ConditionType.AmountOfTowns:
                int townsAmount = 0;
                foreach (var town in BoardManager.instance.player.SettelmentsList)
                {
                    if( town.HasCityUpgade == false ) { townsAmount++; }
                }
                return condition.value1 >= townsAmount;
            case BoonCondition.ConditionType.RolledDouble:
                return (BoardManager.instance.Dice1FinalSide == BoardManager.instance.Dice2FinalSide);

            case BoonCondition.ConditionType.DeseretSurrondedByRoad:
                bool surrondedByRoads = false;
                bool applyEffect = false;
                //int desertCount = 0;
                int amountOfRoads = 0;
                
                
                foreach (var tile in BoardManager.instance.TilesDictionary)
                {
                    if (tile.Value.resourceType != TileClass.ResourceType.Desert) { continue; }                                
                    //desertCount++;
                    surrondedByRoads = false;
                    amountOfRoads = 0;
                    foreach (var side in tile.Value.AdjacentSides)
                    {
                        
                        if (side.HasRoad == true) { amountOfRoads++; }
                        if (amountOfRoads == 6) { surrondedByRoads = true; }
      
                    }                   
                    if (surrondedByRoads && BoardManager.instance.player.TilesSurrondedByRoadsList.Contains(tile.Value) == false) { BoardManager.instance.player.TilesSurrondedByRoadsList.Add(tile.Value); applyEffect = true; }                
                                  
                } 
                return (applyEffect);







                // Add more cases as needed for other condition types
        }

        return false;
    }

 

    private void ApplyEffect(BoonEffect effect)
    {
        Vector3 sourcePosition = Vector3.zero;
        switch (effect.type)
        {
            case BoonEffect.EffectType.AddWood:
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplay[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wood, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.GainVictoryPoints:
                BoardManager.instance.player.AddVictoryPoints(effect.value1);
                break;
            case BoonEffect.EffectType.GainRandomResources:

                for (int i = 0; i < effect.value1; i++)
                {
                    var resources = new TileClass.ResourceType[] { TileClass.ResourceType.Wood, TileClass.ResourceType.Brick, TileClass.ResourceType.Sheep, TileClass.ResourceType.Ore, TileClass.ResourceType.Wheat };
                    int randomIndex = UnityEngine.Random.Range(1, 5);
                    var randomResource = resources[randomIndex];
                    sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplay[this].transform.position;
                    BoardManager.instance.player.AddResource(randomResource, 1 , sourcePosition);   
                }
                break;
            case BoonEffect.EffectType.TransformWoodTilesToStoneTiles:
                var TempTileDic= new Dictionary<Vector3Int, TileClass>();
                foreach (var tile in BoardManager.instance.TilesDictionary)
                {
                    if(tile.Value.resourceType == TileClass.ResourceType.Wood)
                    {
                        tile.Value.resourceType = TileClass.ResourceType.Ore;
                        TempTileDic.Add(tile.Key,tile.Value);
                    }
                }
                BoardManager.instance.mapGenerator.UpdateTileTypeVisual(TempTileDic);
                break;
            case BoonEffect.EffectType.GetRandomRoad:
                var TempSidePosList = new List<Vector3>();
                foreach (var Side in BoardManager.instance.SidesDic)
                {
                    if (Side.Value.CanBeBuiltOn == true && Side.Value.HasRoad == false)
                    {
                        TempSidePosList.Add(Side.Key);
                    }
                }
                int RandomIndex = UnityEngine.Random.Range(0, TempSidePosList.Count);
                Vector3 RandomRoad = TempSidePosList[RandomIndex];
                BoardManager.instance.BuildRoadAt(RandomRoad);
                BoardManager.instance.mapGenerator.UpdateTownsAndRoadsVisual();
                BoardManager.instance.uiManager.CloseAllUi();
                break;







                // Add more cases as needed for other effect types
        }
    }



    public void StoreValues()
    {
        // Store default values
        foreach (var condition in conditions)
        {
            condition.StoreDefaults();
        }
        foreach (var effect in effects)
        {
            effect.StoreDefaults();
        }
    }

  

    public void ResetToDefaults()
    {
        foreach (var condition in conditions)
        {
            condition.ResetConditionToDefaults(); // Reset conditions

        }

        foreach (var effect in effects)
        {
            effect.ResetEffectToDefaults(); // Reset effects
        }
    }
}


