using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenges : MonoBehaviour
{
    public GameObject testprefab;
    List<TileClass> OuterCircleTiles = new List<TileClass>();
    [SerializeField]  public int RobberChallengeTurn;







    private void Start()
    {

    }

    private void OnDestroy()
    {
        BoardManager.OnDiceRolled -= RobbersChallenge;
    }

    public void SetUpPlayerChallenges(PlayerClass player)
    {
        BoardManager.OnDiceRolled += RobbersChallenge;
    }

    public void RobbersChallenge()
    {
        if(BoardManager.instance.CurrentTurn >= RobberChallengeTurn)
        {
            var TheTilesDic = BoardManager.instance.TilesDictionary;
            List<TileClass> RemainingOuterCircleTiles = new List<TileClass>();


            foreach (var Tile in TheTilesDic)
            {
                if (Tile.Value.AdjacentTiles.Count <= 5 && Tile.Value.hasRobber == false)
                {
                    RemainingOuterCircleTiles.Add(Tile.Value);
                }

            }


            if (RemainingOuterCircleTiles.Count > 0)
            {
                int RandomIndex = Random.Range(0, RemainingOuterCircleTiles.Count);
                var BlockedTile = RemainingOuterCircleTiles[RandomIndex];
                BlockedTile.PlaceRobber();
                Destroy(BlockedTile.MyNumberPrefab);
                Instantiate(testprefab, BlockedTile.TileWorldPostion, Quaternion.identity);
                AudioManagerScript.instance.PlaySFX(AudioManagerScript.instance.TileDrowningSplash);
            }

        }


    }
}
