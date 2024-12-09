using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitDie_Skill : AbstractSkillSlot
{
    
    private TheDiceScript DieToSplit;
    private DiceBox_Skill TheDiceBox;


    private void Start()
    {
        MaxDiceCap = 1;
        MaxUsesPerTurn = 1;




    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {
        return DiceInSlotList.Count < MaxDiceCap && HaveEnoughUsages();
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {
        
        DieToSplit = dice;

        DestroyAllDiceInSlot();
        ActivateSlotEffect();
        UseSlot();
    }



    protected override void OnDiceRemoved(TheDiceScript dice)
    {

    }



    public override void ActivateSlotEffect()
    {

        int ogDieResult = DieToSplit.DieResult;
        int half1;
        int half2;


        if(ogDieResult == 1) // spliting a die with a 1, will result in 2 dice of 1s
        {
            half1 = 1;
            half2 = 1;
        }
        else
        {
            half1 = ogDieResult / 2;
            half2 = ogDieResult - half1;
        }




        

        if (TheDiceBox == null)
        {

            TheDiceBox = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;

        }


        TheDiceBox.SpawnADie(DieToSplit.specialDie, half1);
        TheDiceBox.SpawnADie(DieToSplit.specialDie, half2);





    }

}
