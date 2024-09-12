using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


public class SkillSlotManager : MonoBehaviour
{
    public AbstractSkillSlot[] Allslots; // Reference to all slots in the scene
    [HideInInspector] public Dictionary<SkillName, AbstractSkillSlot> SkillSlotsDictionary = new Dictionary<SkillName, AbstractSkillSlot>();
    [SerializeField] AbstractSkillSlot DiceBox;
    [SerializeField] private GameObject NormalDicePrefab;



    private void Start()
    {

        foreach (var skillSlot in Allslots)
        {
            SkillSlotsDictionary[skillSlot.SkillName] = skillSlot;
        }




    }

    public void RollNewDice()
    {
        DiceBox.DestroyAllDiceInSlot();

        foreach (var slot in Allslots)
        {
            if(slot.DestroyDiceInsideUponRoll == true)
            {
                slot.DestroyAllDiceInSlot();
            }


            
        }
        DiceBox.ActivateSlotEffect();

    }



    public void IncreaseMaxDiceCap(SkillName skillName)
    {

    }

}
