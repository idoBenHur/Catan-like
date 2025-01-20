using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileClass;

public class PricesClass
{
    public static readonly Dictionary<ResourceType, int> RoadCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Rum, 1 }
    };

    public static readonly Dictionary<ResourceType, int> TownCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Wood, 1 },
        { ResourceType.Rum, 1 },
        { ResourceType.Gold, 1 },
        { ResourceType.Gunpowder, 1 }
    };

    public static readonly Dictionary<ResourceType, int> CityCost = new Dictionary<ResourceType, int>
    {
        { ResourceType.Gem, 2 },
        { ResourceType.Gunpowder, 2 }
    };

    public static readonly Dictionary<ResourceType, int> MeterUpgrade = new Dictionary<ResourceType, int>
    {
        { ResourceType.Gem, 1 },
        { ResourceType.Gunpowder, 1 },
        { ResourceType.Gold, 1 }
    };

    public static readonly Dictionary<ResourceType, int> RemoveFog = new Dictionary<ResourceType, int>
    {
        { ResourceType.Wood, 1 },

    };
}
