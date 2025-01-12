using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//this skill slot, charge up a button when the meter is filled. this button activtedt the remove fog action/indicators

public class DiceMeter3_Skill : AbstractSkillSlot
{
    
    [SerializeField] private Sprite[] DiceSides;
    [SerializeField] private Sprite CheckMarkImage;
    [SerializeField] private GameObject ShadowDiePrefab;
    [SerializeField] private RectTransform ShdowDiceParent;
    [SerializeField] private Button effectButton;


    private RectTransform slotRectTransform;
    private Dictionary<GameObject,int > ShadowDiceDic = new Dictionary< GameObject,int >(); // Maps dice values to shadow dice
    private int buttonUsages = 0;
    private int DieAmountReq;





    private void Start()
    {
        DieAmountReq = 2;
        MaxDiceCap = 6;

        slotRectTransform = GetComponent<RectTransform>();
        slotRectTransform.sizeDelta = new Vector2(slotRectTransform.sizeDelta.x, DieAmountReq * 100f);
        ShdowDiceParent.sizeDelta = slotRectTransform.sizeDelta;

        UpdateButtonState();

        SpawnShadowDice();

    }



    private void SpawnShadowDice()
    {

        if (ShadowDiceDic.Count > 0)
        {
            Debug.Log("there is still shadow dice in the shadow dice dic");
            return;
        }

        slotRectTransform.sizeDelta = new Vector2(slotRectTransform.sizeDelta.x, DieAmountReq * 100f);
        ShdowDiceParent.sizeDelta = slotRectTransform.sizeDelta;

        for (int i = DieAmountReq; i > 0; i--)
        {
            GameObject shadowDie = Instantiate(ShadowDiePrefab, ShdowDiceParent);
            int dieNumber = Random.Range(1, 7);
            shadowDie.GetComponent<Image>().sprite = DiceSides[dieNumber - 1];
            
            ShadowDiceDic.Add(shadowDie, dieNumber);
        }
    }



    public override bool CanAcceptDice(TheDiceScript dice)
    {
        foreach(var shadowDicePrefab in ShadowDiceDic)
        {
            if(shadowDicePrefab.Value == dice.DieResult)
            {
                return true;
            }
        }



        return false;

    }

    protected override void OnDiceAdded(TheDiceScript TheNewDie)
    {

        for (int i = ShadowDiceDic.Count - 1; i >= 0; i--)
        {
            var shadowDicePrefab = ShadowDiceDic.ElementAt(i); // Convert the dictionary to a list temporarily
            if (shadowDicePrefab.Value == TheNewDie.DieResult)
            {
                GameObject shadowDieObj = shadowDicePrefab.Key;
                shadowDieObj.GetComponent<Image>().sprite = CheckMarkImage;
                ShadowDiceDic.Remove(shadowDicePrefab.Key); // Safely remove from the dictionary
                break;
            }
        }

        RemoveDiceFromDiceList(TheNewDie, true); // destroy the die you added


        if(ShadowDiceDic.Count == 0)
        {
            ActivateSlotEffect();
        }



    }


    public override void ActivateSlotEffect() // spawns 6 dice (player can pick 1)
    {

        foreach (Transform child in ShdowDiceParent)
        {
            Destroy(child.gameObject);
        }
        ShadowDiceDic.Clear();

        buttonUsages++;
        UpdateButtonState();


        if(DieAmountReq < MaxDiceCap)
        {
            DieAmountReq++;
        }

        
        SpawnShadowDice();


    }







    protected override void OnDiceRemoved(TheDiceScript dice)
    {



    }


    private void UpdateButtonState()
    {
        if (effectButton != null)
        {
            effectButton.interactable = buttonUsages > 0;
            //  effectButton.GetComponentInChildren<Text>().text = $"Use ({buttonUsages})"; // Update the button text

            TextMeshProUGUI buttonText = effectButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{buttonUsages} uses left";
            }


        }
    }

    public void OnButtonPressed()
    {
        
        if (buttonUsages > 0)
        {
            buttonUsages--;
            UpdateButtonState();

            BoardManager.instance.uiManager.CloseAllUi();
            BoardManager.instance.ShowRemoveFogIndicator();



            Debug.Log("Button effect triggered!");


            
        }
    }




}
