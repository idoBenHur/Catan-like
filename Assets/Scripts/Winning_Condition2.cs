using System.Collections;
using System.Collections.Generic;
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
public class ResourceRequirementSet
{
    public List<ResourceRequirement2> resourceRequirementsList;
}


public class Winning_Condition2 : MonoBehaviour
{
    [SerializeField] private List<ResourceRequirementSet> resourceRequirementsSet;
    [SerializeField] private RectTransform paymentArea;


    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private GameObject BrickPrefab;
    [SerializeField] private GameObject SheepPrefab;
    [SerializeField] private GameObject OrePrefab;
    [SerializeField] private GameObject WheatPrefab;

    private List<ResourceRequirement2> ChosenWinningCondition;  
    private int currentResourceIndex = 0;
    private Dictionary<ResourceType, GameObject> resourcePrefabDictionary = new Dictionary<ResourceType, GameObject>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();






    void Start()
    {
        
        resourcePrefabDictionary.Add(ResourceType.Wood, woodPrefab);
        resourcePrefabDictionary.Add(ResourceType.Brick, BrickPrefab);
        resourcePrefabDictionary.Add(ResourceType.Sheep, SheepPrefab);
        resourcePrefabDictionary.Add(ResourceType.Ore, OrePrefab);
        resourcePrefabDictionary.Add(ResourceType.Wheat, WheatPrefab);


        SelectRandomResourceRequirementList();
        SpawnAllResourcePrefabs();
    }

    private void SpawnAllResourcePrefabs()
    {
        for (int i = 0; i < ChosenWinningCondition.Count; i++)
        {
            ResourceRequirement2 requirement = ChosenWinningCondition[i];

            if (resourcePrefabDictionary.TryGetValue(requirement.resourceType, out GameObject prefabToSpawn))
            {
                GameObject spawnedPrefab = Instantiate(prefabToSpawn, paymentArea);
                spawnedPrefabs.Add(spawnedPrefab);

                ResourcePayPrefab paymentPrefabScript = spawnedPrefab.GetComponent<ResourcePayPrefab>();
                bool isHidden = requirement.isHidden; 
                paymentPrefabScript.Initialize(this, requirement.requiredAmount, isHidden, requirement.awardBoon );

            }

            else
            {
                Debug.LogWarning($"Prefab not found for resource type: {requirement.resourceType}");
            }




        }


    }

    public void PayResource(ResourceType resourceType, int requiredAmount, bool awardBoon, GameObject ThePrefab)
    {
        Dictionary<ResourceType, int> tempCost = new Dictionary<ResourceType, int>();
        tempCost.Add(resourceType, requiredAmount);
        PlayerClass thePlayer = BoardManager.instance.player;

        if (thePlayer.CanAffordToBuild(tempCost) == true)
        {
            thePlayer.SubtractResources(tempCost);
            DestroyCurrentPrefab(ThePrefab);

            if (awardBoon == true && spawnedPrefabs.Count > 0)
            {
                BoardManager.instance.boonManager.GiveBoon();

            }

        }

    }

    private void DestroyCurrentPrefab(GameObject clickedPrefab)
    {
        if (currentResourceIndex < spawnedPrefabs.Count)
        {
            // Destroy the current prefab
            GameObject prefabToDestroy = clickedPrefab; //spawnedPrefabs[currentResourceIndex];
            Destroy(prefabToDestroy);
            spawnedPrefabs.Remove(prefabToDestroy);

            // Move to the next resource and reveal it
           // currentResourceIndex++;
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
        // Reveal the next prefab in the list
        GameObject nextPrefab = spawnedPrefabs[0]; //spawnedPrefabs[currentResourceIndex];
        ResourcePayPrefab nextPaymentPrefab = nextPrefab.GetComponent<ResourcePayPrefab>();

        

        nextPaymentPrefab.UpdateHiddenStatus(false); // Reveal the next resource
    }



    private void SelectRandomResourceRequirementList()
    {
        if (resourceRequirementsSet.Count > 0)
        {
            int randomIndex = Random.Range(0, resourceRequirementsSet.Count); // Pick a random set
            ChosenWinningCondition = resourceRequirementsSet[randomIndex].resourceRequirementsList; // Assign the selected list
        }
        else
        {
            Debug.LogError("No resource requirement sets defined!");
        }
    }





}
