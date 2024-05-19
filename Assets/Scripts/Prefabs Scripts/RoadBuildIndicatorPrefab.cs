using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadBuildIndicatorPrefab : MonoBehaviour
{

    private Vector3 SidePosition;



    public void Setup(Vector3 position)
    {
        SidePosition = position;
        // Optional: Add visual/audio feedback
    }

    void OnMouseDown()
    {

        if (IsPointerOverUIObject())
        {
            return; // If it's over a UI element, don't process the click
        }

        // Communicate back to build the settlement at `cornerPosition`

        BoardManager.instance.BuildRoadAt(SidePosition);
        //FindObjectOfType<BoardManager>().BuildSettlementAt(cornerPosition);
        //Destroy(gameObject); // Remove the indicator after building
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
