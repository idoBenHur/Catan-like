using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterSelect : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image pirateImage;
    [SerializeField] private TextMeshProUGUI pirateNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI AbilitiesText;

    [SerializeField] private Transform diceParent;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Sprite[] diceSprites;





    [Header("Navigation")]
    [SerializeField] public Button leftArrow;
    [SerializeField] public Button rightArrow;
    [SerializeField] public Button pickButton;

    [Header("Character Data")]
    [SerializeField] public List<CharacterConfig> pirates;
    private int currentIndex = 0;

    private void Start()
    {
        leftArrow.onClick.AddListener(() => ChangeCharacter(-1));
        rightArrow.onClick.AddListener(() => ChangeCharacter(1));
        pickButton.onClick.AddListener(() => PickedPirate());


        DisplayCharacter();
    }

    private void ChangeCharacter(int direction)
    {
        if (pirates == null || pirates.Count == 0)
        {
            Debug.LogError("Pirates list is empty! Cannot change character.");
            return;
        }

        // Kill any existing animation before starting a new one
        DOTween.Kill(pirateImage.transform);

        // Get the width of the UI panel (so we know how far to move)
        float panelWidth = pirateImage.rectTransform.rect.width;

        // Move the current image OUT in the selected direction
        pirateImage.transform.DOLocalMoveX(-direction * panelWidth, 0.2f).OnComplete(() =>
        {
            // Change character index
            currentIndex += direction;
            if (currentIndex < 0) currentIndex = pirates.Count - 1;
            if (currentIndex >= pirates.Count) currentIndex = 0;

            // Update the character info
            DisplayCharacter();

            // Reset position (move the new image from the opposite side)
            pirateImage.transform.localPosition = new Vector3(direction * panelWidth, pirateImage.transform.localPosition.y, pirateImage.transform.localPosition.z);

            // Move the new image INTO the panel
            pirateImage.transform.DOLocalMoveX(0, 0.2f);
        });
    }

    private void DisplayCharacter()
    {

        CharacterConfig currentPirate = pirates[currentIndex];

        pirateNameText.text = currentPirate.PirateName;
        descriptionText.text = currentPirate.description;
        pirateImage.sprite = currentPirate.pirateImage;
        AbilitiesText.text = currentPirate.AbilitiesTXT;

        // slotIcon.sprite = currentPirate.slotIcon;



        //dice spwaning//

        // Clear previous dice
        foreach (Transform child in diceParent)
        {
            Destroy(child.gameObject);
        }



        for (int i = 0; i < currentPirate.NormalDiceCount; i++)
        {
            GameObject diceObj = new GameObject("DiceImage", typeof(RectTransform), typeof(Image));
            diceObj.transform.SetParent(diceParent, false);
            Image diceImage = diceObj.GetComponent<Image>();
            diceImage.sprite = diceSprites[i];
           // diceImage.SetNativeSize();
        }

        foreach (var SpecialDie in currentPirate.UniqueDice)
        {
            GameObject diceObj = new GameObject("DiceImage", typeof(RectTransform), typeof(Image));
            diceObj.transform.SetParent(diceParent, false);
            Image diceImage = diceObj.GetComponent<Image>();
            diceImage.sprite = diceSprites[Random.Range(0,6)];
            diceImage.color = SpecialDie.boonColor;
        }

        //slot spwaning//

        foreach(Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        Instantiate(currentPirate.UniqueSlot, slotParent);



    }

    private void PickedPirate()
    {
        if (NEWGameManager.Instance != null)
        {
            NEWGameManager.Instance.SavePickedPirate(pirates[currentIndex]);
        }
    }
}
