using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class levelSelectManager : MonoBehaviour
{

    [SerializeField] private List<LevelConfig> levels; // List of all available LevelConfig assets
    [SerializeField] private List<LevelSelectButtons> levelObjects; // The objects representing levels




    void Start()
    {
        if (levels.Count < 2 || levelObjects.Count < 2)
        {
            Debug.LogError("Not enough levels or level objects! Ensure at least 2 levels and 2 objects are available.");
            return;
        }



        AssignRandomLevels();

    }

    private void AssignRandomLevels()
    {
        // Use LINQ to pick two random levels from the list
        var randomLevels = levels.OrderBy(_ => Random.value).Take(2).ToList();

        // Assign the selected levels to the level objects
        for (int i = 0; i < randomLevels.Count; i++)
        {
            levelObjects[i].AssignLevelConfig(randomLevels[i]);
        }
    }
}
