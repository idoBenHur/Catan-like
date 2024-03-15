using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    bool ButtonPressedShowTownBuildIndicators = false;

    public void ShowTownBuildIndicatorsButton()
    {

        if (ButtonPressedShowTownBuildIndicators == false)
        {
            ButtonPressedShowTownBuildIndicators = true;
            BoardManager.instance.ShowBuildIndicators();
            return;
        }

        if (ButtonPressedShowTownBuildIndicators == true)
        {
            ButtonPressedShowTownBuildIndicators = false;
            foreach (var prefab in BoardManager.instance.IndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            return;
        }
    }
}
