using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicators : MonoBehaviour
{
    public GameObject TownPrefab; 
    public GameObject RoadPrefab;
    private float NeighborsRadius = 1f; 
    private Collider2D[] ColidersInRadius;
    private bool RoadIndicator = false;
    private bool TownIndicator = false;
    private IndicatorPlacementRoad IndicatorPlacementRoadScript;


    private void Start()
    {
        if (gameObject.tag == "RoadIndicator")
        {
            RoadIndicator = true;
        }

        if (gameObject.tag == "TownIndicator")
        {
            TownIndicator = true;
        }

    }



    void OnMouseDown()
    {

        if (TownIndicator == true)
        {
            ColidersInRadius = Physics2D.OverlapCircleAll(transform.position, NeighborsRadius);

            foreach (var hitCollider in ColidersInRadius)
            {
                if (hitCollider.gameObject.CompareTag("Road"))
                {

                    Instantiate(TownPrefab,transform.position,transform.rotation);
                    Destroy(gameObject);

                }


            }


        }


        if (RoadIndicator == true) 
        {

            Instantiate(RoadPrefab, transform.position, transform.rotation);
            //IndicatorPlacementRoadScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<IndicatorPlacementRoad>();
            IndicatorPlacementRoadScript = GameManager.instance.GetComponent<IndicatorPlacementRoad>();
            IndicatorPlacementRoadScript.FindHexesCenterPoint();
            Destroy(gameObject);

        }


    }

}
