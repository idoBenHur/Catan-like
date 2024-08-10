using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillSlotManager : MonoBehaviour
{
    public AbstractSkillSlot[] slots; // Reference to all slots in the scene
    public GameObject dicePrefab;      // Prefab of the dice to be spawned
    public Button resetButton;         // Reference to the reset button

    private void Start()
    {
        resetButton.onClick.AddListener(ResetDice);
    }

    public void ResetDice()
    {
        // Destroy dice in all slots
        foreach (AbstractSkillSlot slot in slots)
        {
            slot.DestroyAllDiceInSlot();
        }

        // Spawn 4 new dice in the first slot (or any other target slot)
      //  slots[0].SpawnNewDice(4);
    }
}
