using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class C_CanUseItem : CustomFunction<bool>
{
    [SerializeField] private InventoryItem _item;

    public override bool Invoke() => Mob.Inventory.Has(_item);

    public override string GetSummary(BaseNodeView nodeView)
    {
        try
        {
            nodeView.Error = "";
            return $"the player has a {_item.name}";
        }
        catch (System.Exception e)
        {
            nodeView.Error = e.Message;
            return "";
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }
}
