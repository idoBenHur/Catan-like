using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceRollAnimation : MonoBehaviour
{
    public Sprite[] DiceSides;
    private Image DiceImage;

    private RectTransform diceRectTransform;
    private Vector3 OGscale;
    private Vector3 MinScale;
    private Sequence TheAnimationSequence;

    public float animationDuration = 0.7f;




    private void Awake()
    {
        DiceImage = GetComponent<Image>();
        diceRectTransform = GetComponent<RectTransform>();
        OGscale = diceRectTransform.localScale;
        MinScale = OGscale / 3;
    }






    public IEnumerator RollDiceAnimation2(int DieResult) // old!
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


    public void NewAnimation(int DieResult)
    {
        DiceImage.sprite = DiceSides[DieResult - 1];

        TheAnimationSequence = DOTween.Sequence();

        // Set rotation parameters
        float finalSpinAngle = 360; // Number of spins        
        

        diceRectTransform.localScale = MinScale;

        // Scale up and rotate clockwise
        TheAnimationSequence.Append(
            diceRectTransform.DOScale(OGscale, animationDuration).SetEase(Ease.OutBack) // Scale up
        );
        TheAnimationSequence.Join(
            diceRectTransform.DORotate(new Vector3(0, 0, finalSpinAngle), animationDuration, RotateMode.FastBeyond360).SetEase(Ease.OutBack) // Clockwise spin
        );
    }

    private void OnDestroy()
    {
        TheAnimationSequence.Kill();


    }


}
