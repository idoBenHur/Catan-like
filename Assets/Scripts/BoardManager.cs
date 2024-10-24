using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;
using static TileClass;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    //main big stuff/scripts

    [SerializeField] private Tilemap tilemap;
    //public TileBase woodTile, brickTile, wheatTile, oreTile, sheepTile, desertTile; // Assign these in the inspector
    [SerializeField] public UiManager uiManager;
    [SerializeField] public MapGenerator mapGenerator;
    [SerializeField] public BoonManager boonManager;
    [SerializeField] private Challenges challenges;
    [SerializeField] public SkillSlotManager skillSlotManager;
    



    public bool FirstTurnIsActive;
    [HideInInspector] public bool DiceStilRolling = false;
    private int FirstTurnPlacedPeices = 0;
    [HideInInspector] public int TotalDice;
    [HideInInspector] public int Dice1FinalSide;
    [HideInInspector] public int Dice2FinalSide;
     public int PlayedAmountInTurn = 0;


    public PlayerClass player;

    // game objects
    [SerializeField] private GameObject WoodIcon;
    [SerializeField] private GameObject CornerIndicatorPrefab;
    [SerializeField] private GameObject SideIndicatorPrefab;
    [SerializeField] private GameObject RoadPrefab;
    [SerializeField] private GameObject RoadPrefab2;
    [SerializeField] private GameObject TownPrefab;
    [SerializeField] private GameObject CityPrefab;
    [SerializeField] private GameObject ResourceGainPS;
    [SerializeField] private UnityEngine.UI.Image Dice1Image;
    [SerializeField] private UnityEngine.UI.Image Dice2Image;
    [SerializeField] private Sprite[] DiceSides;


    // "to be balanced" game parameters
    [HideInInspector] public int CurrentTurn;
    [SerializeField] public int MaxTurn = 40;
    [SerializeField] private int UnluckyMeterMax = 5;
    [SerializeField] bool PlayWithUnluckyMeter = true;
    private int UnluckyMeterProgress = 0;


    // prefabs losts
    [HideInInspector] public List<GameObject> CitiesIndicatorsPrefabList = new List<GameObject>();
    [HideInInspector] public List<GameObject> TownsIndicatorsPrefabList = new List<GameObject>();
    [HideInInspector] public List<GameObject> RoadsIndicatorsPrefabList = new List<GameObject>();

    // main dictionaries
    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> CornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> SidesDic = new Dictionary<Vector3, SidesClass>();


    // events
    public static event Action OnDiceRolled;
    public static event Action OnDicePlayed;
    public static event Action OnRoadBuilt;
    public static event Action OnTownBuilt;
    public static event Action OnUnlcukyRoll;







    public static BoardManager instance;


    private void Awake()
    {
        

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

 


    void Start()
    {


        if (GameManager.Instance.GameState.SeasonNumber == 0)
        {
            player = new PlayerClass();
            mapGenerator.InitialBuildMap();


            TilesDictionary = mapGenerator.InitialTilesDictionary;
            CornersDic = mapGenerator.InitialCornersDic;
            SidesDic = mapGenerator.InitialSidesDic;

            mapGenerator.PlaceInitalSettelments();
            mapGenerator.PlaceInitalRoads();
            
         //   if(FirstTurnIsActive == true) { FirstTurnPlacement(); }
            

        }
        else
        {
            player = GameManager.Instance.GameState.player;
            TilesDictionary = GameManager.Instance.GameState.tilesDic;
            CornersDic = GameManager.Instance.GameState.cornersDic;
            SidesDic = GameManager.Instance.GameState.sidesDic;
            mapGenerator.LoadMapVisuals(TilesDictionary);
            FirstTurnIsActive = false;

        }

        CurrentTurn = 0;

        boonManager.InitializeBoonManager(player);
        challenges.SetUpPlayerChallenges(player);
        uiManager.SetUpUIManager(player);
        uiManager.SetUnluckyMeterSize(UnluckyMeterMax);
      //  winning_Condition.SetupWinningCondition(player);

        StartGame();





    }


    public void DicesPlayed(int Dice1, int dice2)
    {

        Dice1FinalSide = Dice1;
        Dice2FinalSide = dice2;
        TotalDice = Dice1 + dice2;
        PlayedAmountInTurn++;

        OnDicePlayed?.Invoke();


        DistributeResources(TotalDice);
    }

    public void DicesRolled()
    {
       // skillSlotManager.RollNewDice();
        CurrentTurn++;
        PlayedAmountInTurn = 0;
        OnDiceRolled?.Invoke();

       // DOVirtual.DelayedCall(1.3f, () => OnDiceRolled?.Invoke()); // waits for the dice animation to finish
        //  FlushResources();



        if (CurrentTurn > MaxTurn)
        {
            uiManager.EndGame(false); // player lose
        }

    }

    


    private void AddOneToUnluckyMeter() // delete this
    {
        UnluckyMeterProgress++;
        uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
        OnUnlcukyRoll?.Invoke();

        if (UnluckyMeterProgress == UnluckyMeterMax)
        {
            UnluckyMeterProgress = 0;
           // uiManager.OpenUnluckyMeterRewardPannel();
            uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
        }
    }

    public void TemoraraytNextSceneButton()
    {
        GameManager.Instance.UpdatePlayer(player);
        GameManager.Instance.UpdateTile(TilesDictionary);
        GameManager.Instance.UpdateCorner(CornersDic);
        GameManager.Instance.UpdateSide(SidesDic);
        GameManager.Instance.NextSeason();

        SceneManager.LoadScene(1);
        

    }





    private void DistributeResources(int DiceResult)
    {
        bool EarnedResources = false;

        if (DiceResult == 7)
        {
           // ChooseRobberTile();
        }

        else
        {

            foreach (var settelment in player.SettelmentsList)
            {

                foreach (var tile in settelment.AdjacentTiles)
                {

                    if(tile.numberToken != DiceResult)
                    {
                        continue;

                    }
                    else if(tile.numberToken == DiceResult && tile.isBlocked == true)
                    {
                        continue;
                    }

                    else if (tile.numberToken == DiceResult && tile.isBlocked == false && settelment.HasCityUpgade == false)
                    {
                        player.AddResource(tile.resourceType, 2, tile.TileWorldPostion);
                        EarnedResources = true;

                        DOVirtual.DelayedCall(0f, () =>
                        {
                            Instantiate(ResourceGainPS, tile.TileWorldPostion, Quaternion.identity);
                        });
                        


                    }
                    else if (tile.numberToken == DiceResult && tile.isBlocked == false && settelment.HasCityUpgade == true)
                    {
                        player.AddResource(tile.resourceType, 3, tile.TileWorldPostion);
                        EarnedResources = true;
                        DOVirtual.DelayedCall(0.3f, () =>
                        {
                            Instantiate(ResourceGainPS, tile.TileWorldPostion, Quaternion.identity);
                        });
                    }

                }
            }


        }

        if (EarnedResources == false && PlayWithUnluckyMeter == true)
        {
            UnluckyMeterProgress++;
            uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
            OnUnlcukyRoll?.Invoke();          

            if (UnluckyMeterProgress == UnluckyMeterMax)
            {
                UnluckyMeterProgress = 0;
               // uiManager.OpenUnluckyMeterRewardPannel();
                uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
            }

        }

    }






    public void StartGame()
    {

        FirstTurnPlacement();

    }

    private void FirstTurnPlacement() 
    {




        // Build a town
        if (FirstTurnPlacedPeices == 0 || FirstTurnPlacedPeices == 50)
        {
            FirstTurnPlacedPeices++;
            foreach (var corner in CornersDic.Values)
            {

                if (corner.CanBeBuiltOn == true)
                {
                    GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                    TownsIndicatorsPrefabList.Add(indicator);
                    indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner);
                }
                
            }
            
            return;
        }
       

        // Build a road
        if(FirstTurnPlacedPeices == 1 || FirstTurnPlacedPeices == 3)
        {
            FirstTurnPlacedPeices++;

            foreach (var corner in CornersDic.Values)
            {
                bool HasRoadAlready = false;

                if (corner.HasSettlement == true) 
                {
                    foreach (var SideNearTown in corner.AdjacentSides) // checkes if the town already have roads (meaning it was the player first town)
                    {
                        if(SideNearTown.HasRoad == true) 
                        {
                            HasRoadAlready = true;
                        }
                    }

                    if(HasRoadAlready == false) // make sure that on the second time its displys only the second town's roads
                    {
                        foreach (var SideNearTown in corner.AdjacentSides)
                        {
                            SideNearTown.CanBeBuiltOn = true;
                            Quaternion rotation = Quaternion.Euler(0, 0, SideNearTown.RotationZ);
                            GameObject indicator = Instantiate(SideIndicatorPrefab, SideNearTown.Position, rotation);
                            RoadsIndicatorsPrefabList.Add(indicator);
                            indicator.GetComponent<RoadBuildIndicatorPrefab>().Setup(SideNearTown.Position);
                        }
                    }

           


                }
                                                 
            }   


            return;
        }



        if (FirstTurnPlacedPeices == 2)
        {
            FirstTurnPlacedPeices++;
            FirstTurnIsActive = false;
            uiManager.ClosePlacmentScreen();
            return;
        }


    }



    public void ShowCityUpgradeIndicators()
    {
        if (player.CanAffordToBuild(PricesClass.CityCost) == false)
        {

            return;
        }


        foreach (var settelment in player.SettelmentsList)
        {
            if (settelment.HasCityUpgade == false)
            {
                GameObject indicator = Instantiate(CornerIndicatorPrefab, settelment.Position, Quaternion.identity);
                CitiesIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(settelment);
            }
        }
    }

    public void UpgradeSettelmentToCity(CornersClass Settelment, bool isFree = false)
    {
        
        if(isFree == false) { player.SubtractResources(PricesClass.CityCost); }
        

        Settelment.HasCityUpgade = true;

        Destroy(Settelment.BuildingPrefab);

        var cityPrefav = Instantiate(CityPrefab, Settelment.Position, Quaternion.identity);
        Settelment.BuildingPrefab = cityPrefav;
        AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.Build);




        foreach (var indicator in CitiesIndicatorsPrefabList)
        {
            Destroy(indicator.gameObject);
        }
        CitiesIndicatorsPrefabList.Clear();


        ShowCityUpgradeIndicators();

        









    }



    public void ShowBuildIndicatorsTowns()
    {

        if (player.CanAffordToBuild(PricesClass.TownCost) == false)
        {

            return;
        }


        foreach (var corner in CornersDic.Values)
        {
            if (corner.HasSettlement == true)
            {
                continue;
            }

            bool ConnectedToRoad = false;
            bool NearATown = false;


            // go through each adjusted sides of this corner, and chack if they have a road
            foreach (var adjustedSide in corner.AdjacentSides)
            {
                if (adjustedSide.HasRoad == true)
                {
                    ConnectedToRoad = true;
                }
                 
            }

            // go through each adjusted Corner of this corner, and chack if they have a settelment
            foreach (var adjustedCorner in corner.AdjacentCorners)
            {
                if(adjustedCorner.HasSettlement == true)
                {
                    NearATown = true;
                }
            }



          
            if (ConnectedToRoad == true && NearATown == false)
            {
                corner.CanBeBuiltOn = true;
            }
            else
            {
                corner.CanBeBuiltOn = false;
            }


            if (corner.CanBeBuiltOn == true)
            {
                
                GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                TownsIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner);
            }
        }
        
        

        

    }


    public void BuildSettlementAt(Vector3 cornerPosition, bool isFree = false)
    {


        if (CornersDic.TryGetValue(cornerPosition, out CornersClass corner) && corner.CanBeBuiltOn)
        {

            if (FirstTurnIsActive == true) { isFree = true; }

            if (isFree == false) { player.SubtractResources(PricesClass.TownCost); }

            corner.CanBeBuiltOn = false;
            corner.HasSettlement = true;

            var settelmentPrefab = Instantiate(TownPrefab, corner.Position, Quaternion.identity);
            corner.BuildingPrefab = settelmentPrefab;

            AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.Build);


            player.AddSettelment(corner);
            OnTownBuilt?.Invoke();

            foreach (var adjustTile in corner.AdjacentTiles) // gain resources
            {
                // player.AddResource(adjustTile.resourceType, 1, adjustTile.TileWorldPostion);
                adjustTile.underFog = false;
                mapGenerator.PlaceAndRemoveFogTiles();
            }


            foreach (var NeighborCornerKey in corner.AdjacentCorners)
            {
                CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
            }



            foreach (var indicator in TownsIndicatorsPrefabList)
            {
                Destroy(indicator.gameObject);
            }
            TownsIndicatorsPrefabList.Clear();





            //DiceBox_Skill diceBox = skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;
            //diceBox.AddPermaDie();
            //diceBox.AddTempDie(1);


            if (FirstTurnIsActive == true) { FirstTurnPlacement(); }
            else { ShowBuildIndicatorsRoads(); }



        }
    }



    public void ShowBuildIndicatorsRoads()
    {
        if (player.CanAffordToBuild(PricesClass.RoadCost) == false)
        {       
            return;
        }


        foreach (var Side in SidesDic.Values)
        {
            if (Side.CanBeBuiltOn == true && Side.HasRoad == false)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, Side.RotationZ);
                GameObject indicator = Instantiate(SideIndicatorPrefab, Side.Position, rotation);
                RoadsIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<RoadBuildIndicatorPrefab>().Setup(Side.Position);
            }
        }
        return;
    }



    public void BuildRoadAt(Vector3 SidePosition, bool isFree = false)
    {
        if (SidesDic.TryGetValue(SidePosition, out SidesClass Side))// && Side.CanBeBuiltOn)
        {
            if (FirstTurnIsActive == true) { isFree = true; }

            if (isFree == false) { player.SubtractResources(PricesClass.RoadCost); }


            Side.CanBeBuiltOn = false;
            Side.HasRoad = true;
            float rotationZ = Side.RotationZ;
            Quaternion SideRotation;

            if (Mathf.Abs(rotationZ) == 90f)
            {
                SideRotation = Quaternion.Euler(0, 0, 90);
                Instantiate(RoadPrefab, Side.Position, SideRotation);
            }

            // Handle the sideways roads for angles between -90 and 90 degrees (with no upside-down roads)
            else if (rotationZ >= 0 && rotationZ < 90f)
            {
                SideRotation = Quaternion.Euler(0, 0, 23.66f); // Use positive angle for one side
                Instantiate(RoadPrefab2, Side.Position, SideRotation);
            }
            else if (rotationZ < 0 && rotationZ > -90f)
            {
                SideRotation = Quaternion.Euler(0, 0, -23.66f); // Use negative angle for the opposite side
                Instantiate(RoadPrefab2, Side.Position, SideRotation);
            }
            // For larger angles, flip the road instead of rotating it upside down
            else if (Mathf.Abs(rotationZ) > 90f)
            {
                // Ensure the road is not upside down by flipping horizontally (no rotation inversion)
                if (rotationZ > 90f)
                {
                    SideRotation = Quaternion.Euler(0, 180, 23.66f); // Flip the sideways road but keep right-side up
                }
                else
                {
                    SideRotation = Quaternion.Euler(0, 180, -23.66f); // Flip the sideways road on the negative side
                }
                Instantiate(RoadPrefab2, Side.Position, SideRotation);
            }


            AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.Build);
            player.RoadsList.Add(Side);
            OnRoadBuilt?.Invoke();




            foreach (var NeighborsRoads in Side.AdjacentSides)
            {
                NeighborsRoads.CanBeBuiltOn = true;
            }

            foreach (var indicator in RoadsIndicatorsPrefabList)
            {
                Destroy(indicator.gameObject);
            }

            if (FirstTurnIsActive == true) { FirstTurnPlacement(); }
            else { ShowBuildIndicatorsRoads(); }




            

        }
    }

    public void UpgradeUnluckyMeter()
    {

        if (player.CanAffordToBuild(PricesClass.MeterUpgrade) == true)
        {
            player.SubtractResources(PricesClass.MeterUpgrade);
            UnluckyMeterMax--;
            uiManager.SetUnluckyMeterSize(UnluckyMeterMax);
        }


        if (UnluckyMeterProgress == UnluckyMeterMax)
        {
            UnluckyMeterProgress = 0;
           // uiManager.OpenUnluckyMeterRewardPannel();
            uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
        }

    }

    public void IncreaseUnluckyMeter(int IncreaseBy)
    {
        UnluckyMeterMax = UnluckyMeterMax + IncreaseBy;
        uiManager.SetUnluckyMeterSize(UnluckyMeterMax);
    }


    private void FlushResources()
    {
        foreach (var resourceMax in player.ResourcesMaxStorage)
        {
           if(player.CheckResourceAmount(resourceMax.Key) > resourceMax.Value)
            {
                int decreaseAmount = player.PlayerResources[resourceMax.Key] - resourceMax.Value;

                Dictionary<ResourceType, int> tempDic = new Dictionary<ResourceType, int>
                {
                    { resourceMax.Key, decreaseAmount }
                };

                player.SubtractResources(tempDic);
            }
            
        }
    }


}


