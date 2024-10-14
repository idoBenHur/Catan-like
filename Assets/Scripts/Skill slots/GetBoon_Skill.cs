using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;




public class ShadowDiceData
{
    public GameObject DiceObject;
    public bool IsUsed;

    public ShadowDiceData(GameObject diceObject)
    {
        DiceObject = diceObject;
        IsUsed = false;
    }
}

public class GetBoon_Skill : AbstractSkillSlot
{
    private RectTransform slotRectTransform;
    [SerializeField] private Sprite[] DiceSides;
    [SerializeField] private Sprite Checkmark;
    [SerializeField] private Sprite anyDicaeSprite;

    [SerializeField] private GameObject diceShadowPrefab;
    [SerializeField] private GameObject shdowDiceParent;
    [SerializeField] private bool AnyDice;

    private Dictionary<int, List<ShadowDiceData>> ShadowDiceDic = new Dictionary<int, List<ShadowDiceData>>();

    private void Start()
    {
        MaxDiceCap = 2;
        DestroyDiceInsideUponRoll = false;
        slotRectTransform = GetComponent<RectTransform>();
        slotRectTransform.sizeDelta = new Vector2(slotRectTransform.sizeDelta.x, MaxDiceCap * 100f);

        shuffleNumbers();
    }

    private void shuffleNumbers()
    {
        for (int i = 0; i < MaxDiceCap; i++)
        {
            int randomNumber = Random.Range(1, 6);
            if (!ShadowDiceDic.ContainsKey(randomNumber))
            {
                ShadowDiceDic[randomNumber] = new List<ShadowDiceData>();
            }

            spawnDiceRequirementPrefab(randomNumber);
        }
    }


    private void spawnDiceRequirementPrefab(int number)
    {
        GameObject dicePrefab = Instantiate(diceShadowPrefab, shdowDiceParent.transform);

        if(AnyDice == true)
        {
            UpdateDiceSprite(dicePrefab, anyDicaeSprite); 
            number = -1;
        }
        else
        {
            UpdateDiceSprite(dicePrefab, DiceSides[number - 1]);
        }

        if (!ShadowDiceDic.ContainsKey(number))
        {
            ShadowDiceDic[number] = new List<ShadowDiceData>();
        }
        ShadowDiceDic[number].Add(new ShadowDiceData(dicePrefab));
    }




    public override bool CanAcceptDice(TheDiceScript dice)
    {
        int key;
        if(AnyDice == true) { key = -1; }
        else { key = dice.DieResult; }

        return ShadowDiceDic.ContainsKey(key) &&
               ShadowDiceDic[key].Any(d => !d.IsUsed);
    }

    protected override void OnDiceAdded(TheDiceScript TheNewDie)
    {

        int key;
        if(AnyDice == true) { key = -1; }
        else {key = TheNewDie.DieResult;}


        // Find the first unused dice for the result
        var diceData = ShadowDiceDic[key].FirstOrDefault(shadowDice => !shadowDice.IsUsed);

        if (diceData != null)
        {
            
            diceData.IsUsed = true;
            UpdateDiceSprite(diceData.DiceObject, Checkmark);

            
            RemoveDiceFromDiceList(TheNewDie, true);
        }

        // If all dice have been used, activate the slot effect
        if (ShadowDiceDic.Values.All(diceList => diceList.All(d => d.IsUsed)))
        {
            ActivateSlotEffect();
        }

       // SortDiceInSlot();
        
    }

    // Handle sorting dice in the slot visually
    private void SortDiceInSlot()
    {
        var sortedDiceList = DiceInSlotList.OrderByDescending(die => die.DieResult).ToList();
        for (int i = 0; i < sortedDiceList.Count; i++)
        {
            sortedDiceList[i].transform.SetSiblingIndex(i);
        }
    }

    public override void ActivateSlotEffect()
    {
        // Destroy all dice objects in the dictionary
        foreach (var diceList in ShadowDiceDic.Values)
        {
            foreach (var shadowDice in diceList)
            {
                Destroy(shadowDice.DiceObject);
            }
        }

        ShadowDiceDic.Clear();


        // BoardManager.instance.uiManager.OpenSevenSkillRewardPannel();
        BoardManager.instance.boonManager.GiveBoon();
        IncreaseCap();
        shuffleNumbers();
    }

    private void IncreaseCap()
    {
        if (MaxDiceCap <= 6)
        {
            MaxDiceCap += 1;
            slotRectTransform.sizeDelta = new Vector2(slotRectTransform.sizeDelta.x, slotRectTransform.sizeDelta.y + 100f);
        }
    }

    private void UpdateDiceSprite(GameObject dice, Sprite newSprite)
    {
        Image diceImage = dice.GetComponent<Image>();
        diceImage.sprite = newSprite;
    }

    protected override void OnDiceRemoved(TheDiceScript dice)
    {
    }
}
