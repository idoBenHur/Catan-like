using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Boon", menuName = "Boons/GenericBoon")]
public class GenericBoon : ScriptableObject
{
    public string boonName;
    public string description;
    public List<BoonRule> rules = new List<BoonRule>();

    public virtual void Activate()
    {
        foreach (var rule in rules)
        {
            switch (rule.trigger)
            {
                case BoonTrigger.DiceRoll:
                    BoardManager.OnDiceRolled += (diceValue) => CheckAndApplyRule(diceValue, rule);
                    break;
                case BoonTrigger.TurnStart:
                    // Subscribe to a TurnStart event and apply rule
                    break;
                case BoonTrigger.TurnEnd:
                    // Subscribe to a TurnEnd event and apply rule
                    break;
                    // Add cases for other triggers
            }
        }
    }

    private void CheckAndApplyRule(int diceValue, BoonRule rule)
    {
        // Check the rule's condition
        switch (rule.condition)
        {
            case BoonCondition.DiceValueEquals:
                if (diceValue == rule.conditionValue)
                {
                    ApplyEffect(rule.effect, rule.effectValue);
                }
                break;
                // Add cases for other conditions
        }
    }



    private void ApplyEffect(BoonEffect effect, int effectValue)
    {
        switch (effect)
        {
            case BoonEffect.GrantVictoryPoint:
                BoardManager.instance.player.AddVictoryPoints(effectValue);
                // Logic to grant victory points
                break;
            case BoonEffect.GrantResources:
                // Logic to grant resources
                break;
                // Add cases for other effects
        }
    }




    public virtual void Deactivate()
    {
        // Logic to deactivate the boon
    }
}

public enum BoonTrigger
{
    DiceRoll,
    TurnStart,
    TurnEnd,
    // Additional triggers as needed
}

public enum BoonCondition
{
    DiceValueEquals,
    NoCities,
    ResourceCountLessThan,
    // Additional conditions as needed
}

public enum BoonEffect
{
    GrantVictoryPoint,
    GrantResources,
    ModifyDiceRoll,
    // Additional effects as needed
}

[System.Serializable]
public struct BoonRule
{
    public BoonTrigger trigger;
    public BoonCondition condition;
    public int conditionValue;
    public BoonEffect effect;
    public int effectValue;
}
