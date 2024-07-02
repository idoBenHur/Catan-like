using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceBox : MonoBehaviour
{

    [SerializeField] private GameObject DiceBox;
    [SerializeField] private GameObject DicePrefab;

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

    public void RollNewDices()
    {
        // Get a reference to the parent object
        GameObject SeletcBox = this.gameObject;

        // Loop through all children of the parent object
        foreach (Transform child in SeletcBox.transform)
        {
            // Destroy the child object
            Destroy(child.gameObject);
        }

        foreach (Transform child in DiceBox.transform)
        {
            // Destroy the child object
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            Instantiate(DicePrefab, DiceBox.transform);
        }


    }

}
