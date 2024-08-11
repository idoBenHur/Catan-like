using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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
        Played66Xtimes, // speciel dice boon
        PlayedXDices // speciel dice boon

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
        GainRandomResources,
        TransformWoodTilesToStoneTiles,
        GetRandomRoad,
        TransformTownsNearDesertsToCities,
        ChangeTradeRatioXToOne,
        AddBrick,
        AddSheep,
        AddOre,
        AddWheat,
        TransformWheatTilesToDesertTiles,
        TransformToSixAndTwo,
        IncreaseUnluckyMeter,
        TransformAllTilesToNumber,
        SkipTurns,
        AddTemporaryDice, // special dice effect
        AddPermanentDice, // special dice effect
        MultipleRandomTransforms,
        TransformRandomTileToX



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
        OnResourceChange,
        OnDicePlayed

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
                case BoonTrigger.TriggerType.OnDicePlayed:
                     BoardManager.OnDicePlayed += GoThroughConditionsList;                   
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
                case BoonTrigger.TriggerType.OnDicePlayed:
                    BoardManager.OnDicePlayed -= GoThroughConditionsList;
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
                conditionMet = false;
                condition.value2++;
                
                int currentTrades = condition.value2;
                if((currentTrades % (condition.value1) == 0 && currentTrades != 0) == true) { conditionMet = true; condition.value2 = 0; }

                BoardManager.instance.uiManager.UpdateBoonCounter(this, condition.value2);
                return conditionMet;

            case BoonCondition.ConditionType.TownNextToDesert: // there is a town next to a desert
                bool adjacentDesert = false;
                foreach (var settelment in BoardManager.instance.player.SettelmentsList)
                {       
                    if(settelment.HasCityUpgade == true) { continue; }
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
            case BoonCondition.ConditionType.Played66Xtimes:
                conditionIsMet = false;
                if (BoardManager.instance.Dice2FinalSide == 6 && BoardManager.instance.Dice1FinalSide == 6)
                {
                    condition.value2++;
                }

                if (condition.value2 % condition.value1 == 0 && condition.value2 != 0)
                {
                    conditionIsMet = true;
                    condition.value2 = 0;
                }
                BoardManager.instance.uiManager.UpdateBoonCounter(this, condition.value2);
                return conditionIsMet;

            case BoonCondition.ConditionType.PlayedXDices: // count the amount of times the trigger was met

                int DicesPlayedInThisTurn = BoardManager.instance.PlayedAmountInTurn;
                conditionMet = false;

                if (DicesPlayedInThisTurn % condition.value1 == 0 && DicesPlayedInThisTurn != 0)
                {
                    conditionMet = true;

                }

                return conditionMet;










        }

        return false;
    }
    


    private void ApplyEffect(BoonEffect effect)
    {
        Vector3 sourcePosition = Vector3.zero;
        switch (effect.type)
        {
            case BoonEffect.EffectType.AddWood: // gain wood
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wood, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.GainRandomResources: // gain ranodm resources

                for (int i = 0; i < effect.value1; i++)
                {
                    var resources = new TileClass.ResourceType[] { TileClass.ResourceType.Wood, TileClass.ResourceType.Brick, TileClass.ResourceType.Sheep, TileClass.ResourceType.Ore, TileClass.ResourceType.Wheat };
                    int randomIndex = UnityEngine.Random.Range(1, 5);
                    var randomResource = resources[randomIndex];
                    sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
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

            case BoonEffect.EffectType.TransformTownsNearDesertsToCities: // Transform The last town Towns Near Deserts To Cities 

                var lastSettlement = BoardManager.instance.player.SettelmentsList[BoardManager.instance.player.SettelmentsList.Count - 1];

                foreach (var tile in lastSettlement.AdjacentTiles)
                {
                    if (tile.resourceType == TileClass.ResourceType.Desert) { BoardManager.instance.UpgradeSettelmentToCity(lastSettlement, true); }
                }
                
                //the code for making it work retro as well
                //foreach (var settelment in BoardManager.instance.player.SettelmentsList)
                //{
                //    if(settelment.HasCityUpgade == true) { continue; }
                //    foreach (var AdjacentTile in settelment.AdjacentTiles)
                //    {
                //        if (AdjacentTile.resourceType == TileClass.ResourceType.Desert) { BoardManager.instance.UpgradeSettelmentToCity(settelment, true); }
                //    }
                //}
                break;

            case BoonEffect.EffectType.ChangeTradeRatioXToOne: // change the trade ratio, (cannot be change by ports) 
                BoardManager.instance.player.TradeModifier = effect.value1;
                break;

            case BoonEffect.EffectType.AddBrick: // gain Brick
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Brick, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.AddSheep: // gain Sheep
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Sheep, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.AddOre: // gain ore
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Ore, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.AddWheat: // gain wheat
                sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplayDic[this].transform.position;
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wheat, effect.value1, sourcePosition);
                break;
            case BoonEffect.EffectType.TransformWheatTilesToDesertTiles: // transform wheat tile to desert tiles
                foreach(var tile in BoardManager.instance.TilesDictionary)
                {
                   if(tile.Value.resourceType == TileClass.ResourceType.Wheat)
                    {
                        tile.Value.resourceType = TileClass.ResourceType.Desert;
                        tile.Value.numberToken = 7;
                        Destroy(tile.Value.MyNumberPrefab);
                        BoardManager.instance.mapGenerator.UpdateTileTypeVisual(BoardManager.instance.TilesDictionary);
                    }                   
                }
                break;
            case BoonEffect.EffectType.TransformToSixAndTwo: // transform brick and wood to 2, and ore and wheat to 6
                foreach (var tile in BoardManager.instance.TilesDictionary)
                {
                    if (tile.Value.resourceType == TileClass.ResourceType.Wheat || tile.Value.resourceType == TileClass.ResourceType.Ore)
                    {
                        tile.Value.ChangeTileNumber(6);
                    }
                    else if(tile.Value.resourceType == TileClass.ResourceType.Wood || tile.Value.resourceType == TileClass.ResourceType.Brick)
                    {
                        tile.Value.ChangeTileNumber(2);
                    }

                }
                    break;
            case BoonEffect.EffectType.IncreaseUnluckyMeter: // increase the max size of the unlucky meter by X
                BoardManager.instance.IncreaseUnluckyMeter(effect.value1);              
                break;
            case BoonEffect.EffectType.TransformAllTilesToNumber: // transform all tiles numbers to a single number
                foreach (var tile in BoardManager.instance.TilesDictionary)
                {
                    tile.Value.ChangeTileNumber(effect.value1);
                }                                          
                break;
            case BoonEffect.EffectType.SkipTurns:
                BoardManager.instance.CurrentTurn += effect.value1;
                BoardManager.instance.uiManager.UpdateTurnSliderDisplay();
                
                break;
            case BoonEffect.EffectType.AddTemporaryDice: // adds a dice this turn, (will not add to the amount of dice player roll each turn)

               // BoardManager.instance.gameObject.GetComponent<SelectDiceBox>().AddTemporerayDice();

                break;
            case BoonEffect.EffectType.AddPermanentDice:

                BoardManager.instance.gameObject.GetComponent<SelectDiceBox>().AmountOfNewDiceEachRoll++;
                break;
            case BoonEffect.EffectType.MultipleRandomTransforms: // change 3 random tiles -  to desert, 8 and 6

                var tilesValuesList = new List<TileClass>(BoardManager.instance.TilesDictionary.Values);
                List<TileClass> shuffelList = tilesValuesList.OrderBy(x => Random.value).ToList();

                foreach(var tile in shuffelList)
                {
                    if(tile.resourceType != TileClass.ResourceType.Desert && tile.hasRobber == false)
                    {
                        tile.resourceType = TileClass.ResourceType.Desert;
                        tile.numberToken = 7;
                        Destroy(tile.MyNumberPrefab);
                        break;
                    }
                }

                foreach(var tile in shuffelList)
                {
                    if (tile.numberToken != 8 && tile.numberToken != 6 && tile.resourceType != TileClass.ResourceType.Desert && tile.hasRobber == false)
                    {
                        tile.ChangeTileNumber(8);
                        break;
                    }
                }

                foreach (var tile in shuffelList)
                {
                    if (tile.numberToken != 8 && tile.numberToken != 6 && tile.resourceType != TileClass.ResourceType.Desert && tile.hasRobber == false)
                    {
                        tile.ChangeTileNumber(6);
                        break;
                    }
                }

                BoardManager.instance.mapGenerator.UpdateTileTypeVisual(BoardManager.instance.TilesDictionary);

                break;
            case BoonEffect.EffectType.TransformRandomTileToX: // transfoarm random tile number to X number

                var TempList = new List<TileClass>(BoardManager.instance.TilesDictionary.Values);
                List<TileClass> shuffledList = TempList.OrderBy(x => Random.value).ToList(); //shuffle list

                foreach(var tile in shuffledList)
                {
                    if(tile.numberToken != effect.value1 && tile.hasRobber == false)
                    {
                        tile.ChangeTileNumber(effect.value1);
                        break;
                    }
                }

                break;      

        }


        BoardManager.instance.uiManager.ShakeBoonDisplayAnimation(this);

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


