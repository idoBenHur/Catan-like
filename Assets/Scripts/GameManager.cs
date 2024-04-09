using System.Collections.Generic;
using UnityEngine;
using static TileClass;

[System.Serializable]
public class GameState
{

    public int SeasonNumber;
    public PlayerClass player;
    public List<Boon> activeBoons;
    public Dictionary<Vector3Int, TileClass> tilesDic;
    public Dictionary<Vector3, CornersClass> cornersDic;
    public Dictionary<Vector3, SidesClass> sidesDic;

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

}
