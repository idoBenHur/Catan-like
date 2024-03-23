using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Communicate back to build the settlement at `cornerPosition`

        BoardManager.instance.BuildRoadAt(SidePosition);
        //FindObjectOfType<BoardManager>().BuildSettlementAt(cornerPosition);
        //Destroy(gameObject); // Remove the indicator after building
    }

}
