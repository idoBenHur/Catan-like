using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildIndicator : MonoBehaviour
{
    private Vector3 cornerPosition;

    public void Setup(Vector3 position)
    {
        cornerPosition = position;
        // Optional: Add visual/audio feedback
    }

    void OnMouseDown()
    {
        // Communicate back to build the settlement at `cornerPosition`

        BoardManager.instance.BuildSettlementAt(cornerPosition);
        //FindObjectOfType<BoardManager>().BuildSettlementAt(cornerPosition);
        Destroy(gameObject); // Remove the indicator after building
    }
}
