using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;






public enum DiceType
{
    Normal,
    Fire,
    Water,
    // Add more dice types as needed
}

public class NewNewDice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int DieResult { get; private set; }
    public DiceType Type { get; private set; }
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Sprite[] DiceSides;
    private UnityEngine.UI.Image DiceImage;

    [HideInInspector] public AbstractSkillSlot currentSlot; 


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        DiceImage = GetComponent<Image>();


        ChangeDieParent(transform.parent); //inital paraen set

        
    }


    private void Start()
    {
        StartCoroutine(RollTheDice2());
    }

    public void Initialize(int value, DiceType type)
    {
        DieResult = value;
        Type = type;
    }



    public IEnumerator RollTheDice2()
    {
        Debug.Log(DiceSides.Length);

        int Dice1RandomSide = 0;

        for (int i = 0; i <= 10; i++)
        {

            Dice1RandomSide = UnityEngine.Random.Range(1, 7);

            // Set sprite to upper face of dice from array according to random value
            DiceImage.sprite = DiceSides[Dice1RandomSide -1 ];


            // Pause before next itteration
            yield return new WaitForSeconds(0.1f);
        }

        DieResult = Dice1RandomSide;

        Debug.Log(DieResult);
    }






    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform, true); // change parent while moving the die for clean movment
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas.transform) // if the new parent is still the canvas, return to original slot (currentSlot)
        {
            ChangeDieParent(currentSlot.transform);
        }
    }

    public void ChangeDieParent(Transform newParent)
    {
        transform.SetParent(newParent);
        rectTransform.anchoredPosition = Vector2.zero;

        AbstractSkillSlot newSlot = newParent.GetComponent<AbstractSkillSlot>();

        if (newSlot != null)
        {
            currentSlot = newSlot;
        }
    }
}
