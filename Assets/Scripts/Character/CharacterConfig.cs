using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterConfig", menuName = "NewCharacterConfig")]
public class CharacterConfig : ScriptableObject
{

    [Header("Character Properties")]

    [TextArea(5, 10)]
    public string description;

    [TextArea(3, 10)]
    public string AbilitiesTXT;


    public string PirateName;
    public Sprite pirateImage;
    public Color pirateColor = new Color(1f, 1f, 1f, 1f);
    
    public int NormalDiceCount;




    [Tooltip("The Unique skots of the  of Character")]
    public AbstractSkillSlot UniqueSlot;
    public List<GenericBoon> UniqueDice;









}
