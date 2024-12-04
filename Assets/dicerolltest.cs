using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dicerolltest : MonoBehaviour
{
    private RectTransform diceRectTransform;
    public float animationDuration = 1.5f;
    public float angleofstop = 2.3f;
    private Vector3 OGscale;

    private void Awake()
    {
        // Get the RectTransform component of this GameObject
        diceRectTransform = GetComponent<RectTransform>();
        OGscale = diceRectTransform.localScale;
    }

    public void AnimateDiceRoll()
    {
        // Reset the dice to the initial state
       // diceRectTransform.localScale = Vector3.zero; // Start small
       // diceRectTransform.localEulerAngles = Vector3.zero; // Reset rotation

        // Create the animation sequence
        Sequence diceSequence = DOTween.Sequence();

        // Set rotation parameters
        float finalSpinAngle = 360 * angleofstop; // Number of spins        
        Vector3 targetScale = OGscale * 5f;

        // Scale up and rotate clockwise
        diceSequence.Append(
            diceRectTransform.DOScale(targetScale, animationDuration).SetEase(Ease.OutBack) // Scale up
        );
        diceSequence.Join(
            diceRectTransform.DORotate(new Vector3(0, 0, finalSpinAngle), animationDuration, RotateMode.FastBeyond360).SetEase(Ease.OutBack) // Clockwise spin
        );




    }

    // Example trigger for testing
    private void Start()
    {
        AnimateDiceRoll();
    }


    public void ResetDice()
    {
        // Reset the dice to its original scale and rotation
        diceRectTransform.localScale = OGscale;
        diceRectTransform.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Reset the dice and play the animation
            ResetDice();
            AnimateDiceRoll();
        }
    }


}
