using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTest : AbstractSkillSlot
{



    private void Start()
    {
        RequiredDiceCount = 2;
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



}
