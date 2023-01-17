using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListMenuOption
{
    public readonly string Label;
    public readonly string Description;
    public readonly Sprite Icon;
    public GameObject GO;

    public object Metadata;

    public ListMenuOption(string label, string description, Sprite icon)
    {
        Label = label;
        Description = description;
        Icon = icon;
    }

    public ListMenuOption(string label, string description, Sprite icon, object metadata)
    {
        Label = label;
        Description = description;
        Icon = icon;
        Metadata = metadata;
    }
}
