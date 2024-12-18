using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// Manages the selection and activation of boons within the game.
/// </summary>

public class BoonManager : MonoBehaviour
{


// References to other scripts
    private UiManager uiManager; // for the UI interactions
    private BoardManager boardManager;


  // Boon management
    public List<GenericBoon> allBoons;
    private List<GenericBoon> AvailableBoons;
    private List<GenericBoon> activeBoons = new List<GenericBoon>();
    public Button[] boonButtonsList;
    private GenericBoon[] OfferedBoons = new GenericBoon[3];


    // other
    private DiceBox_Skill dicebox;
    [SerializeField] private Sprite ResourceBundleImage;








    public void setup()
    {
        boardManager = GetComponent<BoardManager>();
        uiManager = GetComponent<UiManager>();
        AvailableBoons = new List<GenericBoon>(allBoons);
    }




    private void OnDestroy()
    {
        ResetAllBoons();
    }





    // Prepares and displays boon choices for the player.
    public void GiveBoon()
    {
        SetBoonsButtonWithRandomBoons();
        uiManager.OpenAndCloseBoonSelectionScreenAnimations(true);
    }



   // Fills the boon selection buttons with either available boons or resource bundles.
    public void SetBoonsButtonWithRandomBoons()
    {
        List<GenericBoon> CurrentAvailableBoons = new List<GenericBoon>(AvailableBoons);
        

        for (int i = 0; i < boonButtonsList.Length; i++)
        {
            
            if (CurrentAvailableBoons.Count > 0)
            {
                int randomIndex = Random.Range(0, CurrentAvailableBoons.Count);
                OfferedBoons[i] = CurrentAvailableBoons[randomIndex];
                CurrentAvailableBoons.RemoveAt(randomIndex);
            }
            else
            {
                // Fallback to resource bundles if no boons are left
                OfferedBoons[i] = GenerateRandomResourceBoon(6);
            }

            UpdateBoonButtonVisual(boonButtonsList[i], OfferedBoons[i]);

        }

    }

    // Updates the visual representation of a boon selection button.
    private void UpdateBoonButtonVisual(Button button, GenericBoon boon)
    {
        TextMeshProUGUI boonText = button.GetComponentInChildren<TextMeshProUGUI>();
        Image boonImage = button.transform.GetChild(1).GetComponent<Image>();

        if (boon != null)
        {
            boonText.text = boon.description;
            boonImage.sprite = boon.boonImage;
            boonImage.color = boon.boonColor;
            button.interactable = true;
        }
        else
        {
            boonText.text = "No Boon Available";
            boonImage.sprite = null;
            boonImage.color = Color.clear;
            button.interactable = false;
        }
    }







    // Button click handlers
    public void ChooseBoonButton1() { ChooseBoon(0); }
    public void ChooseBoonButton2() { ChooseBoon(1); }
    public void ChooseBoonButton3() { ChooseBoon(2); }


    // Activates the selected boon or applies its effects if it's a resource bundle.
    private void ChooseBoon(int index)
    {
        GenericBoon selectedBoon = OfferedBoons[index];

        if (selectedBoon.boonName == "Resource Bundle")
        {
            // Apply all effects of the random resource boon
            foreach (var effect in selectedBoon.effects)
            {
                ApplyBoonEffect(effect);
            }
        }
        else
        {

            if (dicebox == null)
            {
                dicebox = boardManager.skillSlotManager.SkillSlotsDictionary[SkillName.DiceBox] as DiceBox_Skill;
            }




            dicebox.AddSpecialDiceToPool(selectedBoon);
            AvailableBoons.Remove(selectedBoon);
            activeBoons.Add(selectedBoon);
            selectedBoon.StoreValues();

            if (selectedBoon.isCounting == true) { uiManager.UpdateBoonCounter(selectedBoon, 0); }

            uiManager.AddAndRemoveActiveBoonsDisplay(selectedBoon, true);
            

        }

        uiManager.OpenAndCloseBoonSelectionScreenAnimations(false);
    }


    public void ResetAllBoons()
    {
        // Reset all active boons to their default states
        foreach (var boon in activeBoons)
        {
            boon.ResetToDefaults();
        }

        activeBoons.Clear();
    }




    // resource bundle edge case:

    private GenericBoon GenerateRandomResourceBoon(int totalResources)
    {
        GenericBoon resourceBoon = ScriptableObject.CreateInstance<GenericBoon>();
        resourceBoon.boonName = "Resource Bundle";
        resourceBoon.boonImage = ResourceBundleImage;
        resourceBoon.effects = new List<BoonEffect>();

        TileClass.ResourceType[] resourceTypes = {
        TileClass.ResourceType.Wood,
        TileClass.ResourceType.Brick,
        TileClass.ResourceType.Sheep,
        TileClass.ResourceType.Ore,
        TileClass.ResourceType.Wheat
    };

        // Dictionary to group resources by type
        Dictionary<TileClass.ResourceType, int> resourceCounts = new Dictionary<TileClass.ResourceType, int>();
        foreach (var type in resourceTypes)
        {
            resourceCounts[type] = 0; // Initialize counts
        }

        int remainingResources = totalResources;

        while (remainingResources > 0)
        {
            int randomAmount = Random.Range(1, remainingResources + 1);
            TileClass.ResourceType randomType = resourceTypes[Random.Range(0, resourceTypes.Length)];

            // Update the resource count
            resourceCounts[randomType] += randomAmount;

            // Add effect to the boon
            resourceBoon.effects.Add(new BoonEffect
            {
                type = randomType switch
                {
                    TileClass.ResourceType.Wood => BoonEffect.EffectType.AddWood,
                    TileClass.ResourceType.Brick => BoonEffect.EffectType.AddBrick,
                    TileClass.ResourceType.Sheep => BoonEffect.EffectType.AddSheep,
                    TileClass.ResourceType.Ore => BoonEffect.EffectType.AddOre,
                    TileClass.ResourceType.Wheat => BoonEffect.EffectType.AddWheat,
                    _ => BoonEffect.EffectType.AddWood
                },
                value1 = randomAmount
            });

            remainingResources -= randomAmount;
        }

        // Build the description
        string description = "Receive:\n";
        foreach (var kvp in resourceCounts)
        {
            if (kvp.Value > 0)
            {
                string icon = kvp.Key switch
                {
                    TileClass.ResourceType.Wood => "<sprite name=wood>",
                    TileClass.ResourceType.Brick => "<sprite name=rum>",
                    TileClass.ResourceType.Sheep => "<sprite name=gold>",
                    TileClass.ResourceType.Ore => "<sprite name=gem>",
                    TileClass.ResourceType.Wheat => "<sprite name=gunpowder>",
                    _ => "?"
                };

                description += $"{kvp.Value}x {icon}\n";
            }
        }

        resourceBoon.description = description.TrimEnd(); // Remove trailing newline
        return resourceBoon;
    }



    private void ApplyBoonEffect(BoonEffect effect)
    {
        

        switch (effect.type)
        {
            case BoonEffect.EffectType.AddWood:
                boardManager.player.AddResource(TileClass.ResourceType.Wood, effect.value1, Vector3.zero);
                break;
            case BoonEffect.EffectType.AddBrick:
                boardManager.player.AddResource(TileClass.ResourceType.Brick, effect.value1, Vector3.zero);
                break;
            case BoonEffect.EffectType.AddSheep:
                boardManager.player.AddResource(TileClass.ResourceType.Sheep, effect.value1, Vector3.zero);
                break;
            case BoonEffect.EffectType.AddOre:
                boardManager.player.AddResource(TileClass.ResourceType.Ore, effect.value1, Vector3.zero);
                break;
            case BoonEffect.EffectType.AddWheat:
                boardManager.player.AddResource(TileClass.ResourceType.Wheat, effect.value1, Vector3.zero);
                break;
        }
    }



}
