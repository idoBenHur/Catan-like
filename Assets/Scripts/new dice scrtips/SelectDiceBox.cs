using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceBox : MonoBehaviour
{

    [SerializeField] private GameObject DiceBox;
    [SerializeField] private GameObject DicePrefab;
    private int AmountOfNewDiceEachRoll = 4;


    //destroys dice when in seleceted box
    private void Update()
    {
        if (transform.childCount == 2)
        {
            int dice1 = 0;
            int dice2 = 0;
            foreach (Transform child in transform)
            {
                NewDiceScript diceNumber = child.GetComponent<NewDiceScript>();
                if (diceNumber != null)
                {
                    if (dice1 == 0) { dice1 = diceNumber.DiceResult; }
                    else if (dice2 == 0) { dice2 = diceNumber.DiceResult; }

                }
                Destroy(child.gameObject);
            }

            BoardManager.instance.DicesPlayed(dice1,dice2);
        }
    }


    // destroy all dice, roll new ones
    public void RollNewDicesBUTTON()
    {
        GameObject SeletcBox = this.gameObject;

        foreach (Transform child in SeletcBox.transform)
        {
            Destroy(child.gameObject);
        }

        int currentDiceCount =0;

        foreach (Transform child in DiceBox.transform)
        {
            currentDiceCount++;
        }

        int AmountOfNewDiceToCreate = AmountOfNewDiceEachRoll - currentDiceCount;

        for (int i = 0; i < AmountOfNewDiceToCreate; i++)
        {
            Instantiate(DicePrefab, DiceBox.transform);
        }

        BoardManager.instance.DicesRolled();
    }

}
