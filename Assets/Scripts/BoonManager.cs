using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonManager : MonoBehaviour
{
    private List<GenericBoon> activeBoons = new List<GenericBoon>();

    public GenericBoon Boon;

    public PlayerClass player;

    // Method to activate a boon



    public void buttonclick()
    {
        ActivateBoon(Boon);
    }

    private void OnDestroy()
    {
        DeactiveAllBoons();
    }

    public void ActivateBoon(GenericBoon boon)
    {
        if (!activeBoons.Contains(boon))
        {
            activeBoons.Add(boon);
            boon.Activate();
            Debug.Log($"Activated boon: {boon.boonName}");
        }
    }

    // Method to deactivate a boon
    public void DeactivateBoon(GenericBoon boon)
    {
        if (activeBoons.Contains(boon))
        {
            boon.Deactivate();
            activeBoons.Remove(boon);
            Debug.Log($"Deactivated boon: {boon.boonName}");
        }
    }


    public void DeactiveAllBoons()
    {
        var tempBoonList = new List<GenericBoon>(activeBoons);
        foreach(var boon in tempBoonList)
        {
            DeactivateBoon(boon);
        }
    }


}
