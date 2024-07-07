using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f; // Adjust this value to change the speed of the movement
    [SerializeField] private Vector2 direction = new Vector2(1, 1); // Movement direction (1, 1) for diagonal
    [SerializeField] private float tileSize = 10.0f; // Size of the grid or the area it moves before repeating
    [SerializeField] GameObject BackgroundGrid;

    private Vector3 startPosition;

    void Start()
    {
       // startPosition = transform.position;
        startPosition = BackgroundGrid.transform.position;
        MoveBackground();
    }

    private void MoveBackground()
    {
        Vector3 endPosition = startPosition + new Vector3(direction.x * tileSize, direction.y * tileSize, 0);
        BackgroundGrid.transform.DOMove(endPosition, tileSize / speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            BackgroundGrid.transform.position = startPosition;
            MoveBackground();
        });
    }

    public void StartGameBUTTOM()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void DiscordBUTTON()
    {
        // DOTween.KillAll();
        Application.OpenURL("https://discord.gg/NXMtgwudQw");
       // Application.ExternalEval("https://discord.gg/NXMtgwudQw");
    }

    public void NewGameModeButton()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(9);
    }
}
