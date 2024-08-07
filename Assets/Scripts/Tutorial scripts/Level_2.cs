using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_2 : MonoBehaviour
{

    [SerializeField] private RectTransform TradeToolTip;
    [SerializeField] private RectTransform StartingText;
    private Vector2 StartPos;

    void Start()
    {

        BoardManager.instance.player.OnResourcesChanged += OpenTradeToolTip;

        StartPos = StartingText.anchoredPosition;
        StartingTextAnimation();

    }

    private void OnDestroy()
    {
        BoardManager.instance.player.OnResourcesChanged -= OpenTradeToolTip;
    }

    private void OpenTradeToolTip()
    {
        
        foreach(var resource in BoardManager.instance.player.PlayerResources)
        {
           if(resource.Value >= 4)
            {
                CanvasGroup TooltipTransperancy = TradeToolTip.GetComponent<CanvasGroup>();
                RectTransform TooltipScale = TradeToolTip.GetComponent<RectTransform>();
                TooltipTransperancy.alpha = 0;
                TooltipScale.localScale = Vector3.zero;

                TradeToolTip.gameObject.SetActive(true);


                DOVirtual.DelayedCall(0.5f, () => 
                {
                    TooltipTransperancy.DOFade(1, 0.3f);
                    TooltipScale.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                });

                BoardManager.instance.player.OnResourcesChanged -= OpenTradeToolTip;
                return;

            }
            
        }
        
    }


    public void CloseTradeToolTip()
    {
        CanvasGroup MeterTransperancy = TradeToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = TradeToolTip.GetComponent<RectTransform>();


        MeterTransperancy.DOFade(0, 0.3f);
        MeterScale.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            TradeToolTip.gameObject.SetActive(false);
        });
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
