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




    private void Update()
    {
        
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
            SpawnADie();





        }

        //for (int i = 0; i < DiceAmoutEachTurn; i++)
        //{

        //    // Instantiate the prefab
        //    GameObject newObject = Instantiate(NormalDicePrefab, transform);
        //    TheDiceScript diceComp = newObject.GetComponent<TheDiceScript>();
        //    // DiceInSlotList.Add(diceComp);
        //   AddDiceToSlotList(diceComp);
        //}
        // BoardManager.instance.skillSlotManager.allDicesOutcome();

    }


    public void SpawnADie(int? OptionalResult = null, bool WithAnimation = true)
    {

 
        GameObject newObject = Instantiate(NormalDicePrefab, transform);
        TheDiceScript diceComp = newObject.GetComponent<TheDiceScript>();

        diceComp.initializeDie(OptionalResult, WithAnimation);






        AddDiceToSlotList(diceComp);
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




    // check for space in bank before destroying all

    public override void DestroyAllDiceInSlot() 
    {

        if (bankSlot != null)
        {
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
            RemoveDiceFromDiceList(dice, true);
           // Destroy(dice.gameObject);
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
       // BoardManager.instance.skillSlotManager.allDicesOutcome();



    }

}
