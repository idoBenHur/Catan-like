using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterConfig", menuName = "NewCharacterConfig")]
public class CharacterConfig : ScriptableObject
{

    [Header("Character Properties")]

    public string PirateName;
    public Sprite pirateImage;
    public string description;
    public int NormalDiceCount;




    [Tooltip("The Unique skots of the  of Character")]
    public AbstractSkillSlot UniqueSlot;
    public List<GenericBoon> UniqueDice;









}
