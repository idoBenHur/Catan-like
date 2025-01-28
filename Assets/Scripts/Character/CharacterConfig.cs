using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterConfig", menuName = "NewCharacterConfig")]
public class CharacterConfig : ScriptableObject
{

    [Header("Character Properties")]

    [Tooltip("The Unique skots of the  of Character")]
    public AbstractSkillSlot UniqueSlot;





}
