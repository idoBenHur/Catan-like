using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBoon_Skill : AbstractSkillSlot
{
    private void Start()
    {
       // RequiredDiceCount = 6;
    }

    public override bool CanAcceptDice(NewNewDice dice)
    {

        foreach(var existingDie in DiceInSlotList)
        {
            if(existingDie.DieResult == dice.DieResult)
            {
                return false;
            }
        }

        return true;
    }

    protected override void OnDiceAdded(NewNewDice dice)
    {

        if (DiceInSlotList.Count == RequiredDiceCount)
        {
            ActivateSlotEffect();
        }


    }



    protected override void OnDiceRemoved(NewNewDice dice)
    {

    }



    public override void ActivateSlotEffect() // play the dice 
    {


        BoardManager.instance.boonManager.GiveBoon();

        DestroyAllDiceInSlot();

    }
}
