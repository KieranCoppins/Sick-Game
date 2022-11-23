using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryRadialMenu : RadialMenu
{
    [SerializeField] CharacterMovement character;

    public override void LoadData()
    {
        if (menuOptions.Count > 0)
            menuOptions.Clear();

        foreach (var item in character.quickbar)
        {
            RadialMenuOption menuOption = new RadialMenuOption(item.name, item.GetDescription(), item.icon);
            menuOptions.Add(menuOption);
        }
    }

    public override void Close()
    {
        if (Open)
        {
            character.selectedItem = character.quickbar[menuOptions.IndexOf(selectedOption)];
            base.Close();
        }
    }
}
