using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBox_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject NormalDicePrefab;
    private int DiceAmoutEachTurn;
    [SerializeField ] private AbstractSkillSlot bankSlot;


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
        //adjusting slot size of the parent (imapacting the play slot as well)
        // for each 4 dice, incresing the size by 200f. if there are less then 4 dice, defualt to 100f
        float newHeight = 100f;

        if (DiceInSlotList.Count >= 5)
        {
            


            int numberOfGroupsOfFour = Mathf.CeilToInt(DiceInSlotList.Count / 4f);


            if (numberOfGroupsOfFour > 1)
            {
                newHeight = 100f + (numberOfGroupsOfFour - 1) * 200f; // For each group of 4 beyond the first, add 200
            }

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
        //adjusting slot size of the parent (imapacting the play slot as well)
        // for each 4 dice, incresing the size by 200f. if there are less then 4 dice, defualt to 100f



        float newHeight = 100f;
        int numberOfGroupsOfFour = Mathf.CeilToInt(DiceInSlotList.Count / 4f);

        if (numberOfGroupsOfFour > 1)
        {
            newHeight = 100f + (numberOfGroupsOfFour - 1) * 200f; //
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
    
    public void MoveDiceToBank() 
    {


        bankSlot.AddDiceToSlotList(DiceInSlotList[0]);
        this.RemoveDiceFromDiceList(DiceInSlotList[0]);
    }


    public override void DestroyAllDiceInSlot() // check for space in bank before destroying all
    {

        if (bankSlot != null)
        {
            Debug.Log(this.DiceInSlotList.Count);
            if (bankSlot.DiceInSlotList.Count == 0 && this.DiceInSlotList.Count > 0)
            {

                MoveDiceToBank();
            }
        }
        else
        {
            Debug.Log("bank slot is null");
        }




        foreach (TheDiceScript dice in new List<TheDiceScript>(DiceInSlotList))
        {
            RemoveDiceFromDiceList(dice);
            Destroy(dice.gameObject);
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
        BoardManager.instance.skillSlotManager.allDicesOutcome();



    }

}
