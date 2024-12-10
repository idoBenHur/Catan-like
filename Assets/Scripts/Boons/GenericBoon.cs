using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;


[System.Serializable]
public class BoonCondition
{
    public enum ConditionType
    {
        DicePlayedEquals,
        ResourceCount,
        AfterXAmountOfTrades,
        TownNextToDesert,
        AmountOfHarbors,
        MoreSettelmentsThenResources,
        AmountOfTowns,
        PlayedDouble, 
        DeseretSurrondedByRoad,
        AmounOfRoadsBuilt,
        Played66Xtimes, //  dice boon
        PlayedXDices, //  dice boon
        PlayedNumberX,
        PlayedEvenOrOdd,
        RolledTriple

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
        AddWood = 0,
        GainRandomResources = 1,
        TransformWoodTilesToStoneTiles = 2,
        GetRandomRoad= 3,
        TransformTownsNearDesertsToCities =4,
        ChangeTradeRatioXToOne = 5,
        AddBrick = 6 ,
        AddSheep = 7,
        AddOre = 8,
        AddWheat = 9,
        TransformWheatTilesToDesertTiles = 10,
        TransformToSixAndTwo = 11,
        TransformAllTilesToNumber = 13,
        SkipTurns = 14,
        AddTemporaryDice = 15, // special dice effect
        AddPermanentDice = 16, // special dice effect
        MultipleRandomTransforms = 17,
        TransformRandomTileToX = 18,
        Increase7RewardAmount = 19,
        RemoveRandomFog = 20,
        TransformRandomTownToCity



        //
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
public enum Or_And_Condition_type
{
    AndCondition, // all condition must be true
    OrCondition, // at least on condition must be true

}




[CreateAssetMenu(fileName = "New Generic Boon", menuName = "Boons/GenericBoon")]
public class GenericBoon : ScriptableObject
{
    public string boonName;
    [TextArea(3, 10)] public string description;
    public Sprite boonImage;
    public Color boonColor = Color.white;  // Default color is white





    //public List<BoonTrigger> triggers = new List<BoonTrigger>();
    public List<BoonCondition> conditions;
    public List<BoonEffect> effects = new List<BoonEffect>();
    public Or_And_Condition_type Or_And_Condition = Or_And_Condition_type.AndCondition; // default as and
    public bool isCounting = false;




   





    public void Activate2()
    {

        if(CheckAllConditions() == true)
        {
            foreach (var effect in effects)
            {
                ApplyEffect(effect);
            }
        }

    }




    public bool CheckAllConditions() // Checks all conditions of boon, refers to the "type" condition (at least 1 true / all must be true)
    {

        if (Or_And_Condition == Or_And_Condition_type.AndCondition)
        {
            return conditions.All(IsConditionMet);
        }
        else if (Or_And_Condition == Or_And_Condition_type.OrCondition)
        {
            return conditions.Any(IsConditionMet);
        }


        return false;
    }





    private bool IsConditionMet(BoonCondition condition)
    {


        switch (condition.type)
        {
            case BoonCondition.ConditionType.DicePlayedEquals: // dice = X
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

            case BoonCondition.ConditionType.PlayedDouble: // Rolled Double
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
            case BoonCondition.ConditionType.Played66Xtimes: // every X times you play double 6 
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

            case BoonCondition.ConditionType.PlayedXDices: // count the amount of times the dice were played

                int DicesPlayedInThisTurn = BoardManager.instance.PlayedAmountInTurn;
                conditionMet = false;
                condition.value2 = DicesPlayedInThisTurn;
                BoardManager.instance.uiManager.UpdateBoonCounter(this, condition.value2);

                if (DicesPlayedInThisTurn % condition.value1 == 0 && DicesPlayedInThisTurn != 0)
                {
                    conditionMet = true;

                }

                return conditionMet;
            case BoonCondition.ConditionType.PlayedNumberX: // played spesific number
                return BoardManager.instance.TotalDice == condition.value1;
            case BoonCondition.ConditionType.PlayedEvenOrOdd: // if bool true, contdition will be met if the played number is even. if bool falue, contdition will be met if the played number is odd;
                if (condition.boolValue == true)
                {
                    return BoardManager.instance.TotalDice % 2 == 0;
                }
                else return BoardManager.instance.TotalDice % 2 != 0;
            case BoonCondition.ConditionType.RolledTriple:

                var diceList = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox].DiceInSlotList;
                Dictionary<int, int> diceCount = new Dictionary<int, int>();

                foreach ( var die in diceList ) 
                {
                    
                    if (diceCount.ContainsKey(die.DieResult))
                    {
                        diceCount[die.DieResult]++;
                    }
                    else
                    {
                        diceCount[die.DieResult] = 1;
                    }

                    // Check if any dice value has occurred 3 or more times
                    if (diceCount[die.DieResult] >= 3)
                    {
                        return true;
                    }
                }

                return false;
















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

                DiceBox_Skill diceBox = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;

                for (int i = 0; i < effect.value1; i++)
                {
                    diceBox.SpawnADie();
                }

                break;
            case BoonEffect.EffectType.AddPermanentDice:


                diceBox = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;
                diceBox.AddPermaDie();
                diceBox.SpawnADie();


                break;
            case BoonEffect.EffectType.MultipleRandomTransforms: // change 3 random tiles -  to desert, 8 and 6

                var tilesValuesList = new List<TileClass>(BoardManager.instance.TilesDictionary.Values);
                List<TileClass> shuffelList = tilesValuesList.OrderBy(x => Random.value).ToList();

                foreach(var tile in shuffelList)
                {
                    if(tile.resourceType != TileClass.ResourceType.Desert && tile.isBlocked == false)
                    {
                        tile.resourceType = TileClass.ResourceType.Desert;
                        tile.numberToken = 7;
                        Destroy(tile.MyNumberPrefab);
                        break;
                    }
                }

                foreach(var tile in shuffelList)
                {
                    if (tile.numberToken != 8 && tile.numberToken != 6 && tile.resourceType != TileClass.ResourceType.Desert && tile.isBlocked == false)
                    {
                        tile.ChangeTileNumber(8);
                        break;
                    }
                }

                foreach (var tile in shuffelList)
                {
                    if (tile.numberToken != 8 && tile.numberToken != 6 && tile.resourceType != TileClass.ResourceType.Desert && tile.isBlocked == false)
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
                    if(tile.numberToken != effect.value1 && tile.isBlocked == false)
                    {
                        tile.ChangeTileNumber(effect.value1);
                        break;
                    }
                }

                break; 
            
            case BoonEffect.EffectType.Increase7RewardAmount:
                int currentAmount =  BoardManager.instance.uiManager.AmountToReward;
                BoardManager.instance.uiManager.AmountToReward = currentAmount * 2;

                break;
            case BoonEffect.EffectType.RemoveRandomFog:

                List<TileClass> tempTileList = new List<TileClass>(BoardManager.instance.TilesDictionary.Values);
                List<TileClass> validTiles = tempTileList.FindAll(tile => tile.underFog == true);

                if(validTiles.Count > 0)
                {
                    int randomIndex = Random.Range(0, validTiles.Count);
                    TileClass selectedTile = validTiles[randomIndex];                 
                    selectedTile.underFog = false;
                    BoardManager.instance.mapGenerator.PlaceAndRemoveFogTiles();
                }

                break;
            case BoonEffect.EffectType.TransformRandomTownToCity:
                List<CornersClass> TempBuildingList = new List<CornersClass>(BoardManager.instance.player.SettelmentsList);
                List<CornersClass> OnlySettelmentsList = TempBuildingList.FindAll(Settelments => Settelments.HasCityUpgade == false);

                if (OnlySettelmentsList.Count > 0)
                {
                    int randomIndex = Random.Range(0, OnlySettelmentsList.Count);
                    CornersClass selectedSettelment = OnlySettelmentsList[randomIndex];
                    BoardManager.instance.UpgradeSettelmentToCity(selectedSettelment, true);

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


