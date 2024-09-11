using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileClass;




[System.Serializable]
public class ResourceRequirement
{
    public ResourceType resourceType;
    public int requiredAmount;
}


public class Winning_Condition : MonoBehaviour
{

    [SerializeField] private List<ResourceRequirement> WinningConditions = new List<ResourceRequirement>();
    private PlayerClass Player;


    public void SetupWinningCondition(PlayerClass player_From_BoardManager) // called on start
    {
        Player = player_From_BoardManager;

        if (Player != null)
        {
            Player.OnResourcesChanged += CheckForWin;
        }

        BoardManager.instance.uiManager.ShowWinningCondition(WinningConditions);
    }


    private void OnDestroy()
    {
        if (Player != null)
        {
            Player.OnResourcesChanged -= CheckForWin;
        }
    }





    public void CheckForWin()
    {
        foreach (ResourceRequirement requirement in WinningConditions)
        {
            // Get the player's current amount of the required resource
            int playerResourceAmount = BoardManager.instance.player.CheckResourceAmount(requirement.resourceType);

            // If the player doesn't have enough of this resource, they haven't won
            if (playerResourceAmount < requirement.requiredAmount)
            {
                return; // Exit early if the player hasn't met this requirement
            }
        }

        BoardManager.instance.uiManager.ShowTheButtonWin();

    }
}
