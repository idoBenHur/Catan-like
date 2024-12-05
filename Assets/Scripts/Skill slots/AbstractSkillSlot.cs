using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public enum SkillName
{
    DiceBox,
    SevenAbility,
    PlayDice,
    Bank,
    Boons,
    SplitDie
    // Add more skill types as needed
}



public abstract class AbstractSkillSlot : MonoBehaviour, IDropHandler
{
    public SkillName SkillName;
    public bool DestroyDiceInsideUponRoll = true;
    [HideInInspector] public int MaxDiceCap;

    protected int MaxUsesPerTurn = -1;
    private int remainingUsesThisTurn;
    private Image slotImage;
    private Color originalColor;
    private Color disabledColor = new Color(0.52f, 0.52f, 0.5f);

    public List<TheDiceScript> DiceInSlotList = new List<TheDiceScript>();




    protected virtual void Awake()
    {
        slotImage = GetComponent<Image>();
        if (slotImage != null)
        {
            originalColor = slotImage.color;
        }
        else
        {
            Debug.LogWarning($"No Image component found on {gameObject.name}. Slot won't visually indicate usage limits.");
        }



        ResetUsesPerTurn();
        BoardManager.OnDiceRolled += ResetUsesPerTurn;

    }

    private void OnDestroy()
    {
        BoardManager.OnDiceRolled -= ResetUsesPerTurn; // Unsubscribe on destroy
    }



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
           // BoardManager.instance.skillSlotManager.allDicesOutcome();
            OnDiceAdded(dice);
        }



       
    }


    public void RemoveDiceFromDiceList(TheDiceScript dice, bool destroyDice = false)
    {



        if (DiceInSlotList.Contains(dice))
        {
            DiceInSlotList.Remove(dice);
            OnDiceRemoved(dice);
        }

        if(destroyDice == true)
        {
            Destroy(dice.gameObject);
            BoardManager.instance.skillSlotManager.allDicesOutcome();

        }


    }



    public virtual void DestroyAllDiceInSlot()
    {
      
        foreach (TheDiceScript dice in new List<TheDiceScript>(DiceInSlotList))
        {
            RemoveDiceFromDiceList(dice, true);
           
        }

        DiceInSlotList.Clear(); // Clear the list after destruction
      //  BoardManager.instance.skillSlotManager.allDicesOutcome();
    }



    public void ResetUsesPerTurn()
    {
        if(MaxUsesPerTurn == -1)
        {
            remainingUsesThisTurn = int.MaxValue;
        }
        else
        {
            remainingUsesThisTurn = MaxUsesPerTurn;
        }
        
        UpdateSlotVisual();
    }


    public void UseSlot()
    {
        if (MaxUsesPerTurn != -1)
        {
            remainingUsesThisTurn--;
        }
                      
        UpdateSlotVisual();
    }




    public bool HaveEnoughUsages()
    {
        return remainingUsesThisTurn > 0;
    }



    private void UpdateSlotVisual()
    {
        if (slotImage == null) return;

        // Change the color based on remaining uses
        if (remainingUsesThisTurn == 0)
        {
            slotImage.color = disabledColor; 
        }
        else
        {
            slotImage.color = originalColor; 
        }
    }











    public abstract bool CanAcceptDice(TheDiceScript dice);

    protected abstract void OnDiceAdded(TheDiceScript dice);
    protected abstract void OnDiceRemoved(TheDiceScript dice);

    public abstract void ActivateSlotEffect();

}
