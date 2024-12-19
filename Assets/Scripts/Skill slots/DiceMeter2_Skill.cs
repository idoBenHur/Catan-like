using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceMeter2_Skill : AbstractSkillSlot
{

    private RectTransform slotRectTransform;
    [SerializeField] private RectTransform ShdowDiceParent;



    [SerializeField] private Sprite[] DiceSides;
    [SerializeField] private Sprite CheckMarkImage;

    [SerializeField] private GameObject ShadowDiePrefab;
    [SerializeField] private GameObject NormalDiePrefab;

    [SerializeField] private Color activeColor = Color.green; // Color when the effect is active
    private Color TheOriginalColor; // To store the original color of the slot
    private Image ThisSlotImage;



    private Dictionary<int, GameObject> ShadowDiceDic = new Dictionary<int, GameObject>(); // Maps dice values to shadow dice
    private HashSet<int> AcceptedDieNumbers = new HashSet<int>();

    private bool effectIsOn = false;
    private bool IsSpawningDice = false;



    private void Start()
    {
        MaxDiceCap = 6;
        slotRectTransform = GetComponent<RectTransform>();
        slotRectTransform.sizeDelta = new Vector2(slotRectTransform.sizeDelta.x, MaxDiceCap * 100f);

        ShdowDiceParent.sizeDelta = slotRectTransform.sizeDelta;

        ThisSlotImage = GetComponent<Image>();
        TheOriginalColor = ThisSlotImage.color;


        SpawnShadowDice();
    }


    private void SpawnShadowDice()
    {

        if(ShadowDiceDic.Count > 0)
        {
            return;
        }


        for (int i = 6; i >= 1; i--)
        {
            GameObject shadowDie = Instantiate(ShadowDiePrefab, ShdowDiceParent);
            shadowDie.GetComponent<Image>().sprite = DiceSides[i - 1]; 
            ShadowDiceDic.Add(i, shadowDie);
        }
    }




    public override bool CanAcceptDice(TheDiceScript dice)
    {




        // the effect have been actived, and the reward dice are being added to the meter
        if (effectIsOn == true && IsSpawningDice == true)
        {
            return true;
        }

        // the effect have been actived, dice have alreday being added to the meter and i dont allow to more dice to be added
        else if (effectIsOn == true && IsSpawningDice == false)
        {
            return false;
        }

        // the effect is has no been actived yet, and i want to allow only certien dice 
        else if (effectIsOn == true && AcceptedDieNumbers.Contains(dice.DieResult) == true)
        {
            return false;
        }     

        

        return true;
    }

    protected override void OnDiceAdded(TheDiceScript TheNewDie)
    {

        if(effectIsOn == false)
        {
            RemoveDiceFromDiceList(TheNewDie, true);

            AcceptedDieNumbers.Add(TheNewDie.DieResult);

            if (ShadowDiceDic.TryGetValue(TheNewDie.DieResult, out GameObject shadowDieObj))
            {
                
                shadowDieObj.GetComponent<Image>().sprite = CheckMarkImage;
            }

            if (AcceptedDieNumbers.Count == MaxDiceCap)
            {
                ActivateSlotEffect();
            }

            

        }



    }


    public override void ActivateSlotEffect() // spawns 6 dice (player can pick 1)
    {
        effectIsOn = true;

        ThisSlotImage.color = activeColor;


        foreach (Transform child in ShdowDiceParent)
        {
            Destroy(child.gameObject);
        }
        AcceptedDieNumbers.Clear();
        ShadowDiceDic.Clear();

        IsSpawningDice = true;
        for (int i = 1; i <= 6; i++)
        {
            GameObject dieObj = Instantiate(NormalDiePrefab, this.transform);
            TheDiceScript diceScript = dieObj.GetComponent<TheDiceScript>();
            AddDiceToSlotList(diceScript);
            diceScript.initializeDie(null, i);

        }
        IsSpawningDice = false;

        slotRectTransform.DOShakePosition(0.5f, strength: new Vector3(10, 10, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);






    }









    protected override void OnDiceRemoved(TheDiceScript dice)
    {
        

        if(effectIsOn == true)
        {
            DestroyAllDiceInSlot();
            effectIsOn = false;
            SpawnShadowDice();
            ThisSlotImage.color = TheOriginalColor;
        }

    }



    private void OnDestroy()
    {
        slotRectTransform.DOKill();
    }


}
