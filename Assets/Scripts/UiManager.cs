using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TileClass;

public class UiManager : MonoBehaviour
{
    bool ButtonPressedShowTownBuildIndicators = false;
    bool ButtonPressedShowRoadBuildIndicators = false;

    //inventory UI:
    [SerializeField] private PlayerClass player;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI brickText;
    [SerializeField] private TextMeshProUGUI sheepText;
    [SerializeField] private TextMeshProUGUI oreText;
    [SerializeField] private TextMeshProUGUI wheatText;
    [SerializeField] private TextMeshProUGUI DiceDisplay;
    [SerializeField] private TextMeshProUGUI VictoryPoints;

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

    public void ShowTownBuildIndicatorsButton()
    {

        if (ButtonPressedShowTownBuildIndicators == false)
        {
            ButtonPressedShowTownBuildIndicators = true;
            BoardManager.instance.ShowBuildIndicatorsTowns();
            return;
        }

        if (ButtonPressedShowTownBuildIndicators == true)
        {
            ButtonPressedShowTownBuildIndicators = false;
            foreach (var prefab in BoardManager.instance.CornersIndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            return;
        }
    }


    public void ShowRoadBuildIndicatorsButton()
    {
        if (ButtonPressedShowRoadBuildIndicators == false)
        {
            ButtonPressedShowRoadBuildIndicators= true;
            BoardManager.instance.ShowBuildIndicatorsRoads();
            return;

        }

        if (ButtonPressedShowRoadBuildIndicators == true)
        {
            ButtonPressedShowRoadBuildIndicators = false;

            foreach (var prefab in BoardManager.instance.SidesIndicatorsPrefabList)
            {
                Destroy(prefab.gameObject);
            }
            return;

        }

    }

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


    public void RollDiceButton()
    {
        BoardManager.instance.DiceRoll();
    }



    private void SetupButtonListeners()
    {

        // trade with bank toggels:

        offerWoodToggle.onValueChanged.AddListener((isSelected) => SetTradeOffer(isSelected, ResourceType.Wood, offerWoodToggle));
        offerBrickToggle.onValueChanged.AddListener((isSelected) => SetTradeOffer(isSelected, ResourceType.Brick, offerBrickToggle));
        offerSheepToggle.onValueChanged.AddListener((isSelected) => SetTradeOffer(isSelected, ResourceType.Sheep, offerSheepToggle));
        offerOreToggle.onValueChanged.AddListener((isSelected) => SetTradeOffer(isSelected, ResourceType.Ore, offerOreToggle));
        offerWheatToggle.onValueChanged.AddListener((isSelected) => SetTradeOffer(isSelected, ResourceType.Wheat, offerWheatToggle));

        requestWoodToggle.onValueChanged.AddListener((isSelected) => SetRequest(isSelected, ResourceType.Wood, requestWoodToggle));
        requestBrickToggle.onValueChanged.AddListener((isSelected) => SetRequest(isSelected, ResourceType.Brick, requestBrickToggle));
        requestSheepToggle.onValueChanged.AddListener((isSelected) => SetRequest(isSelected, ResourceType.Sheep, requestSheepToggle));
        requestOreToggle.onValueChanged.AddListener((isSelected) => SetRequest(isSelected, ResourceType.Ore, requestOreToggle));
        requestWheatToggle.onValueChanged.AddListener((isSelected) => SetRequest(isSelected, ResourceType.Wheat, requestWheatToggle));


        // pay robber buttons:

        PayRobberWoodButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Wood));
        PayRobberBrickButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Brick));
        PayRobberSheepButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Sheep));
        PayRobberOreButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Ore));
        PayRobberWheatButton.onClick.AddListener(() => CallRemoveRobber(ResourceType.Wheat));
    }





    //trade with bank functions


    private void SetTradeOffer(bool isSelected, ResourceType type, Toggle toggle)
    {
        if (isSelected)
        {
            offerType = type;
            UncheckOtherToggles(toggle, true);
        }
        else if (offerType == type)
        {
            offerType = null;
        }

        CheckTradeValidity();
    }

    private void SetRequest(bool isSelected, ResourceType type, Toggle toggle)
    {
        if (isSelected)
        {
            requestType = type;
            UncheckOtherToggles(toggle, false);
        }
        else if (requestType == type)
        {
            requestType = null;
        }

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
        bool isValidTrade = offerType.HasValue && requestType.HasValue && player.CanTradeWithBank(offerType.Value);

        tradeButton.interactable = isValidTrade; // Enable the button only if the trade is valid

        if (isValidTrade)
        {
            Debug.Log($"Ready to trade {offerType.Value} for {requestType.Value}");
        }
    }


    public void ExecuteTradeButton() // called by button pressed
    {

        if (offerType.HasValue && requestType.HasValue && player.CanTradeWithBank(offerType.Value))
        {
            player.TradeWithBank(offerType.Value, requestType.Value);
            // After trade, you might want to reset or update the UI
            CheckTradeValidity(); // To refresh the UI state
        }

    }








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

}
