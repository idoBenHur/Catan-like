using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class BoonSelectionAnimations : MonoBehaviour
{
    public RectTransform button1;
    //public RectTransform button2;
    //public RectTransform button3;



    public void Showpannel()
    {
        button1.transform.localPosition = new Vector3(0F, -1000F, 0F);
        button1.DOAnchorPos(new Vector2(0f, 0f), 1, false).SetEase(Ease.OutElastic);
    }
}
