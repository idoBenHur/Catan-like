using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelSelectButtons : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI islandNameTxt;
    [SerializeField] private TextMeshProUGUI turnsTxt;
    [SerializeField] private TextMeshProUGUI resouurcesTxt;





    private Transform TileMapParent;
    private Vector3 startingScale;
    private LevelConfig levelConfig;


    void Start()
    {
        TileMapParent = GetComponent<Transform>();
        startingScale = TileMapParent.localScale;
    }

    public void AssignLevelConfig(LevelConfig config)
    {
        levelConfig = config; // Assign the levelConfig dynamically

        turnsTxt.text = $"Turns: {levelConfig.turns}";


        string result = "Resources: ";

        foreach (var resourcePair in levelConfig.resources)
        {

            string spriteTag = resourcePair.ResourceType switch
            {
                TileClass.ResourceType.Wood => "<sprite name=wood>",
                TileClass.ResourceType.Brick => "<sprite name=rum>",
                TileClass.ResourceType.Wheat => "<sprite name=gunpowder>",
                TileClass.ResourceType.Ore => "<sprite name=gem>",
                TileClass.ResourceType.Sheep => "<sprite name=gold>",
                _ => ""
            };

            result += $"{resourcePair.Quantity}{spriteTag} ";

        }


        resouurcesTxt.text = result;


    }







    void OnMouseEnter()
    {


        TileMapParent.DOScale(startingScale * 1.2f, 0.2f);


    }


    private void OnMouseExit()
    {
        TileMapParent.DOScale(startingScale, 0.2f);

    }

    void OnMouseDown()
    {

        Debug.Log("click");

    }
}
