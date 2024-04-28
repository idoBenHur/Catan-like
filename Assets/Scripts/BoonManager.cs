using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoonManager : MonoBehaviour
{


    //scripts
    private PlayerClass player;
    public UiManager uiManager;


    // lists

    public List<GenericBoon> allBoons;
    private List<GenericBoon> activeBoons = new List<GenericBoon>();
    public Button[] boonButtonsList;
    private GenericBoon[] OfferedBoons = new GenericBoon[3];
    

    // other
    
    public int VPForFirstBoon = 3;
    public int VPForSecondBoon = 5;
    public int VPForThirdBoon = 7;
    public int VPForForthBoon = 10;
    public int VPForFifthBoon = 13;
    public int NextVPforboon;




    private void Start()
    {
    }
    private void buttonclick()
    {
        //ActivateBoon(Boon);
        SetBoonsButtonWithRandomBoons();
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
     
        if(CurrnetVictoryPoints == 0)
        {
            NextVPforboon = VPForFirstBoon;
        }
        if(CurrnetVictoryPoints == VPForFirstBoon)
        {
            NextVPforboon = VPForSecondBoon;
            SetBoonsButtonWithRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }
        else if (CurrnetVictoryPoints == VPForSecondBoon)
        {
            NextVPforboon = VPForThirdBoon;
            SetBoonsButtonWithRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }
        else if (CurrnetVictoryPoints == VPForThirdBoon)
        {
            NextVPforboon = VPForForthBoon;
            SetBoonsButtonWithRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }
        else if (CurrnetVictoryPoints == VPForForthBoon)
        {
            NextVPforboon = VPForFifthBoon;
            SetBoonsButtonWithRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }
        else if (CurrnetVictoryPoints == VPForFifthBoon)
        {
           // NextVPforboon = VPForFifthBoon;
            SetBoonsButtonWithRandomBoons();
            uiManager.OpenAndCloseBoonSelectionScreen();
        }



    }




    public void SetBoonsButtonWithRandomBoons()
    {
        List<GenericBoon> availableBoons = new List<GenericBoon>(allBoons);
        for (int i = 0; i < boonButtonsList.Length; i++)
        {
            int randomIndex = Random.Range(0, availableBoons.Count);
            OfferedBoons[i] = availableBoons[randomIndex];

            TextMeshProUGUI boonText = boonButtonsList[i].GetComponentInChildren<TextMeshProUGUI>();
            Image boonImage = boonButtonsList[i].transform.GetChild(1).GetComponent<Image>();

            boonText.text = OfferedBoons[i].description;
            boonImage.sprite = OfferedBoons[i].boonImage;
            boonImage.color = OfferedBoons[i].boonColor;

            



            availableBoons.RemoveAt(randomIndex); // Remove the chosen boon to avoid duplicate selections
        }

    }



    public void ChooseBoonButton1() { ChooseBoon(0); }
    public void ChooseBoonButton2() { ChooseBoon(1); }
    public void ChooseBoonButton3() { ChooseBoon(2); }

    private void ChooseBoon(int index)
    {
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
            uiManager.AddAndRemoveActiveBoonsDisplay(boon, true);

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
