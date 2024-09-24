using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank_Skill : AbstractSkillSlot
{



    private void Start()
    {
        MaxDiceCap = 1;
        SkillName = SkillName.Bank;

    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {
                
        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {

    }



    protected override void OnDiceRemoved(TheDiceScript dice)
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
