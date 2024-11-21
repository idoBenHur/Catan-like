using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using static TileClass;

public class ResourcePayPrefab : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType; // Set this in the prefab (e.g., wood, stone)
    [SerializeField] private TextMeshProUGUI AmountText; // Reference to the text showing the required amount
    [SerializeField] private GameObject NumberCircle;
    [SerializeField] private Button payButton;
    [SerializeField] private GameObject QuestionMarkImage;
    private bool awardBoon = false;
    [HideInInspector] public int requiredAmount; // Set this dynamically when spawning the prefab

    private Winning_Condition2 winningCondition2;

    public void Initialize(Winning_Condition2 manager, int amount, bool isHidden, bool giveBoon)
    {
        winningCondition2 = manager;
        requiredAmount = amount;
        awardBoon = giveBoon;
        AmountText.text = requiredAmount.ToString(); // Update the amount display

        UpdateHiddenStatus(isHidden);
    }

    private void Start()
    {
        payButton.onClick.AddListener(OnPayButtonClicked); // Set up button click handler
    }

    private void OnPayButtonClicked()
    { 
        winningCondition2.PayResource(resourceType, requiredAmount, awardBoon, this.gameObject); // Pay the resource
    }


    public void UpdateHiddenStatus(bool isHidden)
    {
        if(isHidden == true) 
        {
            QuestionMarkImage.SetActive(true);
            NumberCircle.SetActive(false);
        }
        else
        {
            QuestionMarkImage.SetActive(false); // Hide the question mark if revealed
            NumberCircle.SetActive(true);
        }



       // QuestionMarkImage.SetActive(!isHidden); // Hide the question mark if revealed
       // NumberCircle.SetActive(isHidden); // Show the resource amount if revealed
       //// payButton.gameObject.SetActive(isRevealed); // Only show the button when revealed
    }
}
