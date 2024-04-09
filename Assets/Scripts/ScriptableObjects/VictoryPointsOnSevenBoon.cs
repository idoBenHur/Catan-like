using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Boons/VictoryPointsOnSeven")]
public class VictoryPointsOnSevenBoon : Boon
{
    public override void Activate(PlayerClass player)
    {
       // BoardManager.OnDiceRolled += CheckConditions;
    }

    public override void Deactivate(PlayerClass player)
    {
       //BoardManager.OnDiceRolled -= CheckConditions;
    }

    private void CheckConditions(int rollResult)
    {

        if (rollResult == 7)
        {
            bool HasTowns = false;

            foreach (var settelment in BoardManager.instance.player.SettelmentsList)
            {

                if (settelment.HasCityUpgade == true)
                {

                    HasTowns = true;
                    break;
                }
            }

            if (HasTowns == false) 
            {
                Debug.Log("Add points");
                BoardManager.instance.player.AddVictoryPoints(1);
            }
        }
    }

}
