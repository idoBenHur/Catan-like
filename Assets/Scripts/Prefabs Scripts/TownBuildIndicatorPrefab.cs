using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownBuildIndicatorPrefab : MonoBehaviour
{
    private Vector3 cornerPosition;
    private CornersClass ThisCorner;

    public void Setup(CornersClass Corner)
    {
        cornerPosition = Corner.Position;
        ThisCorner = Corner;
        // Optional: Add visual/audio feedback
    }

    void OnMouseDown()
    {
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
}
