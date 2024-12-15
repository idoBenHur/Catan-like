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
    //    [SerializeField] public Winning_condition3 Winning_condition3; //test
    private Winning_Condition4 winningCondition_4;








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


    // prefabs losts
    [HideInInspector] public List<GameObject> CornersIndicatorsPrefabList = new List<GameObject>();
    [HideInInspector] public List<GameObject> SidesIndicatorsPrefabList = new List<GameObject>();


    // main dictionaries
    public Dictionary<Vector3Int, TileClass> TilesDictionary = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3, CornersClass> CornersDic = new Dictionary<Vector3, CornersClass>();
    public Dictionary<Vector3, SidesClass> SidesDic = new Dictionary<Vector3, SidesClass>();


    // events
    public static event Action OnDiceRolled;
    public static event Action OnDicePlayed;
    public static event Action OnRoadBuilt;
    public static event Action OnTownBuilt;
    public static event Action OnCityBuilt;

    //    public static event Action OnUnlcukyRoll;









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
            return;
        }

     //   OnDiceRolled = null;

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

        
        challenges.SetUpPlayerChallenges(player);
        uiManager.SetUpUIManager(player);

        winningCondition_4 = GetComponent<Winning_Condition4>();
        winningCondition_4.initializeFlags(CornersDic.Values.ToList());










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


        foreach (var settelment in player.SettelmentsList)
        {

            foreach (var tile in settelment.AdjacentTiles)
            {

                if(tile.numberToken != DiceResult || tile.isBlocked == true || tile.underFog == true)
                {
                    continue;
                }


                int resourceAmount;

                if (settelment.HasCityUpgade == false)
                {
                    resourceAmount = 2;
                }
                else { resourceAmount = 3; }


                player.AddResource(tile.resourceType, resourceAmount, tile.TileWorldPostion);

                DOVirtual.DelayedCall(0f, () =>
                {
                    Instantiate(ResourceGainPS, tile.TileWorldPostion, Quaternion.identity);
                });



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

            ShowBuildIndicatorsTowns();



            //logic that for game with roads:

            //foreach (var corner in CornersDic.Values)
            //{

            //    if (corner.CanBeBuiltOn == true)
            //    {
            //        GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
            //        TownsIndicatorsPrefabList.Add(indicator);
            //        indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner);
            //    }

            //}

            return;
        }
       

        // Build a road
        if(FirstTurnPlacedPeices == 50 || FirstTurnPlacedPeices == 3)
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
                            SidesIndicatorsPrefabList.Add(indicator);
                            indicator.GetComponent<RoadBuildIndicatorPrefab>().Setup(SideNearTown.Position);
                        }
                    }

           


                }
                                                 
            }   


            return;
        }



        if (FirstTurnPlacedPeices == 1)
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
                CornersIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(settelment, IndicatorType.City);
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

        OnCityBuilt?.Invoke();


        //foreach (var indicator in CitiesIndicatorsPrefabList)
        //{
        //    Destroy(indicator.gameObject);
        //}
        //CitiesIndicatorsPrefabList.Clear();

        uiManager.CloseAllUi();











    }



    public void ShowBuildIndicatorsTowns()
    {


        if (player.CanAffordToBuild(PricesClass.TownCost) == false && FirstTurnIsActive == false)
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




            //all above and this logic is for playing with roads

            //if (ConnectedToRoad == true && NearATown == false)
            //{
            //    corner.CanBeBuiltOn = true;
            //}
            //else
            //{
            //    corner.CanBeBuiltOn = false;
            //    
            //}


            //this is the logic for the new fog


            corner.CanBeBuiltOn = false;
            foreach (var adjustedTiles in corner.AdjacentTiles)
            {
                if (adjustedTiles.underFog == false) // if one tile is not underfog, you can place a town there
                {
                    corner.CanBeBuiltOn = true;
                    break;
                }
            }




            


            if (corner.CanBeBuiltOn == true)
            {
                
                GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                CornersIndicatorsPrefabList.Add(indicator);
                indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner, IndicatorType.Town);
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

            foreach (var adjustTile in corner.AdjacentTiles) 
            {
                


              //  adjustTile.underFog = false;
               // mapGenerator.PlaceAndRemoveFogTiles();
            }


            foreach (var NeighborCornerKey in corner.AdjacentCorners)
            {
                CornersDic[NeighborCornerKey.Position].CanBeBuiltOn = false;
            }


            uiManager.CloseAllUi();
            //foreach (var indicator in TownsIndicatorsPrefabList)
            //{
            //    Destroy(indicator.gameObject);
            //}
            //TownsIndicatorsPrefabList.Clear();





            //DiceBox_Skill diceBox = skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;
            //diceBox.AddPermaDie();
            //diceBox.AddTempDie(1);


            if (FirstTurnIsActive == true) { FirstTurnPlacement(); }
            



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
                SidesIndicatorsPrefabList.Add(indicator);
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

            //foreach (var indicator in RoadsIndicatorsPrefabList)
            //{
            //    Destroy(indicator.gameObject);
            //}

            uiManager.CloseAllUi();

            if (FirstTurnIsActive == true) { FirstTurnPlacement(); }




            

        }
    }


    public void ShowRemoveFogIndicator()
    {
        if (player.CanAffordToBuild(PricesClass.RemoveFog) == false)
        {

            return;
        }


        foreach (var corner in CornersDic.Values)
        {
            foreach (var adjacentTie in corner.AdjacentTiles)
            {
                if (adjacentTie.underFog == true)
                {
                    GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
                    indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner, IndicatorType.FogRemover);
                    CornersIndicatorsPrefabList.Add(indicator);
                    break;
                }

            }
        }
    }

    public void RemoveFogAroundAcorner(CornersClass middlePoint, bool isFree = false)
    {
        if (!CornersDic.TryGetValue(middlePoint.Position, out CornersClass corner))
        {
            Debug.Log("corrner was not found ion the dictionary ");
            return;
        }

        if (isFree == false) { player.SubtractResources(PricesClass.RemoveFog); }




        foreach (var tile in middlePoint.AdjacentTiles)
        {
            RemoveFogFromTile(tile);
        }


        uiManager.CloseAllUi();


    }


    public void RemoveFogFromTile(TileClass theTile)
    {
        theTile.underFog = false;
        mapGenerator.PlaceAndRemoveFogTiles();

        foreach(var corner in theTile.AdjacentCorners)
        {
            if(corner.HaveAFlag == true)
            {
                winningCondition_4.spawnFlagVisual(corner); 
            }
        }

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



    public void DestroyIndicators()
    {
        foreach (var indicator in CornersIndicatorsPrefabList)
        {
            Destroy(indicator.gameObject);
        }
        CornersIndicatorsPrefabList.Clear();


        foreach (var indicator in SidesIndicatorsPrefabList)
        {
            Destroy(indicator.gameObject);
        }
        SidesIndicatorsPrefabList.Clear();




    }


}


