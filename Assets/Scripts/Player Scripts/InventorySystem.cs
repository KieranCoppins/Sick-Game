using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItem, int> _inventory = new Dictionary<InventoryItem, int>();

    /// <summary>
    /// Adds item to inventory. Returns true if item was successfully added
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool Add(InventoryItem item, int quantity = 1)
    {
        if (_inventory.ContainsKey(item))
            if (_inventory[item] < 99)
                _inventory[item] += quantity;
            else
                return false;
        else
            _inventory.Add(item, quantity);

        return true;
    }

    /// <summary>
    /// If we have item in our inventory; use it. Returns true if we were able to use it
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Use(InventoryItem item)
    {
        if (_inventory[item] > 0)
        {
            _inventory[item]--;
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
        foreach(var item in _inventory)
            if (item.Value > 0)
                items.Add(item.Key);
        return items.ToArray();
    }

    public InventoryItem GetItem(string name)
    {
        foreach(var item in _inventory)
        {
            if (item.Key.name == name)
                return item.Key;
        }
        return null;
    }

    /// <summary>
    /// Wipes our inventory of all items
    /// </summary>
    public void Clear()
    {
        _inventory = new Dictionary<InventoryItem, int>();
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
        if (_inventory.ContainsKey(item))
            return _inventory[item] > 0;
        return false;
    }

    /// <summary>
    /// Gets the quantity of the given item in our inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetQuantity(InventoryItem item)
    {
        if (Has(item))
            return _inventory[item];
        return 0;
    }
}
