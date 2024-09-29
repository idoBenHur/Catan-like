using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileClass;





public enum PaymentMode
{
    PayAllAtOnce,
    PayOneByOne
}



[System.Serializable]
public class ResourceRequirement1
{
    public ResourceType resourceType;
    public int requiredAmount;
}


public class Winning_Condition : MonoBehaviour
{

    [SerializeField] public List<ResourceRequirement1> WinningConditions = new List<ResourceRequirement1>();
    private PlayerClass Player;
    [SerializeField] private PaymentMode ThePaymentMode;
    [HideInInspector] public int PaidCount = 0; // used in uimanager


    public void SetupWinningCondition(PlayerClass player_From_BoardManager) // called on start
    {
        Player = player_From_BoardManager;

        if (Player != null)
        {
            Player.OnResourcesChanged += CheckForWin;
        }

        BoardManager.instance.uiManager.ShowWinningCondition(WinningConditions, ThePaymentMode);
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
        if(ThePaymentMode == PaymentMode.PayAllAtOnce)     
        {

            foreach (ResourceRequirement1 requirement in WinningConditions)
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
}
