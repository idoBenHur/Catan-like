using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbstractSkillSlot : MonoBehaviour, IDropHandler
{
    public int RequiredDiceCount;
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
                dice.currentSlot.RemoveDice(dice);
            }
            
            AddDice(dice);
        }
    }

    public void AddDice(NewNewDice dice)
    {
        if (CanAcceptDice(dice))
        {
            DiceInSlotList.Add(dice);
            OnDiceAdded(dice);
            dice.ChangeDieParent(transform);
        }
    }


    public void RemoveDice(NewNewDice dice)
    {
        if (DiceInSlotList.Contains(dice))
        {
            DiceInSlotList.Remove(dice);
            OnDiceRemoved(dice);
        }
    }



    public abstract bool CanAcceptDice(NewNewDice dice);

    protected abstract void OnDiceAdded(NewNewDice dice);
    protected abstract void OnDiceRemoved(NewNewDice dice);

    public abstract void ActivateSlotEffect();

}
