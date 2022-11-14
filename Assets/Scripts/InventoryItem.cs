using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryItem : ScriptableObject
{
    [Tooltip("The icon to be displayed for this consumeable in UI (Currently unused)")]
    public Sprite icon;
    public abstract void Use(BaseCharacter character);
}
