using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TileClass;

[System.Serializable]
public class GameState
{

    public int SeasonNumber;
    public PlayerClass player;
    public Dictionary<Vector3Int, TileClass> tilesDic;
    public Dictionary<Vector3, CornersClass> cornersDic;
    public Dictionary<Vector3, SidesClass> sidesDic;
    public List<int> CompletedLevelsBySceneNumber;


    public GameState()
    {
        CompletedLevelsBySceneNumber = new List<int>();
    }

}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameState gameState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if( gameState == null)
            {
                gameState = new GameState();
            }

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //Debug.Log(gameState.SeasonNumber);  

    }


    private void Start()
    {
       // gameState.CompletedLevelsBySceneNumber = new List<int>();
    }

    public GameState GameState
    {
        get => gameState;
        set => gameState = value;

    }



    public void UpdatePlayer(PlayerClass player)
    {
        gameState.player = player;
    }

    public void UpdateTile(Dictionary<Vector3Int, TileClass> tiles)
    {
        gameState.tilesDic = tiles;
    }

    public void UpdateCorner(Dictionary<Vector3, CornersClass> corners)
    {
        gameState.cornersDic = corners;
    }

    public void UpdateSide(Dictionary<Vector3, SidesClass> sides)
    {
        gameState.sidesDic = sides;
    }

    public void NextSeason()
    {
        gameState.SeasonNumber++;
    }


    public void FinishedLevel(int SceneBuildIndex)
    {
        if (gameState.CompletedLevelsBySceneNumber.Contains(SceneBuildIndex) == false)
        {
            gameState.CompletedLevelsBySceneNumber.Add(SceneBuildIndex);
        }
       
    }


    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextScene()
    {
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        Debug.Log("HI");
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }

    public void BackTLevelSelection()
    {
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }




}
