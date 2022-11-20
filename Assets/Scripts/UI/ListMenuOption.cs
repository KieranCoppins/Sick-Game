using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListMenuOption
{
    public readonly string label;
    public readonly string description;
    public readonly Sprite icon;
    public GameObject go;

    public object Metadata;

    public ListMenuOption(string label, string description, Sprite icon)
    {
        this.label = label;
        this.description = description;
        this.icon = icon;
    }

    public ListMenuOption(string label, string description, Sprite icon, object metadata)
    {
        this.label = label;
        this.description = description;
        this.icon = icon;
        this.Metadata = metadata;
    }
}
