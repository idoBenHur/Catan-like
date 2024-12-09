using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;








public class TheDiceScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int DieResult { get; private set; }
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Sprite[] DiceSides;

    private GameObject diceVisualsParent;
    private GameObject visualInstance;
    private Vector3 visualInstanceOGScale;
    [SerializeField] private GameObject DiceImageChild;
    private DiceRollAnimation DiceRollAnimation;


    [HideInInspector] public bool DraggableActive = true;

    [HideInInspector] public AbstractSkillSlot currentSlot;


    public GenericBoon specialDie;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

    }


    public void initializeDie(GenericBoon diceEffect = null, int? ForcedResult = null)
    {
        DraggableActive = true;
        ChangeDieParent(transform.parent); //inital paraen set

        specialDie = diceEffect;

        PickNumber(ForcedResult);




        SetupDieVisuals();

    }


    private void SetupDieVisuals() 
    {




        diceVisualsParent = GameObject.FindWithTag("DiceVisuals");
        visualInstance = Instantiate(DiceImageChild, diceVisualsParent.transform);
        visualInstanceOGScale = visualInstance.transform.localScale;

        DiceRollAnimation = visualInstance.GetComponent<DiceRollAnimation>();
        DiceRollAnimation.NewAnimation(DieResult);


        if (specialDie != null)
        {
            // DiceImage.sprite = specialDie.boonImage; // Will work nice with new assets!
            visualInstance.GetComponent<Image>().color = specialDie.boonColor;
        }



    }




    public void PlayDice()
    {
        // 
        if (specialDie != null)
        {
            specialDie.Activate2();
        }
    }






    void Update()
    {
        if (visualInstance != null && visualInstance.transform.position != this.transform.position)
        {
            visualInstance.transform.position = Vector3.Lerp(visualInstance.transform.position, this.transform.position, Time.deltaTime * 15f);
        }
    }




    private void Start()
    {


        
    }

 


    private void OnDestroy()
    {
       
        DOTween.Kill(visualInstance.transform);
        Destroy(visualInstance);
    }

    private void PickNumber(int? ForcedResult = null)
    {

        if (ForcedResult != null)
        {
            DieResult = ForcedResult.Value;

        }
        else
        {
            DieResult = UnityEngine.Random.Range(1, 7);
        }

        
       // StartCoroutine(DiceRollAnimation.RollDiceAnimation2(DieResult));
        // StartCoroutine(RollDiceAnimation());
    }










    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DraggableActive == false) { return; }


        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform, true); // change parent while moving the die for clean movment
      //  currentSlot.RemoveDiceFromDiceList(this);
       // BoardManager.instance.skillSlotManager.allDicesOutcome();



    }

    public void OnDrag(PointerEventData eventData)
    {

        if (DraggableActive == false) { return; }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;


        float tilt = eventData.delta.x * 5f * 0.1f; // Tilt based on the horizontal drag (adjust factor for more/less tilt)

        // Apply the tilt (adjust Z-axis rotation based on drag direction)
        visualInstance.transform.eulerAngles = new Vector3(0, 0, tilt);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (DraggableActive == false) { return; }

        if (transform.parent == canvas.transform) // if the new parent is still the canvas, return to original slot (currentSlot)
        {
            ChangeDieParent(currentSlot.transform);
           // currentSlot.AddDiceToSlotList(this);

        }

        //  BoardManager.instance.skillSlotManager.allDicesOutcome();


        visualInstance.transform.rotation = Quaternion.identity;


    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        visualInstance.transform.DOScale(visualInstanceOGScale * 1.2f, 0.3f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {


        visualInstance.transform.DOScale(visualInstanceOGScale, 0.3f);

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
