using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoonCondition
{
    public enum ConditionType
    {
        DiceRollEquals,
        ResourceCountLessThan,
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
        OnTurnStart,
        OnTurnEnd,
        // Add other trigger types as needed
    }

    public TriggerType type;
}


[CreateAssetMenu(fileName = "New Generic Boon", menuName = "Boons/GenericBoon")]
public class GenericBoon : ScriptableObject
{
    public string boonName;
    public string description;
    public List<BoonTrigger> triggers = new List<BoonTrigger>();
    public List<BoonCondition> conditions = new List<BoonCondition>();
    public List<BoonEffect> effects = new List<BoonEffect>();
    public Sprite boonImage;
    public Color boonColor = Color.white;  // Default color is white



    public void Activate()
    {
        Debug.Log("boon active");

        foreach (var trigger in triggers)
        {
            switch (trigger.type)
            {
                case BoonTrigger.TriggerType.OnDiceRoll:
                    BoardManager.OnDiceRolled += GoThroughConditionsList; // Assuming OnDiceRolled is an event you can subscribe to
                    break;
                case BoonTrigger.TriggerType.OnTurnStart:
                    break;
                case BoonTrigger.TriggerType.OnTurnEnd:
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
                case BoonTrigger.TriggerType.OnTurnStart:
                    break;
                case BoonTrigger.TriggerType.OnTurnEnd:
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
            case BoonCondition.ConditionType.DiceRollEquals:
                return BoardManager.instance.TotalDice == condition.value;
            case BoonCondition.ConditionType.ResourceCountLessThan:
                int resourceCount = 0;
                foreach(var resource in BoardManager.instance.player.PlayerResources)
                {
                    resourceCount += resource.Value;
                }
                return resourceCount < condition.value;
                // Add more cases as needed for other condition types
        }

        return false;
    }

 

    private void ApplyEffect(BoonEffect effect)
    {

        switch (effect.type)
        {
            case BoonEffect.EffectType.AddWood:
                BoardManager.instance.player.AddResource(TileClass.ResourceType.Wood, effect.value);
                break;
            case BoonEffect.EffectType.GainVictoryPoints:
                BoardManager.instance.player.AddVictoryPoints(effect.value);
                break;
                // Add more cases as needed for other effect types
        }
    }
}


