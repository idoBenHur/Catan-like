using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiceBox_Skill : AbstractSkillSlot
{
    [SerializeField] private GameObject DicePrefab;
    private int DiceAmoutEachTurn;
    [SerializeField ] private AbstractSkillSlot bankSlot;
    private List<GenericBoon> SpecialDicePool = new List<GenericBoon>();
    private GenericBoon NormalDie;


    private void Start()
    {
        MaxDiceCap = 200;
        DiceAmoutEachTurn = 4;


        // Add an empty "normal die" to the pool as a default
        var normalDiceBoon = ScriptableObject.CreateInstance<GenericBoon>();
        normalDiceBoon.boonName = "Normal Dice";
        normalDiceBoon.effects = new List<BoonEffect>();
        normalDiceBoon.conditions = new List<BoonCondition>();
        NormalDie = normalDiceBoon;

       // dicePool.Add(normalDiceBoon); // Add the default normal die to the pool



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
        int diceLeftToSpawn = DiceAmoutEachTurn;



        for (int i = 0; i < SpecialDicePool.Count && diceLeftToSpawn > 0; i++)
        {
            SpawnADie(SpecialDicePool[i]);
            diceLeftToSpawn--;
        }

        for (int i = 0; i < diceLeftToSpawn; i++)
        {
            SpawnADie(NormalDie);
        }

  

    }

    
    public void SpawnADie(GenericBoon diceEffect = null, int? OptionalResult = null)
    {

 
        GameObject newObject = Instantiate(DicePrefab, transform);
        TheDiceScript diceScript = newObject.GetComponent<TheDiceScript>();

        // if a Special die wasnt mention, spawn normal die
        if (diceEffect == null)
        {
            diceEffect = NormalDie;
        }



        diceScript.initializeDie(diceEffect, OptionalResult);

        AddDiceToSlotList(diceScript);
        BoardManager.instance.skillSlotManager.allDicesOutcome();
       

    }


    public void AddPermaDie()
    {
        if(DiceAmoutEachTurn < MaxDiceCap)
        {
            DiceAmoutEachTurn += 1;

        }
    }


    
    public void AddSpecialDiceToPool(GenericBoon specialDice)
    {
        if (SpecialDicePool.Contains(specialDice) == false)
        {
            SpecialDicePool.Add(specialDice);
        }
    }

    public void RemoveSpecialDiceFromPool(GenericBoon specialDice)
    {
        if (SpecialDicePool.Contains(specialDice))
        {
            SpecialDicePool.Remove(specialDice);
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
