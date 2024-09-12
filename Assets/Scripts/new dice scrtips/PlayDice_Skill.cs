using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDice_Skill : AbstractSkillSlot
{


    private void Start()
    {
        MaxDiceCap = 2;
        SkillName = SkillName.PlayDice;

    }

    public override bool CanAcceptDice(NewNewDice dice)
    {

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(NewNewDice dice)
    {

        if (DiceInSlotList.Count == 2)
        {
            ActivateSlotEffect();
        }


    }



    protected override void OnDiceRemoved(NewNewDice dice)
    {

    }



    public override void ActivateSlotEffect() // play the dice 
    {
        int firstDice = DiceInSlotList[0].DieResult;
        int secondDice = DiceInSlotList[1].DieResult;

        BoardManager.instance.DicesPlayed(firstDice, secondDice);

        DestroyAllDiceInSlot();

    }




}
