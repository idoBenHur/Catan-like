using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlockCheck : MonoBehaviour
{

    [SerializeField] GameObject LockScreenIsland_1;
    [SerializeField] GameObject LockScreenIsland_2;
    [SerializeField] GameObject LockScreenIsland_3;
    [SerializeField] GameObject LockScreenIsland_4;
    [SerializeField] GameObject LockScreenIsland_5;
    [SerializeField] GameObject LockScreenIsland_6;

    [SerializeField] GameObject CheckMarkIsland_Tutorial;
    [SerializeField] GameObject CheckMarkIsland_Island1;
    [SerializeField] GameObject CheckMarkIsland_Island3;
    [SerializeField] GameObject CheckMarkIsland_Island4;


    //  [SerializeField] private List<GameObject> IslandsLockScreens;




    // Start is called before the first frame update
    void Start()
    {
 
        UnlockNewLevels();
    }

    private void UnlockMainIsland1()
    {
        LockScreenIsland_1.SetActive(false);
    }

    private void UnlockNewLevels()
    {
        if(GameManager.Instance.GameState.CompletedLevelsBySceneNumber == null) { Debug.Log("empty"); return; }

        foreach (var SceneIndex in GameManager.Instance.GameState.CompletedLevelsBySceneNumber)
        {
            switch (SceneIndex)
            {
                case 5: // finished tutorial 4
                    LockScreenIsland_1.SetActive(false); // unlock level 1
                    CheckMarkIsland_Tutorial.SetActive(true);
                    break;
                case 6: // finished level 1
                    LockScreenIsland_3.SetActive(false); // unlock level 2
                    LockScreenIsland_1.SetActive(false); // making sure the level played is unlocked
                    CheckMarkIsland_Island1.SetActive(true); // completed level 1
                    break;
                case 7: // finished level 2
                    LockScreenIsland_4.SetActive(false); // unlock level 3
                    LockScreenIsland_3.SetActive(false); // making sure the level played is unlocked
                    CheckMarkIsland_Island3.SetActive(true); // completed level 2
                    break;
                case 8: // finished level 3
                    LockScreenIsland_4.SetActive(false); // making sure the level played is unlocked
                    CheckMarkIsland_Island4.SetActive(true);  // completed level 3
                    break;
                case 20:
                    LockScreenIsland_6.SetActive(false);
                    break;
                case 23:
                    LockScreenIsland_2.SetActive(false);
                    break;
            }




        }


    }
 
}
