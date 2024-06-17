using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boon : ScriptableObject
{
    public string boonName;
    public string description;

    public abstract void Activate(PlayerClass player);
    public abstract void Deactivate(PlayerClass player);

}
