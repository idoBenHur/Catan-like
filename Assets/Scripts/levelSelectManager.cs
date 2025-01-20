using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class levelSelectManager : MonoBehaviour
{

    [SerializeField] private List<LevelConfig> Low_DifficultyLevels; // List of all available LevelConfig assets
    [SerializeField] private List<LevelConfig> Mid_DifficultyLevels; // List of all available LevelConfig assets
    [SerializeField] private List<LevelConfig> High_DifficultyLevels; // List of all available LevelConfig assets







    [SerializeField] private List<LevelSelectButtons> IslandPressingScripts; // The objects representing levels




    void Start()
    {
        if (Low_DifficultyLevels.Count < 2 || IslandPressingScripts.Count < 2)
        {
            Debug.LogError("Not enough levels or level objects! Ensure at least 2 levels and 2 objects are available.");
            return;
        }



        AssignRandomLevels();

    }

    private void AssignRandomLevels()
    {
        // Use LINQ to pick two random levels from the list
        var randomLevels = Low_DifficultyLevels.OrderBy(_ => Random.value).Take(2).ToList();

        // Assign the selected levels to the level objects
        for (int i = 0; i < randomLevels.Count; i++)
        {

            IslandPressingScripts[i].AssignLevelConfig(randomLevels[i]);
        }
    }
}
