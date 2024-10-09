using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


public class SkillSlotManager : MonoBehaviour
{
    public AbstractSkillSlot[] Allslots; // Reference to all slots in the scene
    [HideInInspector] public Dictionary<SkillName, AbstractSkillSlot> SkillSlotsDictionary = new Dictionary<SkillName, AbstractSkillSlot>();




    private void Start()
    {

        foreach (var skillSlot in Allslots)
        {
            SkillSlotsDictionary[skillSlot.SkillName] = skillSlot;
        }




    }

    public void RollNewDice()
    {

        AbstractSkillSlot theDicebox = SkillSlotsDictionary[SkillName.DiceBox];
        theDicebox.DestroyAllDiceInSlot();

      //  DiceBox.DestroyAllDiceInSlot();

        foreach (var slot in Allslots)
        {
            if(slot.DestroyDiceInsideUponRoll == true)
            {
                slot.DestroyAllDiceInSlot();
            }


            
        }
        theDicebox.ActivateSlotEffect();


    }



    public void IncreaseMaxDiceCap(SkillName skillName)
    {
        SkillSlotsDictionary[skillName].MaxDiceCap =+ 1;
    }


    public void allDicesOutcome()
    {
        List<int> dicesList = new List<int>();
        List<int> sumOfPairs = new List<int>();

        dicesList.Clear();

        // get all dice values from slot exsept from boon slot
        foreach (var slot in SkillSlotsDictionary) 
        {
            if(slot.Key == SkillName.Boons) { continue; }

            foreach (var dice in slot.Value.DiceInSlotList) 
            {
                dicesList.Add(dice.DieResult);
            }



            
        }




        // Iterate over each die in the list
        // Compare the current die with every die that comes after it in the list
        // Calculate the sum of the two dice

        for (int firstDieIndex = 0; firstDieIndex < dicesList.Count; firstDieIndex++)
        {

            for (int secondDieIndex = firstDieIndex + 1; secondDieIndex < dicesList.Count; secondDieIndex++)
            {
                int sum = dicesList[firstDieIndex] + dicesList[secondDieIndex];

                sumOfPairs.Add(sum);
            }
        }

        foreach (var number in sumOfPairs)
        {
           // Debug.Log(number);
        }
        

        BoardManager.instance.uiManager.IncreaseNumbersTokenSize(sumOfPairs);

    }
}
