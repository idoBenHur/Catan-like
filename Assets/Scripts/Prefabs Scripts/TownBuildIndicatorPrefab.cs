using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TownBuildIndicatorPrefab : MonoBehaviour
{
    private Vector3 cornerPosition;
    private Vector3 initialScale;

    private CornersClass ThisCorner;

    public void Setup(CornersClass Corner)
    {
        cornerPosition = Corner.Position;
        ThisCorner = Corner;
        // Optional: Add visual/audio feedback
    }



    private void Start()
    {
        initialScale = transform.localScale;
        transform.DOScale(initialScale * 1.1f, 0.5f)
           .SetLoops(-1, LoopType.Yoyo) // Loop forever, scaling back and forth
           .SetEase(Ease.InOutSine);
    }


    private void OnDestroy()
    {
        DOTween.Kill(transform);

    }

    void OnMouseDown()
    {

        if (IsPointerOverUIObject())
        {
           // return; // If it's over a UI element, don't process the click
        }

        // Communicate back to build the settlement at `cornerPosition`

        if (ThisCorner.HasSettlement == false)
        {
            BoardManager.instance.BuildSettlementAt(cornerPosition);
        }
        else if(ThisCorner.HasSettlement == true && ThisCorner.HasCityUpgade == false)
        {
            BoardManager.instance.UpgradeSettelmentToCity(ThisCorner);
        }
        


    }



    private bool IsPointerOverUIObject()
    {
        // Check if the current mouse position is over a UI element
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
