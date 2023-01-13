using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CanUseItem : F_Condition
{
    [SerializeField] InventoryItem item;
    public override bool Invoke() => mob.inventory.Has(item);

    public override string GetSummary(BaseNodeView nodeView)
    {
        try
        {
            nodeView.error = "";
            return $"the player has a {item.name}";
        }
        catch (System.Exception e)
        {
            nodeView.error = e.Message;
            return "";
        }
    }
}
