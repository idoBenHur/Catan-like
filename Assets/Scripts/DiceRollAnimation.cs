using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceRollAnimation : MonoBehaviour
{
    public Sprite[] DiceSides;
    private Image DiceImage;



    public IEnumerator RollDiceAnimation2(int DieResult)
    {

        DiceImage = GetComponent<Image>();

        int Dice1RandomSide = 0;


        for (int i = 0; i <= 3; i++)
        {

            Dice1RandomSide = UnityEngine.Random.Range(1, 7);

            // Set sprite to upper face of dice from array according to random value
            DiceImage.sprite = DiceSides[Dice1RandomSide - 1];


            // Pause before next itteration
            yield return new WaitForSeconds(0.08f);
        }

        DiceImage.sprite = DiceSides[DieResult - 1];


    }
}
