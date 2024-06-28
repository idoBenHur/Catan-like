using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{

    [SerializeField] private GameObject PasueManuObject;
    private bool IsPaused;
    private bool FinishedAnimation;

    // Start is called before the first frame update
    void Start()
    {
        PasueManuObject.SetActive(false);
        FinishedAnimation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsPaused == true)
            {
                 if (FinishedAnimation == true)
                {
                    FinishedAnimation = false;
                    ResumeGame();
                }

            }
            else
            {
                if(FinishedAnimation == true)
                {
                    FinishedAnimation = false;
                    PauseGame();
                }
 
            }
        }

    }


    public void PauseGame()
    {
        PasueManuObject.transform.localPosition = new Vector3(0F, -1200F, 0F);
        PasueManuObject.SetActive(true);
        PasueManuObject.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.7f, false).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Time.timeScale = 0f;
            IsPaused = true;
            FinishedAnimation = true;
        });

    }

    private void ResumeGame()
    {

        Time.timeScale = 1f;
        PasueManuObject.GetComponent<RectTransform>().DOAnchorPosY(-1200f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            
            IsPaused = false;
            PasueManuObject.SetActive(false);
            FinishedAnimation = true;
        });

        



       
       
    }

    public void RestartCurrentSceneBUTTON()
    {
        GameManager.Instance.RestartCurrentScene();
    }

    public void BackToMainMenuBUTTON()
    {
        GameManager.Instance.BackToMainMenu();

    }

    public void ResumeBUTTON()
    {
        ResumeGame();

    }

    public void CheatBUTTON()
    {
        int[] AllLevelsScenesIndexesList = { 5, 6, 7, 8 };

        foreach (int number in AllLevelsScenesIndexesList)
        {
            GameManager.Instance.GameState.CompletedLevelsBySceneNumber.Add(number);
        }


        
    }

    public void OpenSettingBUTTON()
    {
        if (IsPaused == true)
        {
            if (FinishedAnimation == true)
            {
                FinishedAnimation = false;
                ResumeGame();
            }

        }
        else
        {
            if (FinishedAnimation == true)
            {
                FinishedAnimation = false;
                PauseGame();
            }

        }
    }
}
