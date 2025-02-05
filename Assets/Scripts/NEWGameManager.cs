using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NEWGameManager : MonoBehaviour
{
    public static NEWGameManager Instance;

    [HideInInspector] public LevelConfig NextLevelConfig { get; private set; }

    [HideInInspector] public CharacterConfig Pirate { get; private set;}

    [HideInInspector] public int levelsCompleted = 0; 

    [HideInInspector] public List<GenericBoon> OwnedBoons { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            OwnedBoons = new List<GenericBoon>(); // Initialize here

            //
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

      //  Debug.Log($"level config is {NextLevelConfig == null}");
    }




    public void CompletedALevel()
    {
        levelsCompleted++;
    }

    public void AddBoonToOwnedBoons(GenericBoon BoonToAdd)
    {
        OwnedBoons.Add(BoonToAdd);
    }

    public void SavePickedLevel(LevelConfig levelConfig) // savve the level copnfig of the chosen level.
    {
        NextLevelConfig = levelConfig;
        GoToGameplayScene();
    }


    public void SavePickedPirate(CharacterConfig chosenPirate)
    {
        Pirate = chosenPirate;
        

       

        if(Pirate.UniqueDice != null)
        {

            foreach (var uniqeDie in Pirate.UniqueDice)
            {
                AddBoonToOwnedBoons(uniqeDie);
            }

        }
        //

        DOTween.KillAll();
        GoToLevelSelection();
    }


    private void GoToGameplayScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void GoToLevelSelection()
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
