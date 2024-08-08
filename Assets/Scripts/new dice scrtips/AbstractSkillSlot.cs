using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbstractSkillSlot : MonoBehaviour, IDropHandler
{
    public int RequiredDiceCount;
    public List<DiceType> RequiredDiceTypes;

    protected List<NewNewDice> diceInSlot = new List<NewNewDice>();




    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        NewNewDice dice = droppedObject.GetComponent<NewNewDice>();

        if (CanAcceptDice(dice))
        {
            AddDice(dice);
        }
        else
        {
            // Return the dice to its original parent if it cannot be accepted
          //  dice.SetParent(dice.originalParent);
        }
    }

    public void AddDice(NewNewDice dice)
    {
        if (CanAcceptDice(dice))
        {
            diceInSlot.Add(dice);
            OnDiceAdded(dice);
            dice.SetParent(transform);  // Move the dice to the slot's position
        }
    }



    public abstract bool CanAcceptDice(NewNewDice dice);

    protected abstract void OnDiceAdded(NewNewDice dice);

    public abstract void ActivateSlotEffect();

}
