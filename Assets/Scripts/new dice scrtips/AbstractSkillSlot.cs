using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbstractSkillSlot : MonoBehaviour, IDropHandler
{
    public int RequiredDiceCount = 1;
    public List<DiceType> RequiredDiceTypes;

    protected List<NewNewDice> DiceInSlotList = new List<NewNewDice>();





    // Checks if the die can be added to the slot, and if so remove the dice from its previous slot
    // triggred automatically when a draggable object is dropped onto a GameObject with this fucntion 
    public void OnDrop(PointerEventData eventData) 
    {
        GameObject droppedObject = eventData.pointerDrag;
        NewNewDice dice = droppedObject.GetComponent<NewNewDice>();

        if (dice.currentSlot != this && CanAcceptDice(dice))
        {            
            if (dice.currentSlot != null)
            {
                dice.currentSlot.RemoveDiceFromDiceList(dice);
            }
            
            AddDiceToSlotList(dice);
        }
    }

    public void AddDiceToSlotList(NewNewDice dice)
    {
        if (CanAcceptDice(dice))
        {
            DiceInSlotList.Add(dice);
            OnDiceAdded(dice);
            dice.ChangeDieParent(transform);
        }
    }


    public void RemoveDiceFromDiceList(NewNewDice dice)
    {
        if (DiceInSlotList.Contains(dice))
        {
            DiceInSlotList.Remove(dice);
            OnDiceRemoved(dice);
        }
    }



    public virtual void DestroyAllDiceInSlot()
    {
      
        foreach (NewNewDice dice in new List<NewNewDice>(DiceInSlotList))
        {
            Destroy(dice.gameObject);
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
    }



    public abstract bool CanAcceptDice(NewNewDice dice);

    protected abstract void OnDiceAdded(NewNewDice dice);
    protected abstract void OnDiceRemoved(NewNewDice dice);

    public abstract void ActivateSlotEffect();

}
