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
    private List<GenericBoon> AvailableBoons;
    private List<GenericBoon> activeBoons = new List<GenericBoon>();
    public Button[] boonButtonsList;
    private GenericBoon[] OfferedBoons = new GenericBoon[3];






    // other







    private void Start()
    {
        //
        


    }



    public void InitializeBoonManager(PlayerClass playerInstance) //  HAPPENS BEFORE THIS SCRIPT'S "START" FUNCTION
    {
        AvailableBoons = new List<GenericBoon>(allBoons);
        player = playerInstance;


    }



    private void OnDestroy()
    {
        DeactiveAllBoons();
    }






    public void GiveBoon()
    {
        SetBoonsButtonWithRandomBoons();
        uiManager.OpenAndCloseBoonSelectionScreenAnimations(true);

    }




    public void SetBoonsButtonWithRandomBoons()
    {
        List<GenericBoon> availableBoons = new List<GenericBoon>(AvailableBoons);
        

        for (int i = 0; i < boonButtonsList.Length; i++)
        {
            
            if (availableBoons.Count != 0)
            {
                int randomIndex = Random.Range(0, availableBoons.Count);
                OfferedBoons[i] = availableBoons[randomIndex];
            }


            TextMeshProUGUI boonText = boonButtonsList[i].GetComponentInChildren<TextMeshProUGUI>();
            Image boonImage = boonButtonsList[i].transform.GetChild(1).GetComponent<Image>();

            boonText.text = OfferedBoons[i].description;
            boonImage.sprite = OfferedBoons[i].boonImage;
            boonImage.color = OfferedBoons[i].boonColor;

            availableBoons.Remove(OfferedBoons[i]);

            


            // Remove the chosen boon to avoid duplicate selections
        }

    }



    public void ChooseBoonButton1() { ChooseBoon(0); }
    public void ChooseBoonButton2() { ChooseBoon(1); }
    public void ChooseBoonButton3() { ChooseBoon(2); }

    private void ChooseBoon(int index)
    {
        GenericBoon selectedBoon = OfferedBoons[index];
        uiManager.OpenAndCloseBoonSelectionScreenAnimations(false);

        ActivateBoon(selectedBoon);
        AvailableBoons.Remove(selectedBoon);

        
        //CheckBoonMilestones(); // re-call the CheckBoonMilestones to cover "VP overflow" case

       // Invoke("CheckBoonMilestones", 1.0f);

        Debug.Log($"Chosen boon: {selectedBoon.boonName}");




    }





    public void ActivateBoon(GenericBoon boon)
    {
        if (!activeBoons.Contains(boon))
        {
            activeBoons.Add(boon);
            uiManager.AddAndRemoveActiveBoonsDisplay(boon, true);
            boon.Activate();
            boon.StoreValues();
            
            if(boon.isCounting == true) { uiManager.UpdateBoonCounter(boon, 0); }

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
            boon.ResetToDefaults();
            Debug.Log($"Deactivated boon: {boon.boonName}");
        }
    }


    public void DeactiveAllBoons()
    {
        var tempBoonList = new List<GenericBoon>(activeBoons);
        foreach(var boon in tempBoonList)
        {
            DeactivateBoon(boon);
            boon.ResetToDefaults();
            
        }
    }


}
