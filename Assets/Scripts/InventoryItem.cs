using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    public abstract void Use(BaseCharacter character);
}
