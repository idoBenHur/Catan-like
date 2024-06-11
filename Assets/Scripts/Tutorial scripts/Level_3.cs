using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_3 : MonoBehaviour
{

    //[SerializeField] private RectTransform TradeToolTip;
    [SerializeField] private RectTransform StartingText;
    private Vector2 StartPos;


    // Start is called before the first frame update
    void Start()
    {
       StartPos = StartingText.anchoredPosition;
       StartingTextAnimation();


    }





    private void StartingTextAnimation()
    {
        // Move the panel upwards
        StartingText.DOAnchorPosY(StartPos.y + 0f, 5).SetEase(Ease.Linear) // Adjust the 100f to how much you want to move the panel
            .OnComplete(() => {

                CanvasGroup canvasGroup = StartingText.GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                    StartingText.gameObject.SetActive(false);
                });


            });



    }

}
