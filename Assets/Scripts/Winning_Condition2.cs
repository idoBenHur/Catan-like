using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileClass;



[System.Serializable]
public class ResourceRequirement2 
{
    public ResourceType resourceType;
    public int requiredAmount;
    public bool awardBoon;
    public bool isHidden;
}


[System.Serializable]
public class ResourceSelection
{
    public bool Wood;
    public bool Brick;
    public bool Sheep;
    public bool Ore;
    public bool Wheat;

    // Method to get the selected resources as a list
    public List<ResourceType> GetSelectedResources()
    {
        List<ResourceType> selectedResourceTypes = new List<ResourceType>();
        if (Wood) selectedResourceTypes.Add(ResourceType.Wood);
        if (Brick) selectedResourceTypes.Add(ResourceType.Brick);
        if (Sheep) selectedResourceTypes.Add(ResourceType.Sheep);
        if (Ore) selectedResourceTypes.Add(ResourceType.Ore);
        if (Wheat) selectedResourceTypes.Add(ResourceType.Wheat);


        // If no resources are selected, the method will randomly pick resources.
        // Duplicate resources are not added, so the number of different resources selected will vary each time (up to 5) (becuase they are pick randomly).
        if (selectedResourceTypes.Count == 0)
        {
            ResourceType[] allResources = { ResourceType.Wood, ResourceType.Brick, ResourceType.Sheep, ResourceType.Ore, ResourceType.Wheat };
            System.Random rng = new System.Random();

            
            for (int i = 1; i <= 5; i++)
            {
                ResourceType randomResource = allResources[rng.Next(allResources.Length)];

                if(selectedResourceTypes.Contains(randomResource) == false)
                {
                    selectedResourceTypes.Add(randomResource);
                }

                
            }
        }





        return selectedResourceTypes;
    }
}


public class Winning_Condition2 : MonoBehaviour
{

    [SerializeField] private ResourceSelection resourceSelection;
    [SerializeField] private RectTransform paymentArea;
    [SerializeField] private int totalResourcesToWin = 20;

    // Prefabs for resources
    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private GameObject BrickPrefab;
    [SerializeField] private GameObject SheepPrefab;
    [SerializeField] private GameObject OrePrefab;
    [SerializeField] private GameObject WheatPrefab;
    



    private List<ResourceRequirement2> WinningConditions = new List<ResourceRequirement2>();
    private List<TileClass> AllTiles; // List of all tiles on the map
    private Dictionary<ResourceType, GameObject> resourcePrefabDictionary = new Dictionary<ResourceType, GameObject>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();






    void Start()
    {
        



      //  SelectRandomResourceRequirementList();

    }



    public void setup(Dictionary<Vector3Int, TileClass> dic)
    {
        resourcePrefabDictionary.Add(ResourceType.Wood, woodPrefab);
        resourcePrefabDictionary.Add(ResourceType.Brick, BrickPrefab);
        resourcePrefabDictionary.Add(ResourceType.Sheep, SheepPrefab);
        resourcePrefabDictionary.Add(ResourceType.Ore, OrePrefab);
        resourcePrefabDictionary.Add(ResourceType.Wheat, WheatPrefab);



        AllTiles = dic.Values.ToList();

        GenerateWinningCondition();
        SpawnAllResourcePrefabs();


    }

    // Generate winning conditions based on selected resources and probabilities
    private void GenerateWinningCondition()
    {
        //Calculate resource probabilities (GoalResourcesProb)
        Dictionary<ResourceType, float> AllResourcesProb = CalculateAllResourcesProb();
        List<ResourceType> selectedResourceTypes = resourceSelection.GetSelectedResources(); // list of only the selected resources of this level win condition

        // populate a dic with only the selected Resources and thier Probabilities
        Dictionary<ResourceType, float> filteredProb = new Dictionary<ResourceType, float>();
        foreach (var resource in AllResourcesProb)
        {
            if (selectedResourceTypes.Contains(resource.Key))
            {               
                filteredProb[resource.Key] = resource.Value;
            }
        }





        //Determine how much from each resource to "demand"
        float totalWeight = filteredProb.Values.Sum();
        foreach (var resource in filteredProb)
        {
            int requiredAmount = Mathf.RoundToInt((resource.Value / totalWeight) * totalResourcesToWin);

            WinningConditions.Add(new ResourceRequirement2
            {
                resourceType = resource.Key,
                requiredAmount = requiredAmount,
                awardBoon = false,
                isHidden = true
            });
        }

        // intial reveal of first resource
        if (WinningConditions.Count > 0)
            WinningConditions[0].isHidden = false;
    }



    private Dictionary<ResourceType, float> CalculateAllResourcesProb()
    {
        Dictionary<ResourceType, float> ResourcesProb = new Dictionary<ResourceType, float>();

        foreach (var tile in AllTiles)
        {
            if (tile.resourceType == ResourceType.Desert) { continue; }

            if (ResourcesProb.ContainsKey(tile.resourceType) == false)
            {
                ResourcesProb[tile.resourceType] = 0f;

            }

            ResourcesProb[tile.resourceType] += GetDiceProbability(tile.numberToken);
        }

        return ResourcesProb;
    }


    private float GetDiceProbability(int number)
    {
        // Standard Catan dice roll probabilities
        switch (number)
        {
            case 2: return 1f / 36f;
            case 3: return 2f / 36f;
            case 4: return 3f / 36f;
            case 5: return 4f / 36f;
            case 6: return 5f / 36f;
            case 7: return 6f / 36f; // Adjust if 7 triggers a different mechanic
            case 8: return 5f / 36f;
            case 9: return 4f / 36f;
            case 10: return 3f / 36f;
            case 11: return 2f / 36f;
            case 12: return 1f / 36f;
            default: return 0f;
        }
    }





    private void SpawnAllResourcePrefabs()
    {
        foreach (var requirement in WinningConditions)
        {
            if (resourcePrefabDictionary.TryGetValue(requirement.resourceType, out GameObject prefabToSpawn))
            {
                GameObject spawnedPrefab = Instantiate(prefabToSpawn, paymentArea);
                spawnedPrefabs.Add(spawnedPrefab);

                ResourcePayPrefab paymentPrefabScript = spawnedPrefab.GetComponent<ResourcePayPrefab>();
                paymentPrefabScript.Initialize(this, requirement.requiredAmount, requirement.isHidden, requirement.awardBoon);
            }
            else
            {
                Debug.LogWarning($"Prefab not found for resource type: {requirement.resourceType}");
            }
        }


    }

    public void PayResource(ResourceType resourceType, int requiredAmount, bool awardBoon, GameObject clickedPrefab)
    {
        Dictionary<ResourceType, int> tempCost = new Dictionary<ResourceType, int>();
        tempCost.Add(resourceType, requiredAmount);
        PlayerClass thePlayer = BoardManager.instance.player;

        if (thePlayer.CanAffordToBuild(tempCost) == true)
        {
            thePlayer.SubtractResources(tempCost);
            DestroyCurrentPrefab(clickedPrefab);

            if (awardBoon == true && spawnedPrefabs.Count > 0)
            {
                BoardManager.instance.boonManager.GiveBoon();

            }

        }

    }

    private void DestroyCurrentPrefab(GameObject clickedPrefab)
    {
        if (spawnedPrefabs.Contains(clickedPrefab))
        {
            Destroy(clickedPrefab);
            spawnedPrefabs.Remove(clickedPrefab);

            // Reveal the next resource or end the game if all resources are paid
            if (spawnedPrefabs.Count > 0)
            {
                RevealNextResource();
            }
            else
            {
                BoardManager.instance.uiManager.EndGame(true);
                Debug.Log("All resources have been paid!");
            }
        }
    }

    private void RevealNextResource()
    {
        if (spawnedPrefabs.Count > 0)
        {
            GameObject nextPrefab = spawnedPrefabs[0];
            ResourcePayPrefab nextPaymentPrefab = nextPrefab.GetComponent<ResourcePayPrefab>();
            nextPaymentPrefab.UpdateHiddenStatus(false); // Reveal the next resource
        }
    }



    //private void SelectRandomResourceRequirementList()
    //{
    //    if (resourceRequirementsSet.Count > 0)
    //    {
    //        int randomIndex = Random.Range(0, resourceRequirementsSet.Count); // Pick a random set
    //        ChosenWinningCondition = resourceRequirementsSet[randomIndex].resourceRequirementsList; // Assign the selected list
    //    }
    //    else
    //    {
    //        Debug.LogError("No resource requirement sets defined!");
    //    }
    //}





}
