using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank_Skill : AbstractSkillSlot
{



    private void Start()
    {
        RequiredDiceCount = 1;
    }

    public override bool CanAcceptDice(NewNewDice dice)
    {
                
        return DiceInSlotList.Count < RequiredDiceCount;
    }

    protected override void OnDiceAdded(NewNewDice dice)
    {

    }



    protected override void OnDiceRemoved(NewNewDice dice)
    {

    }



    public override void ActivateSlotEffect()
    {

    }



    public override void DestroyAllDiceInSlot()
    {
        // does noting because this is the bank skill
    }
}
