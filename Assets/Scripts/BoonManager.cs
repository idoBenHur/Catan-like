using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonManager : MonoBehaviour
{
    private List<Boon> activeBoons = new List<Boon>();

    public Boon Boon;

    public PlayerClass player;

    // Method to activate a boon



    public void buttonclick()
    {
        ActivateBoon(Boon);
    }

    public void ActivateBoon(Boon boon)
    {
        if (!activeBoons.Contains(boon))
        {
            activeBoons.Add(boon);
            boon.Activate(player);
            Debug.Log($"Activated boon: {boon.boonName}");
        }
    }

    // Method to deactivate a boon
    public void DeactivateBoon(Boon boon)
    {
        if (activeBoons.Contains(boon))
        {
            boon.Deactivate(player);
            activeBoons.Remove(boon);
            Debug.Log($"Deactivated boon: {boon.boonName}");
        }
    }


}
