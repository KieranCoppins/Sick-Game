using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class A_UseItem : CustomAction
{
    [SerializeField] private InventoryItem _item;

    public A_UseItem() : base()
    {
        Flags |= ActionFlags.Interruptor;
    }

    public override IEnumerator Execute()
    {
        // TODO: use some kind of animation here
        Mob.Inventory.Use(_item);
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    public override string GetTitle()
    {
        try
        {
            return $"Use {_item.name}";
        }
        catch
        {
            return "Use Item";
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        try
        {
            nodeView.Error = "";
            return $"The mob will use {_item.name} if they have it in their inventory.";
        }
        catch (System.Exception e)
        {
            nodeView.Error=e.Message;
            return "There was an issue with this node";
        }
    }
}
