using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListMenu : ListMenu
{
    [SerializeField] CharacterMovement character;
    public override void LoadData()
    {
        menuOptions.Clear();
        InventoryItem[] items = character.inventory.GetItems();
        foreach (var item in items)
        {
            string favourited = character.quickbar.Contains(item) ? "*" : "";
            ListMenuOption lmo = new ListMenuOption($"{item.name}{favourited} <pos=92%>x{character.inventory.GetQuantity(item)}</pos>", item.GetDescription(), item.icon);
            lmo.Metadata = item;
            menuOptions.Add(lmo);
        }
    }

    public void UseSelectedItem()
    {
        character.inventory.Use((InventoryItem)SelectedOption.Metadata);
    }

    public void FavouriteSelectedItem()
    {
        InventoryItem item = (InventoryItem)SelectedOption.Metadata;
        if (item != null)
        {
            if (character.quickbar.Contains(item))
                character.quickbar.Remove(item);
            else
                character.quickbar.Add(item);
        }
    }
}
