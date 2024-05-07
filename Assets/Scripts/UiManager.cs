using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TileClass;

public class UiManager : MonoBehaviour
{

    //scripts 
    public Challenges challenges;
    [SerializeField] BoonManager boonManager;



    //  menu toggles

    [SerializeField] private Toggle RoadIndicatorsToggle;
    [SerializeField] private Toggle TownIndicatorsToggle;
    [SerializeField] private Toggle CityIndicatorsToggle;
    [SerializeField] private Toggle TradeToggle;
    private bool isUpdatingToggles = false;

    // resources flying icons

    [SerializeField] private GameObject WoodFlyingIcon;
    [SerializeField] private GameObject BrickFlyingIcon;
    [SerializeField] private GameObject SheepFlyingIcon;
    [SerializeField] private GameObject OreFlyingIcon;
    [SerializeField] private GameObject WheatFlyingIcon;


    //screen UI:
    private PlayerClass player;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI brickText;
    [SerializeField] private TextMeshProUGUI sheepText;
    [SerializeField] private TextMeshProUGUI oreText;
    [SerializeField] private TextMeshProUGUI wheatText;
    [SerializeField] private TextMeshProUGUI DiceDisplay;
    [SerializeField] private TextMeshProUGUI TotalVictoryPointsText;
    [SerializeField] private TextMeshProUGUI VictoryPointsLeftUntilNextBoon;

    [SerializeField] public Slider TurnSlider;
    public RectTransform ChallengeSliderIndicator;
    [SerializeField] private GameObject BoonSelectionScreen;
    [SerializeField] public GameObject TradePannel; //also used as a spawn points for flying icons when trading
    [SerializeField] private GameObject EmptyBoonImagePrefab;
    [SerializeField] private Transform BoonsPanel;
    public Dictionary<GenericBoon, GameObject> BoonIconsDisplay = new Dictionary<GenericBoon, GameObject>();


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

    public void SetUpUIManager(PlayerClass playerInstance)
    {
        player = playerInstance;
        player.OnVictoryPointsChanged += UpdateVictoryPointsDisplay;
        BoardManager.OnDiceRolled += UpdateTurnSliderDisplay;

        UpdateResourceDisplay();  // Initial display update
        UpdateTurnSliderDisplay(); // inital slider postioning
        player.AddVictoryPoints(0); // just to update the visuals 
    }


    void OnDestroy()
    {
        if (player != null)
        {
            player.OnVictoryPointsChanged -= UpdateVictoryPointsDisplay;
            BoardManager.OnDiceRolled -= UpdateTurnSliderDisplay;
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
        TradeToggle.isOn = false;

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

    public void ShowTradePannelToggle()
    {
        if(TradeToggle.isOn == true)
        {
            CloseAllUi(TradeToggle);
            TradePannel.SetActive(true);

           //SetupButtonListeners();
            TradeToggleActivation();


        }
        else
        {
            TradePannel.SetActive(false);
            CloseAllUi();
        }
    }


    //



    public void ResourceAddedAnimation(ResourceType Resource, Vector3 FromPosition)
    {

        float offsetPositionX = FromPosition.x + UnityEngine.Random.Range(-0.5f, 0.5f);
        float offsetPositionY = FromPosition.y + UnityEngine.Random.Range(-0.5f, 0.5f);
        Vector3 spawnPosition = new Vector3(offsetPositionX, offsetPositionY, 0);
        switch (Resource)
        {
            case ResourceType.Wood:
                var woodicon = Instantiate(WoodFlyingIcon, spawnPosition, Quaternion.identity);
                var tweenWood = woodicon.transform.DOMove(woodText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                //var tweenWood = woodicon.transform.DOMove(woodText.transform.position, 1).SetEase(Ease.InBack);
                tweenWood.OnComplete(() => {
                    woodText.text = player.PlayerResources[ResourceType.Wood].ToString();
                    Destroy(woodicon);
                });
                break;

            case ResourceType.Brick:
                var brickicon = Instantiate(BrickFlyingIcon, spawnPosition, Quaternion.identity);
                var tweenBrick = brickicon.transform.DOMove(brickText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                //var tweenBrick = brickicon.transform.DOMove(brickText.transform.position, 1).SetEase(Ease.InBack);
                tweenBrick.OnComplete(() => {
                    brickText.text = player.PlayerResources[ResourceType.Brick].ToString();
                    Destroy(brickicon);
                });
                break;

            case ResourceType.Sheep:
                var Sheepicon = Instantiate(SheepFlyingIcon, spawnPosition, Quaternion.identity);
                var tweenSheep = Sheepicon.transform.DOMove(sheepText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                //var tweenSheep = Sheepicon.transform.DOMove(sheepText.transform.position, 1).SetEase(Ease.InBack);
                tweenSheep.OnComplete(() => {
                    sheepText.text = player.PlayerResources[ResourceType.Sheep].ToString();
                    Destroy(Sheepicon);
                });
                break;

            case ResourceType.Ore:
                var Oreicon = Instantiate(OreFlyingIcon, spawnPosition, Quaternion.identity);
                var tweenOre = Oreicon.transform.DOMove(oreText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                //var tweenOre = Oreicon.transform.DOMove(oreText.transform.position, 1).SetEase(Ease.InBack);
                tweenOre.OnComplete(() => {
                    oreText.text = player.PlayerResources[ResourceType.Ore].ToString();
                    Destroy(Oreicon);
                });
                break;

            case ResourceType.Wheat:
                var Wheaticon = Instantiate(WheatFlyingIcon, spawnPosition, Quaternion.identity);
                var tweenWheat = Wheaticon.transform.DOMove(wheatText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                //var tweenWheat = Wheaticon.transform.DOMove(wheatText.transform.position, 1).SetEase(Ease.InBack);
                tweenWheat.OnComplete(() => {
                    wheatText.text = player.PlayerResources[ResourceType.Wheat].ToString();
                    Destroy(Wheaticon);
                });
                break;
        }
    }

    public void UpdateResourceDisplay()
    {
        woodText.text = player.PlayerResources[ResourceType.Wood].ToString();
        brickText.text = player.PlayerResources[ResourceType.Brick].ToString();
        sheepText.text = player.PlayerResources[ResourceType.Sheep].ToString();
        oreText.text = player.PlayerResources[ResourceType.Ore].ToString();
        wheatText.text = player.PlayerResources[ResourceType.Wheat].ToString();

    }

    public void UpdateVictoryPointsDisplay(int CurrentVictoryPoints)
    {

        string VPText = CurrentVictoryPoints.ToString();
        string VPGoalText = BoardManager.instance.VictoryPointsGoal.ToString();
        TotalVictoryPointsText.text = VPText + "/" + VPGoalText;

        string VPLeftUntilBoon = (boonManager.NextVPforboon - CurrentVictoryPoints).ToString();
        VictoryPointsLeftUntilNextBoon.text = VPLeftUntilBoon + " Victory points until next boon";

    }
    public void UpdateDiceRollDisplay(int DiceResult)
    {
        DiceDisplay.text = DiceResult.ToString();
    }

    public void UpdateTurnSliderDisplay()
    {

        int CurrentTurn= BoardManager.instance.CurrentTurn;
        int maxTurns = BoardManager.instance.MaxTurn;


        TurnSlider.maxValue = maxTurns;
        TurnSlider.value = CurrentTurn;

        if (CurrentTurn == 0)
        {
            int challengeTurn = challenges.RobberChallengeTurn;
            float reletivePostion = (float)challenges.RobberChallengeTurn / maxTurns;

            ChallengeSliderIndicator.anchorMin = new Vector2(reletivePostion, ChallengeSliderIndicator.anchorMin.y);
            ChallengeSliderIndicator.anchorMax = new Vector2(reletivePostion, ChallengeSliderIndicator.anchorMax.y);
            ChallengeSliderIndicator.anchoredPosition = new Vector2(0, ChallengeSliderIndicator.anchoredPosition.y);
        }




    }



    public void RollDiceButton() 
    {
        //BoardManager.instance.DiceRoll();
        if(BoardManager.instance.DiceStilRolling == false)
        {
            StartCoroutine(BoardManager.instance.RollTheDice());
        }
      
        CloseAllUi();

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


    private void TradeToggleActivation() //change trade icons numbers/ratio and active the toggles (if player have resources)
    {
        

        TextMeshProUGUI woodText = offerWoodToggle.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI brickText = offerBrickToggle.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI sheepText = offerSheepToggle.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI oreText = offerOreToggle.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI wheatText = offerWheatToggle.GetComponentInChildren<TextMeshProUGUI>();

        //default rate
        woodText.text = "X4";
        brickText.text = "X4";
        sheepText.text = "X4";
        oreText.text = "X4";
        wheatText.text = "X4";


        //if you have 3:1 port 
        if (player.OwnedHarbors.Any(harbor => harbor.TradeResource == HarborResourceType.Any))
        {
            woodText.text = "X3";
            brickText.text = "X3";
            sheepText.text = "X3";
            oreText.text = "X3";
            wheatText.text = "X3";
        }

        //if you have 2:1 port 
        foreach (var port in player.OwnedHarbors)
        {

            switch (port.TradeResource)
            { 
                case HarborResourceType.Wood:
                    woodText.text = "X" + (port.TradeRatio.ToString());
                    break;
                case HarborResourceType.Brick:
                    brickText.text = "X" + (port.TradeRatio.ToString());
                    break;
                case HarborResourceType.Sheep:
                    sheepText.text = "X" + (port.TradeRatio.ToString());
                    break;
                case HarborResourceType.Ore:
                    oreText.text = "X" + (port.TradeRatio.ToString());
                    break;
                case HarborResourceType.Wheat:
                    wheatText.text = "X" + (port.TradeRatio.ToString());
                    break;


            }
        }

        //if you have an active trade ratio modifer 
        if (player.TradeModifier != null)
        {
            woodText.text = (player.TradeModifier.ToString() + "X");
            brickText.text = (player.TradeModifier.ToString() + "X");
            sheepText.text = (player.TradeModifier.ToString() + "X");
            oreText.text = (player.TradeModifier.ToString() + "X");
            wheatText.text = (player.TradeModifier.ToString() + "X");
        }



        offerWoodToggle.interactable = CanOfferResource(ResourceType.Wood);
        offerBrickToggle.interactable = CanOfferResource(ResourceType.Brick);
        offerSheepToggle.interactable = CanOfferResource(ResourceType.Sheep);
        offerOreToggle.interactable = CanOfferResource(ResourceType.Ore);
        offerWheatToggle.interactable = CanOfferResource(ResourceType.Wheat);



    }
    private bool CanOfferResource(ResourceType resourceType)
    {
        int playerResourceCount = player.PlayerResources[resourceType];
        int requiredAmount = 4;

        if (player.TradeModifier != null) 
        { 
            requiredAmount = player.TradeModifier.Value;
            return playerResourceCount >= requiredAmount;
        }
           
        foreach (var port in player.OwnedHarbors)
        {
            if (port.TradeResource.ToString() == resourceType.ToString())
            {
                requiredAmount = port.TradeRatio;
                break;
            }
            else if (port.TradeResource == HarborResourceType.Any)
            {
                requiredAmount = port.TradeRatio;
            }
        }

        return playerResourceCount >= requiredAmount;
    }



    private void SetTradeSelection(bool isSelected, ResourceType resourceType, Toggle toggle, bool isOffer)
    {
        if (isSelected == true)
        {
            // Determine if this is setting an offer or request and update accordingly
            if (isOffer == true)
            {
                offerType = resourceType; 
            }
            else
            {
                requestType = resourceType; 
            }

            // Ensure that other toggles in the same category (offer or request) are unchecked
            UncheckOtherToggles(toggle, isOffer);
        }
        else
        {
            // If the toggle is deselected, clear the offer or request if it matches the current selection
            if ((isOffer && offerType == resourceType) || (!isOffer && requestType == resourceType))
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
        bool isValidTrade = offerType.HasValue && requestType.HasValue;
        tradeButton.interactable = isValidTrade; // Enable the button only if the trade is valid
        if (isValidTrade)
        {
            Debug.Log($"Ready to trade {offerType.Value} for {requestType.Value}");
        }
    }


    public void ExecuteTradeButton() // called by button pressed
    {    
        if (offerType.HasValue && requestType.HasValue)
        {
            player.TradeWithBank(offerType.Value, requestType.Value);

            TradeToggleActivation();
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



// boons


    public void OpenAndCloseBoonSelectionScreen()
    {
        BoonSelectionScreen.SetActive(!BoonSelectionScreen.activeSelf);
    }

    public void AddAndRemoveActiveBoonsDisplay(GenericBoon boon,bool isAdding, int? counter = null)
    {
        if (isAdding == true)
        {
            //image
            GameObject newBoonImage = Instantiate(EmptyBoonImagePrefab, BoonsPanel);
            Image imageComponent = newBoonImage.GetComponent<Image>();
            imageComponent.sprite = boon.boonImage;
            imageComponent.color = boon.boonColor;

            //tooltip
            ToolTipTrigger toolTipTrigger = newBoonImage.GetComponent<ToolTipTrigger>();
            toolTipTrigger.header = boon.boonName;
            toolTipTrigger.text = boon.description;


            BoonIconsDisplay.Add(boon, newBoonImage);

            if (counter != null)
            {
                UpdateBoonCounter(boon, counter.Value);
            }

            
        }
        else
        {
            var boonToRemove = BoonIconsDisplay[boon];
            BoonIconsDisplay.Remove(boon);
            Destroy(boonToRemove);
            

        }
    }

    public void UpdateBoonCounter(GenericBoon boon,int Counter )
    {
        GameObject icon = BoonIconsDisplay[boon];       
        icon.GetComponentInChildren<TextMeshProUGUI>().text = Counter.ToString();

    }

}
