using System.Collections.Generic;
using UnityEngine;

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
        AmountOfTowns


    }

    public ConditionType type;
    public int value; 
}


[System.Serializable]
public class BoonEffect
{
    public enum EffectType
    {
        AddWood,
        GainVictoryPoints,
        GainRandomResources,
    }

    public EffectType type;
    public int value; 
}


[System.Serializable]
public class BoonTrigger
{
    public enum TriggerType
    {
        OnDiceRoll,
        OnTrade,
        
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

        //triggers:

        foreach (var trigger in triggers)
        {
            switch (trigger.type)
            {
                case BoonTrigger.TriggerType.OnDiceRoll: // dice roll trigger
                    BoardManager.OnDiceRolled += GoThroughConditionsList; 
                    break;
                case BoonTrigger.TriggerType.OnTrade:
                    PlayerClass player = BoardManager.instance.player;
                    player.OnTrade += GoThroughConditionsList;
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
                    player.OnTrade += GoThroughConditionsList;
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
                return BoardManager.instance.TotalDice == condition.value;

            case BoonCondition.ConditionType.ResourceCountLessThan: // less the X resources in hand
                int resourceCount = 0;
                foreach(var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount += resource.Value;
                }
                return resourceCount < condition.value;

            case BoonCondition.ConditionType.AfterXAmountOfTrades: // every X trades
                int currentTrades = BoardManager.instance.player.TradeCount;
                return (currentTrades % condition.value == 0);

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
                return (BoardManager.instance.player.OwnedHarbors.Count >= condition.value);

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
                return condition.value >= townsAmount;  
                






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
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wood, effect.value, sourcePosition);
                break;
            case BoonEffect.EffectType.GainVictoryPoints:
                BoardManager.instance.player.AddVictoryPoints(effect.value);
                break;
            case BoonEffect.EffectType.GainRandomResources:

                for (int i = 0; i < effect.value; i++)
                {
                    var resources = new TileClass.ResourceType[] { TileClass.ResourceType.Wood, TileClass.ResourceType.Brick, TileClass.ResourceType.Sheep, TileClass.ResourceType.Ore, TileClass.ResourceType.Wheat };
                    int randomIndex = UnityEngine.Random.Range(1, 5);
                    var randomResource = resources[randomIndex];
                    sourcePosition = BoardManager.instance.uiManager.BoonIconsDisplay[this].transform.position;
                    BoardManager.instance.player.AddResource(randomResource, 1 , sourcePosition);   
                }
                break;





                // Add more cases as needed for other effect types
        }
    }
}


