using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RobberPrefab : MonoBehaviour
{
    public TileClass currentTile; // The tile this robber is on
    public static int SelctedRobbersCounter = 0;
    private bool IsSelected = false;
    public GameObject SelectedIndicatorGameobject;
    


    private void OnMouseDown()
    {

        UiManager.UIinstance.RobberInteraction(this);




    }


 
}
