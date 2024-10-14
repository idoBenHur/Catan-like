using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public enum SkillName
{
    DiceBox,
    SevenAbility,
    PlayDice,
    Bank,
    Boons
    // Add more skill types as needed
}



public abstract class AbstractSkillSlot : MonoBehaviour, IDropHandler
{
    public SkillName SkillName;
    public bool DestroyDiceInsideUponRoll = true;
    [HideInInspector] public int MaxDiceCap;
    public List<DiceType> RequiredDiceTypes;
    

    public List<TheDiceScript> DiceInSlotList = new List<TheDiceScript>();





    // Checks if the die can be added to the slot, and if so remove the dice from its previous slot
    // triggred automatically when a draggable object is dropped onto a GameObject with this fucntion 
    public void OnDrop(PointerEventData eventData) 
    {
        
        GameObject droppedObject = eventData.pointerDrag;
        TheDiceScript dice = droppedObject.GetComponent<TheDiceScript>();

        if (dice.DraggableActive == false) { return; }


        if (CanAcceptDice(dice)) //dice.currentSlot != this &&
        {
            if (dice.currentSlot != null)
            {
                dice.currentSlot.RemoveDiceFromDiceList(dice);
            }
            
            AddDiceToSlotList(dice);
        }
    }

    public void AddDiceToSlotList(TheDiceScript dice)
    {
        if (CanAcceptDice(dice))
        {
            DiceInSlotList.Add(dice);           
            dice.ChangeDieParent(transform);
            OnDiceAdded(dice);
        }

        DynamicSlotResizer resizer = GetComponent<DynamicSlotResizer>();
        if (resizer != null)
        {
            resizer.AdjustSlotSize();
        }

       BoardManager.instance.skillSlotManager.allDicesOutcome();
    }


    public void RemoveDiceFromDiceList(TheDiceScript dice, bool destroyDice = false)
    {

        DynamicSlotResizer resizer = GetComponent<DynamicSlotResizer>();
        if (resizer != null)
        {
            resizer.AdjustSlotSize();
        }

        if (DiceInSlotList.Contains(dice))
        {
            DiceInSlotList.Remove(dice);
            OnDiceRemoved(dice);
        }

        if(destroyDice == true)
        {
            Destroy(dice.gameObject);
        }

      // BoardManager.instance.skillSlotManager.allDicesOutcome();

    }



    public virtual void DestroyAllDiceInSlot()
    {
      
        foreach (TheDiceScript dice in new List<TheDiceScript>(DiceInSlotList))
        {
            RemoveDiceFromDiceList(dice);
            Destroy(dice.gameObject);
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
        BoardManager.instance.skillSlotManager.allDicesOutcome();
    }



    public abstract bool CanAcceptDice(TheDiceScript dice);

    protected abstract void OnDiceAdded(TheDiceScript dice);
    protected abstract void OnDiceRemoved(TheDiceScript dice);

    public abstract void ActivateSlotEffect();

}
