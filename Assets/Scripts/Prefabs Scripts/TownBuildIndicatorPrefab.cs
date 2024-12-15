using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum IndicatorType
{
    Town,
    City,
    FogRemover

}


public class TownBuildIndicatorPrefab : MonoBehaviour
{
    private Vector3 cornerPosition;
    private Vector3 initialScale;
    private CornersClass ThisCorner;
    public IndicatorType ThisIndicatorType;







    public void Setup(CornersClass Corner, IndicatorType indicatorType )
    {
        cornerPosition = Corner.Position;
        ThisCorner = Corner;
        ThisIndicatorType = indicatorType;

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

        switch (ThisIndicatorType)
        {
            case IndicatorType.Town:
                BoardManager.instance.BuildSettlementAt(cornerPosition);
                break;

            case IndicatorType.City:
                BoardManager.instance.UpgradeSettelmentToCity(ThisCorner);
                break;
            case IndicatorType.FogRemover:
                BoardManager.instance.RemoveFogAroundAcorner(ThisCorner);

                break;

        }






    }



    private bool IsPointerOverUIObject()
    {
        // Check if the current mouse position is NOT over a UI element
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
