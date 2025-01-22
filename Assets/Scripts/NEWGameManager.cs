using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NEWGameManager : MonoBehaviour
{
    public static NEWGameManager Instance;

    [HideInInspector] public LevelConfig NextLevelConfig { get; private set; }

    [HideInInspector] public int levelsCompleted;

    [HideInInspector] public List<GenericBoon> OwnedBoons;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


            //
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

      //  Debug.Log($"level config is {NextLevelConfig == null}");
    }







    public void SavePickedLevel(LevelConfig levelConfig) // savve the level copnfig of the chosen level.
    {
        NextLevelConfig = levelConfig;
        GoToGameplayScene();
    }



    private void GoToGameplayScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void BackTLevelSelection()
    {
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(0);
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
        
        Time.timeScale = 1f;
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }




}
