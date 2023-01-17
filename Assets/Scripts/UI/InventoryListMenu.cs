using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListMenu : ListMenu
{
    [SerializeField] private CharacterMovement _character;
    public override void LoadData()
    {
        MenuOptions.Clear();
        InventoryItem[] items = _character.Inventory.GetItems();
        foreach (var item in items)
        {
            string favourited = _character.InventoryQuickbar.Contains(item) ? "*" : "";
            ListMenuOption lmo = new ListMenuOption($"{item.name}{favourited} <pos=92%>x{_character.Inventory.GetQuantity(item)}</pos>", item.GetDescription(), item.Icon);
            lmo.Metadata = item;
            MenuOptions.Add(lmo);
        }
    }

    public void UseSelectedItem()
    {
        _character.Inventory.Use((InventoryItem)SelectedOption.Metadata);
    }

    public void FavouriteSelectedItem()
    {
        InventoryItem item = (InventoryItem)SelectedOption.Metadata;
        if (item != null)
        {
            if (_character.InventoryQuickbar.Contains(item))
                _character.InventoryQuickbar.Remove(item);
            else
                _character.InventoryQuickbar.Add(item);
        }
    }
}
