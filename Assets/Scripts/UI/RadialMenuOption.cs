using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RadialMenuOption
{
    [SerializeField] public readonly string label;
    [SerializeField] public readonly string description;
    [SerializeField] public readonly Sprite icon;

    public RadialMenuOption(string label, string description, Sprite icon)
    {
        this.label = label;
        this.description = description;
        this.icon = icon;
    }
}
