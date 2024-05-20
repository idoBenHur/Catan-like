using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using System.Linq;
//using Unity.Mathematics;
using Unity.VisualScripting;
using static UnityEditor.Progress;
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
    [SerializeField] private BoonManager boonManager;
    [SerializeField] private Challenges challenges;


   
    private bool FirstTurnIsActive;
    [HideInInspector] public bool DiceStilRolling = false;
    private int FirstTurnPlacedPeices = 0;
    [HideInInspector] public int TotalDice;
    [HideInInspector] public int Dice1FinalSide;
    [HideInInspector] public int Dice2FinalSide;


    public PlayerClass player;

    // game objects
    [SerializeField] private GameObject WoodIcon;
    [SerializeField] private GameObject CornerIndicatorPrefab;
    [SerializeField] private GameObject SideIndicatorPrefab;
    [SerializeField] private RobberPrefab robberPrefab;
    [SerializeField] private GameObject RoadPrefab;
    [SerializeField] private GameObject TownPrefab;
    [SerializeField] private GameObject CityPrefab;
    [SerializeField] private GameObject ResourceGainPS;
    [SerializeField] private UnityEngine.UI.Image Dice1Image;
    [SerializeField] private UnityEngine.UI.Image Dice2Image;
    [SerializeField] private Sprite[] DiceSides;


    // "to be balanced" game parameters
    [HideInInspector] public int CurrentTurn;
    [SerializeField] public int MaxTurn = 40;
    [SerializeField] public int VictoryPointsGoal = 10;
    [SerializeField] private int UnluckyMeterMax = 5;
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
    public static event Action OnRoadBuilt;
    public static event Action OnTownBuilt;







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
            FirstTurnIsActive = true;
            FirstTurnPlacement();


        }
        else
        {
            player = GameManager.Instance.GameState.player;
            TilesDictionary = GameManager.Instance.GameState.tilesDic;
            CornersDic = GameManager.Instance.GameState.cornersDic;
            SidesDic = GameManager.Instance.GameState.sidesDic;
            mapGenerator.LoadMapVisuals(TilesDictionary);
            Debug.Log("victory points " + player.VictoryPoints);
            FirstTurnIsActive = false;

        }

        CurrentTurn = 0;

        
        boonManager.SetPlayerInBoonManager(player);
        challenges.SetUpPlayerChallenges(player);
        uiManager.SetUpUIManager(player);
        uiManager.SetUnluckyMeterSize(UnluckyMeterMax);





    }

    public IEnumerator RollTheDice()
    {               
        DiceStilRolling = true;
        int Dice1RandomSide = 0;
        int Dice2RandomSide = 0;

        
        for (int i = 0; i <= 10; i++)
        {

            Dice1RandomSide = UnityEngine.Random.Range(1, 7);
            Dice2RandomSide = UnityEngine.Random.Range(1, 7);

            // Set sprite to upper face of dice from array according to random value
            Dice1Image.sprite = DiceSides[Dice1RandomSide - 1];
            Dice2Image.sprite = DiceSides[Dice2RandomSide - 1];

            // Pause before next itteration
            yield return new WaitForSeconds(0.1f);
        }


        Dice1FinalSide = Dice1RandomSide;
        Dice2FinalSide = Dice2RandomSide;
        TotalDice = Dice1FinalSide + Dice2FinalSide;

        CurrentTurn++;
        OnDiceRolled?.Invoke();
        DistributeResources(TotalDice);

        // uiManager.UpdateDiceRollDisplay(TotalDice);


        Color newColor;
        if (UnityEngine.ColorUtility.TryParseHtmlString("#1C3643", out newColor))
        {
            // Apply the color to the Image component
            uiManager.diceBackground.color =  newColor;
        }


        

        
        DiceStilRolling = false;
        

 
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
                    else if(tile.numberToken == DiceResult && tile.hasRobber == true)
                    {
                        continue;
                    }

                    else if (tile.numberToken == DiceResult && tile.hasRobber == false && settelment.HasCityUpgade == false)
                    {
                        player.AddResource(tile.resourceType, 1, tile.TileWorldPostion);
                        EarnedResources = true;

                        DOVirtual.DelayedCall(0f, () =>
                        {
                            Instantiate(ResourceGainPS, tile.TileWorldPostion, Quaternion.identity);
                        });
                        


                    }
                    else if (tile.numberToken == DiceResult && tile.hasRobber == false && settelment.HasCityUpgade == true)
                    {
                        player.AddResource(tile.resourceType, 2, tile.TileWorldPostion);
                        EarnedResources = true;
                        DOVirtual.DelayedCall(0.3f, () =>
                        {
                            Instantiate(ResourceGainPS, tile.TileWorldPostion, Quaternion.identity);
                        });
                    }

                }
            }


        }

        if (EarnedResources == false)
        {
            UnluckyMeterProgress++;
            uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);

            if(UnluckyMeterProgress == UnluckyMeterMax)
            {
                UnluckyMeterProgress = 0;
                uiManager.OpenUnluckyMeterRewardPannel();
                uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
            }

        }

    }

    private void ChooseRobberTile()
    {
        List<TileClass> NearPlayerTiles = new List<TileClass>();
        List<TileClass> AwayFromPlayerTiles = new List<TileClass>();

        foreach (var corner in player.SettelmentsList)
        {
            foreach( var AdjacentTile in corner.AdjacentTiles)
            {
                if (!NearPlayerTiles.Contains(AdjacentTile) && AdjacentTile.hasRobber ==false)
                {
                    NearPlayerTiles.Add(AdjacentTile);
                }
            }
        }


        foreach(var tile in TilesDictionary) 
        {
            if(NearPlayerTiles.Contains(tile.Value) == false)
            {
                AwayFromPlayerTiles.Add(tile.Value);
            }
        }


        if(AwayFromPlayerTiles.Count > 0)
        {
            int randomindex = UnityEngine.Random.Range(0, AwayFromPlayerTiles.Count);
            AwayFromPlayerTiles[randomindex].PlaceRobber();


            RobberPrefab robber = Instantiate(robberPrefab, AwayFromPlayerTiles[randomindex].TileWorldPostion + (Vector3.right * 0.5f), Quaternion.identity);
            robber.currentTile = AwayFromPlayerTiles[randomindex];

        }



    }


    public void RemoveRobber(RobberPrefab robber, ResourceType resourceType)
    {
        Dictionary<ResourceType, int> amountToReduce = new Dictionary<ResourceType, int> // create temp dic with relvent costs to use to exsiting subtruct resources function 
        {
            { resourceType, 4 }
        };

        player.SubtractResources(amountToReduce);
        robber.currentTile.RemoveRobber(); // Update the tile status
        Destroy(robber.gameObject); // Remove the robber prefab from the scene
    }


    private void FirstTurnPlacement() 
    {


        // Debug.Log("amount of Placed Peices in the first turn " + FirstTurnPlacedPeices);



        // Build a town
        if (FirstTurnPlacedPeices == 0 || FirstTurnPlacedPeices == 2)
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



        if (FirstTurnPlacedPeices == 4)
        {
            FirstTurnPlacedPeices++;
            FirstTurnIsActive = false;
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

        Instantiate(CityPrefab, Settelment.Position, Quaternion.identity);

        player.AddVictoryPoints(1, Settelment.Position);


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

        //first turn
        if (CornersDic.TryGetValue(cornerPosition, out CornersClass corner) && corner.CanBeBuiltOn)
        {
            

            if ( FirstTurnIsActive == true)  
            {
                corner.CanBeBuiltOn = false;
                corner.HasSettlement = true;

                Instantiate(TownPrefab, corner.Position, Quaternion.identity);

               
                player.AddSettelment(corner);
                player.AddVictoryPoints(1,cornerPosition);
                OnTownBuilt?.Invoke();

                foreach (var adjustTile in corner.AdjacentTiles)
                {
                    player.AddResource(adjustTile.resourceType, 1, adjustTile.TileWorldPostion);
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

                FirstTurnPlacement();

            }

            // not first turn
            else 
            {   
                if (isFree == false) { player.SubtractResources(PricesClass.TownCost); }
                

                corner.CanBeBuiltOn = false;
                corner.HasSettlement = true;

                Instantiate(TownPrefab, corner.Position, Quaternion.identity);


                player.AddSettelment(corner);
                player.AddVictoryPoints(1, cornerPosition);
                OnTownBuilt?.Invoke();



                foreach (var NeighborCornerKey in corner.AdjacentCorners)
                {
                    CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
                }



                foreach (var indicator in TownsIndicatorsPrefabList)
                {
                    Destroy(indicator.gameObject);
                }
                TownsIndicatorsPrefabList.Clear();

                ShowBuildIndicatorsTowns();

                



            }


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
        if (SidesDic.TryGetValue(SidePosition, out SidesClass Side) && Side.CanBeBuiltOn)
        {

            if (FirstTurnIsActive == true)
            {
                Side.CanBeBuiltOn = false;
                Side.HasRoad = true;

                Quaternion SideRotation = Quaternion.Euler(0, 0, Side.RotationZ);
                Instantiate(RoadPrefab, Side.Position, SideRotation);
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

                FirstTurnPlacement();
            }

            else
            {

                if (isFree == false) { player.SubtractResources(PricesClass.RoadCost); }
                
                Side.CanBeBuiltOn = false;
                Side.HasRoad = true;

                Quaternion SideRotation2 = Quaternion.Euler(0, 0, Side.RotationZ);
                Instantiate(RoadPrefab, Side.Position, SideRotation2);
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


                ShowBuildIndicatorsRoads();

                

            }


            

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
            uiManager.OpenUnluckyMeterRewardPannel();
            uiManager.UpdateUnluckyMeterProgress(UnluckyMeterProgress);
        }

    }
}


