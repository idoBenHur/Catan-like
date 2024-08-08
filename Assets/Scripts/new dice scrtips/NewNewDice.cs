using UnityEngine;
using UnityEngine.EventSystems;




public enum DiceType
{
    Normal,
    Fire,
    Water,
    // Add more dice types as needed
}

public class NewNewDice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Value { get; private set; }
    public DiceType Type { get; private set; }

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [HideInInspector] public Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(int value, DiceType type)
    {
        Value = value;
        Type = type;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //originalParent = transform.parent;
        transform.SetParent(canvas.transform); // Detach from the parent and attach to the canvas
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If the dice is not dropped on a valid slot, return to the original parent
        if (transform.parent == originalParent)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void SetParent(Transform newParent)
    {
        transform.SetParent(newParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
