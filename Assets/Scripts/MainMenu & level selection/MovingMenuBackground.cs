using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMenuBackground : MonoBehaviour
{
    public float speed = 0.1f; // Adjust this value to change the speed of the movement
    public Vector2 direction = new Vector2(1, 1); // Movement direction (1, 1) for diagonal
    public float tileSize = 10.0f; // Size of the grid or the area it moves before repeating

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        MoveBackground();
    }

    void MoveBackground()
    {
        Vector3 endPosition = startPosition + new Vector3(direction.x * tileSize, direction.y * tileSize, 0);
        transform.DOMove(endPosition, tileSize / speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.position = startPosition;
            MoveBackground();
        });
    }
}
