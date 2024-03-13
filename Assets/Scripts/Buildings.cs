using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    private IndicatorPlacementRoad IndicatorPlacementRoadScript;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("asd");
        if (gameObject.tag == "Town")
        {
            

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 1f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag("TownIndicator"))
                {
                    Destroy(hitCollider.gameObject);
                }

            }

        }

        if (gameObject.tag == "Road")
        {


        }





    }
}