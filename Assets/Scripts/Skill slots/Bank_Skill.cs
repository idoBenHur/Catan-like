using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank_Skill : AbstractSkillSlot
{
    [SerializeField] AbstractSkillSlot dicebox;


    private void Start()
    {
        MaxDiceCap = 1;
        DestroyDiceInsideUponRoll = false;

    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {
            
        if (DiceInSlotList.Count == MaxDiceCap)
        {


            dicebox.AddDiceToSlotList(DiceInSlotList[0]);
            this.RemoveDiceFromDiceList(DiceInSlotList[0]);


        }

        return true; //DiceInSlotList.Count < MaxDiceCap;
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




}
