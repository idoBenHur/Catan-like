using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_4 : MonoBehaviour
{
    [SerializeField] private RectTransform ChallengeToolTip;
    [SerializeField] private RectTransform BoonsToolTip;
    [SerializeField] private RectTransform StartingText;
    private Vector2 StartPos;


    // Start is called before the first frame update
    void Start()
    {
        StartPos = StartingText.anchoredPosition;
        StartingTextAnimation();
        BoardManager.OnDiceRolled += ShowChallengeToolTip;
        BoardManager.OnDiceRolled += ShowBoonsToolTip;

    }

    private void OnDestroy()
    {
        BoardManager.OnDiceRolled -= ShowChallengeToolTip;
        BoardManager.OnDiceRolled -= ShowBoonsToolTip;

    }




    private void StartingTextAnimation()
    {
        // Move the panel upwards
        StartingText.DOAnchorPosY(StartPos.y + 0f, 3).SetEase(Ease.Linear) // Adjust the 100f to how much you want to move the panel
            .OnComplete(() => {

                CanvasGroup canvasGroup = StartingText.GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                    StartingText.gameObject.SetActive(false);
                });


            });



    }

    private void ShowChallengeToolTip()
    {
        int currentTurn = BoardManager.instance.CurrentTurn;

        if(currentTurn == 4)
        {

            RectTransform ChallengeIndicator = BoardManager.instance.uiManager.ChallengeSliderIndicator;

            CanvasGroup TooltipTransperancy = ChallengeToolTip.GetComponent<CanvasGroup>();
            RectTransform TooltipScale = ChallengeToolTip.GetComponent<RectTransform>();
            TooltipTransperancy.alpha = 0;
            TooltipScale.localScale = Vector3.zero;

            TooltipScale.position = new Vector3(ChallengeIndicator.position.x, ChallengeIndicator.position.y -0.7f, ChallengeIndicator.position.z);
            ChallengeToolTip.gameObject.SetActive(true);


            DOVirtual.DelayedCall(0.5f, () =>
            {
                TooltipTransperancy.DOFade(1, 0.3f);
                TooltipScale.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            });

            BoardManager.OnDiceRolled -= ShowChallengeToolTip;
            return;





        }
    }

    private void ShowBoonsToolTip()
    {
        int currentTurn = BoardManager.instance.CurrentTurn;

        if (currentTurn == 2)
        {

           

            CanvasGroup TooltipTransperancy = BoonsToolTip.GetComponent<CanvasGroup>();
            RectTransform TooltipScale = BoonsToolTip.GetComponent<RectTransform>();
            TooltipTransperancy.alpha = 0;
            TooltipScale.localScale = Vector3.zero;

            BoonsToolTip.gameObject.SetActive(true);


            DOVirtual.DelayedCall(0.5f, () =>
            {
                TooltipTransperancy.DOFade(1, 0.3f);
                TooltipScale.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            });

            BoardManager.OnDiceRolled -= ShowBoonsToolTip;
            return;





        }

    }



    public void CloseChallengeToolTipBUTTON()
    {

        CanvasGroup ToolTipTransperancy = ChallengeToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = ChallengeToolTip.GetComponent<RectTransform>();


        ToolTipTransperancy.DOFade(0, 0.3f);
        MeterScale.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            ChallengeToolTip.gameObject.SetActive(false);
        });


        
    }

    public void CloseBoonsToolTipBUTTON()
    {
        CanvasGroup ToolTipTransperancy = BoonsToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = BoonsToolTip.GetComponent<RectTransform>();


        ToolTipTransperancy.DOFade(0, 0.3f);
        MeterScale.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            BoonsToolTip.gameObject.SetActive(false);
        });

    }
}
