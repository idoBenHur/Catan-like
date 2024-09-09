using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceBox : MonoBehaviour
{

    [SerializeField] private GameObject DiceBox;
    [SerializeField] private GameObject DicePrefab;
    [SerializeField] private GameObject SelectBox;

    public int AmountOfNewDiceEachRoll = 5;
    private int MaxDiceAmountInDiceBox = 6;


    //destroys dice when in seleceted box
    private void Update()
    {
        if (SelectBox.transform.childCount == 2)
        {
            int dice1 = 0;
            int dice2 = 0;
            foreach (Transform child in SelectBox.transform)
            {
                NewDiceScript diceNumber = child.GetComponent<NewDiceScript>();
                if (diceNumber != null)
                {
                    if (dice1 == 0) { dice1 = diceNumber.DiceResult; }
                    else if (dice2 == 0) { dice2 = diceNumber.DiceResult; }

                }
                Destroy(child.gameObject);
            }

           // BoardManager.instance.DicesPlayed(dice1,dice2);
        }
    }


    // destroy all dice, roll new ones
    private void RollNewDicesBUTTON() // OLD
    {

        BoardManager.instance.uiManager.CloseAllUi();


        GameObject SeletcBox = this.gameObject;

        foreach (Transform child in SeletcBox.transform)
        {
            Destroy(child.gameObject);
        }


        foreach (Transform child in DiceBox.transform)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < AmountOfNewDiceEachRoll; i++)
        {
            Instantiate(DicePrefab, DiceBox.transform);
        }

       // BoardManager.instance.DicesRolled();
    }



    private void AddTemporerayDice()
    {

        if(DiceBox.transform.childCount < MaxDiceAmountInDiceBox)
        {
            Instantiate(DicePrefab, DiceBox.transform);
        }
    }
}
