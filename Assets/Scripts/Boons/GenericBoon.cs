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
        ResourceCount,
        AfterXAmountOfTrades,
        TownNextToDesert,
        AmountOfHarbors,
        MoreSettelmentsThenResources,
        AmountOfTowns,
        RolledDouble,
        DeseretSurrondedByRoad,
        AmounOfRoadsBuilt,
        
    }

    public ConditionType type;

    public int value1;
    private int defaultValue1;

    public int value2;
    private int defaultValue2;

    public bool boolValue;
    private bool defaultBoolValue;



    public void StoreDefaults()
    {
        defaultValue1 = value1;
        defaultValue2 = value2;
        defaultBoolValue = boolValue;
    }
    

    public void ResetConditionToDefaults()
    {
        // Reset values to defaults
        value1 = defaultValue1;
        value2 = defaultValue2;
        boolValue = defaultBoolValue;
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
        GetRandomRoad,
        TransformTownsNearDesertsToCities,
        Trade1To1IfXAmountOfResources

    }

    public EffectType type;

    public int value1; 
    private int defaultValue1;

    public bool boolValue;
    private bool defaultBoolValue;

    public void StoreDefaults()
    {
        defaultValue1 = value1;
        defaultBoolValue = boolValue;


    }
    public void ResetEffectToDefaults()
    {
        value1 = defaultValue1;
        boolValue = defaultBoolValue;
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
        OnRoadBuilt,
        OnTownBuilt,
        OnResourceChange

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
    public bool isCounting = false;
    



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
                case BoonTrigger.TriggerType.OnTownBuilt:
                    BoardManager.OnTownBuilt += GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnResourceChange:
                    player.OnResourcesChanged += GoThroughConditionsList;
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
                case BoonTrigger.TriggerType.OnTownBuilt:
                    BoardManager.OnTownBuilt -= GoThroughConditionsList;
                    break;
                case BoonTrigger.TriggerType.OnResourceChange:
                    BoardManager.instance.player.OnResourcesChanged -= GoThroughConditionsList;
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

            case BoonCondition.ConditionType.ResourceCount: // if boolValue == false, return true if resources are less then X. if boolValue == true  return ture if resources are more then X
                int resourceCount = 0;
                bool conditionMet = false;
                foreach(var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount += resource.Value;
                }
                if(condition.boolValue == false) { conditionMet = resourceCount < condition.value1; } // less then
                if(condition.boolValue == true) { conditionMet = resourceCount > condition.value1; } // more then

                return conditionMet;

            case BoonCondition.ConditionType.AfterXAmountOfTrades: // every X trades
                condition.value2++;
                BoardManager.instance.uiManager.UpdateBoonCounter(this, condition.value2);
                int currentTrades = condition.value2;
                return (currentTrades % (condition.value1) == 0 && currentTrades != 0);

            case BoonCondition.ConditionType.TownNextToDesert: // there is a town next to a desert
                bool adjacentDesert = false;
                foreach (var settelment in BoardManager.instance.player.SettelmentsList)
                {                   
                    foreach(var AdjacentTile in settelment.AdjacentTiles)
                    {
                        if(AdjacentTile.resourceType == TileClass.ResourceType.Desert ) { adjacentDesert = true;}
                        
                    }                  
                }
                return (adjacentDesert);

            case BoonCondition.ConditionType.AmountOfHarbors: // X amount of harbors               
                return (BoardManager.instance.player.OwnedHarbors.Count >= condition.value1);

            case BoonCondition.ConditionType.MoreSettelmentsThenResources: // More Settelments Then Resources
                resourceCount = 0;
                 
                foreach (var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount = resourceCount + resource.Value;
                }    
                return (BoardManager.instance.player.SettelmentsList.Count >= resourceCount);

            case BoonCondition.ConditionType.AmountOfTowns: // X amount of towns 
                int townsAmount = 0;
                foreach (var town in BoardManager.instance.player.SettelmentsList)
                {
                    if( town.HasCityUpgade == false ) { townsAmount++; }
                }
                return condition.value1 >= townsAmount;

            case BoonCondition.ConditionType.RolledDouble: // Rolled Double
                return (BoardManager.instance.Dice1FinalSide == BoardManager.instance.Dice2FinalSide);

            case BoonCondition.ConditionType.DeseretSurrondedByRoad: // Deseret Surronded By Road
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

            case BoonCondition.ConditionType.AmounOfRoadsBuilt: // every X amount of Roads Built
                bool conditionIsMet = false;
                condition.value2++;
                if (condition.value2 % condition.value1 == 0 && condition.value2 != 0)
                {
                    conditionIsMet = true;
                    condition.value2 = 0;
                }
                BoardManager.instance.uiManager.UpdateBoonCounter(this, condition.value2);
                return (conditionIsMet);
                 







                
        }

        return false;
    }
    


    private void ApplyEffect(BoonEffect effect)
    {
        Vector3 sourcePosition = Vector3.zero;
        switch (effect.type)
        {
            case BoonEffect.EffectType.AddWood: // gain wood
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplay[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wood, effect.value1, sourcePosition);
                break;

            case BoonEffect.EffectType.GainVictoryPoints:
                BoardManager.instance.player.AddVictoryPoints(effect.value1);
                break;

            case BoonEffect.EffectType.GainRandomResources: // gain ranodm resources

                for (int i = 0; i < effect.value1; i++)
                {
                    var resources = new TileClass.ResourceType[] { TileClass.ResourceType.Wood, TileClass.ResourceType.Brick, TileClass.ResourceType.Sheep, TileClass.ResourceType.Ore, TileClass.ResourceType.Wheat };
                    int randomIndex = UnityEngine.Random.Range(1, 5);
                    var randomResource = resources[randomIndex];
                    sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplay[this].transform.position;
                    BoardManager.instance.player.AddResource(randomResource, 1 , sourcePosition);   
                }
                break;

            case BoonEffect.EffectType.TransformWoodTilesToStoneTiles: // transform wood tiles to stone tiles
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

            case BoonEffect.EffectType.GetRandomRoad: // gain random road
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
                BoardManager.instance.BuildRoadAt(RandomRoad,true);
                BoardManager.instance.mapGenerator.UpdateTownsAndRoadsVisual();
                BoardManager.instance.uiManager.CloseAllUi();
                break;

            case BoonEffect.EffectType.TransformTownsNearDesertsToCities: // Transform Towns Near Deserts To Cities

                foreach (var settelment in BoardManager.instance.player.SettelmentsList)
                {
                    if(settelment.HasCityUpgade == true) { continue; }
                    foreach (var AdjacentTile in settelment.AdjacentTiles)
                    {
                        if (AdjacentTile.resourceType == TileClass.ResourceType.Desert) { BoardManager.instance.UpgradeSettelmentToCity(settelment, true); }
                    }
                }
                break;

            case BoonEffect.EffectType.Trade1To1IfXAmountOfResources: // exceptional boon that contain the "condition" inside

                int resourceCount = 0;
                foreach (var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount += resource.Value;
                }
                if (resourceCount >= effect.value1) { BoardManager.instance.player.TradeModifier = 1; }
                else BoardManager.instance.player.TradeModifier = null;

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


