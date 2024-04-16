using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoonManager : MonoBehaviour
{
    public List<GenericBoon> allBoons;
    private List<GenericBoon> activeBoons = new List<GenericBoon>();
    public Button[] boonButtons;
    private GenericBoon[] OfferedBoons = new GenericBoon[3];
    public UiManager uiManager;

    public GenericBoon Boon;

    private PlayerClass player;

    // Method to activate a boon



    public void buttonclick()
    {
        //ActivateBoon(Boon);
        PullRandomBoons();
    }


    public void SetPlayerInBoonManager(PlayerClass playerInstance)
    {
        player = playerInstance;
        player.OnVictoryPointsChanged += BoonMeter;
    }



    private void OnDestroy()
    {
        DeactiveAllBoons();
    }



    private void BoonMeter(int CurrnetVictoryPoints)
    {
        if(CurrnetVictoryPoints == 3)
        {
            PullRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }
    }




    public void PullRandomBoons()
    {
        List<GenericBoon> availableBoons = new List<GenericBoon>(allBoons);
        for (int i = 0; i < boonButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, availableBoons.Count);
            OfferedBoons[i] = availableBoons[randomIndex];

            TextMeshProUGUI tmp = boonButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = OfferedBoons[i].description;
            
            availableBoons.RemoveAt(randomIndex); // Remove the chosen boon to avoid duplicate selections
        }

    }



    public void ChooseBoonButton1() { ChooseBoon(0); }
    public void ChooseBoonButton2() { ChooseBoon(1); }
    public void ChooseBoonButton3() { ChooseBoon(2); }

    private void ChooseBoon(int index)
    {
        Debug.Log("hi");
        GenericBoon selectedBoon = OfferedBoons[index];
        uiManager.OpenAndCloseBoonSelectionScreen();
        ActivateBoon(selectedBoon);

        // Activate the chosen boon here and add it to the player's active boons
        Debug.Log($"Chosen boon: {selectedBoon.boonName}");


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
