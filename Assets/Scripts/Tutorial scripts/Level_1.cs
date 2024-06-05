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

    }

    private void GatherPeasantsAnimation()
    {
        // Move the panel upwards
        GatherPeasantsOnject.DOAnchorPosY(startPos.y + 30f, 5).SetEase(Ease.Linear) // Adjust the 100f to how much you want to move the panel
            .OnComplete(() => {

                CanvasGroup canvasGroup = GatherPeasantsOnject.GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear);
                

            });
    }

    public void CloseUnluckyMeterToolTipButton()
    {
        UnluckyMeterToolTip.gameObject.SetActive(false);
    }


}
