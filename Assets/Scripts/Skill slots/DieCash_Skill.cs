using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCash_Skill : AbstractSkillSlot
{
    private TheDiceScript DieToCash;
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

        DieToCash = dice;

        DestroyAllDiceInSlot();
        ActivateSlotEffect();
        UseSlot();
    }



    protected override void OnDiceRemoved(TheDiceScript dice)
    {

    }



    public override void ActivateSlotEffect()
    {

        int ogDieResult = DieToCash.DieResult;


        if (TheDiceBox == null)
        {

            TheDiceBox = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;

        }



        for (int i = 0; i < ogDieResult; i++) 
        {
            TheDiceBox.SpawnADie(DieToCash.DieEffecttType, 1); // always spawns 1s, if it was special die, it will keep its effect
        }









    }
}
