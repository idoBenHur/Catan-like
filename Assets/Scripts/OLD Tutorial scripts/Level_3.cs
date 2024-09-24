using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_3 : MonoBehaviour
{

    [SerializeField] private RectTransform UpgradeMeterToolTip;
    [SerializeField] private RectTransform StartingText;
    private Vector2 StartPos;


    // Start is called before the first frame update
    void Start()
    {
       StartPos = StartingText.anchoredPosition;
       StartingTextAnimation();

        BoardManager.instance.player.OnResourcesChanged += OpenUpgradeMeterToolTip;

    }

    private void OnDestroy()
    {
        BoardManager.instance.player.OnResourcesChanged -= OpenUpgradeMeterToolTip;
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



    private void OpenUpgradeMeterToolTip()
    {

        if (BoardManager.instance.player.CanAffordToBuild(PricesClass.MeterUpgrade))
        {

            CanvasGroup TooltipTransperancy = UpgradeMeterToolTip.GetComponent<CanvasGroup>();
            RectTransform TooltipScale = UpgradeMeterToolTip.GetComponent<RectTransform>();
            TooltipTransperancy.alpha = 0;
            TooltipScale.localScale = Vector3.zero;

            UpgradeMeterToolTip.gameObject.SetActive(true);


            DOVirtual.DelayedCall(0.5f, () =>
            {
                TooltipTransperancy.DOFade(1, 0.3f);
                TooltipScale.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            });

            BoardManager.instance.player.OnResourcesChanged -= OpenUpgradeMeterToolTip;
            
            return;


        }
    }


    public void CloseTradeToolTip()
    {
        CanvasGroup MeterTransperancy = UpgradeMeterToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = UpgradeMeterToolTip.GetComponent<RectTransform>();


        MeterTransperancy.DOFade(0, 0.3f);
        MeterScale.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            UpgradeMeterToolTip.gameObject.SetActive(false);
        });
    }

}
