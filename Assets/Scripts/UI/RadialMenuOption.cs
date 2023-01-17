using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RadialMenuOption
{
    [SerializeField] public readonly string Label;
    [SerializeField] public readonly string Description;
    [SerializeField] public readonly Sprite Icon;

    public RadialMenuOption(string label, string description, Sprite icon)
    {
        Label = label;
        Description = description;
        Icon = icon;
    }
}
