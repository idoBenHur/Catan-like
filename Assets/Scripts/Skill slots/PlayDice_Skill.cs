using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDice_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject particleEffect;

    private void Start()
    {
        MaxDiceCap = 2;

    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {

        if (DiceInSlotList.Count == 2)
        {
            ActivateSlotEffect();
        }


    }



    protected override void OnDiceRemoved(TheDiceScript dice)
    {

    }



    public override void ActivateSlotEffect() // play the dice 
    {
        int firstDice = DiceInSlotList[0].DieResult;
        int secondDice = DiceInSlotList[1].DieResult;

        BoardManager.instance.DicesPlayed(firstDice, secondDice);

        if(firstDice + secondDice == 7)
        {
            BoardManager.instance.uiManager.OpenSevenSkillRewardPannel();

        }

        

        DestroyAllDiceInSlot();

        Instantiate(particleEffect);

    }




}
