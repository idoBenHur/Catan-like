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
        DiceAmoutEachTurn = 2;

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

    public override void ActivateSlotEffect() // spawns new dices up to its DiceAmoutEachTurn
    {
        
        for (int i = 0; i < DiceAmoutEachTurn; i++)
        {
            // Instantiate the prefab
            GameObject newObject = Instantiate(NormalDicePrefab, transform);
            TheDiceScript diceComp = newObject.GetComponent<TheDiceScript>();
           // DiceInSlotList.Add(diceComp);
           AddDiceToSlotList(diceComp);
        }

    }


    public void AddTempDie(int amount)
    {

        for(int i = 0; i < amount; i++) 
        {

            if (DiceInSlotList.Count < MaxDiceCap)
            {
                GameObject newObject = Instantiate(NormalDicePrefab, transform);
                TheDiceScript diceComp = newObject.GetComponent<TheDiceScript>();
                AddDiceToSlotList(diceComp);
                //DiceInSlotList.Add(diceComp);
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
