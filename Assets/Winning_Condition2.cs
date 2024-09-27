using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileClass;

public class Winning_Condition2 : MonoBehaviour
{
    public List<ResourceRequirement> resourceRequirementsList; // Set up your resource requirements
    public RectTransform paymentArea; // UI container for payment prefabs
    private int currentResourceIndex = 0;

    // Dictionary to store resource type and its corresponding prefab
    private Dictionary<ResourceType, GameObject> resourcePrefabDictionary = new Dictionary<ResourceType, GameObject>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();


    public GameObject woodPrefab;
    public GameObject BrickPrefab;
    public GameObject SheepPrefab;
    public GameObject OrePrefab;
    public GameObject WheatPrefab;



    void Start()
    {
        
        resourcePrefabDictionary.Add(ResourceType.Wood, woodPrefab);
        resourcePrefabDictionary.Add(ResourceType.Brick, BrickPrefab);
        resourcePrefabDictionary.Add(ResourceType.Sheep, SheepPrefab);
        resourcePrefabDictionary.Add(ResourceType.Ore, OrePrefab);
        resourcePrefabDictionary.Add(ResourceType.Wheat, WheatPrefab);



        SpawnAllResourcePrefabs();
    }

    private void SpawnAllResourcePrefabs()
    {
        for (int i = 0; i < resourceRequirementsList.Count; i++)
        {
            ResourceRequirement requirement = resourceRequirementsList[i];

            if (resourcePrefabDictionary.TryGetValue(requirement.resourceType, out GameObject prefabToSpawn))
            {
                GameObject spawnedPrefab = Instantiate(prefabToSpawn, paymentArea);
                spawnedPrefabs.Add(spawnedPrefab);

                ResourcePayPrefab paymentPrefabScript = spawnedPrefab.GetComponent<ResourcePayPrefab>();
                bool isRevealed = i == 0; //Reveal only the first resource
                paymentPrefabScript.Initialize(this, requirement.requiredAmount, isRevealed);

            }

            else
            {
                Debug.LogWarning($"Prefab not found for resource type: {requirement.resourceType}");
            }




        }


    }

    public void PayResource(ResourceType resourceType, int requiredAmount)
    {
        Dictionary<ResourceType, int> tempCost = new Dictionary<ResourceType, int>();
        tempCost.Add(resourceType, requiredAmount);
        PlayerClass thePlayer = BoardManager.instance.player;

        if(thePlayer.CanAffordToBuild(tempCost) == true ) 
        {
            thePlayer.SubtractResources(tempCost);
            DestroyCurrentPrefab();
        }
        
    }

    private void DestroyCurrentPrefab()
    {
        if (currentResourceIndex < spawnedPrefabs.Count)
        {
            // Destroy the current prefab
            GameObject prefabToDestroy = spawnedPrefabs[currentResourceIndex];
            Destroy(prefabToDestroy);

            // Move to the next resource and reveal it
            currentResourceIndex++;
            if (currentResourceIndex < spawnedPrefabs.Count)
            {
                RevealNextResource();
            }
            else
            {
                Debug.Log("All resources have been paid!");
            }
        }
    }

    private void RevealNextResource()
    {
        // Reveal the next prefab in the list
        GameObject nextPrefab = spawnedPrefabs[currentResourceIndex];
        ResourcePayPrefab nextPaymentPrefab = nextPrefab.GetComponent<ResourcePayPrefab>();
        nextPaymentPrefab.ToggleReveal(true); // Reveal the next resource
    }



}
