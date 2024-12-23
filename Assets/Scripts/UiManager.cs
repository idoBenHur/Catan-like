using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using static TileClass;

public class UiManager : MonoBehaviour
{

    //scripts 
    public Challenges challenges;
    [SerializeField] BoonManager boonManager;
    [SerializeField] private Canvas MainCanvas;
    [SerializeField] private UIDrawer UIDrawer;
    [SerializeField] private SpriteCursor spriteCursor;




    //  menu toggles

    [SerializeField] private Toggle RoadIndicatorsToggle;
    [SerializeField] private Toggle TownIndicatorsToggle;
    [SerializeField] private Toggle CityIndicatorsToggle;
    [SerializeField] private Toggle TradeToggle;
    [SerializeField] private Toggle FogRemoveToggle;

    private Toggle[] BuildToggles;

    private bool isUpdatingToggles = false;

    //  flying icons

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
    [SerializeField] private GameObject FloatingErrorTextPrefab;
    [SerializeField] public Image diceBackground;
    [SerializeField] public GameObject TradePannel; //also used as a spawn points for flying icons when trading
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject VictoryScreen;
    [SerializeField] private GameObject PlacementPhaseScreen;
    [SerializeField] private GameObject Sevenflag;
    private Vector2 sevenFlagOGPos;




    // challenge slider

    [SerializeField] public Slider TurnSlider;
    public RectTransform ChallengeSliderIndicator;
    [SerializeField] private TextMeshProUGUI TurnLeftUntilChallengeText;
    [SerializeField] private TextMeshProUGUI TurnLeftUntilDeathText;


    // boons Ui
    [SerializeField] private GameObject BoonSelectionScreen;
    [SerializeField] private GameObject EmptyBoonImagePrefab;
    [SerializeField] private Transform BoonsIconsPanel;
    public Dictionary<GenericBoon, GameObject> BoonIconsDisplayDic = new Dictionary<GenericBoon, GameObject>();
    [SerializeField] private RectTransform BoonsSelectionButtonsParent;
    [SerializeField] private Image BoonsSelectionScreenBackground;
    [SerializeField] private GameObject BoonsAndBlackBackground;


    private ResourceType? offerType = null;
    private ResourceType? requestType = null;


    // circular progress bar

    


    [SerializeField] GameObject SevenSkillPannel;
    [SerializeField] private Button WoodRewardButton;
    [SerializeField] private Button BrickRewardButton;
    [SerializeField] private Button SheepRewardButton;
    [SerializeField] private Button OreRewardButton;
    [SerializeField] private Button WheatRewardButton;




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






    // tiles effect
    private List<TileClass> PreviouslyAffectedTiles = new List<TileClass>();

    // roll button
    [SerializeField] private GameObject rollButton;









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

        sevenFlagOGPos = Sevenflag.GetComponent<RectTransform>().anchoredPosition;

    }

    public void SetUpUIManager(PlayerClass playerInstance)
    {
        player = playerInstance;
        BoardManager.OnDiceRolled += UpdateTurnSliderDisplay;
        player.OnResourcesChanged += ShowInteractableToggels;

        ShowInteractableToggels();
        UpdateResourceDisplay();  // Initial display update
        UpdateTurnSliderDisplay(); // inital slider postioning

        //inital update for total VP and next boon milestone
       // boonManager.CheckBoonMilestones(); 

        
    }


    void OnDestroy()
    {
        //if (player != null)
        //{
        //    BoardManager.OnDiceRolled -= UpdateTurnSliderDisplay;
        //}
        BoardManager.OnDiceRolled -= UpdateTurnSliderDisplay;
        player.OnResourcesChanged -= ShowInteractableToggels;
    }



    public void CloseAllUi(Toggle changedToggle = null)
    {

        if (isUpdatingToggles)
        {
            return;
        }

        isUpdatingToggles = true;


        BoardManager.instance.DestroyIndicators();

        // Turn off all toggles except the "changedToggle"
        if (changedToggle != null && changedToggle.isOn == true)
        {
            
            foreach (var toggle in BuildToggles)
            {
                if (toggle != changedToggle)
                {
                    toggle.isOn = false;
                }
                    
            }   
        }

        // If there is no exception for keeping a toggle on, turn off all toggles
        else if (changedToggle == null)
        {
            foreach (var toggle in BuildToggles)
            {               
                toggle.isOn = false;          
            }
        }


  



        //TownIndicatorsToggle.isOn = false;
        //RoadIndicatorsToggle.isOn = false;
        //CityIndicatorsToggle.isOn = false;
        //TradeToggle.isOn = false;



        isUpdatingToggles = false;



    }


    // build roads/towns toggles

    public void ShowTownBuildIndicatorsToggle()
    {
        CloseAllUi(TownIndicatorsToggle); // close all other UI menues exsept this one



        // print not Enough Resources error
        if (player.CanAffordToBuild(PricesClass.TownCost) == false && isUpdatingToggles == false)
        {
            //TownIndicatorsToggle.isOn = false;
            SpawnErrorText("Not enough resources!");

        }



        if (TownIndicatorsToggle.isOn == true)
        {
           
            BoardManager.instance.ShowBuildIndicatorsTowns();

            //if(BoardManager.instance.TownsIndicatorsPrefabList.Count == 0)
            //{
            //  //  TownIndicatorsToggle.isOn = false;
            //    SpawnErrorText("no vaild place for a town");
            //}

        }
        else
        {
            CloseAllUi();
        }


    }


    public void ShowRoadBuildIndicatorsToggle()
    {

        CloseAllUi(RoadIndicatorsToggle);


        // print not Enough Resources error
        if (player.CanAffordToBuild(PricesClass.RoadCost) == false && isUpdatingToggles == false)
        {
            RoadIndicatorsToggle.isOn = false;
            SpawnErrorText("Not enough resources!");
            
        }



        if (RoadIndicatorsToggle.isOn == true)
        {
            BoardManager.instance.ShowBuildIndicatorsRoads();
            
        }
        else
        {
            CloseAllUi();
        }



    }


    public void ShowCityBuildIndicatorToggle()
    {
        CloseAllUi(CityIndicatorsToggle);

        // print not Enough Resources error
        if (player.CanAffordToBuild(PricesClass.CityCost) == false && isUpdatingToggles == false)
        {
           // CityIndicatorsToggle.isOn = false;
            SpawnErrorText("Not enough resources!");

        }



        if (CityIndicatorsToggle.isOn == true)
        {
            BoardManager.instance.ShowCityUpgradeIndicators();

            //checks if a city can be placed

            bool hasAtLeast1Town = false;

            foreach (var town in BoardManager.instance.player.SettelmentsList)
            {               
                if( town.HasCityUpgade == false)
                {
                    hasAtLeast1Town = true;
                    break;
                }

            }
            if (hasAtLeast1Town == false)
            {
                SpawnErrorText("no vaild place for a city");
                CloseAllUi(); // turn off all toggles including this       
            }



        }
        else
        {
            CloseAllUi();
        }


    }

    public void ShowTradePannelToggle()
    {

        CloseAllUi(TradeToggle);


        if (TradeToggle.isOn == true)
        {
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



    private void ShowInteractableToggels()
    {
        CityIndicatorsToggle.interactable = player.CanAffordToBuild(PricesClass.CityCost);
        RoadIndicatorsToggle.interactable = player.CanAffordToBuild(PricesClass.RoadCost);
        TownIndicatorsToggle.interactable = player.CanAffordToBuild(PricesClass.TownCost);
        FogRemoveToggle.interactable = player.CanAffordToBuild(PricesClass.RemoveFog);


    }


    public void ShowFogRemoveIndicatorsToggle()
    {
        CloseAllUi(FogRemoveToggle);


        // print not Enough Resources error
        if (player.CanAffordToBuild(PricesClass.RoadCost) == false && isUpdatingToggles == false)
        {
            RoadIndicatorsToggle.isOn = false;
            SpawnErrorText("Not enough resources!");

        }




        if (FogRemoveToggle.isOn == true)
        {
            BoardManager.instance.ShowRemoveFogIndicator();
        }
        else
        {
            CloseAllUi();
        }



    }



    //



    public void ResourceAddedAnimation(ResourceType Resource, Vector3 FromPosition)
    {
       // AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceCreate);

        DOVirtual.DelayedCall(0f, () =>
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
                    tweenWood.OnComplete(() =>
                    {
                        woodText.text = player.PlayerResources[ResourceType.Wood].ToString();
                        //AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceGain);
                        Destroy(woodicon);
                    });
                    break;

                case ResourceType.Brick:
                    var brickicon = Instantiate(BrickFlyingIcon, spawnPosition, Quaternion.identity);
                    var tweenBrick = brickicon.transform.DOMove(brickText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                    //var tweenBrick = brickicon.transform.DOMove(brickText.transform.position, 1).SetEase(Ease.InBack);
                    tweenBrick.OnComplete(() =>
                    {
                        brickText.text = player.PlayerResources[ResourceType.Brick].ToString();
                        // AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceGain);
                        Destroy(brickicon);
                    });
                    break;

                case ResourceType.Sheep:
                    var Sheepicon = Instantiate(SheepFlyingIcon, spawnPosition, Quaternion.identity);
                    var tweenSheep = Sheepicon.transform.DOMove(sheepText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                    //var tweenSheep = Sheepicon.transform.DOMove(sheepText.transform.position, 1).SetEase(Ease.InBack);
                    tweenSheep.OnComplete(() =>
                    {
                        sheepText.text = player.PlayerResources[ResourceType.Sheep].ToString();
                        // AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceGain);
                        Destroy(Sheepicon);
                    });
                    break;

                case ResourceType.Ore:
                    var Oreicon = Instantiate(OreFlyingIcon, spawnPosition, Quaternion.identity);
                    var tweenOre = Oreicon.transform.DOMove(oreText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                    //var tweenOre = Oreicon.transform.DOMove(oreText.transform.position, 1).SetEase(Ease.InBack);
                    tweenOre.OnComplete(() =>
                    {
                        oreText.text = player.PlayerResources[ResourceType.Ore].ToString();
                        // AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceGain);
                        Destroy(Oreicon);
                    });
                    break;

                case ResourceType.Wheat:
                    var Wheaticon = Instantiate(WheatFlyingIcon, spawnPosition, Quaternion.identity);
                    var tweenWheat = Wheaticon.transform.DOMove(wheatText.transform.position, 70).SetSpeedBased(true).SetEase(Ease.InQuint);
                    //var tweenWheat = Wheaticon.transform.DOMove(wheatText.transform.position, 1).SetEase(Ease.InBack);
                    tweenWheat.OnComplete(() =>
                    {
                        wheatText.text = player.PlayerResources[ResourceType.Wheat].ToString();
                        // AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.ResourceGain);
                        Destroy(Wheaticon);
                    });
                    break;
            }
        });

    }



    public void UpdateResourceDisplay()
    {
        woodText.text = player.PlayerResources[ResourceType.Wood].ToString();
        brickText.text = player.PlayerResources[ResourceType.Brick].ToString();
        sheepText.text = player.PlayerResources[ResourceType.Sheep].ToString();
        oreText.text = player.PlayerResources[ResourceType.Ore].ToString();
        wheatText.text = player.PlayerResources[ResourceType.Wheat].ToString();

    }




    public void UpdateTurnSliderDisplay()
    {

        int CurrentTurn= BoardManager.instance.CurrentTurn;
        int maxTurns = BoardManager.instance.MaxTurn;


        TurnSlider.maxValue = maxTurns;
        TurnSlider.value = CurrentTurn;

        TurnLeftUntilDeathText.text = (maxTurns - CurrentTurn) + " Turns left";




        // update challenge indicator text 

        if((challenges.TsunamiChallengeTurn - CurrentTurn) <= 0)
        {
            TurnLeftUntilChallengeText.text = "Active!";
        }
        else { TurnLeftUntilChallengeText.text = (challenges.TsunamiChallengeTurn - CurrentTurn) + " Turns left"; }


        // inital challenge icon placment
        if (CurrentTurn == 0)
        {
            int challengeTurn = challenges.TsunamiChallengeTurn;
            float reletivePostion = (float)challenges.TsunamiChallengeTurn / maxTurns;

            ChallengeSliderIndicator.anchorMin = new Vector2(reletivePostion, ChallengeSliderIndicator.anchorMin.y);
            ChallengeSliderIndicator.anchorMax = new Vector2(reletivePostion, ChallengeSliderIndicator.anchorMax.y);
            ChallengeSliderIndicator.anchoredPosition = new Vector2(0, ChallengeSliderIndicator.anchoredPosition.y);
        }




    }

    public void EndGame(bool playerWon)
    {

        if (playerWon == true)

        {
            VictoryScreen.SetActive(true);

            GameManager.Instance.FinishedLevel(SceneManager.GetActiveScene().buildIndex);


        }
        else if (playerWon == false)
        {
            GameOverScreen.SetActive(true);
        }
        
    }

    public void RestartCurrentSceneBUTTON()
    {
        DOTween.KillAll();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.RestartCurrentScene();
    }

    public void NextSceneBUTTON()
    {
        DOTween.KillAll();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
        GameManager.Instance.NextScene();
    }

    public void BackToMainMenuBUTTON()
    {
        DOTween.KillAll();
        //SceneManager.LoadScene(0);
        GameManager.Instance.BackToMainMenu();
    }

    public void BackTLevelSelectionBUTTON()
    {
        DOTween.KillAll();
        // SceneManager.LoadScene(1);
        GameManager.Instance.BackTLevelSelection();
    }

    public void DiscordBUTTON()
    {
        //Application.ExternalEval("https://discord.gg/NXMtgwudQw");
        Application.OpenURL("https://discord.gg/NXMtgwudQw");

    }




    public void NewRollDinceBUTTON()
    {


        rollButton.transform.DOPunchScale(transform.localScale * -0.1f, 0.3f, 5, 0.5f);
        CloseAllUi();
        BoardManager.instance.skillSlotManager.RollNewDice();
       // DOVirtual.DelayedCall(1.3f, () => BoardManager.instance.DicesRolled());


        BoardManager.instance.DicesRolled();
        

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




        // gain seven skill reawards button
        WoodRewardButton.onClick.AddListener(() => SevenSkillReward(ResourceType.Wood));
        BrickRewardButton.onClick.AddListener(() => SevenSkillReward(ResourceType.Brick));
        SheepRewardButton.onClick.AddListener(() => SevenSkillReward(ResourceType.Sheep));
        OreRewardButton.onClick.AddListener(() => SevenSkillReward(ResourceType.Ore));
        WheatRewardButton.onClick.AddListener(() => SevenSkillReward(ResourceType.Wheat));




        // build toggles added to the build toggle list

        BuildToggles = new Toggle[]
        {
            RoadIndicatorsToggle,
            TownIndicatorsToggle,
            CityIndicatorsToggle,
            TradeToggle,
            FogRemoveToggle
        };


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





 


// boons


    public void OpenAndCloseBoonSelectionScreenAnimations(bool Open)
    {
        
        //BoonSelectionScreen.SetActive(!BoonSelectionScreen.activeSelf);

        Color BlackColor = BoonsSelectionScreenBackground.color;

        if (Open == true) //enter
        {
            BoonSelectionScreen.SetActive(true);

            spriteCursor.ChangeCursorHand();


            BoonsSelectionButtonsParent.transform.localPosition = new Vector3(0F, -1200F, 0F);
            BoonsSelectionButtonsParent.DOAnchorPosY(0f, 0.8f, false).SetEase(Ease.OutBack);



            BlackColor.a = 0f;
            BoonsSelectionScreenBackground.color = BlackColor;
            BoonsSelectionScreenBackground.DOFade(0.99f, 0.5f);
            AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.BoonScreenOpen);

        }

        else //exit
        {
            var exitAnimation = DOTween.Sequence();

            exitAnimation.Append(BoonsSelectionButtonsParent.DOAnchorPosY(-1200f, 0.5f).SetEase(Ease.InBack));
            exitAnimation.Join(BoonsSelectionScreenBackground.DOFade(0f, 0.5f));

            exitAnimation.OnComplete(() =>
            {
                spriteCursor.ChangeCursorHand();
                UIDrawer.ClearDrawing(0);
                UIDrawer.ClearDrawing(1);
                UIDrawer.ClearDrawing(2);
                BoonSelectionScreen.SetActive(false);



            });
                
                

        }



    }

    public void AddAndRemoveActiveBoonsDisplay(GenericBoon boon,bool isAdding, int? counter = null)
    {
        if (isAdding == true)
        {
            //image
            GameObject newBoonImage = Instantiate(EmptyBoonImagePrefab, BoonsIconsPanel);
            Image imageComponent = newBoonImage.GetComponent<Image>();
            imageComponent.sprite = boon.boonImage;
            imageComponent.color = boon.boonColor;

            //tooltip
            ToolTipTrigger toolTipTrigger = newBoonImage.GetComponent<ToolTipTrigger>();
            toolTipTrigger.header = boon.boonName;
            toolTipTrigger.text = boon.description;


            BoonIconsDisplayDic.Add(boon, newBoonImage);

            if (counter != null)
            {
                UpdateBoonCounter(boon, counter.Value);
            }

            
        }
        else
        {
            var boonToRemove = BoonIconsDisplayDic[boon];
            BoonIconsDisplayDic.Remove(boon);
            Destroy(boonToRemove);
            

        }
    }

    public void UpdateBoonCounter(GenericBoon boon,int Counter )
    {
        GameObject icon = BoonIconsDisplayDic[boon];       
        icon.GetComponentInChildren<TextMeshProUGUI>().text = Counter.ToString();

    }

    public void ShowMapButton()
    {
        BoonsAndBlackBackground.SetActive(!BoonsAndBlackBackground.activeSelf);
    }


    public void ShakeBoonDisplayAnimation(GenericBoon boon)
    {
        float duration = 0.5f;
        float strength = 0.5f;

        if(BoonIconsDisplayDic.ContainsKey(boon) == true)
        {
            BoonIconsDisplayDic[boon].transform.DOShakePosition(duration, strength).SetDelay(0.1f);
            BoonIconsDisplayDic[boon].transform.DOShakeRotation(duration, strength).SetDelay(0.1f);
            BoonIconsDisplayDic[boon].transform.DOShakeScale(duration, strength).SetDelay(0.1f);
        }


    }

   




  

    public void OpenSevenSkillRewardPannel()
    {
        SevenSkillPannel.SetActive(true);
    }

    private void SevenSkillReward(ResourceType resourceType)
    {

        player.AddResource(resourceType, 1, SevenSkillPannel.transform.position);
        SevenSkillPannel.SetActive(false);
        
 
    }


 
    public void SpawnErrorText(string ErrorText)
    {

        GameObject floatingText = Instantiate(FloatingErrorTextPrefab, MainCanvas.transform);
        TextMeshProUGUI textComponent = floatingText.GetComponent<TextMeshProUGUI>();
        textComponent.text = ErrorText;



        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(floatingText.transform.DOMoveY(floatingText.transform.position.y + 5, 4f));
        sequence.Join(textComponent.DOFade(0, 1f).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(floatingText));
    }


    // playtest tutroial screen

 

    public void ClosePlacmentScreen()
    {
        PlacementPhaseScreen.SetActive(false);
    }

    // win conditions









    // numbers attention


    public void IncreaseNumbersTokenSize(List<int> listsOfSums)
    {

        Vector3 originalScale = new Vector3(0.25f, 0.25f, 0.25f);
        Vector3 increaseScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 1.5f, originalScale.z);


        foreach (var Settelment in BoardManager.instance.player.SettelmentsList)
        {
            foreach (var tile in Settelment.AdjacentTiles) 
            {
                if (tile.MyNumberPrefab == null) { continue; }

                SpriteRenderer spriteRenderer = tile.MyNumberPrefab.GetComponent<SpriteRenderer>();

                if (listsOfSums.Contains(tile.numberToken) == true && tile.underFog == false)
                {
                    tile.MyNumberPrefab.transform.DOScale(new Vector3(increaseScale.x, increaseScale.y, increaseScale.z), 0.5f);//.SetDelay(1.3f);
                    spriteRenderer.DOColor(new Color32(0xCD, 0xF8, 0xCF, 0xFF), 0.5f);//.SetDelay(1.3f);
                    PreviouslyAffectedTiles.Add(tile);

                }

                else
                {
                    if (tile.MyNumberPrefab.transform.localScale != originalScale)
                    {
                        tile.MyNumberPrefab.transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.5f);
                        spriteRenderer.DOColor(new Color32(0xFA, 0xFA, 0xFA, 0xFF), 0.5f);
                        PreviouslyAffectedTiles.Add(tile);
                    }

                }


            }





        }




        bool contains7 = listsOfSums.Contains(7);

        RectTransform SevenFlagRectTransform = Sevenflag.GetComponent<RectTransform>();

        if (contains7 == false && SevenFlagRectTransform.anchoredPosition == sevenFlagOGPos) // no 7, og pos == do nothing
        {
            return;
        }

        else if (contains7 == false && SevenFlagRectTransform.anchoredPosition != sevenFlagOGPos) // no 7, diffrent pos == back to og pos
        {
            SevenFlagRectTransform.DOKill();
            SevenFlagRectTransform.DOAnchorPosY(sevenFlagOGPos.y, 0.2f);

        }

        else if (contains7 == true && SevenFlagRectTransform.anchoredPosition == sevenFlagOGPos) // has 7, og pos == move up
        {

            SevenFlagRectTransform.DOKill();
            SevenFlagRectTransform.DOAnchorPosY(65f, 0.2f);
        }

 





       


    }












}
