using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDice_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject particleEffect;
    private DiceBox_Skill diceBox;

    private void Start()
    {
        MaxDiceCap = 2;
        diceBox = BoardManager.instance.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;

    }

    public override bool CanAcceptDice(TheDiceScript dice)
    {

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {

        if (DiceInSlotList.Count == MaxDiceCap)
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



        if (firstDice + secondDice == 7)
        {
            BoardManager.instance.uiManager.OpenSevenSkillRewardPannel();

        }

        else if (BoardManager.instance.player.OwnANumber(firstDice + secondDice) == false) 
        {
            ReturnToDiceBox();
            return;
        }



        BoardManager.instance.DicesPlayed(firstDice, secondDice);

        foreach (var die in DiceInSlotList)
        {
            die.PlayDice();
        }


        if(particleEffect != null)
        {
            Instantiate(particleEffect, this.transform);

        }


        DestroyAllDiceInSlot();

    }



    private void ReturnToDiceBox()
    {
        foreach (var dice in new List<TheDiceScript>(DiceInSlotList)) // new copy of this list to not change it will iterating it
        {
            // Remove the die from this slot
            RemoveDiceFromDiceList(dice);

            // Add the die back to the dice box
            diceBox.AddDiceToSlotList(dice);
        }
    }



}
