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
    [HideInInspector] public SkillName SkillName;
    public bool DestroyDiceInsideUponRoll = true;
    [HideInInspector] public int MaxDiceCap;
    public List<DiceType> RequiredDiceTypes;
    

    public List<NewNewDice> DiceInSlotList = new List<NewNewDice>();





    // Checks if the die can be added to the slot, and if so remove the dice from its previous slot
    // triggred automatically when a draggable object is dropped onto a GameObject with this fucntion 
    public void OnDrop(PointerEventData eventData) 
    {
        GameObject droppedObject = eventData.pointerDrag;
        NewNewDice dice = droppedObject.GetComponent<NewNewDice>();

        if (dice.DraggableActive == false) { return; }


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
            dice.ChangeDieParent(transform);
            OnDiceAdded(dice);
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
            RemoveDiceFromDiceList(dice);
            Destroy(dice.gameObject);
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
    }



    public abstract bool CanAcceptDice(NewNewDice dice);

    protected abstract void OnDiceAdded(NewNewDice dice);
    protected abstract void OnDiceRemoved(NewNewDice dice);

    public abstract void ActivateSlotEffect();

}
