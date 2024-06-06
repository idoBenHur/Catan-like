using DG.Tweening;
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
    private Tween tooltipTween;


    public void OnPointerEnter(PointerEventData eventData)
    {


        //ToolTipSystem.Show(text, header);
        tooltipTween = DOVirtual.DelayedCall(0.3f, () => ToolTipSystem.Show(text, header));


    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (tooltipTween != null && tooltipTween.IsActive())
        {
            tooltipTween.Kill();
        }

        ToolTipSystem.Hide();
    }
}
