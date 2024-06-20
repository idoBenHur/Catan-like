using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapLevelSelection : MonoBehaviour
{
    //this script is attached to each island part parent
    [SerializeField] Transform IslandParent;
    [SerializeField] GameObject LockedOverlay;
    [SerializeField] GameObject Island;
    private Vector3 startingScale;
    public int LevelSceneIndex;

    void Start()
    {
        startingScale = IslandParent.localScale;
    }

    void OnMouseEnter()
    {

        Island.GetComponent<TilemapRenderer>().sortingOrder = 3;
        var tilerenderer = LockedOverlay.GetComponentInChildren<TilemapRenderer>();
        if (tilerenderer != null)
        {
            tilerenderer.sortingOrder = 4;
        }
      


        transform.DOScale(IslandParent.localScale * 1.2f, 0.2f);    


    }

    void OnMouseExit()
    {
        transform.DOScale(startingScale, 0.2f);

        Island.GetComponent<TilemapRenderer>().sortingOrder = 1;

        var tilerenderer = LockedOverlay.GetComponentInChildren<TilemapRenderer>();
        if (tilerenderer != null)
        {
            tilerenderer.sortingOrder = 2;
        }


    }

    void OnMouseDown()
    {
        if(LockedOverlay.activeSelf == false)
        {
            DOTween.KillAll();
            SceneManager.LoadScene(LevelSceneIndex);
        }


    }
}
