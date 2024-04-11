using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TileClass;

public class UiManager : MonoBehaviour
{

    // build menu

    [SerializeField] private Toggle RoadIndicatorsToggle;
    [SerializeField] private Toggle TownIndicatorsToggle;
    [SerializeField] private Toggle CityIndicatorsToggle;
    private bool isUpdatingToggles = false;




    //inventory UI:
    [SerializeField] private PlayerClass player;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI brickText;
    [SerializeField] private TextMeshProUGUI sheepText;
    [SerializeField] private TextMeshProUGUI oreText;
    [SerializeField] private TextMeshProUGUI wheatText;
    [SerializeField] private TextMeshProUGUI DiceDisplay;
    [SerializeField] private TextMeshProUGUI VictoryPoints;
    [SerializeField] private Slider TurnSlider;

    private ResourceType? offerType = null;
    private ResourceType? requestType = null;


    //trade buttons:

    [SerializeField] private Button tradeButton;

    [SerializeField] private Toggle offerWoodToggle;
    [SerializeField] private Toggle offerBrickToggle;
    [SerializeField] private Toggle offerSheepToggle;
    [SerializeField] private Toggle offerOreToggle;
    [SerializeField] private Toggle offerWheatToggle;

    [SerializeField] private Toggle requestWoodToggle;
    [SerializeField] private Toggle requestBrickToggle;
    [SerializeField] private Toggle requestSheepToggle;
    [SerializeField] private Toggle requestOreToggle;
    [SerializeField] private Toggle requestWheatToggle;
    

    // pay robber buttons
    [SerializeField] private Button PayRobberWoodButton;
    [SerializeField] private Button PayRobberBrickButton;
    [SerializeField] private Button PayRobberSheepButton;
    [SerializeField] private Button PayRobberOreButton;
    [SerializeField] private Button PayRobberWheatButton;

    private RobberPrefab selectedRobber;
    [SerializeField] private GameObject PayRobberUiPannel;



    public static UiManager UIinstance;


    private void Awake()
    {
        if (UIinstance == null)
        {
            UIinstance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }


    private void Start()
    {
        SetupButtonListeners();
    }

    public void SetPlayerInUIManager(PlayerClass playerInstance)
    {
        player = playerInstance;
        player.OnResourcesChanged += UpdateResourceDisplay;
        UpdateResourceDisplay();  // Initial display update
    }


    void OnDestroy()
    {
        if (player != null)
        {
            player.OnResourcesChanged -= UpdateResourceDisplay;
        }
    }



    public void CloseAllUi(Toggle CurrentToggle = null)
    {
        if (isUpdatingToggles)
        {
            return;
        }

        isUpdatingToggles = true;




        // destroy indicators prefabs
        if (BoardManager.instance.TownsIndicatorsPrefabList.Count > 0)
        {
            foreach (var prefab in BoardManager.instance.TownsIndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            BoardManager.instance.TownsIndicatorsPrefabList.Clear();
        }

        if (BoardManager.instance.RoadsIndicatorsPrefabList.Count > 0)
        {
            foreach (var prefab in BoardManager.instance.RoadsIndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            BoardManager.instance.RoadsIndicatorsPrefabList.Clear();
        }

        if (BoardManager.instance.CitiesIndicatorsPrefabList.Count > 0)
        {
            foreach (var prefab in BoardManager.instance.CitiesIndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            BoardManager.instance.CitiesIndicatorsPrefabList.Clear();
        }


        // close all toggles
        bool wasOn = CurrentToggle != null && CurrentToggle.isOn;

        TownIndicatorsToggle.isOn = false;
        RoadIndicatorsToggle.isOn = false;
        CityIndicatorsToggle.isOn = false;

        if (wasOn == true)
        {

            CurrentToggle.isOn = true;
        }

        isUpdatingToggles = false;



    }


    // build roads/towns buttons

    public void ShowTownBuildIndicatorsToggle()
    {

    
        
        if (TownIndicatorsToggle.isOn == true)
        {
            CloseAllUi(TownIndicatorsToggle);
            BoardManager.instance.ShowBuildIndicatorsTowns();

            if(BoardManager.instance.TownsIndicatorsPrefabList.Count == 0)
            {
                TownIndicatorsToggle.isOn = false;
            }

        }
        else
        {
            CloseAllUi();
        }


    }


    public void ShowRoadBuildIndicatorsToggle()
    {

        
        if (RoadIndicatorsToggle.isOn == true)
        {
            BoardManager.instance.ShowBuildIndicatorsRoads();
            CloseAllUi(RoadIndicatorsToggle);
        }
        else
        {
            CloseAllUi();
        }



    }


    public void ShowCityBuildIndicatorToggle()
    {

        
        if (CityIndicatorsToggle.isOn == true)
        {
            CloseAllUi(CityIndicatorsToggle);
            BoardManager.instance.ShowCityUpgradeIndicators();

            

        }
        else
        {
            CloseAllUi();
        }


    }


    //




    private void UpdateResourceDisplay()
    {
        woodText.text = player.PlayerResources[ResourceType.Wood].ToString();
        brickText.text = player.PlayerResources[ResourceType.Brick].ToString();
        sheepText.text = player.PlayerResources[ResourceType.Sheep].ToString();
        oreText.text = player.PlayerResources[ResourceType.Ore].ToString();
        wheatText.text = player.PlayerResources[ResourceType.Wheat].ToString();

    }

    public void UpdateVictoryPointsDisplay()
    {
        VictoryPoints.text = player.VictoryPoints.ToString();

    }
    public void UpdateDiceRollDisplay(int DiceResult)
    {
        DiceDisplay.text = DiceResult.ToString();
    }

    public void UpdateTurnSliderDisplay(int CurrentTurn)
    {


        TurnSlider.value = (float)CurrentTurn;


    }



    public void RollDiceButton() 
    {
        BoardManager.instance.DiceRoll();
    }



    private void SetupButtonListeners()
    {

        // trade with bank toggels:

        offerWoodToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Wood, offerWoodToggle, true));
        offerBrickToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Brick, offerBrickToggle, true));
        offerSheepToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Sheep, offerSheepToggle, true));
        offerOreToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Ore, offerOreToggle, true));
        offerWheatToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Wheat, offerWheatToggle, true));

        requestWoodToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Wood, requestWoodToggle,false));
        requestBrickToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Brick, requestBrickToggle, false));
        requestSheepToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Sheep, requestSheepToggle, false));
        requestOreToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Ore, requestOreToggle, false));
        requestWheatToggle.onValueChanged.AddListener((isSelected) => SetTradeSelection(isSelected, ResourceType.Wheat, requestWheatToggle, false));


        // pay robber buttons:

        PayRobberWoodButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Wood));
        PayRobberBrickButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Brick));
        PayRobberSheepButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Sheep));
        PayRobberOreButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Ore));
        PayRobberWheatButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Wheat));
    }





    //trade with bank functions






    private void SetTradeSelection(bool isSelected, ResourceType type, Toggle toggle, bool isOffer)
    {
        if (isSelected == true)
        {
            // Determine if this is setting an offer or request and update accordingly
            if (isOffer == true)
            {
                offerType = type; 
            }
            else
            {
                requestType = type; 
            }

            // Ensure that other toggles in the same category (offer or request) are unchecked
            UncheckOtherToggles(toggle, isOffer);
        }
        else
        {
            // If the toggle is deselected, clear the offer or request if it matches the current selection
            if ((isOffer && offerType == type) || (!isOffer && requestType == type))
            {
                if (isOffer)
                {
                    offerType = null; 
                }
                else
                {
                    requestType = null; 
                }
            }
        }

        // Check if a valid trade can be made with the current selections
        CheckTradeValidity();
    }






    private void UncheckOtherToggles(Toggle changedToggle, bool isOfferToggle)
    {
        Toggle[] toggles = isOfferToggle ? new Toggle[] { offerWoodToggle, offerBrickToggle, offerSheepToggle, offerOreToggle, offerWheatToggle }
                                          : new Toggle[] { requestWoodToggle, requestBrickToggle, requestSheepToggle, requestOreToggle, requestWheatToggle };

        foreach (var toggle in toggles)
        {
            if (toggle != changedToggle)
            {
                toggle.isOn = false;
            }
        }
    }




    private void CheckTradeValidity()
    {
        //bool isValidTrade = offerType.HasValue && requestType.HasValue && player.CanTradeWithBank(offerType.Value);
        bool isValidTrade = offerType.HasValue && requestType.HasValue && player.PlayerResources[offerType.Value] >= 4;


        tradeButton.interactable = isValidTrade; // Enable the button only if the trade is valid

        if (isValidTrade)
        {
            Debug.Log($"Ready to trade {offerType.Value} for {requestType.Value}");
        }
    }


    public void ExecuteTradeButton() // called by button pressed
    {

        
        if (offerType.HasValue && requestType.HasValue && player.PlayerResources[offerType.Value] >= 4)
        {
            player.TradeWithBank(offerType.Value, requestType.Value);
            // After trade, you might want to reset or update the UI
            CheckTradeValidity(); // To refresh the UI state
        }
        
        
     
        

    }






    // robber

    public void RobberInteraction(RobberPrefab robber)
    {



        if (PayRobberUiPannel.activeSelf == true)
        {
            PayRobberUiPannel.SetActive(false);
            return;
        }

       
        
        selectedRobber = robber;
        bool isInteracting = robber;


        PayRobberUiPannel.SetActive(isInteracting);


        if (isInteracting)
        {
            PayRobberWoodButton.interactable = player.PlayerResources[ResourceType.Wood] >= 4;
            PayRobberBrickButton.interactable = player.PlayerResources[ResourceType.Brick] >= 4;
            PayRobberSheepButton.interactable = player.PlayerResources[ResourceType.Sheep] >= 4;
            PayRobberOreButton.interactable = player.PlayerResources[ResourceType.Ore] >= 4;
            PayRobberWheatButton.interactable = player.PlayerResources[ResourceType.Wheat] >= 4;

        }
        
    }





    private void CallRemoveRobber(ResourceType resourceType)
    {
        PayRobberUiPannel.SetActive(false);
        BoardManager.instance.RemoveRobber(selectedRobber, resourceType);
//        RobberInteraction(null);

    }


    //



}
