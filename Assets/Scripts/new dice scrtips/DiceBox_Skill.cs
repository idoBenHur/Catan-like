using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBox_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject NormalDicePrefab;
    private int DiceAmoutEachTurn; 


    private void Start()
    {
        MaxDiceCap = 6;
        DiceAmoutEachTurn = 4;

        SkillName = SkillName.DiceBox;
    }


    public override bool CanAcceptDice(NewNewDice dice)
    {

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(NewNewDice dice)
    {

    }



    protected override void OnDiceRemoved(NewNewDice dice)
    {

    }

    public override void ActivateSlotEffect() // spawns new dices up to its max cap
    {
        
        for (int i = 0; i < DiceAmoutEachTurn; i++)
        {
            // Instantiate the prefab
            GameObject newObject = Instantiate(NormalDicePrefab, transform);
            NewNewDice diceComp = newObject.GetComponent<NewNewDice>();
            DiceInSlotList.Add(diceComp);
        }

    }


    public void AddTempDie(int amount)
    {

        for(int i = 0; i < amount; i++) 
        {

            if (DiceInSlotList.Count < MaxDiceCap)
            {
                GameObject newObject = Instantiate(NormalDicePrefab, transform);
                NewNewDice diceComp = newObject.GetComponent<NewNewDice>();
                DiceInSlotList.Add(diceComp);
            }
            else
            {
                return;
            }

        }

    }

    public void AddPermaDie()
    {
        DiceAmoutEachTurn += 1;
    }
    




}
