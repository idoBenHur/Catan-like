using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HarborResourceType { Wood, Brick, Sheep, Ore, Wheat, Any }  // Assuming 'Any' for 3:1 harbors

public class HarborClass
{
    public HarborResourceType TradeResource { get; private set; }
    public int TradeRatio { get; private set; }
    public GameObject HarborGameObject;



    public HarborClass(HarborResourceType resourceType, int tradeRatio)
    {
        TradeResource = resourceType;
        TradeRatio = tradeRatio;
    }
}

