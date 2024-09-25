using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GetBoon_Skill : AbstractSkillSlot
{
    private void Start()
    {
        MaxDiceCap = 6;
        SkillName = SkillName.Boons;
        DestroyDiceInsideUponRoll = false;

    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {

        
        //foreach(var existingDie in DiceInSlotList)
        //{
        //    if(existingDie.DieResult == dice.DieResult)
        //    {
        //        return false;
        //    }
        //}

       /// return true;
       /// 

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(TheDiceScript TheNewDie)
    {
        TheNewDie.DraggableActive = false;


        if (DiceInSlotList.Count == MaxDiceCap)
        {
            ActivateSlotEffect();
        }


        //sorting dice gameobjects from top to bottom in boonSlot
        var sortedDiceList = DiceInSlotList.OrderByDescending(die => die.DieResult).ToList();
        for (int i = 0; i < sortedDiceList.Count; i++)
        {
            sortedDiceList[i].transform.SetSiblingIndex(i);
        }

    }



    protected override void OnDiceRemoved(TheDiceScript dice)
    {

    }



    public override void ActivateSlotEffect() // play the dice 
    {


        BoardManager.instance.boonManager.GiveBoon();

        DestroyAllDiceInSlot();

    }
}