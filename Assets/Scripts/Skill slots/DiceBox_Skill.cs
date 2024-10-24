using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBox_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject NormalDicePrefab;
    private int DiceAmoutEachTurn; 


    private void Start()
    {
        MaxDiceCap = 200;
        DiceAmoutEachTurn = 2;

    }


    public override bool CanAcceptDice(TheDiceScript dice)
    {

        return DiceInSlotList.Count < MaxDiceCap;
    }

    protected override void OnDiceAdded(TheDiceScript dice)
    {
        //adjusting slot size, of the parent (imapacting the play slot as well)

        if (DiceInSlotList.Count >= 5)
        {
            float newHeight = 100f;

            // Calculate the number of groups of 4 (rounding up to ensure we account for leftover items)
            int numberOfGroupsOfFour = Mathf.CeilToInt(DiceInSlotList.Count / 4f);

            // Adjust the height (each group of 4 adds 200, starting from 300 when there are more than 4 items)
            if (numberOfGroupsOfFour > 1)
            {
                newHeight = 100f + (numberOfGroupsOfFour - 1) * 200f; // For each group of 4 beyond the first, add 200
            }

            // Get the RectTransform of the parent
            RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();

            if (parentRectTransform != null)
            {
                // Set the new height while keeping the current width
                Vector2 newSize = parentRectTransform.sizeDelta;
                newSize.y = newHeight; // Set the calculated height
                parentRectTransform.sizeDelta = newSize;
            }

        }

 
    }
        



    protected override void OnDiceRemoved(TheDiceScript dice)
    {

        //adjusting slot size, of the parent (imapacting the play slot as well)

        float newHeight = 100f;

        // Calculate the number of groups of 4 (rounding up to ensure we account for leftover items)
        int numberOfGroupsOfFour = Mathf.CeilToInt(DiceInSlotList.Count / 4f);

        // Adjust the height (each group of 4 adds 200, starting from 300 when there are more than 4 items)
        if (numberOfGroupsOfFour > 1)
        {
            newHeight = 100f + (numberOfGroupsOfFour - 1) * 200f; // For each group of 4 beyond the first, add 200
        }

        RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();

        if (parentRectTransform != null)
        {
            Vector2 newSize = parentRectTransform.sizeDelta;
            newSize.y = newHeight; 
            parentRectTransform.sizeDelta = newSize;
        }
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
        BoardManager.instance.skillSlotManager.allDicesOutcome();

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
        BoardManager.instance.skillSlotManager.allDicesOutcome();

    }

    public void AddPermaDie()
    {
        if(DiceAmoutEachTurn < MaxDiceCap)
        {
            DiceAmoutEachTurn += 1;

        }
    }
    




}
