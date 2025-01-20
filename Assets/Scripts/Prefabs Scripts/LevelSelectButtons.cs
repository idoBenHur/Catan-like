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
    private LevelConfig ChosenlevelConfig;


    void Start()
    {
        TileMapParent = GetComponent<Transform>();
        startingScale = TileMapParent.localScale;

    }

    public void AssignLevelConfig(LevelConfig config)
    {
       

        ChosenlevelConfig = config; // Assign the levelConfig dynamically

        turnsTxt.text = $"Turns: {ChosenlevelConfig.turns}";


        string result = "Resources: ";
        foreach (var resourcePair in ChosenlevelConfig.resources)
        {

            string spriteTag = resourcePair.ResourceType switch
            {
                TileClass.ResourceType.Wood => "<sprite name=wood>",
                TileClass.ResourceType.Rum => "<sprite name=rum>",
                TileClass.ResourceType.Gunpowder => "<sprite name=gunpowder>",
                TileClass.ResourceType.Gem => "<sprite name=gem>",
                TileClass.ResourceType.Gold => "<sprite name=gold>",
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

        if (ChosenlevelConfig != null)
        {
            
            NEWGameManager.Instance.SavePickedLevel(ChosenlevelConfig); // Store the selected level in the GameManager
            
        }
        else
        {
            Debug.LogError("No LevelConfig assigned to this object!");
        }


    }
}
