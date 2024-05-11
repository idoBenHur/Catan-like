using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public string header;

    //[Multiline()]
    [TextArea(3, 10)]
    public string text;


    public void OnPointerEnter(PointerEventData eventData)
    {


        ToolTipSystem.Show(text, header);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //LeanTween.cancel(delay.uniqueId);
        ToolTipSystem.Hide();
    }
}
