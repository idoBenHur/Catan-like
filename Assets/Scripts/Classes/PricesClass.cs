using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileClass;

public class PricesClass
{
    public static readonly Dictionary<ResourceType, int> RoadCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Brick, 1 }
    };

    public static readonly Dictionary<ResourceType, int> TownCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Brick, 1 },
        { ResourceType.Sheep, 1 },
        { ResourceType.Wheat, 1 }
    };

    public static readonly Dictionary<ResourceType, int> CityCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Ore, 3 },
        { ResourceType.Wheat, 2 }
    };

    public static readonly Dictionary<ResourceType, int> MeterUpgrade = new Dictionary<ResourceType, int>
    {
        { ResourceType.Ore, 1 },
        { ResourceType.Wheat, 1 },
        { ResourceType.Sheep, 1 }
    };
}
