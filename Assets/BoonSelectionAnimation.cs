using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoonSelectionAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 OriginalScale;

    private void Start()
    {
        OriginalScale = this.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 newScale = OriginalScale * 1.2f;
        this.transform.DOScale(newScale, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {       
        this.transform.DOScale(OriginalScale, 0.3f);
    }
}
