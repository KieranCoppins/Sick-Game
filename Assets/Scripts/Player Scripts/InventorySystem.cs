using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InventorySystem : MonoBehaviour
{
    Dictionary<InventoryItem, int> inventory = new Dictionary<InventoryItem, int>();

    /// <summary>
    /// Adds item to inventory. Returns true if item was successfully added
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool Add(InventoryItem item, int quantity = 1)
    {
        if (inventory.ContainsKey(item))
            if (inventory[item] < 99)
                inventory[item] += quantity;
            else
                return false;
        else
            inventory.Add(item, quantity);

        return true;
    }

    /// <summary>
    /// If we have item in our inventory; use it. Returns true if we were able to use it
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Use(InventoryItem item)
    {
        if (inventory[item] > 0)
        {
            inventory[item]--;
            item.Use(GetComponent<BaseCharacter>());
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all the items in the inventory that we have
    /// </summary>
    /// <returns></returns>
    public InventoryItem[] GetItems()
    {
        List<InventoryItem> items = new List<InventoryItem>();
        foreach(var item in inventory)
            if (item.Value > 0)
                items.Add(item.Key);
        return items.ToArray();
    }

    /// <summary>
    /// Wipes our inventory of all items
    /// </summary>
    public void Clear()
    {
        inventory = new Dictionary<InventoryItem, int>();
    }

    /// <summary>
    /// A debug command that adds 99 of every item in the game
    /// </summary>
    public void DEBUG_AddAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(InventoryItem).Name);
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            InventoryItem item = AssetDatabase.LoadAssetAtPath<InventoryItem>(path);
            Add(item, 99);
        }
    }

    /// <summary>
    /// Returns true if we have at least 1 of the given item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Has(InventoryItem item)
    {
        if (inventory.ContainsKey(item))
            return inventory[item] > 0;
        return false;
    }
}