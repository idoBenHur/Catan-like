using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumbersAssigner : MonoBehaviour
{
    private static List<int> availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };
    private int number; // The number this prefab instance holds.

    public TMP_Text numberDisplay; // Reference to a TextMesh Pro text component

    void Awake()
    {

    }

    private void Start()
    {
        if (availableNumbers.Count == 0)
        {
            ResetAvailableNumbers(); // Repopulate the list if it's empty
        }
        AssignNumber();
    }

    void AssignNumber()
    {
        if (availableNumbers.Count > 0)
        {
            int RandomIndex = Random.Range(0, availableNumbers.Count); // Randomly pick an index to make the distribution unpredictable.
            number = availableNumbers[RandomIndex];
            availableNumbers.RemoveAt(RandomIndex); // Remove the assigned number from the list.

            numberDisplay.text = number.ToString();

            if (number == 6 || number == 8)
            {
                numberDisplay.color = Color.red;
                numberDisplay.fontStyle = FontStyles.Bold;
            }


            
        }

    }

    // Optional: Reset the list when all prefabs are destroyed, e.g., at the end of a level or game session.
    // This can be called from another script when needed.
    public static void ResetAvailableNumbers()
    {
        availableNumbers = new List<int> { 3, 4, 5, 6, 8, 9, 10, 11, 3, 4, 5, 6, 8, 9, 10, 11, 12, 2 };
    }
}
