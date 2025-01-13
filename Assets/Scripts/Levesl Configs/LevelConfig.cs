using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelConfig", menuName = "NewLevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Properties")]
    [Tooltip("The number of turns the player has in this level.")]
    public int turns;

    [Header("Resource Quantities")]
    [Tooltip("The resources available on the map and their respective quantities.")]
    public List<ResourceEntry> resources;











}
