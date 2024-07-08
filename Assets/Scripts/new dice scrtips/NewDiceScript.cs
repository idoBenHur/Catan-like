using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewDiceScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private Sprite[] DiceSides;
    private UnityEngine.UI.Image DiceImage;
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
   // private bool DiceStilRolling = false;
    private Transform originalParent;
    public int DiceResult;



    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        originalPosition = transform.position;
        originalParent = transform.parent;
        DiceImage = GetComponent<Image>();

        StartCoroutine(RollTheDice());
        
    }



    public IEnumerator RollTheDice()
    {
       // DiceStilRolling = true;
        int Dice1RandomSide = 0;

        for (int i = 0; i <= 10; i++)
        {

            Dice1RandomSide = UnityEngine.Random.Range(1, 7);

            // Set sprite to upper face of dice from array according to random value
            DiceImage.sprite = DiceSides[Dice1RandomSide - 1];
            

            // Pause before next itteration
            yield return new WaitForSeconds(0.1f);
        }

        DiceResult = Dice1RandomSide;


       //  DiceStilRolling = false;



    }



    void Update()
    {
        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            transform.position = targetPosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BoardManager.instance.uiManager.CloseAllUi();
        originalPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        isDragging = true;
        transform.SetParent(canvas.transform);
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // OnDrag is required for IDragHandler but can be left empty
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        if (!IsDroppedInBox(out Transform newParent))
        {
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }
        else if (newParent.CompareTag("DestroyBox"))
        {
            Destroy(gameObject);
           // BoardManager.instance.AddOneToUnluckyMeter();
        }
        else
        {
            transform.SetParent(newParent);
            transform.localPosition = Vector3.zero;
        }
    }

    private bool IsDroppedInBox(out Transform boxTransform)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Box"))
            {
                boxTransform = result.gameObject.transform;
                return true;
            }
            if (result.gameObject.CompareTag("LimitedBox"))
            {
                if (result.gameObject.transform.childCount < 2)
                {
                    boxTransform = result.gameObject.transform;
                    return true;
                }
            }
            if (result.gameObject.CompareTag("DestroyBox"))
            {
                boxTransform = result.gameObject.transform;
                return true;
            }
        }

        boxTransform = null;
        return false;
    }
}
