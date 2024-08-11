using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBox_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject NormalDicePrefab;


    private void Start()
    {
        RequiredDiceCount = 4;
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

    public override void ActivateSlotEffect() // spawns new dices
    {
        
        for (int i = 0; i < RequiredDiceCount; i++)
        {
            // Instantiate the prefab
            GameObject newObject = Instantiate(NormalDicePrefab, transform);
            NewNewDice diceComp = newObject.GetComponent<NewNewDice>();

            DiceInSlotList.Add(diceComp);
        }




    }





}
