using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryRadialMenu : RadialMenu
{
    [SerializeField] private CharacterMovement _character;

    public override void LoadData()
    {
        if (MenuOptions.Count > 0)
            MenuOptions.Clear();

        foreach (var item in _character.InventoryQuickbar)
        {
            RadialMenuOption menuOption = new RadialMenuOption(item.name, item.GetDescription(), item.Icon);
            MenuOptions.Add(menuOption);
        }
    }

    public override void Close()
    {
        if (Open)
        {
            _character.SelectedItem = _character.InventoryQuickbar[MenuOptions.IndexOf(SelectedOption)];
            base.Close();
        }
    }
}
