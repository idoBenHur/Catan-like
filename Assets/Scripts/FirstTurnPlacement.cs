using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTurnPlacement : MonoBehaviour
{
    public GameObject CornerIndicatorPrefab;
    public List<GameObject> IndicatorsPrefabList = new List<GameObject>();
    public Dictionary<Vector3, CornersClass> CornersDicCopy;


    // Start is called before the first frame update
    void Start()
    {
        CornersDicCopy = BoardManager.instance.CornersDic;


        
        foreach(var corner in CornersDicCopy.Values)
        {

            GameObject indicator = Instantiate(CornerIndicatorPrefab, corner.Position, Quaternion.identity);
            IndicatorsPrefabList.Add(indicator);
            indicator.GetComponent<TownBuildIndicatorPrefab>().Setup(corner.Position);
                
        }


    }

    private void Update()
    {

    }


}
