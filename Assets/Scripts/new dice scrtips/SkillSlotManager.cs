using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillSlotManager : MonoBehaviour
{
    public AbstractSkillSlot[] Allslots; // Reference to all slots in the scene
    [SerializeField] AbstractSkillSlot DiceBox;
        

    private void Start()
    {
        
    }

    public void SpawnNewDiceInSlot()
    {
        DiceBox.DestroyAllDiceInSlot();

        foreach (var slot in Allslots)
        {
            slot.DestroyAllDiceInSlot();
        }
        DiceBox.ActivateSlotEffect();





    }
}
