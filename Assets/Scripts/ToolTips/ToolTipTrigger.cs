using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //private static LTDescr delay;
    public string header;

    [Multiline()]
    public string text;


    public void OnPointerEnter(PointerEventData eventData)
    {
        //delay= LeanTween.delayedCall(0.5f, () =>
        //{
        //    ToolTipSystem.Show(text, header);

        //});

        ToolTipSystem.Show(text, header);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //LeanTween.cancel(delay.uniqueId);
        ToolTipSystem.Hide();
    }
}
