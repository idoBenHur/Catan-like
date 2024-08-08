using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTest : AbstractSkillSlot
{
    public override bool CanAcceptDice(NewNewDice dice)
    {
        // Example: This slot accepts any dice
        RequiredDiceCount = 1;
        return diceInSlot.Count < RequiredDiceCount;
    }

    protected override void OnDiceAdded(NewNewDice dice)
    {
        // Specific logic for adding dice to SplitDiceSlot
        Debug.Log("Dice added to SplitDiceSlot.");
    }

    public override void ActivateSlotEffect()
    {
        // Implement the effect for splitting the dice
        if (diceInSlot.Count > 0)
        {
           // NewNewDice dice = diceInSlot[0];
            // Split dice logic
            Debug.Log("Split Dice Slot Activated: Splitting dice ");
        }
    }
}
