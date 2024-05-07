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
    private List<int> boonMilestones;




    // other

    [SerializeField] private int VPForFirstBoon = 3;
    [SerializeField] private int VPForSecondBoon = 5;
    [SerializeField] private int VPForThirdBoon = 7;
    [SerializeField] private int VPForForthBoon = 10;
    [SerializeField] private int VPForFifthBoon = 13;
    [SerializeField] private int lastVPChecked = 0;
    [SerializeField] public int NextVPforboon;
    private bool BoonPannelOpen = false;







    private void Start()
    {
        AvailableBoons = new List<GenericBoon>(allBoons);
        
    }
    private void buttonclick()
    {
        //ActivateBoon(Boon);
        SetBoonsButtonWithRandomBoons();
    }


    public void SetPlayerInBoonManager(PlayerClass playerInstance)
    {
        boonMilestones = new List<int> { VPForFirstBoon, VPForSecondBoon, VPForThirdBoon, VPForForthBoon, VPForFifthBoon };
        player = playerInstance;
        player.OnVictoryPointsChanged += CheckBoonMilestones;
    }



    private void OnDestroy()
    {
        DeactiveAllBoons();
    }



    private void CheckBoonMilestones(int CurrnetVictoryPoints)
    {

        Debug.Log("boon pannel open: " + BoonPannelOpen);
        if (BoonPannelOpen == true) { return; }
        

        // check if the player should get a boon
        boonMilestones.Sort();
        foreach (var milestone in boonMilestones)
        {
            if (CurrnetVictoryPoints >= milestone && lastVPChecked < milestone)
            {
                lastVPChecked = milestone; 
                SetBoonsButtonWithRandomBoons();
                uiManager.OpenAndCloseBoonSelectionScreen();
                BoonPannelOpen = true;
                break;
            }
        }


        //find the next milestone
        foreach (int milestone in boonMilestones)
        {
            if (CurrnetVictoryPoints < milestone)
            {
                NextVPforboon = milestone;
                break;  
            }
        }




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
        uiManager.OpenAndCloseBoonSelectionScreen();
        BoonPannelOpen = false;

        ActivateBoon(selectedBoon);
        AvailableBoons.Remove(selectedBoon);

        int currnentVP = player.VictoryPoints;
        CheckBoonMilestones(currnentVP); // re-call the CheckBoonMilestones to cover "VP overflow" case

        Debug.Log($"Chosen boon: {selectedBoon.boonName}");


    }





    public void ActivateBoon(GenericBoon boon)
    {
        if (!activeBoons.Contains(boon))
        {
            activeBoons.Add(boon);
            boon.Activate();
            boon.StoreValues();
            uiManager.AddAndRemoveActiveBoonsDisplay(boon, true);
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
