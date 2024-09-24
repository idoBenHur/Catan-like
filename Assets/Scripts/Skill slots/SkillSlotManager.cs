using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


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

}
