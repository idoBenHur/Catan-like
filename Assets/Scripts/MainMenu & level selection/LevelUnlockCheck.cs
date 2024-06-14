using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlockCheck : MonoBehaviour
{

    [SerializeField] GameObject IslandOneLockScreen;
    // Start is called before the first frame update
    void Start()
    {
        if( GameManager.Instance.GameState.FinishedTutorial == true )
        {
            
            UnlockMainIsland1();
        }
    }

    private void UnlockMainIsland1()
    {
        IslandOneLockScreen.SetActive(false);
    }
 
}
