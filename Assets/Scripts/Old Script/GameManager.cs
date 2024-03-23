using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private bool ButtonPressedBuildTown = false;
    private bool ButtonPressedBuildRoad = false;

    public int Wood;
    public int Sheep;
    public int Rock;
    public int Tit;
    public int wheat;
    public Text WoodDisplayText;

    public static GameManager instance;



    private void Awake()
    {
        instance = this;
    }


    public void ReduceResources(string BuildingType)
    {
        if (instance != null) { }
    }





    public void ShuffleMapButton() 
    {

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Number");

        foreach (GameObject obj in gameObjects)
        {
            Destroy(obj);
        }
        
        MapGenerator MapGeneratorScript = GetComponent<MapGenerator>();
        MapGeneratorScript.ReshuffleBoard();






    }

    public void BuildTownButton()
    
    {
        if (ButtonPressedBuildTown == false && ButtonPressedBuildRoad == false)
        {
            ButtonPressedBuildTown = true;
            IndicatorPlacementTown cityplacementScript = GetComponent<IndicatorPlacementTown>();
            cityplacementScript.PlacePrefabsAtCorners();
            return;
        }


        if (ButtonPressedBuildTown == true && ButtonPressedBuildRoad == false)
        {
            ButtonPressedBuildTown = false;
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("TownIndicator");

            foreach (GameObject obj in gameObjects)
            {
                Destroy(obj);
            }
            return;
        }



        
    }


    public void BuildRoadButton()

    {
        if (ButtonPressedBuildRoad == false && ButtonPressedBuildTown == false)
        {
            ButtonPressedBuildRoad = true;
            IndicatorPlacementRoad RoadplacementScript = GetComponent<IndicatorPlacementRoad>();
            RoadplacementScript.FindHexesCenterPoint();
            return;
        }


        if (ButtonPressedBuildRoad == true && ButtonPressedBuildTown == false)
        {
            ButtonPressedBuildRoad = false;
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("RoadIndicator");

            // Loop through the array and destroy each object
            foreach (GameObject obj in gameObjects)
            {
                Destroy(obj);
            }
            return;
        }




    }


}



