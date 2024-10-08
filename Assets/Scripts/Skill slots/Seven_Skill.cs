using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seven_Skill : AbstractSkillSlot
{
    int SUMDiceInSlot = 0;




    private void Start()
    {
        // RequiredDiceCount = 2;
    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {

        if (DiceInSlotList.Count == 0)
        {
            return true;
        }

        else if (DiceInSlotList.Count == 1)
        {
            return SUMDiceInSlot + dice.DieResult == 7;
        }

        return false;
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {


        SUMDiceInSlot += dice.DieResult;


        if(SUMDiceInSlot == 7)
        {
            ActivateSlotEffect();
        };


    }



    protected override void OnDiceRemoved(TheDiceScript dice)
    {
        SUMDiceInSlot -= dice.DieResult;
    }



    public override void ActivateSlotEffect() // play the dice 
    {


        BoardManager.instance.uiManager.OpenSevenSkillRewardPannel();

        DestroyAllDiceInSlot();

    }
}
