using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;


public class Level_1 : MonoBehaviour
{

    [SerializeField] private RectTransform GatherPeasantsOnject;
    [SerializeField] private RectTransform UnluckyMeterToolTip;
    private Vector2 startPos;


    // Start is called before the first frame update
    void Start()
    {

        startPos = GatherPeasantsOnject.anchoredPosition; // Save the starting position
        GatherPeasantsAnimation();
     //   BoardManager.OnUnlcukyRoll += OpenUnluckyMeterToolTip;

    }

    private void OnDestroy()
    {
      //  BoardManager.OnUnlcukyRoll -= OpenUnluckyMeterToolTip;

    }




    private void GatherPeasantsAnimation()
    {
        // Move the panel upwards
        GatherPeasantsOnject.DOAnchorPosY(startPos.y + 30f, 5).SetEase(Ease.Linear) // Adjust the 100f to how much you want to move the panel
            .OnComplete(() => {

                CanvasGroup canvasGroup = GatherPeasantsOnject.GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                    GatherPeasantsOnject.gameObject.SetActive(false);
                });


            });

        

    }




    private void OpenUnluckyMeterToolTip()
    {

        CanvasGroup MeterTransperancy = UnluckyMeterToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = UnluckyMeterToolTip.GetComponent<RectTransform>();
        MeterTransperancy.alpha = 0;
        MeterScale.localScale = Vector3.zero;

        UnluckyMeterToolTip.gameObject.SetActive(true);
        MeterTransperancy.DOFade(1, 0.3f);
        MeterScale.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        //BoardManager.OnUnlcukyRoll -= OpenUnluckyMeterToolTip;

    }

    public void CloseUnluckyMeterToolTipButton()
    {
        CanvasGroup MeterTransperancy = UnluckyMeterToolTip.GetComponent<CanvasGroup>();
        RectTransform MeterScale = UnluckyMeterToolTip.GetComponent<RectTransform>();


        MeterTransperancy.DOFade(0, 0.3f);
        MeterScale.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            UnluckyMeterToolTip.gameObject.SetActive(false);
        });

    }

    


}
