using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceBox : MonoBehaviour
{

    [SerializeField] private GameObject DiceBox;
    [SerializeField] private GameObject DicePrefab;


    //destroys dice when in seleceted box
    private void Update()
    {
        if (transform.childCount == 2)
        {
            int sum = 0;
            foreach (Transform child in transform)
            {
                NewDiceScript dice = child.GetComponent<NewDiceScript>();
                if (dice != null)
                {
                    sum += dice.DiceResult;
                }
                Destroy(child.gameObject);
            }

            BoardManager.instance.DicesPlayed(sum);
        }
    }


    // destroy all dice, roll new ones
    public void RollNewDices()
    {
        GameObject SeletcBox = this.gameObject;

        foreach (Transform child in SeletcBox.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in DiceBox.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            Instantiate(DicePrefab, DiceBox.transform);
        }

        BoardManager.instance.DicesRolled();
    }

}
